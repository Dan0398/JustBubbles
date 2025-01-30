using UnityEngine;

namespace Gameplay.Field
{
    public partial class BubbleField: MonoBehaviour, IField
    {
        public Vector3 PlaceToPos(Place place) => PlaceToPos(place.Line, place.Column);
        
        public Vector3 PlaceToPos(int LineNumber, int PlaceNumber)
        {
            /*
            Vector3 Result = StartPoint;
            Result += Vector3.down  * LineHeight * LineNumber;
            Result += Vector3.right * BubbleSize * PlaceNumber;
            Result += Vector3.right * ShiftWidth * (Shifted? 1 : 0);
            */
            return new Vector3(StartPoint.x + BubbleSize * PlaceNumber + ShiftWidth * (LineShifted(LineNumber)? 1 : 0), StartPoint.y -LineHeight * LineNumber, 0);
        }
        
        int GetLineNumberForPos(Vector3 Pos)
        {
            if (Pos.y < StartPoint.y - FieldUsableSpace) return -1;
            float Height = Pos.y - LineHeight * 0.5f;
            int Result = Mathf.FloorToInt((Lines[0].OnScene.position.y - Height)/(float)LineHeight);
            Debug.DrawLine(StartPoint, new Vector3(StartPoint.x, StartPoint.y - Result * LineHeight), Color.magenta, 0.5f);
            return Result;
        }
        
        int GetPlaceNumberForPos(Vector3 Pos, bool Shifted)
        {
            float XPos = Pos.x;
            if (Shifted) XPos -= ShiftWidth;
            return Mathf.RoundToInt((XPos - StartPoint.x)/BubbleSize);
        }
        
        int GetPlaceNumberForPos(Vector3 Pos, int LineID) => GetPlaceNumberForPos(Pos, LineShifted(LineID));
        
        bool LineShifted(int LineID)
        {
            bool Shifted = Lines[0].Shifted;
            if (LineID % 2 ==1) Shifted = !Shifted;
            return Shifted;
        }
        
        Place PosToPlace(Vector3 Pos)
        {
            var LineId = GetLineNumberForPos(Pos);
            return new Place(LineId, GetPlaceNumberForPos(Pos, LineId));
        }
    }
}