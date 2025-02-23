namespace Content.Merge.Selector
{
    public class ThemeSelector
    {
        private ThemesList _config;
        private System.Action<string, bool> _onSelect;
        private int _selected;
        
        public ThemeSelector(ThemesList cfg, System.Action<string, bool> OnSelect)
        {
            _selected = 0;
            _config = cfg;
            _onSelect = OnSelect;
        }
        
        public ThemesList.Theme Selected => _config.Themes[_selected];
        
        public void GoToNext()
        {
            _selected ++;
            if (_selected == _config.Themes.Length) _selected = 0;
        }
        
        public void GoToPrev()
        {
            _selected --;
            if (_selected < 0) _selected = _config.Themes.Length - 1;
        }
        
        public void Select() => _onSelect.Invoke(Selected.BundlePath, Selected.AdsRequired);
    }
}