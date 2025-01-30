namespace Content.Merge.Selector
{
    public class SizeSelector
    {
        SizesList config;
        System.Action<Gameplay.Merge.Barrier.SizeType> onSelect;
        int selected;
        
        public SizesList.Size Selected => config.Orientations[selected]; 
        
        public SizeSelector(SizesList cfg, System.Action<Gameplay.Merge.Barrier.SizeType> OnSelect)
        {
            selected = 0;
            config = cfg;
            onSelect = OnSelect;
        }
        
        public void GoToNext()
        {
            selected ++;
            if (selected == config.Orientations.Length) selected = 0;
        }
        
        public void GoToPrev()
        {
            selected --;
            if (selected < 0) selected = config.Orientations.Length - 1;
        }
        
        public void Select() => onSelect.Invoke(Selected.Data);
    }
}