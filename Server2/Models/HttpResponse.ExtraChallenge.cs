namespace HttpMessageParser.Models
{
    public partial class HttpResponse
    {
        /// <summary>
        /// Returns the value of a specific header from the request. This is case-insensitive.
        /// </summary>
        /// <param name="headerName">The name of the header to retrieve.</param>
        /// <returns>
        /// The value of the specified header if it exists; otherwise, null.
        /// </returns>
        /// <exception cref="ArgumentException">If the <paramref name="headerName"/> is null or empty.</exception>"
        public string GetHeaderValue(string headerName)
        {
            if (string.IsNullOrEmpty(headerName)) { throw new ArgumentException(nameof(headerName)); }
            var header = Headers?.FirstOrDefault(h => string.Equals(h.Key, headerName,StringComparison.OrdinalIgnoreCase));
            return header?.Value;
        }

        /// <summary>
        /// Returns a boolean indicating whether the response is successful.
        /// </summary>
        /// <returns>
        /// True if the status code indicates a success, otherwise false.
        /// </returns>
        public bool IsSuccess()
        {
            if (string.IsNullOrEmpty(StatusCode.ToString()))
                return false;

            var ms=StatusCode.ToString();
            return ms.StartsWith("2");
        }

        /// <summary>
        /// Returns a boolean indicating whether the response is a client error.
        /// </summary>
        /// <returns>
        /// True if the status code indicates a client error, otherwise false.
        /// </returns>
        public bool IsClientError()
        {
            if(string.IsNullOrEmpty(StatusCode.ToString())) return false;
            var ms=StatusCode.ToString();
            return ms.StartsWith("4");
 
        }

        /// <summary>
        /// Returns a boolean indicating whether the response is a server error.
        /// </summary>
        /// <returns>
        /// True if the status code indicates a server error, otherwise false.
        /// </returns>
        public bool IsServerError()
        {
            if (string.IsNullOrEmpty(StatusCode.ToString())) { return false; }
            var ms=StatusCode.ToString();
            return ms.StartsWith("5");
        }
    }
}
