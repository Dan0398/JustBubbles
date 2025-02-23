using UnityEngine;
using Utils.Observables;

namespace Gameplay.Merge
{
    [System.Serializable]
    public class SaveModel
    {
        public ObsInt Points, Money;
        public SizeType FieldSize;
        public string BundlePath;
        public UnitStatus[] Units;
        
        public SaveModel(SizeType fieldSize, string bundlePath)
        {
            FieldSize = fieldSize;
            BundlePath = bundlePath;
            Points = 0;
            Money = 0;
            Units = new UnitStatus[0];
        }
        
        [System.Serializable]
        public struct UnitStatus
        {
            public int ID;
            public SerVector2 Pos, Vel;
            public int Ang, AngVel; 
        }
        
        [System.Serializable]
        public struct SerVector2
        {
            public float x, y;
            
            public SerVector2(float X, float Y)
            {
                x = (float)System.Math.Round(X, 3);
                y = (float)System.Math.Round(Y, 3);
            }
            
            public SerVector2(Vector2 vec) : this(vec.x, vec.y){}
            
            public Vector2 ToVector() => new Vector2(x, y);
        }
    }
}