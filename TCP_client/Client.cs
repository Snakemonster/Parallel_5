using System.Net.Sockets;
using System.Text;
using XORCipher_FS;

namespace TCP_client;

public class Client
{
    private readonly int _port;
    private readonly string _name;
    private readonly string _server;
    private readonly string _password;

    private TcpClient _client;
    private NetworkStream _stream;
    private XORCipher _cipher;
    
    public Client(string name, string password, string server = "127.0.0.1", int port = 8888)
    {
        _name = name;
        _password = password;
        _server = server;
        _port = port;

        _client = new TcpClient();
        _stream = null;
        _cipher = new XORCipher();
    }

    public void RunCLient()
    {
        try
        {
            _client.Connect(_server, _port);
            _stream = _client.GetStream();

            var message = _cipher.Encrypt(_name, _password);
            var data = Encoding.Unicode.GetBytes(message);
            _stream.Write(data, 0, data.Length);

            var receiveThread = new Thread(ReceiveMessage);
            receiveThread.Start();
            Console.WriteLine($"Welcome {_name}");
            SendMessage();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            Disconnect();
        }
    }
    
    void SendMessage()
    {
        Console.Write("Write message: ");
             
        while (true)
        {
            var message = Console.ReadLine();
            var fullMessage = _cipher.Encrypt(message, _password);
            var data = Encoding.Unicode.GetBytes(fullMessage);
            _stream.Write(data, 0, data.Length);
        }
    }

    private void ReceiveMessage()
    {
        while (true)
        {
            try
            {
                var data = new byte[64];
                var builder = new StringBuilder();
                var bytes = 0;
                do
                {
                    bytes = _stream.Read(data, 0, data.Length);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (_stream.DataAvailable);
 
                var message = builder.ToString();
                var decryptedMessage = _cipher.Decrypt(message, _password);
                Console.WriteLine(decryptedMessage);
            }
            catch
            {
                Console.WriteLine("Connection is lose!");
                Console.ReadLine();
                Disconnect();
            }
        }
    }

    private void Disconnect()
    {
        _stream.Close();
        _client.Close();
        Environment.Exit(0);
    }
}