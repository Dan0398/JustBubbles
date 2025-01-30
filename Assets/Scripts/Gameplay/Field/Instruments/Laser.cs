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
            /*
            var Step = direction.normalized * BubbleSize * 0.6f;
            endpoint += Step;
            ValidatePlace(ref place);
            while(place.Valid && !place.Busy)
            {
                endpoint += Step;
                place = PosToPlace(endpoint);
                ValidatePlace(ref place);
            }
            return place;
            */
            if (NeighborPlaces == null)
            {
                NeighborPlaces = new Place[6];
            }
            GetNeighborPlaces(place, 1, ref NeighborPlaces);
            var Angle = Vector2.SignedAngle(Vector2.up, direction);
            //Debug.Log($"Center:{place.Line}x{place.PlaceID}; Angle:{Angle}");
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
                ValidatePlace(ref NeighborPlaces[Indexes[id]]);
                if (NeighborPlaces[Indexes[id]].Valid && NeighborPlaces[Indexes[id]].Busy) 
                {
                    return NeighborPlaces[Indexes[id]];
                }
            }
            id--;
            /*
            //Debug.DrawRay(PlaceToPos(NeighborPlaces[index]), Vector3.down, Color.white, 5);
            string result = string.Empty;
            for (int i =0; i < Indexes.Count; i++)
            {
                result += Indexes[i].ToString();
            }
            Debug.Log(result);
            */
            return NeighborPlaces[Indexes[id]];// new Place(Lines[Place.Line][Place.PlaceID], 
        }
        
        public void TryChangeLinesPosInDamaged(ref List<Instruments.Laser.DamagedBubble> damagedBubbles, int oldLinesCount)
        {
            var newLinesCount = Lines.Count;
            for (int i = 0; i < damagedBubbles.Count; i++)
            {
                var newLineNumber = damagedBubbles[i].fieldPlace.Line + newLinesCount - oldLinesCount;
                if (newLineNumber < 0 || newLineNumber > newLinesCount)
                {
                    damagedBubbles.RemoveAt(i);
                    i--;
                    continue;
                }
                damagedBubbles[i].fieldPlace = new Field.Place(newLineNumber, damagedBubbles[i].fieldPlace.Column);
            }
        }
        
        public void CleanDamagedBubble(ref Instruments.Laser.DamagedBubble target)
        {
            var ValidPlaces = new List<Place>(1) {target.fieldPlace};
            Effects.PopBubbles(CollectBubbles(ValidPlaces));
            SeekNonConnectedToRoot();
            if (NonRootChank.Count > 0)
            {
                Effects.AnimateFallUnconnectedBubbles(CollectBubbles(NonRootChank));
            }
            ReactOnBubbleSet.Invoke(ValidPlaces, NonRootChank, typeof(Instruments.Laser));
            CleanEmptyLines();
            OnFieldRefreshed?.Invoke();
        }
    }
}
