namespace Gameplay.Field
{
    [System.Serializable]
    public struct Place
    {
        public bool Valid, Busy;
        public int Line, Column;
        
        public Place(int Line, int Place)
        {
            Valid = true;
            Busy = false;
            this.Line = Line;
            this.Column = Place;
        }
    }
}