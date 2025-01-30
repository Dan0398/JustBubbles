#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Utils.Observables
{
    [CustomPropertyDrawer(typeof(ObsList<>), true)]
    public class List_Inspector: PropertyDrawer
    {
        const int myHeight = 19;
        class ClickablePoints
        {
            public Rect Point;
            public System.Action OnClick;
            public ClickablePoints(Rect r, System.Action cl) { Point = r; OnClick = cl;}
        }
        ClickablePoints[] MyPoints;
        int WorkingRect = 0;
        int ActualLine = 0;
        GUIStyle Header;
        Rect Base;
        SerializedProperty prop;
        
        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
        {
            prop = property;
            Base = position;
            WorkingRect = 0;
            ActualLine = 0;
            EditorGUI.BeginProperty(position, label, prop);
            DrawHeader(label.text);
            DrawChilds();
            ProcessInput();
            EditorGUI.EndProperty();
        }
        
        Vector2 ActualLinePos => Base.position + Vector2.up * myHeight * ActualLine;
        
        void DrawHeader(string Label)
        {
            var HeaderRect = new Rect(ActualLinePos, new Vector2(Base.width*0.94f, myHeight));
            
            RegisterClickableRect(HeaderRect, ()=> { prop.isExpanded = !prop.isExpanded; });
            if (Header == null)
            {
                Header = new GUIStyle();
                Header.fontStyle = FontStyle.Bold;
                Header.normal.textColor = GUI.contentColor;
                Header.normal.background = null;
                var Fade = new Texture2D(1,1);
                Fade.SetPixel(1,1, new Color(0.35f,0.35f,0.35f,1));
                Fade.Apply();
                Header.hover.background = Fade;
                Header.hover.textColor = GUI.contentColor;
            }
            EditorGUI.LabelField(HeaderRect, Label, Header);
            var ContentRect = new Rect(ActualLinePos + Vector2.right * Base.width * 0.94f, new Vector2(Base.width*0.06f, myHeight));
            int Count = prop.CountInProperty();//("Count").intValue;
            var EditedCount = EditorGUI.TextField(ContentRect, Count.ToString());
            if (System.Int32.TryParse(EditedCount, out int NewCount))
            {
                if (Count != NewCount)
                {
                    if (Count>NewCount)
                    {
                        
                    }
                    else 
                    {
                        
                    }
                }
            }
            else 
            {
                
            }
            ActualLine++;
        }
        
        void DrawChilds()
        {
            if (!prop.isExpanded) return;
        }
        
        void RegisterClickableRect(Rect r, System.Action onClick)
        {
            if (MyPoints == null || MyPoints.Length ==0)
            {
                MyPoints = new ClickablePoints[1];
            }
            if (MyPoints.Length == WorkingRect || MyPoints[WorkingRect] == null || MyPoints[WorkingRect].Point.Equals(r))
            {
                if (MyPoints.Length == WorkingRect)
                {
                    System.Array.Resize(ref MyPoints, WorkingRect+1);
                }
                MyPoints[WorkingRect] = new ClickablePoints(r, onClick);
            }
            WorkingRect++;
        }
        
        void ProcessInput()
        {
            if (Event.current.rawType == EventType.MouseDown)
            {
                for (int i=0; i< MyPoints.Length; i++)
                {
                    if (MyPoints[i].Point.Contains(Event.current.mousePosition))
                    {
                        MyPoints[i].OnClick?.Invoke();
                        return;
                    }
                }
            }
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return myHeight * ActualLine;
        }
    }
}
#endif