using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Field
{
    public partial class BubbleField: MonoBehaviour, IField
    {
        public void ProcessExplosion(Vector3 ExplodePos, int ExplodeRange)
        {
            if (ExplodeRange < 1) return;
            var Center = PosToPlace(ExplodePos);
            Place[] PlacesUnderAttack = new Place[RequireToGetAllNeighborPoses(ExplodeRange)+1];
            GetNeighborPlaces(Center, ExplodeRange, ref PlacesUnderAttack);
            PlacesUnderAttack[^1] = Center;
            List<Place> ValidPlaces = new(PlacesUnderAttack.Length / 4 * 3);
            for (int i = 0; i < PlacesUnderAttack.Length; i++)
            {
                ValidatePlace(ref PlacesUnderAttack[i], true);
                if (PlacesUnderAttack[i].Valid && PlacesUnderAttack[i].Busy) ValidPlaces.Add(PlacesUnderAttack[i]);
            }
            if (ValidPlaces.Count == 0) return;
            List<Bubble> Exploded = CollectBubbles(ValidPlaces, true);
            _effects.HideBubbles(Exploded);
            SeekNonConnectedToRoot();
            if (_nonRootChank.Count > 0)
            {
                _effects.AnimateFallUnconnectedBubbles(CollectBubbles(_nonRootChank));
            }
            
            _reactOnBubbleSet.Invoke(ValidPlaces, _nonRootChank, typeof(Instruments.Bomb.Bomb));
            CleanEmptyLines();
            OnFieldRefreshed?.Invoke();
        }
    }
}