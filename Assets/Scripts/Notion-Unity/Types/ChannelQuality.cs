namespace Notion.Unity
{
    public class ChannelQuality
    {
        public float StandardDeviation { get; set; }
        public Status Status { get; set; }
    }

    public enum Status
    {
        Great,
        Good,
        Bad,
        NoContact
    }
}