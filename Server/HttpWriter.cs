using HttpMessageParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpMessageParser
{
    public class HttpWriter : IResponseWriter
    {

        public string WriteResponse(HttpResponse response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            if (response.Protocol == null || response.StatusCode == null || response.StatusText == null) { throw new ArgumentException("Required response properties cannot be null"); }
            
            var sb = new StringBuilder();
            //var sc= new StringBuilder();
            sb.Append($"{response.Protocol} {response.StatusCode} {response.StatusText}\n");
            //sc.Append($"{response.Protocol} {response.StatusCode} {response.StatusText}\n");
            if (response.Headers !=null)
            {
                foreach (var header in response.Headers) { sb.Append($"{header.Key}: {header.Value}\n"); }
            }

            sb.Append("\n");

            if (!string.IsNullOrEmpty(response.Body)) {sb.Append(response.Body);}
            
            
            return sb.ToString();
            //throw new NotImplementedException();
        }
    }
}
