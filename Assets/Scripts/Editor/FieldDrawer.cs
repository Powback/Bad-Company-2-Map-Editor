using UnityEngine;
using UnityEditor;
using System.Collections;
using BC2;

[CustomPropertyDrawer (typeof (Field))]
public class FieldDrawer : PropertyDrawer {
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
		// Using BeginProperty / EndProperty on the parent property means that
		// prefab override logic works on the entire property.
		EditorGUI.BeginProperty (position, label, property);
		
		// Draw label
		position = EditorGUI.PrefixLabel (position, GUIUtility.GetControlID (FocusType.Passive), label);
		
		// Don't make child fields be indented
		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;
		
		// Calculate rects
		//var amountRect = new Rect (position.x, position.y, 30, position.height);
		var valueRect = new Rect (position.x, position.y, Percent(position.x,50), position.height);
		var refRect = new Rect (position.x + Percent(position.x,50), position.y, Percent(position.x,48), position.height);

		
		// Draw fields - passs GUIContent.none to each so they are drawn without labels
		//EditorGUI.PropertyField (amountRect, property.FindPropertyRelative ("name"), GUIContent.none);
		//if (property.FindPropertyRelative ("value").boolValue) {
		//	EditorGUI.Toggle (unitRect, property.FindPropertyRelative ("value").stringValue);
		//} else {
		EditorGUI.PropertyField (valueRect, property.FindPropertyRelative ("value"), GUIContent.none);
		//}
		EditorGUI.PropertyField (refRect, property.FindPropertyRelative ("reference"), GUIContent.none);
		
		// Set indent back to what it was
		EditorGUI.indentLevel = indent;
		
		EditorGUI.EndProperty ();
	}
	public float Percent(float pos, float pc) {
		float width = EditorGUIUtility.currentViewWidth;
		return(((width - pos) / 100) * pc);
	}
}