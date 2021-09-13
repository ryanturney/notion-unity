namespace Notion.Unity
{
    public class DeviceInfo
    {
        public string ApiVersion { get; set; }
        public string[] ChannelNames { get; set; }
        public int Channels { get; set; }
        public string DeviceId { get; set; }
        public string DeviceNickname { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string ModelName { get; set; }
        public int ModelVersion { get; set; }
        public string OsVersion { get; set; }
        public int SamplingRate { get; set; }
    }
}