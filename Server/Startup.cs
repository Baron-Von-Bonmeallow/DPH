using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using HttpMessageParser;
//using ASP.NET;

namespace WebPlatformServer
{
    public class Server 
    {
        private TcpListener _listener;
        private int _port;
        private string _rootDirectory;
        private Dictionary<string, string> _contentTypes;
        private bool _isRunning;
        public Server(int port = 8080, string rootDirectory="static") 
        {
            _port = port;
            _rootDirectory = rootDirectory;
            Contentype();
            if (!Directory.Exists(_rootDirectory)) {  Directory.CreateDirectory(_rootDirectory); }
        }
        private void Contentype() 
        {
            _contentTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { ".html", "text/html" },
                { ".htm", "text/html" },
                { ".css", "text/css" },
                { ".js", "application/javascript" },
                { ".json", "application/json" },
                { ".png", "image/png" },
                { ".jpg", "image/jpeg" },
                { ".jpeg", "image/jpeg" },
                { ".gif", "image/gif" },
                { ".svg", "image/svg+xml" },
                { ".ico", "image/x-icon" },
                { ".txt", "text/plain" },
                { ".xml", "application/xml" }
            };
        }
        private void HandleConnection(object state) 
        {
            while(_isRunning) 
            {
            try
                {
                    TcpClient client = _listener.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(HandleClient, client);

                }
                catch  (Exception ex)
                {
                    if (_isRunning) { Console.WriteLine(ex.ToString()); }
                }
            }
        }
        //private void Handlerequest(NetworkStream stream, string filePath) 
        //{
        
        //}
        private void HandleClient(object state) 
        {
            TcpClient client = (TcpClient)state;
            try 
            { 
            using(client)
            using (NetworkStream stream = client.GetStream()) 
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) return;
                    string request = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    string[] requestLines = request.Split('\n');

                    if (requestLines.Length == 0) return;

                    string[] requestParts = requestLines[0].Split(' ');
                    if (requestParts.Length < 3) return;


                    HttpParser httpParser = new HttpParser();
                    HttpMessageParser.Models.HttpRequest httpRequest = httpParser.ParseRequest(request);

                    string method = httpRequest.Method;
                    string path = httpRequest.RequestTarget;

                    if (method != "GET")
                    {
                        SendErrorResponse(stream, 405, "Method not allow");
                        return;
                    }
                    ProcessRequest(stream, path);

                }
            }
            catch
            {
            
            }
        }

        private void ProcessRequest(NetworkStream stream, string path)
        {
            try 
            {
            if(path == "/") 
                {
                    path = "/index.html";
                }
                if (path.Contains("?")) 
                {
                    path = path.Split('?')[0];
                }
                if (path.Contains("..") || path.Contains("//"))
                {
                    SendErrorResponse(stream, 403, "Forbidden");
                    return;
                }
            }
            catch 
            {
            
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
        private string GetContentype(string filePath) 
        {
            string extension = Path.GetExtension(filePath);
            if (_contentTypes.TryGetValue(extension, out string contentype)){ return contentype; }
            string sta = "application/octet-stream";
            return sta;
        }
        private void SendErrorResponse(NetworkStream stream, int code, string message) 
        {
            string html = $"<html><body><h1>{code} {message}</h1><p>{DateTime.Now}</p></body></html>";
            byte[] htmlBytes = Encoding.UTF8.GetBytes(html);
            string headers = $"HTTP/1.1 {code} {message}\r\n" +
                          $"Content-Type: text/html\r\n" +
                          $"Content-Length: {htmlBytes.Length}\r\n" +
                          $"Connection: close\r\n" +
                          $"\r\n";
            byte[] headerBytes = Encoding.UTF8.GetBytes(headers);
            stream.Write(headerBytes, 0, headerBytes.Length);
            stream.Write(htmlBytes, 0, htmlBytes.Length);
            Console.WriteLine($"{code}:{message}");
        }
        public void Stop()
        {
            _isRunning = false;
            _listener?.Stop();
            Console.WriteLine("Server stopped");
        }
    }
}
