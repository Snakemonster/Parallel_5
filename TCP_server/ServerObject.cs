using System.Net;
using System.Net.Sockets;
using System.Text;
using XORCipher;


namespace TCP_test;

public class ServerObject
{
    private TcpListener _tcpListener;
    private List<ClientObject> _clients = new();
    public XorCipher Cipher { get; private set; }
    protected internal readonly string _password;

    public ServerObject()
    {
        Console.WriteLine();
        _password = XorCipher.GetRandomKey(553463, 38);
        Console.WriteLine($"Password is {_password}\nNobody tell it!\n");
    }
    protected internal void AddConnection(ClientObject clientObject)
    {
        _clients.Add(clientObject);
    }

    protected internal void RemoveConnection(string id)
    {
        var client = _clients.FirstOrDefault(c => c.Id == id);
        if (client != null)
            _clients.Remove(client);
    }

    protected internal void Listen()
    {
        try
        {
            _tcpListener = new TcpListener(IPAddress.Any, 8888);
            _tcpListener.Start();
            Cipher = new XorCipher();
            
            Console.WriteLine("Server is started. Waiting for connections...");
            
            while (true)
            {
                var tcpClient = _tcpListener.AcceptTcpClient();

                var clientObject = new ClientObject(tcpClient, this);
                var clientThread = new Thread(clientObject.Process);
                clientThread.Start();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Disconnect();
        }
    }

    protected internal void BroadcastMessage(string message, string id)
    {
        var fullMessage = Cipher.Encrypt(message, _password);
        var data = Encoding.Unicode.GetBytes(fullMessage);
        foreach (var client in _clients.Where(client => client.Id != id))
        {
            client.Stream.Write(data, 0, data.Length);
        }
    }
    
    protected internal void Disconnect()
    {
        _tcpListener.Stop();

        foreach (var client in _clients)
        {
            client.Close();
        }

        Environment.Exit(0);
    }
}
