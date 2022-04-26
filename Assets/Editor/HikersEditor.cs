using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Hiker))]
public class HikersEditor : Editor
{
    Hiker hikerObject;
    private void Awake()
    {
        hikerObject = (Hiker)target;
    }

    void MyGUI()
    {

        // if (GUILayout.Button("Add Level"),GUILayout.Height(35.0f)){

        // }
        hikerObject.firstName = EditorGUILayout.TextField("First Name", hikerObject.firstName);
        for (int i = 0; i < hikerObject.appearanceLevels.Count; i++)
        {
            // EditorGUILayout.LabelField(hikerObject.appearanceLevels[i].ToString());
            // hikerObject.appearanceLevels[i] = EditorGUILayout.TextField("Level #", hikerObject.appearanceLevels[i]);
        }
    }
    public override void OnInspectorGUI()
    {
        MyGUI();
        //Will draw default inspector:
        //        DrawDefaultInspector();
    }
}
