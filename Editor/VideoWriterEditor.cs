using UnityEditor;
using UnityEngine;
public class VideoWriterEditor
{
    [MenuItem("Window/VideoWriter/CreateVideoWriter")]
    static void Create()
    {
        if (GameObject.FindObjectOfType<VideoWriter>() == null)
        { 
            GameObject videoWriter = (GameObject)GameObject.Instantiate(Resources.Load("VideoWriter"));
            videoWriter.name = "VideoWriter";
        }
    }
}