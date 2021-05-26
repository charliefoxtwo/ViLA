namespace ViLA
{
    public class PluginManifest
    {
        public string Entrypoint { get; set; } = null!;
        public string? Version { get; set; }
        public string? Releases { get; set; }
    }
}