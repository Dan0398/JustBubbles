namespace Data
{
    [System.Serializable]
    public class Merge : IAbstractData
    {
        public const int SlotsCount = 4;
        
        public bool Tried;
        public Gameplay.Merge.SaveModel[] SaveSlots;
        
        public void SetValuesAsFromStart()
        {
            SaveSlots = new Gameplay.Merge.SaveModel[SlotsCount];
        }
    }
}