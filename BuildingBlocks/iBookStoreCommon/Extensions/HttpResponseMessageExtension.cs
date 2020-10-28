using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using iBookStoreCommon.Infrastructure;

namespace iBookStoreCommon.Extensions
{
    public static class HttpResponseMessageExtension
    {
        public static async Task EnsureSuccessfulResultAsync(this HttpResponseMessage httpResponseMessage)
        {
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                string responseString = null;
                if (httpResponseMessage.Content != null)
                {
                    responseString = await httpResponseMessage.Content.ReadAsStringAsync();
                    httpResponseMessage.Content.Dispose();
                }

                throw new HttpResponseException(httpResponseMessage.StatusCode == HttpStatusCode.BadRequest ?
                    responseString ?? httpResponseMessage.ReasonPhrase : httpResponseMessage.ReasonPhrase,
                    (int)httpResponseMessage.StatusCode);
            }
        }
    }
}
