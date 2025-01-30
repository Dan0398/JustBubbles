using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BubbleColor = Gameplay.Bubble.BubbleColor;

namespace Gameplay
{
    public static class ColorPicker
    {
        public static Color GetColorByEnum(BubbleColor RequiredColor)
        {
                 if (RequiredColor == BubbleColor.Red)      return Color.red;
            else if (RequiredColor == BubbleColor.Green)    return Color.green;
            else if (RequiredColor == BubbleColor.Yellow)   return Color.yellow;
            else if (RequiredColor == BubbleColor.Purple)   return new Color32(193,  0,255,255);
            else if (RequiredColor == BubbleColor.Cyan)     return Color.cyan;
            else if (RequiredColor == BubbleColor.Orange)   return new Color32(255,128,  0,255);
            else return Color.black;
        }
        
        public static BubbleColor GetRandomColor(int MaxID = 6, float RandomizeFactor = 1)
        {
            if (RandomizeFactor < 1)
            {
                if (Random.Range(0.0f, 1.0f) >= RandomizeFactor)
                {
                    return OldColor;
                }
            }
            return (BubbleColor) Random.Range(0, MaxID);
            /*
            BubbleColor newColor = OldColor;
            while (newColor == OldColor)
            {
                newColor = (BubbleColor) Random.Range(0, MaxID);
            }
            OldColor = newColor;
            return OldColor;
            */
        }
        
        public static BubbleColor GetRandomColor(List<BubbleColor> Available, float RandomizeFactor = 1)
        {
            if (Available.Count == 1) return Available[0];
            if (Available.Count < 1)
            {
                throw new System.Exception($"Available colors count is {Available.Count}. Require more");
            }
            if (Available.Contains(OldColor) && RandomizeFactor<1)
            {
                if (Random.Range(0.0f, 1.0f) >= RandomizeFactor)
                {
                    return OldColor;
                }
            }
            //return Available[Random.Range(0, Available.Count)];// (BubbleColor) Random.Range(0, MaxID);
            BubbleColor newColor = OldColor;
            for (int i = 0; i < 2; i++)
            {
                if (newColor != OldColor) break;
                if (newColor == OldColor || !Available.Contains(newColor))
                {
                    newColor = Available[Random.Range(0, Available.Count)];// (BubbleColor) Random.Range(0, MaxID);
                }
            }
            OldColor = newColor;
            return OldColor;
        }
        
        static BubbleColor OldColor = BubbleColor.Red;
        
        public static List<BubbleColor> GiveColorsByCount(int Count)
        {
            List<BubbleColor> Result = new(Count);
            var Values = System.Enum.GetValues(typeof(BubbleColor));
            for(int i = 0; i < Count; i ++)
            {
                Result.Add((BubbleColor)Values.GetValue(i));
            }
            return Result;
        }
    }
}
