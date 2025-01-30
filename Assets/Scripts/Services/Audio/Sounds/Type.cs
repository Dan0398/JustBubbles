namespace Services.Audio.Sounds
{
    [System.Serializable]
    public enum SoundType
    {
        None = 0,
        ButtonClick = 1,
        ButtonHover = 2,
        Victory = 3,
        InstrumentPicked = 4,
        InstrumentFail = 5,
        Record = 6,
        InfoShow = 7,
        Counter = 8,
        Unwrap = 9,
        
        BubbleSet = 21,
        BubblePop = 22,
        BubbleShoot = 23,
        CircleBubbleSwitch = 24,
        BubbleWallHit = 25,
        
        BombIdle = 30,
        BombFly = 31,
        BombExplode = 33,
        
        SniperTake = 40,
        SniperShoot = 41,
        
        LaserIdle = 50,
        LaserUse = 51,
        
        Merge = 60,
        Merge_Saved = 61,
        Merge_Shaker = 62,
        Merge_Drop = 63,
        
        Settings_SoundsTest = 100,
        Revive_TickTack = 101,
    }
}