namespace Services.Bundles
{
    public partial class Request
    {
        [System.Serializable]
        public enum LoadedStatus
        {
            None,
            NotLoaded,
            Success,
            LittleFail,
            CriticalFailture
        }
    }
}