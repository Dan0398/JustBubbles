using System;

namespace Content.Merge.Selector
{
    public class Request
    {
        public readonly int slotID;
        public readonly SizeSelector Size;
        public readonly ThemeSelector Theme;
        public readonly System.Action onDone;
        
        public Gameplay.Merge.Barrier.SizeType selectedOrientation  { get; private set; }
        public bool ShowAds                                         { get; private set; }
        public string selectedTheme                                 { get; private set; }
        
        public Request(int SlotID, SizesList OrientationCfg, ThemesList ThemeCfg, System.Action OnDone)
        {
            slotID = SlotID;
            onDone = OnDone;
            Size = new SizeSelector(OrientationCfg, (s) => selectedOrientation = s);
            Theme = new ThemeSelector(ThemeCfg, (name, useAds) => 
            {
                selectedTheme = name;
                ShowAds = useAds;
            });
        }

        internal void Dispose()
        {
            
        }
    }
}