using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class WebMethods {

    public static async Task<HttpResponseMessage> Request(HttpMethod pMethod, string pUrl, string pJsonContent, Dictionary<string, string> pHeaders = null) {
        var httpRequestMessage = new HttpRequestMessage {
            Method = pMethod,
            RequestUri = new Uri(pUrl)
        };
        if (pHeaders != null) {
            foreach (var head in pHeaders) {
                httpRequestMessage.Headers.Add(head.Key, head.Value);
            }
        }
        var client = new HttpClient();
        switch (pMethod.Method) {
            case "POST":
                HttpContent httpContent = new StringContent(pJsonContent, Encoding.UTF8, "application/json");
                httpRequestMessage.Content = httpContent;
                using (var ct = new CancellationTokenSource(new TimeSpan(0, 0, 5))) {
                    return await client.PostAsync(pUrl, httpContent, ct.Token).ConfigureAwait(false);
                }
        }
        using (var ct = new CancellationTokenSource(new TimeSpan(0, 0, 5))) {
            return await client.GetAsync(pUrl, ct.Token).ConfigureAwait(false);
        }
        //return await client.SendAsync(httpRequestMessage);
    }
}