using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using HttpMessageParser;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection.Metadata;

namespace Web_Application
{
    internal class Program
    {
        private const int Port = 8080;
        private const string rootDirectory = "static";
        static void Main(string[] arg)
        {
            IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Loopback, Port);
            Socket listener = new Socket(
                serverEndpoint.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);
            listener.Bind(serverEndpoint);
            listener.Listen();
            Console.WriteLine($"Server is listening on {serverEndpoint.Port}");
            while (true)
            {
                Socket clientSocket = listener.Accept();
                Console.WriteLine($"Accepted connection from {clientSocket.RemoteEndPoint}");
                Task.Run(() =>
                {
                    try
                    {
                        byte[] buffer = new byte[1024];
                        int bytesRead = clientSocket.Receive(buffer);
                        Console.WriteLine($"Received {bytesRead} bytes from client.");

                        string receivedData = System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);

                        // Create an instance of HttpParser and use it
                        HttpParser httpParser = new HttpParser();
                        HttpMessageParser.Models.HttpRequest httpRequest = httpParser.ParseRequest(receivedData);
                        
                        string metodo = httpRequest.Method;
                        string path = httpRequest.RequestTarget;

                        if(metodo != "GET") { string response = "HTTP/1.1 405 Method not allowed"; }
                        if (!File.Exists(path)) { string response = "HTTP/1.1 404 Not found"; }
                        else
                        {
                            byte[] bytes = File.ReadAllBytes(rootDirectory + path);
                            string content = File.ReadAllText(rootDirectory + path);
                            string response = $"HTTP/1.1 200 OK \nContent - Type: text / html\nContent - Length: {bytes.Length}";
                        }

                        /*
                         * Del receivedData extraer:
                         * Ruta/Path
                         * Metodo
                         * 
                         * string metodo = ...
                         * if(metodo != "GET"){
                         * string response = "HTTP/1.1 405 Method not allowed"
                         * }
                         * 
                         * SI el metodo es GET
                         * if (!File.Exists(rootDirectory + path)){
                         * string response = "HTTP/1.1 404 Not found"
                         * }
                         * 
                         * byte[] bytes = File.ReadAllBytes(rootDirectory + path);
                         * string content = File.ReadAllText(...)
                         * response = "HTTP/1.1 200 OK
                         * Content-Type: text/html
                         * Content-Length: bytes.Length
                         * 
                         * <contenido del archivo>
                         * */



                        Console.WriteLine($"Received data: {receivedData} from {clientSocket.RemoteEndPoint}");
                        string responseMessage = $"You said: {receivedData}";
                        byte[] responseBytes = System.Text.Encoding.ASCII.GetBytes(responseMessage);
                        clientSocket.Send(responseBytes);
                    }
                    finally
                    {
                        // Ensure the client socket is disposed of after processing.
                        clientSocket.Dispose();
                        Console.WriteLine("Client socket disposed.");
                    }
                }
                );
            
            }

        }
        private string DirectoryListing(string dirPath, string urlPath)
        {
            var files = Directory.GetFiles(dirPath);
            var directories = Directory.GetDirectories(dirPath);
            StringBuilder html = new StringBuilder();
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html><head>");
            html.AppendLine("<title>Directory Listing</title>");
            html.AppendLine("<style>");
            html.AppendLine("body { font-family: Arial, sans-serif; margin: 40px; }");
            html.AppendLine("h1 { color: #333; }");
            html.AppendLine("ul { list-style: none; padding: 0; }");
            html.AppendLine("li { margin: 5px 0; }");
            html.AppendLine("a { text-decoration: none; color: #0066cc; }");
            html.AppendLine("a:hover { text-decoration: underline; }");
            html.AppendLine("</style>");
            html.AppendLine("</head><body>");
            html.AppendLine($"<h1>Directory: {urlPath}</h1>");
            html.AppendLine("<ul>");
            if (!urlPath.Contains("/"))
            {
                string parentUrl = urlPath.EndsWith("/") ? urlPath[..^1] : urlPath;
                parentUrl = parentUrl.Substring(0, parentUrl.LastIndexOf('/') + 1);
                html.AppendLine($"<li><a href='{parentUrl}'>[Parent Directory]</a></li>");
            }
            foreach (string dir in directories)
            {
                string dirName = Path.GetFileName(dir);
                html.AppendLine($"<li><a href='{urlPath.TrimEnd('/')}/{dirName}/'>{dirName}/</a></li>");
            }
            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                html.AppendLine($"<li><a href='{urlPath.TrimEnd('/')}/{fileName}'>{fileName}</a></li>");
            }

            html.AppendLine("</ul>");
            html.AppendLine("</body></html>");

            return html.ToString();
        }

    }
}
