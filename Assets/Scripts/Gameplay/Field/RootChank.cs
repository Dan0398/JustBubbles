using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Field
{
    public partial class BubbleField: MonoBehaviour, IField
    {
        private List<Place> _rootChank, _nonRootChank;
        
        private void SeekNonConnectedToRoot(List<Place> SameColor = null)
        {
            PrepareLists();
            ProcessRootChank();
            GetNonRootChankActivePlaces();
            
            void PrepareLists()
            {
                if (_neighborPlaces == null)
                {
                    _neighborPlaces = new Place[6];
                    _helpPlaces = new Place[6];
                    _shiftFXPlaces = new Place[RequireToGetAllNeighborPoses(OutlineFXShiftOnAppend)];
                }
                if (_rootChank == null)
                {
                    _rootChank = new List<Place>(100);
                }
                else 
                {
                    _rootChank.Clear();
                }
                if (_nonRootChank == null)
                {
                    _nonRootChank = new List<Place>(50);
                }
                else
                {
                    _nonRootChank.Clear();
                }
            }
            
            void ProcessRootChank()
            {
                SetupRootChunk();
                GrowRootChunk();
                
                void SetupRootChunk()
                {
                    for (int i = 0; i < BubblesCountPerLine;i++)
                    {
                        if (_lines[0][i] == null) continue;
                        if (InIgnoreList(0, i)) continue;
                        _rootChank.Add(new Place(0,i));
                    }
                }
                
                void GrowRootChunk()
                {
                    if (_rootChank.Count == 0) return;
                    bool FoundNew = true;
                    for(int i = 0; i < _rootChank.Count; i++)
                    {
                        var Count = GetNeighborPlaces(_rootChank [i], 1, ref _neighborPlaces);
                        for (int k = 0; k < Count; k ++)
                        {
                            ValidatePlace(ref _neighborPlaces[k]);
                            if (!_neighborPlaces[k].Valid) continue;
                            if (!_neighborPlaces[k].Busy) continue;
                            if (PlaceInIgnoreList(_neighborPlaces[k])) continue;
                            FoundNew = true;
                            for (int l = 0; l< _rootChank.Count; l++)
                            {
                                if (_rootChank[l].Line == _neighborPlaces[k].Line && _rootChank[l].Column == _neighborPlaces[k].Column)
                                {
                                    FoundNew = false;
                                    break;
                                }
                            }
                            if (FoundNew)
                            {
                                _rootChank.Add(_neighborPlaces[k]);
                            }
                        }
                    }
                    
                    bool PlaceInIgnoreList(Place Target) => InIgnoreList(Target.Line, Target.Column); 
                }
            }
            
            void GetNonRootChankActivePlaces()
            {
                bool NonRoot = true;
                for (int Line = 1; Line < _lines.Count; Line++)
                {
                    for (int Place = 0; Place < BubblesCountPerLine; Place++)
                    {
                        if (_lines[Line][Place] == null) continue;
                        if (InIgnoreList(Line, Place)) continue;
                        NonRoot = true;
                        foreach(var RootPlace in _rootChank)
                        {
                            if (RootPlace.Line == Line && RootPlace.Column == Place)
                            {
                                NonRoot = false;
                                break;
                            }
                        }
                        if (NonRoot)
                        {
                            _nonRootChank.Add(new Place(Line, Place));
                        }
                    }
                }
            }
            
            bool InIgnoreList(int Line, int PlaceID)
            {
                if (SameColor == null) return false;
                foreach(var place in SameColor)
                {
                    if (place.Line == Line && place.Column == PlaceID)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}