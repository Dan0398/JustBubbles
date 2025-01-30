#if UNITY_WEBGL
namespace Data
{
    public class Complex
    {
        public bool IsReady         { get; set; }
        public User User            { get; set; }
        public Settings Settings    { get; set; }
        public Merge Merge          { get; set; }
        
        public Complex()
        {
            IsReady = false;
            User = null;
            Settings = null;
            Merge = null;
        }
    }
}
#endif