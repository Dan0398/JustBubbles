using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Field
{
    public partial class BubbleField: MonoBehaviour, IField
    {
        const int OutlineFXShiftOnAppend = 2;
        
        Place[] NeighborPlaces, HelpPlaces, ShiftFXPlaces;
        Place Center;
        List<Place> SameColor;
        
        public void PlaceUserBubble(Gameplay.User.ICircleObject NewBubble, Gameplay.User.Trajectory trajectory, bool isMulticolor)
        {
            Vector3 Pos = trajectory.PosOnWay;
            int StepBacks = 0;
            Gameplay.Bubble usualBubble = isMulticolor? null : (Gameplay.Bubble)NewBubble;
            
            CheckAndClearArrays();
            GetPosAndPlace();
            if (NewBubble == null) return;
            if (!Center.Valid)
            {
                ReactOnBubbleSet.Invoke(SameColor, NonRootChank, typeof(Instruments.Bubble.Circle));
                if (isMulticolor)
                {
                }
                else
                {
                    Pool.Hide(usualBubble);
                    ColorStats.DecountByBubble(usualBubble);
                }
                return;
            }
            if (!isMulticolor) ColorStats.IncrementByBubble(usualBubble);
            CollectSameColored();
            if ((!isMulticolor && SameColor.Count < 3) || (isMulticolor && SameColor.Count < 2))
            {
                ReactOnBubbleSet.Invoke(SameColor, NonRootChank, typeof(Instruments.Bubble.Circle));
                PrepareAndShiftBubblesFX();
                OnFieldRefreshed?.Invoke();
                return;
            }
            
            SeekNonConnectedToRoot(SameColor);
            
            if(isMulticolor) Effects.PlayPopEffectAt(NewBubble.MyTransform.position);
            Effects.PopBubbles(CollectBubbles(SameColor));
            if (NonRootChank.Count > 0)
            {
                Effects.AnimateFallUnconnectedBubbles(CollectBubbles(NonRootChank));
            }
            ReactOnBubbleSet.Invoke(SameColor, NonRootChank, typeof(Instruments.Bubble.Circle));
            CleanEmptyLines();
            OnFieldRefreshed?.Invoke();
            
            void CheckAndClearArrays()
            {
                NeighborPlaces ??= new Place[6];
                HelpPlaces ??= new Place[6];
                ShiftFXPlaces ??= new Place[RequireToGetAllNeighborPoses(OutlineFXShiftOnAppend)];
                NonRootChank ??= new List<Place>(20);
                NonRootChank.Clear();
                SameColor ??= new List<Place>(20);
                SameColor.Clear();
            }
            
            void GetPosAndPlace()
            {
                Center.Line = GetLineNumberForPos(Pos);
                if (Center.Line < 0)
                {
                    Center.Valid = false;
                    return;
                }
                while (Center.Line >= Lines.Count) CreateLine();
                Center.Column = GetPlaceNumberForPos(Pos, Lines[Center.Line].Shifted);
                ValidatePlace(ref Center, true);
                if (Center.Valid && !Center.Busy && NeighborsExists(ref Center))
                {
                    //Debug.Log("Catched!!");
                }
                else 
                {
                    float Dist = float.MaxValue;
                    var Count = GetNeighborPlaces(Center, 1, ref NeighborPlaces);
                    for(int i = 0; i < Count; i++)
                    {
                        ValidatePlace(ref NeighborPlaces[i], true);
                        if (!NeighborPlaces[i].Valid) continue;
                        if (NeighborPlaces[i].Busy) continue;
                        if (NeighborsExists(ref NeighborPlaces[i]))
                        {
                            var NewDist = Vector3.Distance(Pos, PlaceToPos(NeighborPlaces[i]));
                            if (NewDist < Dist)
                            {
                                Dist = NewDist;
                                Center.Line = NeighborPlaces[i].Line;
                                Center.Column = NeighborPlaces[i].Column;
                                Center.Valid = true;
                            }
                        }
                    }
                }
                while (Center.Line >= Lines.Count) CreateLine();
                ValidatePlace(ref Center, true);
                if (!Center.Valid || Center.Busy)
                {
                    if (StepBacks == 20)
                    {
                        Effects.AnimateFallUnconnectedBubbles(new List<Bubble>{usualBubble});
                        NewBubble = null;
                        return;
                    }
                    trajectory.MakeStep();
                    Pos = trajectory.PosOnWay;
                    GetPosAndPlace();
                    StepBacks++;
                    return;
                }
                if (!isMulticolor) Lines[Center.Line][Center.Column] = usualBubble;
            }
            
            void CreateLine()
            {
                var Line = new LineOfBubbles(transform, !Lines[Lines.Count-1].Shifted, BubblesCountPerLine); 
                var Point = new Vector3(StartPoint.x, Lines[0].OnScene.position.y);
                Point += Vector3.down * LineHeight * (Lines.Count);
                if (Line.Shifted)
                {
                    Point += Vector3.right * ShiftWidth;
                }
                Line.OnScene.position = Point;
                Lines.Add(Line);
            }
            
            bool NeighborsExists(ref Place Center)
            {
                var Count = GetNeighborPlaces(Center, 1, ref HelpPlaces);
                for(int i = 0; i < Count; i++)
                {
                    ValidatePlace(ref HelpPlaces[i]);
                    if (HelpPlaces[i].Valid && HelpPlaces[i].Busy)
                    {
                        return true;
                    }
                }
                return false;
            }
            
            void CollectSameColored()
            {
                Bubble.BubbleColor[] Colors;
                if (!isMulticolor) 
                {
                    Colors = new Bubble.BubbleColor[] { NewBubble.MyColor };
                }
                else
                {
                    var values = System.Enum.GetValues(typeof(Bubble.BubbleColor));
                    Colors = new Bubble.BubbleColor[values.Length];
                    for (int i = 0; i < values.Length; i++)
                    {
                        Colors[i] = (Bubble.BubbleColor) values.GetValue(i);
                    }
                }
                foreach(var Color in Colors)
                {
                    List<Place> coloredPlaces = new(2) { Center };
                    for (int i = 0; i < coloredPlaces.Count; i++)
                    {
                        var Count = GetNeighborPlaces(coloredPlaces[i], 1, ref NeighborPlaces);
                        for (int k = 0; k < Count; k++)
                        {
                            ValidatePlace(ref NeighborPlaces[k]);
                            if (!NeighborPlaces[k].Valid) continue;
                            if (!NeighborPlaces[k].Busy) continue;
                            if (Lines[NeighborPlaces[k].Line][NeighborPlaces[k].Column].MyColor != Color) continue;
                            bool FoundNew = true;
                            foreach (var Pos in coloredPlaces)
                            {
                                if (Pos.Line == NeighborPlaces[k].Line && Pos.Column == NeighborPlaces[k].Column)
                                {
                                    FoundNew = false;
                                    break;
                                }
                            }
                            if (FoundNew) 
                            {
                                coloredPlaces.Add(NeighborPlaces[k]);
                                //SameColor.Add(NeighborPlaces[k]); 
                            }
                        }
                    }
                    coloredPlaces.Remove(Center);
                    if (coloredPlaces.Count >= 2)
                    {
                        SameColor.AddRange(coloredPlaces);
                    }
                }
                if (!isMulticolor)
                {
                    SameColor.Add(Center);
                }
            }
            
            void PrepareAndShiftBubblesFX()
            {
                const int OutlineShift = 2;
                
                var Count = GetNeighborPlaces(Center, OutlineShift, ref ShiftFXPlaces);
                List<Place> ForShift = new List<Place>(Count);
                for (int i = 0; i < Count; i ++)
                {
                    ValidatePlace(ref ShiftFXPlaces[i]);
                    if (!ShiftFXPlaces[i].Valid) continue;
                    if (!ShiftFXPlaces[i].Busy) continue;
                    ForShift.Add(ShiftFXPlaces[i]);
                }
                if (!isMulticolor)
                {
                    Effects.ShiftBubblesAfterContact(usualBubble, CollectBubbles(ForShift, false), trajectory.LastDirOnWay, Pos, OutlineShift * BubbleSize);
                }
            }
        }
        
        
        void CleanEmptyLines()
        {
            for (int i = Lines.Count - 1; i > 0 ; i--)
            {
                if (Lines[i].RequireToClean())
                {
                    var Cleared = Lines[i].CleanLineAndGetCount(Pool);
                    ColorStats.DecountByBubble(Cleared);
                    Lines.RemoveAt(i);
                }
            }
        }
        
        List<Bubble> CollectBubbles(List<Place> Places, bool RequireClean = true)
        {
            List<Bubble> Result = new List<Bubble>(Places.Count);
            for (int i=0; i< Places.Count; i++)
            {
                Result.Add(CollectBubble(Places[i], RequireClean));
            }
            return Result;
        }
        
        public Bubble CollectBubble(Place place, bool RequireClean = true)
        {
            var Bubble = Lines[place.Line][place.Column] ?? throw new System.NullReferenceException($"Trying to collect null bubble at {place.Line}x{place.Column}");
            if (RequireClean)
            {
                Lines[place.Line][place.Column] = null;
                ColorStats.DecountByBubble(Bubble);
            }
            return Bubble;
        }
        
        void ValidatePlace(ref Place Target, bool IgnoreEmptyLines = false)
        {
            Target.Valid = Target.Line >= 0 && Target.Column >= 0 && Target.Column < BubblesCountPerLine;
            if (!IgnoreEmptyLines)
            {
                Target.Valid = Target.Valid && Target.Line < Lines.Count;
            }
            Target.Busy = false;
            if (Target.Valid && Target.Line < Lines.Count)
            {
                Target.Busy = Lines[Target.Line][Target.Column] != null;
            }
        }
    }
}