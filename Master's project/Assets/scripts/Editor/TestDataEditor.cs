using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DataTest))]
[CanEditMultipleObjects]
public class TestDataEditor : Editor
{

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		DataTest m = (DataTest)target;


		GUILayout.BeginHorizontal();
	

		if (GUILayout.Button("Run"))
		{
			m.RunCode();
		}

		GUILayout.EndHorizontal();
	}
}
