using Footsteps;
using UnityEditor;
using UnityEngine;

public class CharacterCreatorEditor : EditorWindow
{
    Vector3 axis;
    GameObject baseCharacter;
    bool MakePrefab;
    float scale;
    [MenuItem("Window/Character Creator")]
    public static void ShowWindow()
    {
        GetWindow<CharacterCreatorEditor>("Character Creator");
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("Seperating GameObjects Tool", EditorStyles.boldLabel);
        GUILayout.Space(5);
        axis = EditorGUILayout.Vector3Field("Starting Axis", axis);
        GUILayout.Space(5);
        if (GUILayout.Button("SEPERATE SELECTED ON AXIS"))
        {
            GameObject[] sg = Selection.gameObjects;
            for (int i = 0; i < sg.Length; i++)
            {
                //Undo.RecordObject(sg[i], "undo z pos");
                sg[i].transform.SetPositionZ(i * axis.z);
            }
        }


        GUILayout.Space(10);
        GUILayout.Label("Character Creation Tools", EditorStyles.boldLabel);
        GUILayout.Space(5);
        baseCharacter = (GameObject)EditorGUILayout.ObjectField(baseCharacter, typeof(GameObject), true);
        MakePrefab = GUILayout.Toggle(MakePrefab, "Make Gameobject a prefab");
        GUILayout.Space(5);
        if (GUILayout.Button("Reassign Components"))
        {
            foreach (GameObject selectedObj in Selection.gameObjects)
            {
                selectedObj.GetComponent<PlayerController>().ReassignCasters();
            }
        }
        if (GUILayout.Button("PASTE COMPONENTS"))
        {
            if (baseCharacter == null) return;
            //GameObject selectedObj = Selection.gameObjects[0];
            foreach (GameObject selectedObj in Selection.gameObjects)
            {
                MakeNewCharacter(selectedObj);
            }

        }
        GUILayout.Space(10);
        GUILayout.Label("Selection Tools", EditorStyles.boldLabel);
        GUILayout.Space(5);
        if (GUILayout.Button("OPEN SPHERE COLLIDER"))
        {
            GameObject selectedObj = Selection.gameObjects[0];
            Animator selectedAnim = selectedObj.GetComponent<Animator>();
            Selection.activeObject = selectedAnim.GetBoneTransform(HumanBodyBones.Head).gameObject;
        }
        GUILayout.Space(5);
        if (GUILayout.Button("OPEN LEFT LEG COLLIDER"))
        {
            GameObject selectedObj = Selection.gameObjects[0];
            Animator selectedAnim = selectedObj.GetComponent<Animator>();
            Selection.activeObject = selectedAnim.GetBoneTransform(HumanBodyBones.LeftFoot).GetComponentInChildren<FootstepTrigger>().gameObject;
        }
        GUILayout.Space(5);
        if (GUILayout.Button("OPEN RIGHT LEG COLLIDER"))
        {
            GameObject selectedObj = Selection.gameObjects[0];
            Animator selectedAnim = selectedObj.GetComponent<Animator>();
            Selection.activeObject = selectedAnim.GetBoneTransform(HumanBodyBones.RightFoot).GetComponentInChildren<FootstepTrigger>().gameObject;
        }
        GUILayout.Space(5);
        EditorGUILayout.HelpBox("Remeber to change the animator controller if need be", MessageType.Info);
        GUILayout.Space(5);
        scale = EditorGUILayout.FloatField("Enter a uniform scale", scale);

        if (GUILayout.Button("SET SCALE"))
        {
            Selection.gameObjects[0].transform.localScale = Vector3.one * scale;
        }
        GUILayout.Space(5);


        if (GUILayout.Button("DESTROY (USE WITH CAUTION)"))
            DestroyImmediate(Selection.gameObjects[0]);

        GameObject[] so = Selection.gameObjects;
        for (int i = 0; i < so.Length; i++)
        {
            if (so[i].GetComponent<Animator>() != null)
            {
                if (!so[i].GetComponent<Animator>().isHuman)
                {
                    GUILayout.Space(2);
                    EditorGUILayout.HelpBox(so[i].gameObject + " is not a humanoid rig", MessageType.Error);

                }
            }
        }

    }

    private void MakeNewCharacter(GameObject selectedObj)
    {
        Animator selectedAnim = selectedObj.GetComponent<Animator>();
        selectedAnim.runtimeAnimatorController = baseCharacter.GetComponent<Animator>().runtimeAnimatorController;
        selectedAnim.applyRootMotion = false;
        if (!selectedAnim.isHuman)
        {
            Extensions.Debug(selectedAnim.gameObject.name + " is not a humaniod rig");
            return;
        }
        UnityEditorInternal.ComponentUtility.CopyComponent(baseCharacter.GetComponent<CapsuleCollider>());
        UnityEditorInternal.ComponentUtility.PasteComponentAsNew(selectedObj);

        AddRigidbody(selectedObj);
        selectedObj.tag = "Player";
        AddRayCastCheckers(selectedObj);

        UnityEditorInternal.ComponentUtility.CopyComponent(baseCharacter.GetComponent<PlayerController>());
        UnityEditorInternal.ComponentUtility.PasteComponentAsNew(selectedObj);

        selectedObj.GetComponent<PlayerController>().ReassignCasters();

        UnityEditorInternal.ComponentUtility.CopyComponent(baseCharacter.GetComponent<AnimationController>());
        UnityEditorInternal.ComponentUtility.PasteComponentAsNew(selectedObj);

        UnityEditorInternal.ComponentUtility.CopyComponent(baseCharacter.GetComponent<CharacterFootsteps>());
        UnityEditorInternal.ComponentUtility.PasteComponentAsNew(selectedObj);


        UnityEditorInternal.ComponentUtility.CopyComponent(baseCharacter.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Head).GetComponentInChildren<SphereCollider>());
        UnityEditorInternal.ComponentUtility.PasteComponentAsNew(selectedAnim.GetBoneTransform(HumanBodyBones.Head).gameObject);


        GameObject ftrigger = baseCharacter.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.LeftFoot).GetComponentInChildren<FootstepTrigger>().gameObject;
        Instantiate(ftrigger, selectedAnim.GetBoneTransform(HumanBodyBones.LeftFoot));
        Instantiate(ftrigger, selectedAnim.GetBoneTransform(HumanBodyBones.RightFoot));
        if (MakePrefab)
        {
            // Set the path as within the Assets folder,
            // and name it as the GameObject's name with the .Prefab format
            string localPath = "Assets/Prefabs/Characters/" + selectedObj.name + ".prefab";

            // Make sure the file name is unique, in case an existing Prefab has the same name.
            localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
            PrefabUtility.SaveAsPrefabAssetAndConnect(selectedObj, localPath, InteractionMode.UserAction);
        }
    }

    private void AddRigidbody(GameObject selectedObj)
    {
        Rigidbody a = null;
        if (!selectedObj.HasRigidbody())
            a = selectedObj.AddComponent<Rigidbody>();
        else
            a = selectedObj.GetComponent<Rigidbody>();
        Rigidbody base_rb = baseCharacter.GetComponent<Rigidbody>();
        a.constraints = base_rb.constraints;
    }

    void AddRayCastCheckers(GameObject selectedObj)
    {

        for (int i = 2; i < baseCharacter.transform.childCount; i++)
        {
            Instantiate(baseCharacter.transform.GetChild(i), selectedObj.transform, false);
        }
    }
}
