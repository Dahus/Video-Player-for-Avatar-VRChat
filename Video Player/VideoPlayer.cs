using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.WSA;

[System.Serializable]
public class VideoPlayer : MonoBehaviour
{
    // Textures
    public List<Texture2D> TextureArray;
    // Place of spawn objects with texture
    public GameObject FrameParent;
    // Name your folder in which will have been saved all files
    public string FolderAnimationName = "Video";
    // Name your folder in which will have been saved files materials
    public string FolderMaterialsName = "Materials";
    // Name your .animation file
    public string AnimationName = "VideoAnim";
    // Quantity secound length video
    [Header ("Number of seconds how long your video is")]
    public float VideoLength;
}

