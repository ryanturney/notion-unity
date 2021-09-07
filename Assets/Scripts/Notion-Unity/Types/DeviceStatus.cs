namespace Notion.Unity
{
    public class DeviceStatus
    {
        public float Battery { get; set; }
        public bool Charging { get; set; }
        public bool SleepMode { get; set; }
        public SleepModeReason SleepModeReason { get; set; }
        public string SSID { get; set; }
        public State State { get; set; }
        public float UpdatingProgress { get; set; }
    }

    public enum SleepModeReason
    {
        Null,
        Charging,
        Updating
    }

    public enum State
    {
        Online,
        Offline,
        Updating,
        Booting,
        ShuttingOff
    }
}