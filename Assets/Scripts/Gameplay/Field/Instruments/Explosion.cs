using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Field
{
    public partial class BubbleField: MonoBehaviour, IField
    {
        internal void ProcessExplosion(Vector3 ExplodePos, int ExplodeRange)
        {
            if (ExplodeRange < 1) return;
            var Center = PosToPlace(ExplodePos);
            Place[] PlacesUnderAttack = new Place[RequireToGetAllNeighborPoses(ExplodeRange)+1];
            GetNeighborPlaces(Center, ExplodeRange, ref PlacesUnderAttack);
            PlacesUnderAttack[PlacesUnderAttack.Length-1] = Center;
            List<Place> ValidPlaces = new(PlacesUnderAttack.Length / 4 * 3);
            for (int i = 0; i < PlacesUnderAttack.Length; i++)
            {
                ValidatePlace(ref PlacesUnderAttack[i], true);
                if (PlacesUnderAttack[i].Valid && PlacesUnderAttack[i].Busy) ValidPlaces.Add(PlacesUnderAttack[i]);
            }
            if (ValidPlaces.Count == 0) return;
            List<Bubble> Exploded = CollectBubbles(ValidPlaces, true);
            /*
            float RangeFixed = ExplodeRange * ExplodeRange;
            List<Bubble> Exploded = new (8);
            for (int Line = 0; Line < Lines.Count; Line++)
            {
                for (int Pl = 0; Pl < BubblesCountPerLine; Pl++)
                {
                    if (Lines[Line][Pl] == null) continue;
                    if ((ExplodePos-PlaceToPos(Line, Pl)).sqrMagnitude > RangeFixed) continue;
                    Exploded.Add(Lines[Line][Pl]);
                    Lines[Line][Pl] = null;
                }
            }
            if (Exploded.Count == 0) return;
            */
            Effects.HideBubbles(Exploded);
            SeekNonConnectedToRoot();
            if (NonRootChank.Count > 0)
            {
                Effects.AnimateFallUnconnectedBubbles(CollectBubbles(NonRootChank));
            }
            
            ReactOnBubbleSet.Invoke(ValidPlaces, NonRootChank, typeof(Instruments.Bomb.Bomb));
            CleanEmptyLines();
            OnFieldRefreshed?.Invoke();
        }
        
    }
}