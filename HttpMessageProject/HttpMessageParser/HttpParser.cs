using HttpMessageParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpMessageParser
{
    public class HttpParser : IRequestParser
    {
        public HttpRequest ParseRequest(string request)
        {
            
            if (request == null) {  throw new ArgumentNullException(nameof(request));}
            if (string.IsNullOrWhiteSpace(request)) {  throw new ArgumentException(nameof(request));}
            var lines = request.Split(new[] { "\n" }, StringSplitOptions.None);
            if (lines.Length == 0) { throw new ArgumentException("Invalid HTTP request format", nameof(request));}

            var rqlines = lines[0].Split(' ');
            if(rqlines.Length != 3) { throw new ArgumentException("Invalid HTTP request line format", nameof(request)); }

            var method = rqlines[0];
            var path = rqlines[1];
            var protocol = rqlines[2];


            if (string.IsNullOrEmpty(method)) { throw new ArgumentException("HTTP method is required", nameof(request)); }

            if (string.IsNullOrEmpty(path) || !path.Contains("/")) { throw new ArgumentException("Path must include at least '/'", nameof(request)); }
            
            if (!protocol.StartsWith("HTTP/")||string.IsNullOrEmpty(protocol)) { throw new ArgumentException("Protocol must start with 'HTTP/'", nameof(request)); }
            
            var headers=new Dictionary<string, string>();
            var BL=new List<string>();
            bool isBody = false;

            for(int i = 1; i < lines.Length; i++) 
            {
                if (string.IsNullOrEmpty(lines[i])) { isBody= true; continue; }
                if (!isBody) 
                {
                    var heaParts = lines[i].Split(new[] { ':' },2);
                    if (heaParts.Length != 2 || string.IsNullOrWhiteSpace(heaParts[0]) || string.IsNullOrWhiteSpace(heaParts[1])) { throw new ArgumentException($"Invalid header format: {lines[i]}", nameof(request)); }
                    headers[heaParts[0].Trim()] = heaParts[1].Trim();
                }
                else { BL.Add(lines[i]); }
            }

            string bodyContent = string.Join("\r\n", BL);
            string? finalBody = string.IsNullOrWhiteSpace(bodyContent) ? null : bodyContent;

            return new HttpRequest
            {
                Method = method,
                RequestTarget = path,
                Protocol = protocol,
                Headers = headers,
                //Body = string.Join("\r\n", BL)
                Body = finalBody
            };
            //throw new NotImplementedException();
        }
    }
}
