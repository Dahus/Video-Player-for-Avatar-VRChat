using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.VersionControl;
using UnityEditor.Animations;
using System.IO;
using static BlackStartX.GestureManager.Editor.Data.GestureManagerStyles;
using static Thry.AnimationParser;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Math.EC.Multiplier;
using UnityEngine.Video;
using UnityEngine.UIElements;
using System.Reflection.Emit;
using UnityEditor.SceneManagement;

public class Loading
{
    // Event for updating the progress bar
    public event Action<int, int, string> ProgressUpdated;

    public void progressBar(int frame, int max, string label)
    {
        Rect rect = GUILayoutUtility.GetRect(18, 18, "TextField");
        frame = EditorGUI.IntSlider(rect, frame, 0, max);
        EditorGUI.ProgressBar(rect, (float)frame / max, label);
        EditorGUILayout.Space(10);

        // Trigger an event when progress is updated
        ProgressUpdated?.Invoke(frame, max, label);
    }
}

[CustomEditor(typeof(VideoPlayer))]
[System.Serializable]
public class VideoEditor : Editor
{

    protected VideoPlayer video;

    public void OnEnable()
    {
        video = (VideoPlayer)target;
    }

    static float timeIntervalStorage;

    static int numberOfElements;

    //static bool animatorControllerCreated = false;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);

        GUIStyle smallerFontStyle = new GUIStyle(EditorStyles.label);
        smallerFontStyle.alignment = TextAnchor.MiddleCenter;

        Loading loading = new Loading();

        EditorGUILayout.BeginHorizontal();
        if (video != null && video.TextureArray != null && video.TextureArray.Count > 1 && video.FrameParent != null && video.AnimationName != null && video.VideoLength > 0
            && video.FolderAnimationName != null && video.FolderMaterialsName != null)
        {
            int maxFrame = video.TextureArray.Count;

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = 14; 
            buttonStyle.alignment = TextAnchor.MiddleCenter;
            buttonStyle.normal.textColor = new Color(0.36f, 1f, 0.36f, 1f); 

            if (GUILayout.Button("Create a video", buttonStyle))
            {

                toDoGameObject(maxFrame);

                // Update window of hierarchy 
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            }
        }
        else
        {
            smallerFontStyle.fontSize = 14;
            smallerFontStyle.normal.textColor = new Color(1f, 0.36f, 0.36f, 1f);
            EditorGUILayout.LabelField("Fill out in the fields!", smallerFontStyle);
        }

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(4);
        smallerFontStyle.fontSize = 10;
        smallerFontStyle.normal.textColor = Color.gray;
        EditorGUILayout.LabelField("powered by Dahus", smallerFontStyle);

        // Creation progress bar

        //loading.ProgressUpdated += OnProgressUpdated;
        //loading.progressBar(numberOfElements, video.TextureArray.Count, "Creating...");
        //Repaint();

        void toDoGameObject(int maxFrame)
        {
            GameObject frame;

            AnimationClip animationClipON = new AnimationClip();
            AnimationClip animationClipOFF = new AnimationClip();

            SetLoopTime(animationClipON, true);
            SetLoopTime(animationClipOFF, false);

            var curveBindingsON = AnimationUtility.GetCurveBindings(animationClipON);
            foreach (var curveBindingON in curveBindingsON)
            {
                Console.WriteLine($"CurveBinding: PropertyName={curveBindingON.propertyName}, Path={curveBindingON.path}");
            }

            var curveBindingsOFF = AnimationUtility.GetCurveBindings(animationClipOFF);
            foreach (var curveBindingOFF in curveBindingsOFF)
            {
                Console.WriteLine($"CurveBinding: PropertyName={curveBindingOFF.propertyName}, Path={curveBindingOFF.path}");
            }

            CreateAnimationFolder();
            AssetDatabase.CreateAsset(animationClipON, $"Assets/{video.FolderAnimationName}/{video.AnimationName}ON.anim");
            AssetDatabase.CreateAsset(animationClipOFF, $"Assets/{video.FolderAnimationName}/{video.AnimationName}OFF.anim");

            CreateMaterialsFolder();

            float timeInterval = video.VideoLength / maxFrame; //96 / 14 = 6,8 96 * 14 = 
            timeIntervalStorage = timeInterval;

            for (numberOfElements = 0; numberOfElements < maxFrame; numberOfElements++)
            {
                frame = new GameObject($"Frame ({numberOfElements + 1})");

                frame.transform.parent = video.FrameParent.transform;

                // Create RectTransform
                RectTransform rectTransform = frame.AddComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(100, 100);

                frame.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 0, 0);
                frame.GetComponent<RectTransform>().rotation = new Quaternion(0,0,0,0);
                frame.GetComponent<RectTransform>().localScale = new Vector3 (1, 1, 1);

                //Create Material
                Material material = new Material(Shader.Find("Standard"));
                material.mainTexture = video.TextureArray[numberOfElements];
                AssetDatabase.CreateAsset(material, $"Assets/{video.FolderAnimationName}/{video.FolderMaterialsName}/{frame.name}.mat");

                // Create Renderer
                MeshRenderer renderer = frame.AddComponent<MeshRenderer>();
                renderer.material = material;

                // Create MeshFilter
                MeshFilter meshFilter = frame.AddComponent<MeshFilter>();
                Mesh quadMesh = Resources.GetBuiltinResource<Mesh>("Quad.fbx");
                meshFilter.sharedMesh = quadMesh;

                //frame = GameObject.CreatePrimitive(PrimitiveType.Quad);
                meshFilter.sharedMesh = frame.GetComponent<MeshFilter>().sharedMesh;


                renderer.enabled = false;
                //frame.SetActive(false);

                //Animator animator = frame.GetComponent<Animator>();
                //if (animator == null)
                //{
                //    animator = frame.AddComponent<Animator>();
                //}

                //animator.SetFloat("Alpha", material.color.a);

                toDoAnimationON(frame, animationClipON, timeInterval);
                toDoAnimationOFF(frame, animationClipOFF);
            }
        }

        void toDoAnimationON(GameObject frame, AnimationClip animationClip, float timeInterval)
        {
            EditorCurveBinding curveBinding = new EditorCurveBinding();
            curveBinding.type = typeof(MeshRenderer);
            curveBinding.path = GetHierarchyPathWithoutTopObject(frame);
            curveBinding.propertyName = "m_Enabled";
            //curveBinding.propertyName = "m_IsActive";


            AnimationCurve curve = new AnimationCurve();


            curve.AddKey(timeIntervalStorage - 0.01f, 0.0f); 
            curve.AddKey(timeIntervalStorage, 1.0f);
            curve.AddKey(timeIntervalStorage + timeInterval, 0.0f);

            timeIntervalStorage = timeInterval + timeIntervalStorage;


            AnimationUtility.SetEditorCurve(animationClip, curveBinding, curve);
        }

        void toDoAnimationOFF(GameObject frame, AnimationClip animationClip)
        {
            EditorCurveBinding curveBinding = new EditorCurveBinding();
            curveBinding.type = typeof(MeshRenderer);
            //curveBinding.type = typeof(GameObject);
            curveBinding.path = GetHierarchyPathWithoutTopObject(frame);
            curveBinding.propertyName = "m_Enabled";
            //curveBinding.propertyName = "m_IsActive";

            AnimationCurve curve = new AnimationCurve();

            // Frame off
            curve.AddKey(0.0f, 0.0f);

            AnimationUtility.SetEditorCurve(animationClip, curveBinding, curve);
        }

        void SetLoopTime(AnimationClip clip, bool loopTime)
        {
            AnimationClipSettings clipSettings = AnimationUtility.GetAnimationClipSettings(clip);
            clipSettings.loopTime = loopTime;
            AnimationUtility.SetAnimationClipSettings(clip, clipSettings);
        }

        void CreateAnimationFolder()
        {
            string path = $"Assets/{video.FolderAnimationName}";

            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder("Assets", $"{video.FolderAnimationName}");
            }
            else
            {
                return;
            }
        }

        void CreateMaterialsFolder()
        {
            string path = $"Assets/{video.FolderAnimationName}/{video.FolderMaterialsName}";

            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder($"Assets/{video.FolderAnimationName}", $"{video.FolderMaterialsName}");
            }
            else
            {
                return;
            }
        }

        string GetHierarchyPathWithoutTopObject(GameObject obj)
        {
            if (obj == null)
            {
                return "";
            }

            Transform parrentTransform = obj.transform.parent;

            if (parrentTransform == null)
            {
                return "No parent";
            }

            string path = obj.name;

            while (parrentTransform.parent != null)
            {
                obj = parrentTransform.gameObject;
                parrentTransform = obj.transform.parent;
                path = obj.name + "/" + path;
            }

            return path;
        }

        void OnProgressUpdated(int current, int max, string label)
        {
            Debug.Log($"Progress updated: {current}/{max}, Label: {label}");
        }


        if (GUI.changed)
        {
            EditorUtility.SetDirty(video);
            EditorSceneManager.MarkSceneDirty(video.gameObject.scene);
        }
    }
}