// See https://aka.ms/new-console-template for more information
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Threading.Channels;
using System.Xml.Linq;


public class ClientPipe {
    NamedPipeClientStream pipeClient;
    StreamReader sr;
    bool logging = false;
    string fileName = "";
    public ClientPipe(string name,bool logging)
    {
        this.pipeClient = new NamedPipeClientStream(".", name, PipeDirection.In);
        this.logging = logging;
        this.sr= new StreamReader(pipeClient);
    }
    public ClientPipe(string name, bool logging,string fileName)
    {
        this.pipeClient = new NamedPipeClientStream(".", name, PipeDirection.In);
        this.logging= logging;
        this.sr = new StreamReader(pipeClient);
        this.fileName = fileName;
    }
    public void connect()
    {
        if(this.logging) Console.Write("Attempting to connect to pipe...\n");
        try
        {
            pipeClient.Connect();
        }
        catch(Exception e)
        {
            if (this.logging) Console.WriteLine(e.Message);
        }
    }
    public string readNextLine()
    {
        try
        {
            bool leave = false;
            string output = "";
            while (!leave)
            {

                char tem;
                tem = (char)sr.Read();
                if (tem == '\uffff')
                {
                    return output;
                    leave = true;
                }
                if (tem == '\u0005' || tem == '\b' || tem == '\uffff' || tem == '\u0004')
                {
                    return output;
                }
                else
                {
                    output += ((char)tem);
                }

            }
            return "";
        }
        catch(Exception e)
        {
            if (this.logging) Console.WriteLine(e.Message);
            return "";
        }
    }
    public void close()
    {
        try
        {
            pipeClient.Close();
        }
        catch (Exception e)
        {
            if (this.logging) Console.WriteLine(e.Message);
        }
    }
         
}


public class ServerPipe
{
    bool logging=false;
    string fileName = "";
    NamedPipeServerStream server;
    MemoryStream stream = new MemoryStream();
    BinaryWriter writer;
    public ServerPipe(string name, bool logging)
    {
       this.server = new NamedPipeServerStream(name);
        writer = new BinaryWriter(stream);
        this.logging = logging;
       
    }
    public ServerPipe(string name, bool logging,string fileName)
    {
        this.server = new NamedPipeServerStream(name);
        writer = new BinaryWriter(stream);
        this.logging = logging;
        this.fileName = fileName;
    }
    public void connect()
    {
        if (this.logging) Console.WriteLine("Server Starting Waiting for connection...");
        this.server.WaitForConnection();
    }
    public void clearStream()
    {
        stream.SetLength(0);
    }
    public void writeStream(string data)
    {
        writer.Write(data);
        
    }
    public void sendStream()
    {
        try
        {
            server.Write(stream.ToArray(), 0, stream.ToArray().Length);
        }
        catch(Exception e)
        {
           if(this.logging) Console.WriteLine(e.Message);
        }

    }

    public void close()
    {
        try
        {
            server.Disconnect();
        }
        catch (Exception e)
        {
            if(this.logging)Console.WriteLine(e.Message);
        }
    }

}