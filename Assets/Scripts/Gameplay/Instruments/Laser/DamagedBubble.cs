namespace Gameplay.Instruments
{
    public partial class Laser : BaseInstrument
    {
        [System.Serializable]
        public class DamagedBubble
        {
            public Field.Place fieldPlace;
            public int Health;
            
            public DamagedBubble(Field.Place target, int health)
            {
                fieldPlace = target;
                Health = health;
            }
            
            public bool IsDamageCauseDeath()
            {
                Health --;
                return Health <= 0;
            }
        }
    }
}