namespace Content.Merge.Selector
{
    public class SizeSelector
    {
        private System.Action<Gameplay.Merge.SizeType> _onSelect;
        private SizesList _config;
        private int _selected;
        
        public SizesList.Size Selected => _config.Orientations[_selected]; 
        
        public SizeSelector(SizesList cfg, System.Action<Gameplay.Merge.SizeType> OnSelect)
        {
            _selected = 0;
            _config = cfg;
            _onSelect = OnSelect;
        }
        
        public void GoToNext()
        {
            _selected ++;
            if (_selected == _config.Orientations.Length) _selected = 0;
        }
        
        public void GoToPrev()
        {
            _selected --;
            if (_selected < 0) _selected = _config.Orientations.Length - 1;
        }
        
        public void Select() => _onSelect.Invoke(Selected.Data);
    }
}