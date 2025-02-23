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
            return new Vector3(_startPoint.x + BubbleSize * PlaceNumber + _shiftWidth * (LineShifted(LineNumber)? 1 : 0), _startPoint.y -_lineHeight * LineNumber, 0);
        }
        
        private int GetLineNumberForPos(Vector3 Pos)
        {
            if (Pos.y < _startPoint.y - _fieldUsableSpace) return -1;
            float Height = Pos.y - _lineHeight * 0.5f;
            int Result = Mathf.FloorToInt((_lines[0].OnScene.position.y - Height)/(float)_lineHeight);
            Debug.DrawLine(_startPoint, new Vector3(_startPoint.x, _startPoint.y - Result * _lineHeight), Color.magenta, 0.5f);
            return Result;
        }
        
        private int GetPlaceNumberForPos(Vector3 Pos, bool Shifted)
        {
            float XPos = Pos.x;
            if (Shifted) XPos -= _shiftWidth;
            return Mathf.RoundToInt((XPos - _startPoint.x)/BubbleSize);
        }
        
        private int GetPlaceNumberForPos(Vector3 Pos, int LineID) => GetPlaceNumberForPos(Pos, LineShifted(LineID));
        
        private bool LineShifted(int LineID)
        {
            bool Shifted = _lines[0].Shifted;
            if (LineID % 2 ==1) Shifted = !Shifted;
            return Shifted;
        }
        
        private Place PosToPlace(Vector3 Pos)
        {
            var LineId = GetLineNumberForPos(Pos);
            return new Place(LineId, GetPlaceNumberForPos(Pos, LineId));
        }
    }
}