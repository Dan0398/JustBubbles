using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Field
{
    public partial class BubbleField: MonoBehaviour, IField
    {
        const int OutlineFXShiftOnAppend = 2;
        
        private Place[] _neighborPlaces, _helpPlaces, _shiftFXPlaces;
        private Place _center;
        private List<Place> _sameColor;
        
        public void PlaceUserBubble(Gameplay.User.ICircleObject NewBubble, Gameplay.User.Trajectory trajectory, bool isMulticolor)
        {
            Vector3 Pos = trajectory.PosOnWay;
            int StepBacks = 0;
            Gameplay.Bubble usualBubble = isMulticolor? null : (Gameplay.Bubble)NewBubble;
            
            CheckAndClearArrays();
            GetPosAndPlace();
            if (NewBubble == null) return;
            if (!_center.Valid)
            {
                _reactOnBubbleSet.Invoke(_sameColor, _nonRootChank, typeof(Instruments.Bubble.Circle));
                if (isMulticolor)
                {
                }
                else
                {
                    _pool.Hide(usualBubble);
                    ColorStats.DecountByBubble(usualBubble);
                }
                return;
            }
            if (!isMulticolor) ColorStats.IncrementByBubble(usualBubble);
            CollectSameColored();
            if ((!isMulticolor && _sameColor.Count < 3) || (isMulticolor && _sameColor.Count < 2))
            {
                _reactOnBubbleSet.Invoke(_sameColor, _nonRootChank, typeof(Instruments.Bubble.Circle));
                PrepareAndShiftBubblesFX();
                OnFieldRefreshed?.Invoke();
                return;
            }
            
            SeekNonConnectedToRoot(_sameColor);
            
            if(isMulticolor) _effects.PlayPopEffectAt(NewBubble.MyTransform.position);
            _effects.PopBubbles(CollectBubbles(_sameColor));
            if (_nonRootChank.Count > 0)
            {
                _effects.AnimateFallUnconnectedBubbles(CollectBubbles(_nonRootChank));
            }
            _reactOnBubbleSet.Invoke(_sameColor, _nonRootChank, typeof(Instruments.Bubble.Circle));
            CleanEmptyLines();
            OnFieldRefreshed?.Invoke();
            
            void CheckAndClearArrays()
            {
                _neighborPlaces ??= new Place[6];
                _helpPlaces ??= new Place[6];
                _shiftFXPlaces ??= new Place[RequireToGetAllNeighborPoses(OutlineFXShiftOnAppend)];
                _nonRootChank ??= new List<Place>(20);
                _nonRootChank.Clear();
                _sameColor ??= new List<Place>(20);
                _sameColor.Clear();
            }
            
            void GetPosAndPlace()
            {
                _center.Line = GetLineNumberForPos(Pos);
                if (_center.Line < 0)
                {
                    _center.Valid = false;
                    return;
                }
                while (_center.Line >= _lines.Count) CreateLine();
                _center.Column = GetPlaceNumberForPos(Pos, _lines[_center.Line].Shifted);
                ValidatePlace(ref _center, true);
                if (_center.Valid && !_center.Busy && NeighborsExists(ref _center))
                {
                    //Debug.Log("Catched!!");
                }
                else 
                {
                    float Dist = float.MaxValue;
                    var Count = GetNeighborPlaces(_center, 1, ref _neighborPlaces);
                    for(int i = 0; i < Count; i++)
                    {
                        ValidatePlace(ref _neighborPlaces[i], true);
                        if (!_neighborPlaces[i].Valid) continue;
                        if (_neighborPlaces[i].Busy) continue;
                        if (NeighborsExists(ref _neighborPlaces[i]))
                        {
                            var NewDist = Vector3.Distance(Pos, PlaceToPos(_neighborPlaces[i]));
                            if (NewDist < Dist)
                            {
                                Dist = NewDist;
                                _center.Line = _neighborPlaces[i].Line;
                                _center.Column = _neighborPlaces[i].Column;
                                _center.Valid = true;
                            }
                        }
                    }
                }
                while (_center.Line >= _lines.Count) CreateLine();
                ValidatePlace(ref _center, true);
                if (!_center.Valid || _center.Busy)
                {
                    if (StepBacks == 20)
                    {
                        _effects.AnimateFallUnconnectedBubbles(new List<Bubble>{usualBubble});
                        NewBubble = null;
                        return;
                    }
                    trajectory.MakeStep();
                    Pos = trajectory.PosOnWay;
                    GetPosAndPlace();
                    StepBacks++;
                    return;
                }
                if (!isMulticolor) _lines[_center.Line][_center.Column] = usualBubble;
            }
            
            void CreateLine()
            {
                var Line = new LineOfBubbles(transform, !_lines[^1].Shifted, BubblesCountPerLine); 
                var Point = new Vector3(_startPoint.x, _lines[0].OnScene.position.y);
                Point += _lines.Count * _lineHeight * Vector3.down;
                if (Line.Shifted)
                {
                    Point += Vector3.right * _shiftWidth;
                }
                Line.OnScene.position = Point;
                _lines.Add(Line);
            }
            
            bool NeighborsExists(ref Place Center)
            {
                var Count = GetNeighborPlaces(Center, 1, ref _helpPlaces);
                for(int i = 0; i < Count; i++)
                {
                    ValidatePlace(ref _helpPlaces[i]);
                    if (_helpPlaces[i].Valid && _helpPlaces[i].Busy)
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
                    List<Place> coloredPlaces = new(2) { _center };
                    for (int i = 0; i < coloredPlaces.Count; i++)
                    {
                        var Count = GetNeighborPlaces(coloredPlaces[i], 1, ref _neighborPlaces);
                        for (int k = 0; k < Count; k++)
                        {
                            ValidatePlace(ref _neighborPlaces[k]);
                            if (!_neighborPlaces[k].Valid) continue;
                            if (!_neighborPlaces[k].Busy) continue;
                            if (_lines[_neighborPlaces[k].Line][_neighborPlaces[k].Column].MyColor != Color) continue;
                            bool FoundNew = true;
                            foreach (var Pos in coloredPlaces)
                            {
                                if (Pos.Line == _neighborPlaces[k].Line && Pos.Column == _neighborPlaces[k].Column)
                                {
                                    FoundNew = false;
                                    break;
                                }
                            }
                            if (FoundNew) 
                            {
                                coloredPlaces.Add(_neighborPlaces[k]);
                            }
                        }
                    }
                    coloredPlaces.Remove(_center);
                    if (coloredPlaces.Count >= 2)
                    {
                        _sameColor.AddRange(coloredPlaces);
                    }
                }
                if (!isMulticolor)
                {
                    _sameColor.Add(_center);
                }
            }
            
            void PrepareAndShiftBubblesFX()
            {
                const int OutlineShift = 2;
                
                var Count = GetNeighborPlaces(_center, OutlineShift, ref _shiftFXPlaces);
                List<Place> ForShift = new List<Place>(Count);
                for (int i = 0; i < Count; i ++)
                {
                    ValidatePlace(ref _shiftFXPlaces[i]);
                    if (!_shiftFXPlaces[i].Valid) continue;
                    if (!_shiftFXPlaces[i].Busy) continue;
                    ForShift.Add(_shiftFXPlaces[i]);
                }
                if (!isMulticolor)
                {
                    _effects.ShiftBubblesAfterContact(usualBubble, CollectBubbles(ForShift, false), trajectory.LastDirOnWay, Pos, OutlineShift * BubbleSize);
                }
            }
        }
        
        
        private void CleanEmptyLines()
        {
            for (int i = _lines.Count - 1; i > 0 ; i--)
            {
                if (_lines[i].RequireToClean())
                {
                    var Cleared = _lines[i].CleanLineAndGetCount(_pool);
                    ColorStats.DecountByBubble(Cleared);
                    _lines.RemoveAt(i);
                }
            }
        }
        
        private List<Bubble> CollectBubbles(List<Place> Places, bool RequireClean = true)
        {
            var Result = new List<Bubble>(Places.Count);
            for (int i=0; i< Places.Count; i++)
            {
                Result.Add(CollectBubble(Places[i], RequireClean));
            }
            return Result;
        }
        
        public Bubble CollectBubble(Place place, bool RequireClean = true)
        {
            var Bubble = _lines[place.Line][place.Column] ?? throw new System.NullReferenceException($"Trying to collect null bubble at {place.Line}x{place.Column}");
            if (RequireClean)
            {
                _lines[place.Line][place.Column] = null;
                ColorStats.DecountByBubble(Bubble);
            }
            return Bubble;
        }
        
        private void ValidatePlace(ref Place Target, bool IgnoreEmptyLines = false)
        {
            Target.Valid = Target.Line >= 0 && Target.Column >= 0 && Target.Column < BubblesCountPerLine;
            if (!IgnoreEmptyLines)
            {
                Target.Valid = Target.Valid && Target.Line < _lines.Count;
            }
            Target.Busy = false;
            if (Target.Valid && Target.Line < _lines.Count)
            {
                Target.Busy = _lines[Target.Line][Target.Column] != null;
            }
        }
    }
}