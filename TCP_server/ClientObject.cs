using System.Net.Sockets;
using System.Text;

namespace TCP_test;

public class ClientObject
{
    protected internal string Id { get; private set; }
    protected internal NetworkStream Stream { get; private set; }
    private string _userName;
    private TcpClient _client;
    private ServerObject _server;

    public ClientObject(TcpClient tcpClient, ServerObject serverObject)
    {
        Id = Guid.NewGuid().ToString();
        _client = tcpClient;
        _server = serverObject;
        serverObject.AddConnection(this);
    }

    public void Process()
    {
        try
        {
            Stream = _client.GetStream();
            var message = GetMessage();
            _userName = message;

            message = _userName + " join to chat";
            _server.BroadcastMessage(message, Id);
            Console.WriteLine(message);
            while (true)
            {
                try
                {
                    message = GetMessage();
                    message = String.Format($"{_userName}: {message}");
                    Console.WriteLine(message);
                    _server.BroadcastMessage(message, Id);
                }
                catch
                {
                    message = String.Format($"{_userName}: leave chat");
                    Console.WriteLine(message);
                    _server.BroadcastMessage(message, Id);
                    break;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        finally
        {
            _server.RemoveConnection(Id);
            Close();
        }
    }

    private string GetMessage()
    {   
        var data = new byte[64];
        var builder = new StringBuilder();
        var bytes = 0;
        do
        {
            bytes = Stream.Read(data, 0, data.Length);
            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
        } while (Stream.DataAvailable);

        var decryptedMessage = _server.Cipher.Decrypt(builder.ToString(), _server._password);
        return decryptedMessage;
    }

    protected internal void Close()
    {
        Stream.Close();
        _client.Close();
    }
}