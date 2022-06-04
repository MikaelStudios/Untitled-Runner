using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu()]
public class Tracks : ScriptableObject
{
    public TrackType m_type;
    public Track[] m_tracks;

    [Header("Sky Colors")]
    public Color sky_UpColor;
    public Color sky_DownColor;

    public Track[] GetTracksOfDifficulty(Difficulty d)
    {
        List<Track> trackOfd = new List<Track>();
        for (int i = 0; i < m_tracks.Length; i++)
        {
            if (m_tracks[i].m_difficulty == d)
                trackOfd.Add(m_tracks[i]);
        }
        return trackOfd.ToArray();
    }
}
#if  UNITY_EDITOR
[CustomEditor(typeof(Tracks)), CanEditMultipleObjects]
public class TracksEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        serializedObject.Update();

        GUILayout.Space(10);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_type")/*,EditorStyles.boldFont*/);
        GUILayout.Space(10);

        //GUILayout.BeginHorizontal();
        //GUILayout.Label("Sky Colors", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("sky_UpColor"), new GUIContent("Up"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("sky_DownColor"), new GUIContent("Down"));
        //GUILayout.EndHorizontal();

        GUILayout.Space(10);
        EditorList.Show(serializedObject.FindProperty("m_tracks"), EditorListOption.ListLabel | EditorListOption.Buttons);
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
public enum TrackType
{
    Forest, Autum, Spooky
}
public enum Difficulty
{
    Easy, Medium, Hard
}



[System.Serializable]
public struct Track
{
    public GameObject m_track;
    public Difficulty m_difficulty;
}
#if  UNITY_EDITOR

[CustomPropertyDrawer(typeof(Track))]
public class TrackDrawer : PropertyDrawer
{

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return label != GUIContent.none && Screen.width < 333 ? (16f + 18f) : 16f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        int oldIndentLevel = EditorGUI.indentLevel;
        label = EditorGUI.BeginProperty(position, label, property);
        Rect contentPosition = EditorGUI.PrefixLabel(position, label);
        if (position.height > 16f)
        {
            position.height = 16f;
            EditorGUI.indentLevel += 1;
            contentPosition = EditorGUI.IndentedRect(position);
            contentPosition.y += 18f;
        }
        contentPosition.width *= 0.75f;
        EditorGUI.indentLevel = 0;
        EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("m_track"), GUIContent.none);
        contentPosition.x += contentPosition.width;
        contentPosition.width /= 3f;
        EditorGUIUtility.labelWidth = 14f;
        EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("m_difficulty"), new GUIContent("C"));
        EditorGUI.EndProperty();
        EditorGUI.indentLevel = oldIndentLevel;
    }
}

#endif