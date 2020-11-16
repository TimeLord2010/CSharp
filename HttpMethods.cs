using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

public class HttpMethods {
    
    HttpClient client;
    /// <summary>
    /// HttpClient is disposible. However: https://stackoverflow.com/a/15708633/10008842
    /// </summary>
    public HttpClient Client {
        get {
            if (client == null) client = new HttpClient();
            return client;
        }
        set => client = value;
    }

    public async Task<HttpResponseMessage> Post(string url, Dictionary<string, string> values) {
        var content = new FormUrlEncodedContent(values);
        return await Client.PostAsync(url, content);
    }

    public async Task<HttpResponseMessage> Post(string url, string json) {
        return await Client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
        //var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
        //httpWebRequest.ContentType = "application/json";
        //httpWebRequest.Method = "POST";
        //using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream())) {
        //    streamWriter.Write(json);
        //}
        //var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        //using var streamReader = new StreamReader(httpResponse.GetResponseStream());
        //return streamReader.ReadToEnd();
    }

    public async Task<HttpResponseMessage> Post(string url, object anonymous_type) {
        return await Post(url, new JavaScriptSerializer().Serialize(anonymous_type));
    }

}