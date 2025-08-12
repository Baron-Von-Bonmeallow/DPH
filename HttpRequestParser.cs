namespace HttpRequestParser
{
  public class HttpRequestParser:IRequestParser
  {
  public HttpRequest ParserRequest(string requestText)
    {
    if (requestText == null) throw new ArgumentNullException(nameof(requestText));

    if(string.IsNullOrWhiteSpace(reuestText)) throw new ArgumentException("",nameof(requestText));

    var request = new HttpRequest();
    }
  }
  
}
