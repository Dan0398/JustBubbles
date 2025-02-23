#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Utils.Observables
{
    [CustomPropertyDrawer(typeof(Observable<>), true)]
    public partial class Observable_Inspector: PropertyDrawer
    {
        private SerializedProperty _inspection;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _inspection ??= property.FindPropertyRelative("inspector");
            EditorGUI.PropertyField(position, _inspection, label);
        }
    }
}
#endif