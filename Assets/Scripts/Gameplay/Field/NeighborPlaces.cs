using UnityEngine;

namespace Gameplay.Field
{
    public partial class BubbleField: MonoBehaviour, IField
    {
        private int GetNeighborPlaces(Place Center, int Outline, ref Place[] TargetArray)
        {
            return GetNeighborPlaces(Center, Outline, ref TargetArray, 0);
            
            int GetNeighborPlaces(Place Center, int Outline, ref Place[] TargetArray, int PlaceID = 0)
            {
                AppendOnMyLine(ref TargetArray, ref PlaceID);
                AppendExtremumLines(ref TargetArray, ref PlaceID);
                AppendSideLines(ref TargetArray, ref PlaceID);
                
                if (Outline > 1)
                {
                    return GetNeighborPlaces(Center, Outline - 1, ref TargetArray, PlaceID);
                }
                return PlaceID;
                
                void AppendOnMyLine(ref Place[] TargetArray, ref int PlaceID)
                {
                    /*
                    + - центр
                    O - игнорируемый в текущем шаге
                    X - включаемое место на текущем шаге
                    ■ - включённое на предыдущих шагах
                    
                    O O O O O O O
                    O O O O O O O
                    O X O + O X O
                    O O O O O O O
                    O O O O O O O
                    */
                    
                    TargetArray[PlaceID].Line = Center.Line;
                    TargetArray[PlaceID].Column = Center.Column - Outline;
                    PlaceID++;
                    TargetArray[PlaceID].Line = Center.Line;
                    TargetArray[PlaceID].Column = Center.Column + Outline;
                    PlaceID++;
                }
                
                void AppendExtremumLines(ref Place[] TargetArray, ref int PlaceID)
                {
                    /*
                    O O O X O O O
                    O O O O O O O
                    O ■ O + O ■ O
                    O O O O O O O
                    O O O X O O O
                    */
                    int StartPos = - Mathf.FloorToInt(Outline / 2f);
                    int EndPos = - StartPos;
                    
                    FixShift(Outline, ref StartPos, ref EndPos);
                    AppendAtLine(ref TargetArray, ref PlaceID, true);
                    AppendAtLine(ref TargetArray, ref PlaceID, false);
                
                    
                    void AppendAtLine(ref Place[] TargetArray, ref int PlaceID, bool OnTop)
                    {
                        int Line = Center.Line + Outline * (OnTop? 1 : -1);
                        for (int i = StartPos; i <= EndPos; i++)
                        {
                            TargetArray[PlaceID].Line = Line;
                            TargetArray[PlaceID].Column = Center.Column + i;
                            PlaceID++;
                        }
                    }
                }
                
                void AppendSideLines(ref Place[] TargetArray, ref int PlaceID)
                {
                    /*
                    O O O ■ O O O
                    0 O X 0 X O O
                    O ■ O + O ■ O
                    0 O X O X O O
                    O O O ■ O O O
                    */
                    AppendLineByDir(ref TargetArray, ref PlaceID, true);
                    AppendLineByDir(ref TargetArray, ref PlaceID, false);
                    
                    void AppendLineByDir(ref Place[] TargetArray, ref int PlaceID, bool GoTop)
                    {
                        for (int i = 1; i < Outline; i++)
                        {
                            var Line = Center.Line + i * (GoTop? 1 : -1);
                            var Outstand = Outline - Mathf.CeilToInt(i / 2f);
                            int StartPos = - Outstand;
                            int EndPos = Outstand;
                            FixShift(i, ref StartPos, ref EndPos);
                            
                            TargetArray[PlaceID].Line = Line;
                            TargetArray[PlaceID].Column = Center.Column + StartPos;
                            PlaceID++;
                            
                            TargetArray[PlaceID].Line = Line;
                            TargetArray[PlaceID].Column = Center.Column + EndPos;
                            PlaceID++;
                        }
                    }
                }
                
                void FixShift(int LineID, ref int StartPos, ref int EndPos)
                {
                    if (Mathf.Abs(LineID) %2 == 0) return;
                    if (LineShifted(Center.Line))
                    {
                        EndPos++;
                    }
                    else
                    {
                        StartPos--;
                    }
                }
            }
        }
        
        private int RequireToGetAllNeighborPoses(int Outline)
        {
            return Factorial(6, Outline);

            static int Factorial(int Target ,int Outline)
            {
                if (Outline > 1) return Target * Outline + Factorial(Target, Outline-1);
                return Target*Outline;
            }
        }
    }
}