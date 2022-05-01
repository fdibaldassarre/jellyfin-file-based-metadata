using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Globalization;

namespace Jellyfin.Plugin.FileBasedMetadata
{
    public static class JsonFileReader
    {
        public static async Task<T> ReadAsync<T>(string filePath)
        {
            using FileStream stream = File.OpenRead(filePath);
            return await JsonSerializer.DeserializeAsync<T>(stream).ConfigureAwait(false);
        }
    }

    public static class LocalFileProvider
    {
        public static HttpResponseMessage ReadLocalFile(string url) {
            if(string.IsNullOrEmpty(url) || !File.Exists(url)) {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            long length = new FileInfo(url).Length;
            HttpContent content = new StreamContent(File.OpenRead(url));
            content.Headers.Add("Content-Length", length.ToString(CultureInfo.InvariantCulture));
            content.Headers.Add("Content-Type", "image/png");

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = content;
            return response;
        }
    }
}