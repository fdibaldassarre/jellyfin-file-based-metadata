using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;


namespace Jellyfin.Plugin.FileBasedMetadata
{
    public class FileBasedMetadataSeriesProvider : IRemoteMetadataProvider<Series, SeriesInfo>
    {

        public string Name => "FileBasedMetadata";

        private readonly ILogger<FileBasedMetadataSeriesProvider> _logger;

        public FileBasedMetadataSeriesProvider(
            ILogger<FileBasedMetadataSeriesProvider> logger
        )
        {
            _logger = logger;
        }

        public async Task<MetadataResult<Series>> GetMetadata(SeriesInfo info, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieve metadata {Name}:{Path}", info.Name, info.Path);
            ShowMetadata item = await getMetadata(info.Path);
            if(item == null) {
                return new MetadataResult<Series>();
            }
            Series series = new Series();
            series.Name = item.Name;
            series.OriginalTitle = item.OriginalTitle;
            series.Overview = item.Description;
            if(!string.IsNullOrEmpty(item.PremiereDate)) {
                series.PremiereDate = DateTime.ParseExact(item.PremiereDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            if(!string.IsNullOrEmpty(item.EndDate)) {
                series.EndDate = DateTime.ParseExact(item.EndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            series.Genres = item.Tags;
            if(!string.IsNullOrEmpty(item.Studio)) {
                series.AddStudio(item.Studio);
            }

            var result = new MetadataResult<Series>();
            result.HasMetadata = true;
            result.Item = series;
            return result;
        }

        private async Task<ShowMetadata> getMetadata(string folderPath) {
            string metadataFile = Path.Combine(folderPath, ".metadata.jellyfin.json");
            if(!File.Exists(metadataFile)) {
                return null;
            }
            ShowMetadata result = await JsonFileReader.ReadAsync<ShowMetadata>(metadataFile);
            if(string.IsNullOrEmpty(result.ImagePath)) {
                string coverFile = Path.Combine(folderPath, ".cover.png");
                _logger.LogInformation("Set cover for {Name}: {Url}", result.Name, coverFile);
                if(File.Exists(coverFile)) {
                    result.ImagePath = coverFile;
                }
            }
            return result;
        }

        public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(SeriesInfo searchInfo, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieve metadata {Name}:{Path}", searchInfo.Name, searchInfo.Path);
            string path = searchInfo.Path;
            if(string.IsNullOrEmpty(path)) {
                // Workaround: use name instead of path
                _logger.LogWarning("Using name {Name} instead of path", searchInfo.Name);
                path = searchInfo.Name;
            }
            ShowMetadata item = await getMetadata(path);
            List<RemoteSearchResult> result = new List<RemoteSearchResult>();
            if(item != null) {
                RemoteSearchResult searchResult = new RemoteSearchResult();
                searchResult.Name = item.Name;
                searchResult.ImageUrl = item.ImagePath;
                searchResult.SearchProviderName = Name;
                result.Add(searchResult);
            }
            return result;
        }

        public Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get Image: {Path}", url);
            return Task.FromResult(LocalFileProvider.ReadLocalFile(url));
        }
    }

}