using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using HttpMessageParser;
using HttpMessageParser.Models;

namespace Web_Application
{
    internal class Program
    {
        private const int Port = 8080;
        private const string rootDirectory = "static";

        private static readonly Dictionary<string, string> Contentypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
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


                        if (bytesRead == 0)
                        {
                            clientSocket.Dispose();
                            return;
                        }
                        Console.WriteLine($"Received {bytesRead} bytes from client.");
                        string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                        HttpParser httpParser = new HttpParser();
                        HttpRequest httpRequest;
                        try
                        {
                            httpRequest = httpParser.ParseRequest(receivedData);
                        }
                        catch (Exception ex) { SER(clientSocket, 400, "Bad Request"); return; }
                        string metodo = httpRequest.Method;
                        string path = httpRequest.RequestTarget;
                        if (metodo != "GET")
                        {
                            SER(clientSocket, 405, "Method Not Allowed");
                            return;
                        }
                        if (path.Contains("..") || path.Contains("//"))
                        {
                            SER(clientSocket, 403, "Forbidden");
                            return;
                        }

                        if (path == "/")
                        {
                            path = "/index.html";
                        }

                        if (path.Contains("?"))
                        {
                            path = path.Split('?')[0];
                        }
                        string FP = Path.Combine(rootDirectory, path.TrimStart('/'));
                        if (File.Exists(FP))
                        {
                            SF(clientSocket, FP);
                        }
                        else if (Directory.Exists(FP))
                        {
                            string indexF = Path.Combine(FP, "index.html");
                            if (File.Exists(indexF)) { SF(clientSocket, indexF); }
                            else
                            {
                                string htmlist = DRClist(FP, path);
                                HTMLRes(clientSocket, htmlist, 200, "OK");
                            }
                        }
                        else
                        {
                            SER(clientSocket, 404, "Not Found");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error with the server:{ex.Message}");
                    }
                    finally { clientSocket.Close(); Console.WriteLine("Sesion Closed"); }
                });
            }
        }
        private static void SF(Socket clientSocket, string FP)
        {
            byte[] FB = File.ReadAllBytes(FP);
            string CT = GetCT(FP);
        }
        private static string GetCT(string FP)
        {
            string extension = Path.GetExtension(FP);
            if (Contentypes.TryGetValue(extension, out string ct)) { return ct; }
            return "application/octet-stream";

        }
        private static string DRClist(string dirp, string urlp)
        {
            try
            {
                var F = Directory.GetFiles(dirp);
                var D = Directory.GetDirectories(dirp);
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
                html.AppendLine($"<h1>Directory: {urlp}</h1>");
                html.AppendLine("<ul>");
                if (urlp != "/")
                {
                    string father = urlp.TrimEnd('/');
                    int EndSlash = father.LastIndexOf('/');
                    html.AppendLine($"<li><a href='{father}'>[Parent Directory]</a></li>");
                }
                foreach (var d in D)
                {
                    string Dname = Path.GetFileName(d);
                    html.AppendLine($"<li><a href='{urlp.TrimEnd('/')}/{Dname}/'>{Dname}/</a></li>");
                }
                foreach (var f in F)
                {
                    string filename = Path.GetFileName(f);
                    html.AppendLine($"<li><a href='{urlp.TrimEnd('/')}/{filename}'>{filename}</a></li>");
                }
                html.AppendLine("</ul>");
                html.AppendLine("</body></html>");

                return html.ToString();
            }
            catch (Exception ex) { Console.WriteLine($"Directory Listing Error:{ex.Message}"); };
            return "<html><body><h1>500 Internal Server Error</h1><p>Error generating directory listing</p></body></html>";
        }

        private static void HTMLRes(Socket clientSocket, string html, int statuscode, string statusmessage)
        {
            try
            {
                HttpResponse response = new HttpResponse
                {
                    Protocol = "HTTP/1.1",
                    StatusCode = statuscode,
                    StatusText = statusmessage,
                    Headers = new Dictionary<string, string>
                    {
                        {"Content-Type", "text/html"},
                        {"Content-Length", html.Length.ToString()},
                        {"Connection", "close"}
                    },
                    Body = html
                };
                HttpWriter httpWriter = new HttpWriter();
                string completeResponse = httpWriter.WriteResponse(response);
                byte[] responseBytes = Encoding.UTF8.GetBytes(completeResponse);
                clientSocket.Send(responseBytes);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error:{ex.Message})");
            }
        }
        private static void SER(Socket clientSocket, int statusCode, string statusMessage)
        {
            string html = $"<html><body><h1>{statusCode} {statusMessage}</h1></body></html>";
            HTMLRes(clientSocket, html, statusCode, statusMessage);
            Console.WriteLine($"Error: {statusCode} {statusMessage}");
        }

    }
}