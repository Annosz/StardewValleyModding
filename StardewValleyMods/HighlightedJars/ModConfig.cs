namespace HighlightedJars
{
    public enum HighlightType
    {
        Highlight,
        Bubble,
    }

    public class ModConfig
    {
        public HighlightType HighlightType { get; set; } = HighlightType.Highlight;

        public bool HighlightJars { get; set; } = true;
        public bool HighlightKegs { get; set; } = true;
        public bool HighlightCasks { get; set; } = true;
    }
}
