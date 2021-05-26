using Newtonsoft.Json;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace ViLA
{
    public class GithubReleaseResponse
    {
        [JsonProperty("tag_name")]
        public string TagName { get; set; } = null!;

        public string Name { get; set; } = null!;

        public bool Draft { get; set; }

        public bool Prerelease { get; set; }

        [JsonProperty("html_url")]
        public string HtmlUrl { get; set; } = null!;
    }
}