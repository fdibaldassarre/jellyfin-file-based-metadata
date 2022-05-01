using System;

namespace Jellyfin.Plugin.FileBasedMetadata
{
    public class ShowMetadata
    {
        public ShowMetadata() {
            Name = "Unknown series";
            Tags = new string[]{};
        }
        public string Name {get; set;}
        public string? OriginalTitle {get; set;}
        public string? Description {get; set;}
        // Premiere date in ISO8601
        public string? PremiereDate { get; set; }
        // End date in ISO8601
        public string? EndDate { get; set; }

        public string? Studio {get; set;}
        public string[] Tags {get; set;}

        public string? ImagePath {get; set;}

    }
}