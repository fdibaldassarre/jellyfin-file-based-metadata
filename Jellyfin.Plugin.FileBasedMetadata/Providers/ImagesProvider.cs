using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Net.Http;
using System.Net.Http.Headers;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;


namespace Jellyfin.Plugin.FileBasedMetadata
{
    public class FileBasedMetadataImagesProvider : IRemoteImageProvider
    {
        public string Name => "FileBasedMetadata";

        private readonly ILogger<FileBasedMetadataImagesProvider> _logger;

        public FileBasedMetadataImagesProvider(
            ILogger<FileBasedMetadataImagesProvider> logger
        ) {
            _logger = logger;
        }

        public IEnumerable<ImageType> GetSupportedImages(BaseItem item)
        {
            return new[] { ImageType.Primary };
        }

        public bool Supports(BaseItem item)
        {
            return item is Series || item is Season || item is Movie;
        }

        public Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get image from {Url}", url);
            return Task.FromResult(LocalFileProvider.ReadLocalFile(url));
        }

        public Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get images for {Name}: {Path}", item.Name, item.Path);
            List<RemoteImageInfo> result = new List<RemoteImageInfo>();
            string coverFile = Path.Combine(item.Path, ".cover.png");
            if(File.Exists(coverFile)) {
                result.Add(CreateImageInfo(coverFile, ImageType.Primary));
            }
            return Task.FromResult<IEnumerable<RemoteImageInfo>>(result);
        }

        private RemoteImageInfo CreateImageInfo(string url, ImageType type) {
            RemoteImageInfo info = new RemoteImageInfo();
            info.ProviderName = Name;
            info.Url = url;
            info.Type = type;
            return info;
        }

    }
}