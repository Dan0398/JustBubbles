using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Field
{
    public partial class BubbleField: MonoBehaviour, IField
    {
        public Place GiveBubbleByDirection(ref Vector3 endpoint, Vector3 direction)
        {
            Place place = PosToPlace(endpoint);
            ValidatePlace(ref place);
            if (place.Valid && place.Busy) return place;
            _neighborPlaces ??= new Place[6];
            GetNeighborPlaces(place, 1, ref _neighborPlaces);
            var Angle = Vector2.SignedAngle(Vector2.up, direction);
            /*
               x -  Центр
               | -  Угол 0 градусов
            +++|--- Мера угла
              4|5
             0 X 1
              2 3
            */
            int id = 0;
            List<int> Indexes = new(1);
            for (int delta = 0; delta <= 90; delta += 30, id++)
            {
                if (Angle > 0)
                {
                         if (Angle < delta                                && !Indexes.Contains(5)) Indexes.Add(5);
                    else if ((Angle < (60  - delta) || (Angle >   -delta))&& !Indexes.Contains(4)) Indexes.Add(4);
                    else if ((Angle < (120 - delta) || (Angle > 60-delta))&& !Indexes.Contains(0)) Indexes.Add(0);
                    else Indexes.Add(2);
                }
                else 
                {
                         if (Angle > - delta      && !Indexes.Contains(4)) Indexes.Add(4);
                    else if ((Angle > (-60 +delta) || Angle < (   -delta)) && !Indexes.Contains(5)) Indexes.Add(5);
                    else if ((Angle > (-120+delta) || Angle < (-60-delta)) && !Indexes.Contains(1)) Indexes.Add(1);
                    else Indexes.Add(3);
                }
                ValidatePlace(ref _neighborPlaces[Indexes[id]]);
                if (_neighborPlaces[Indexes[id]].Valid && _neighborPlaces[Indexes[id]].Busy) 
                {
                    return _neighborPlaces[Indexes[id]];
                }
            }
            id--;
            return _neighborPlaces[Indexes[id]];
        }
        
        public void TryChangeLinesPosInDamaged(ref List<Instruments.Laser.DamagedBubble> damagedBubbles, int oldLinesCount)
        {
            var newLinesCount = _lines.Count;
            for (int i = 0; i < damagedBubbles.Count; i++)
            {
                var newLineNumber = damagedBubbles[i].FieldPlace.Line + newLinesCount - oldLinesCount;
                if (newLineNumber < 0 || newLineNumber > newLinesCount)
                {
                    damagedBubbles.RemoveAt(i);
                    i--;
                    continue;
                }
                damagedBubbles[i].FieldPlace = new Field.Place(newLineNumber, damagedBubbles[i].FieldPlace.Column);
            }
        }
        
        public void CleanDamagedBubble(ref Instruments.Laser.DamagedBubble target)
        {
            var ValidPlaces = new List<Place>(1) {target.FieldPlace};
            _effects.PopBubbles(CollectBubbles(ValidPlaces));
            SeekNonConnectedToRoot();
            if (_nonRootChank.Count > 0)
            {
                _effects.AnimateFallUnconnectedBubbles(CollectBubbles(_nonRootChank));
            }
            _reactOnBubbleSet.Invoke(ValidPlaces, _nonRootChank, typeof(Instruments.Laser));
            CleanEmptyLines();
            OnFieldRefreshed?.Invoke();
        }
    }
}