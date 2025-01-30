namespace Services.Bundles
{
    public partial class Request
    {
        [System.Serializable]
        public enum Priority
        {
            Highest_InterruptWork,
            High,
            Mid,
            Low,
            None
        }
    }
}