using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Field
{
    public partial class BubbleField: MonoBehaviour, IField
    {
        
        List<Place> RootChank, NonRootChank;
        
        void SeekNonConnectedToRoot(List<Place> SameColor = null)
        {
            PrepareLists();
            ProcessRootChank();
            GetNonRootChankActivePlaces();
            
            void PrepareLists()
            {
                if (NeighborPlaces == null)
                {
                    NeighborPlaces = new Place[6];
                    HelpPlaces = new Place[6];
                    ShiftFXPlaces = new Place[RequireToGetAllNeighborPoses(OutlineFXShiftOnAppend)];
                }
                if (RootChank == null)
                {
                    RootChank = new List<Place>(100);
                }
                else 
                {
                    RootChank.Clear();
                }
                if (NonRootChank == null)
                {
                    NonRootChank = new List<Place>(50);
                }
                else
                {
                    NonRootChank.Clear();
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
                        if (Lines[0][i] == null) continue;
                        if (InIgnoreList(0, i)) continue;
                        RootChank.Add(new Place(0,i));
                    }
                }
                
                void GrowRootChunk()
                {
                    if (RootChank.Count == 0) return;
                    bool FoundNew = true;
                    for(int i = 0; i < RootChank.Count; i++)
                    {
                        var Count = GetNeighborPlaces(RootChank [i], 1, ref NeighborPlaces);
                        for (int k = 0; k < Count; k ++)
                        {
                            ValidatePlace(ref NeighborPlaces[k]);
                            if (!NeighborPlaces[k].Valid) continue;
                            if (!NeighborPlaces[k].Busy) continue;
                            if (PlaceInIgnoreList(NeighborPlaces[k])) continue;
                            FoundNew = true;
                            for (int l = 0; l< RootChank.Count; l++)
                            {
                                if (RootChank[l].Line == NeighborPlaces[k].Line && RootChank[l].Column == NeighborPlaces[k].Column)
                                {
                                    FoundNew = false;
                                    break;
                                }
                            }
                            if (FoundNew)
                            {
                                RootChank.Add(NeighborPlaces[k]);
                            }
                        }
                    }
                    
                    bool PlaceInIgnoreList(Place Target) => InIgnoreList(Target.Line, Target.Column); 
                }
            }
            
            void GetNonRootChankActivePlaces()
            {
                bool NonRoot = true;
                for (int Line = 1; Line < Lines.Count; Line++)
                {
                    for (int Place = 0; Place < BubblesCountPerLine; Place++)
                    {
                        if (Lines[Line][Place] == null) continue;
                        if (InIgnoreList(Line, Place)) continue;
                        NonRoot = true;
                        foreach(var RootPlace in RootChank)
                        {
                            if (RootPlace.Line == Line && RootPlace.Column == Place)
                            {
                                NonRoot = false;
                                break;
                            }
                        }
                        if (NonRoot)
                        {
                            NonRootChank.Add(new Place(Line, Place));
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