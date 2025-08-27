using System.Net.Mime;
using System.Web;

namespace HttpMessageParser.Models
{
    public partial class HttpRequest
    {
        /// <summary>
        /// Returns the value of a specific header from the request. This is case-insensitive.
        /// </summary>
        /// <param name="headerName">The name of the header to retrieve.</param>
        /// <returns>
        /// The value of the specified header if it exists; otherwise, null.
        /// </returns>
        /// <exception cref="ArgumentException">If the <paramref name="headerName"/> is null or empty.</exception>"
        public string? GetHeaderValue(string headerName)
        {
            if (string.IsNullOrWhiteSpace(headerName)) throw new ArgumentException(nameof(headerName));
            var header = Headers?.FirstOrDefault(h =>string.Equals(h.Key, headerName,StringComparison.OrdinalIgnoreCase));
            return header?.Value;
            //if (headerName == null) throw new ArgumentNullException();
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Extracts the query parameters from the <c>RequestTarget</c> property.
        /// </summary>
        /// <remarks>
        /// If no query parameters are present, this method will return an empty dictionary.
        /// </remarks>
        /// <returns>
        /// A dictionary where the keys are the parameter names and the values are the parameter values.
        /// </returns>
        /// <exception cref="FormatException">If the <c>RequestTarget</c> contains query parameters, but they are malformed.</exception>"
        public Dictionary<string, string> GetQueryParameters()
        {
            var queryParams = new Dictionary<string, string>();
            if (!RequestTarget.Contains("?")) { return queryParams; }
            var queryString = RequestTarget.Split('?')[1];
            var pairs = queryString.Split('&');
            foreach (var pair in pairs)
            {
                var parts = pair.Split('=');
                if (parts.Length != 2)
                    throw new FormatException("Malformed query string");

                queryParams[HttpUtility.UrlDecode(parts[0])] = HttpUtility.UrlDecode(parts[1]);
            }

            return queryParams;
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Extracts the form data from the request body. This supports both the "application/x-www-form-urlencoded" 
        /// and the "multipart/form-data" content types.
        /// </summary>
        /// <remarks>
        /// If the request has no body or the body does not contain form data, this method will return an empty dictionary.
        /// </remarks>
        /// <returns>
        /// A dictionary where the keys are the form field names and the values are the form field values.
        /// </returns>
        /// <exception cref="FormatException">If the contents of the body are not in the correct format.</exception>
        public Dictionary<string, string> GetFormData()
        {
            var formData = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(Body))
                return formData;

            var contentType = GetHeaderValue("Content-Type");
            if (contentType == null)
                return formData;

            if (contentType.StartsWith("application/x-www-form-urlencoded"))
            {
                var pairs = Body.Split('&');
                foreach (var pair in pairs)
                {
                    var parts = pair.Split(new[] { '=' }, 2);
                    if (parts.Length != 2)
                        throw new FormatException("Malformed form data");

                    formData[HttpUtility.UrlDecode(parts[0])] = HttpUtility.UrlDecode(parts[1]);
                }
            }
            else if (contentType.StartsWith("multipart/form-data"))
            {
                var boundaryKey = contentType.Contains(";boundary=") ? ";boundary=" : "boundary=";
                var boundaryIndex = contentType.IndexOf(boundaryKey);
                if (boundaryIndex == -1)
                    throw new FormatException("Missing boundary in multipart form data");

                var boundary = "--" + contentType.Substring(boundaryIndex + boundaryKey.Length).Trim('"', ' ');

                var parts = Body.Split(new[] { boundary }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var part in parts)
                {
                    if (string.IsNullOrWhiteSpace(part.Trim()))
                        continue;

                    if (part.TrimEnd().EndsWith("--"))
                        continue;

                    var nameStart = part.IndexOf("name=\"");
                    if (nameStart == -1)
                        throw new FormatException("Missing name in form-data part");

                    nameStart += 6;
                    var nameEnd = part.IndexOf("\"", nameStart);
                    if (nameEnd == -1)
                        throw new FormatException("Malformed name in form-data part");

                    var name = part.Substring(nameStart, nameEnd - nameStart);

                    var valueStart = part.IndexOf("\n\n");
                    if (valueStart == -1)
                        throw new FormatException("Malformed form-data part");

                    valueStart += 2;
                    var value = part.Substring(valueStart).TrimEnd('\n', '\r');

                    formData[name] = value;
                }
            }

            return formData;
        }
    }
}
