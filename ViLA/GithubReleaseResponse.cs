using Newtonsoft.Json;

namespace ViLA
{
    public class GithubReleaseResponse
    {
        [JsonProperty("tag_name")]
        public string TagName { get; set; }

        public string Name { get; set; }

        public bool Draft { get; set; }

        public bool Prerelease { get; set; }

        [JsonProperty("html_url")]
        public string HtmlUrl { get; set; }
    }
}