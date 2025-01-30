#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Utils.Observables
{
    [CustomPropertyDrawer(typeof(Observable<>), true)]
    public partial class Observable_Inspector: PropertyDrawer
    {
        SerializedProperty inspection;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (inspection == null)
            {
                inspection = property.FindPropertyRelative("inspector");
            }
            EditorGUI.PropertyField(position, inspection, label);
        }
    }
}
#endif