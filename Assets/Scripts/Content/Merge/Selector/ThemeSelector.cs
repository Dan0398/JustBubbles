namespace Content.Merge.Selector
{
    public class ThemeSelector
    {
        ThemesList config;
        System.Action<string, bool> onSelect;
        int selected;
        
        public ThemeSelector(ThemesList cfg, System.Action<string, bool> OnSelect)
        {
            selected = 0;
            config = cfg;
            onSelect = OnSelect;
        }
        
        public ThemesList.Theme Selected => config.Themes[selected];
        
        public void GoToNext()
        {
            selected ++;
            if (selected == config.Themes.Length) selected = 0;
        }
        
        public void GoToPrev()
        {
            selected --;
            if (selected < 0) selected = config.Themes.Length - 1;
        }
        
        public void Select() => onSelect.Invoke(Selected.BundlePath, Selected.AdsRequired);
    }
}