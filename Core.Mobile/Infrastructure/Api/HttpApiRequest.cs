using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core
{
    public interface IHttpApiRequest
    {
        Task<TResponse> PostAsync<TResponse>(string requestUri, object body, Dictionary<string, string> headers = null);
        Task PostAsync(string requestUri, object body, Dictionary<string, string> headers = null);
        Task PatchAsync(string requestUri, object body, Dictionary<string, string> headers = null);
        Task<TResponse> DeleteAsync<TResponse>(string requestUri, Dictionary<string, string> headers = null);
        Task<TResponse> PutAsync<TResponse>(string requestUri, object body, Dictionary<string, string> headers = null);
        Task PutAsync(string requestUri, object body, Dictionary<string, string> headers = null);
        Task<TResponse> GetAsync<TResponse>(string requestUri, Dictionary<string, string> headers = null);
    }

    public sealed class HttpApiRequest : IHttpApiRequest
    {
        string BaseAddress => AppConfiguration.ApiUrl;

        public async Task<TResponse> PostAsync<TResponse>(string requestUri, object body, Dictionary<string, string> headers = null)
            => await RestAuxAsync<TResponse>(HttpMethod.Post, requestUri, body, headers);

        public async Task PostAsync(string requestUri, object body, Dictionary<string, string> headers = null)
           => await RestAuxAsync(HttpMethod.Post, requestUri, body, headers);

        public async Task PatchAsync(string requestUri, object body, Dictionary<string, string> headers = null)
          => await RestAuxAsync(new HttpMethod("PATCH"), requestUri, body, headers);

        public async Task<TResponse> DeleteAsync<TResponse>(string requestUri, Dictionary<string, string> headers = null)
          => await RestAuxAsync<TResponse>(HttpMethod.Delete, requestUri, headers: headers);

        public async Task<TResponse> PutAsync<TResponse>(string requestUri, object body, Dictionary<string, string> headers = null)
            => await RestAuxAsync<TResponse>(HttpMethod.Put, requestUri, body, headers);

        public async Task PutAsync(string requestUri, object body, Dictionary<string, string> headers = null)
            => await RestAuxAsync(HttpMethod.Put, requestUri, body, headers);

        public async Task<TResponse> GetAsync<TResponse>(string requestUri, Dictionary<string, string> headers = null)
            => await RestAuxAsync<TResponse>(HttpMethod.Get, requestUri, headers: headers);

        async Task<TResponse> RestAuxAsync<TResponse>(HttpMethod httpMethod, string requestUri, object body, Dictionary<string, string> headers = null)
        {
            var json = JsonConvert.SerializeObject(body);
            using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                return await RestAuxAsync<TResponse>(httpMethod, requestUri, content, headers);
        }

        async Task RestAuxAsync(HttpMethod httpMethod, string requestUri, object body, Dictionary<string, string> headers = null)
        {
            var json = JsonConvert.SerializeObject(body);
            using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                await RestAuxAsync(httpMethod, requestUri, content, headers);
        }

        async Task<TResponse> RestAuxAsync<TResponse>(HttpMethod httpMethod, string requestUri, HttpContent requestContent = null, Dictionary<string, string> headers = null)
            => (TResponse)await RestAuxAsync(typeof(TResponse), httpMethod, requestUri, requestContent, headers);

        async Task RestAuxAsync(HttpMethod httpMethod, string requestUri, HttpContent requestContent = null, Dictionary<string, string> headers = null)
            => await RestAuxAsync(null, httpMethod, requestUri, requestContent, headers);

        async Task<object> RestAuxAsync(Type returnType, HttpMethod httpMethod, string requestUri, HttpContent requestContent = null, Dictionary<string, string> headers = null)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(httpMethod, $"{BaseAddress}{requestUri}"))
            {
                if (headers == null)
                    headers = new Dictionary<string, string>();

                headers.Add("Accept", "application/json");
                headers.Add("Culture", Thread.CurrentThread.CurrentUICulture.Name);

                foreach (var item in headers)
                    client.DefaultRequestHeaders.Add(item.Key, item.Value);

                request.Content = requestContent;

                using (var response = await client.SendAsync(request).ConfigureAwait(false))
                using (var content = response.Content)
                {
                    if (returnType?.Name == typeof(byte[]).Name)
                        using (var stream = await content.ReadAsStreamAsync())                        
                            using (MemoryStream ms = new MemoryStream())
                            {
                                stream.CopyTo(ms);
                                return ms.ToArray();
                            }
                        
                    var result = await content?.ReadAsStringAsync();

                    switch (response.StatusCode)
                    {
                        case System.Net.HttpStatusCode.OK:
                            if (returnType != null)
                                return JsonConvert.DeserializeObject(result, returnType);
                            return null;
                        case System.Net.HttpStatusCode.NoContent:
                            return null;
                    }

                    throw new ApiException(result)
                    {
                        StatusCode = response.StatusCode,
                        Content = result,
                    };
                }
            }
        }
    }
}
