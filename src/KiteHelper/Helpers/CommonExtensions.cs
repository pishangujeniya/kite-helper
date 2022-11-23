using System.Net;
using System.Text;

namespace KiteHelper.Helpers
{
    public static class CommonExtensions
    {
        /// <summary>
        /// Reads Response content in string from WebException
        /// </summary>
        /// <param name="webException"></param>
        /// <returns></returns>
        public static (HttpStatusCode statusCode, string? responseString) GetResponseStringNoException(this WebException webException)
        {
            if (webException.Response is HttpWebResponse response)
            {
                Stream responseStream = response.GetResponseStream();

                StreamReader streamReader = new(responseStream, Encoding.Default);

                string responseContent = streamReader.ReadToEnd();
                HttpStatusCode statusCode = response.StatusCode;

                streamReader.Close();
                responseStream.Close();
                response.Close();

                return (statusCode, responseContent);
            }
            else
            {
                return (HttpStatusCode.InternalServerError, null);
            }
        }
    }
}
