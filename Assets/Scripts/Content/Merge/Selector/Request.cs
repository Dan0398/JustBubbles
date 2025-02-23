namespace Content.Merge.Selector
{
    public class Request
    {
        public readonly int SlotID;
        public readonly SizeSelector Size;
        public readonly ThemeSelector Theme;
        public readonly System.Action OnDone;
        
        public Gameplay.Merge.SizeType SelectedOrientation  { get; private set; }
        public bool ShowAds                                 { get; private set; }
        public string SelectedTheme                         { get; private set; }
        
        public Request(int slotID, SizesList orientationCfg, ThemesList themeCfg, System.Action onDone)
        {
            SlotID = slotID;
            OnDone = onDone;
            Size = new SizeSelector(orientationCfg, (s) => SelectedOrientation = s);
            Theme = new ThemeSelector(themeCfg, (name, useAds) => 
            {
                SelectedTheme = name;
                ShowAds = useAds;
            });
        }
    }
}