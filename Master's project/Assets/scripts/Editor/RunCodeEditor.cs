using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(main))]
[CanEditMultipleObjects]
public class RunCodeEditor : Editor
{
	public string[] options = new string[] { "Incremental", "D&C" };
	public enum algorithm { INC, DD }

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		main m = (main)target;

		
		GUILayout.BeginHorizontal();
		m.index = EditorGUILayout.Popup(m.index, options);

		if (GUILayout.Button("Run"))
		{
			m.ExecuteCode();
		}

		if (GUILayout.Button("Empty container"))
		{
			m.clearContainer();
		}


		GUILayout.EndHorizontal();
	}
}
