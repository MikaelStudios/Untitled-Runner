#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class AudioAutoplay : EditorWindow
{

    [MenuItem("Tools/Audio Autoplay")]
    static void Init()
    {
        var window = EditorWindow.GetWindow(typeof(AudioAutoplay));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Audio files will now play on selection change.");

        GUILayout.Space(10);
        if (GUILayout.Button("Stop All Clips"))
            StopAllClips();
    }

    void OnSelectionChange()
    {

        UnityEngine.Object[] clips = Selection.GetFiltered(typeof(AudioClip), SelectionMode.Unfiltered);

        if (clips != null && clips.Length == 1)
        {
            StopAllClips();
            AudioClip clip = (AudioClip)clips[0];
            PlayClip(clip);
        }
        else
            StopAllClips();

    }
    /*
     * How to play audio in Editor using an Editor script
     * 
     * http://forum.unity3d.com/threads/way-to-play-audio-in-editor-using-an-editor-script.132042/
     */
    public static void PlayClip(AudioClip clip, int startSample = 0, bool loop = false)
    {
        System.Reflection.Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        System.Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        System.Reflection.MethodInfo method = audioUtilClass.GetMethod(
            "PlayClip",
            System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public,
            null,
            new System.Type[] { typeof(AudioClip), typeof(int), typeof(bool) },
            null
        );
        method.Invoke(
            null,
            new object[] { clip, startSample, loop }
        );
    }
    public static void StopAllClips()
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        System.Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "StopAllClips",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new System.Type[] { },
            null
        );
        method.Invoke(
            null,
            new object[] { }
        );
    }
}
#endif