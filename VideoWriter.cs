using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;
using UnityEngine.Rendering;
using UnityEditor;
#if USE_GIMBL_NAME
using Gimbl;
#endif

public class VideoWriter : MonoBehaviour
{
    public bool writeVideo = true;
    public float frameRate = 24;
    public int quality = 75;
    // The Encoder Thread
    private string fileName;
    private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
    private byte[] im;
    private BinaryWriter stream;
    IEnumerator Start()
    {
        if (writeVideo)
        {
#if USE_GIMBL_NAME
            LoggerObject log = FindObjectOfType<LoggerObject>();
            if (log)
            {
                fileName = $"{Path.GetFileNameWithoutExtension(log.logFile.filePath)}.univideo";
            }
#endif
            // Create file using timestamp (default).
            if (fileName==null)
            {
                string fileName = $"{DateTime.Now.Year}_{DateTime.Now.Month}_{DateTime.Now.Day}__{DateTime.Now.Hour}-{DateTime.Now.Minute}-{DateTime.Now.Second}.univideo";
                
            }
            // Create file stream.
            stream = new BinaryWriter(File.Open(Path.Combine(PlayerPrefs.GetString("unityvideowriter.outputfolder","C:\\"), fileName), FileMode.Create));
            // Start loop
            while (true)
            {
                yield return new WaitForSeconds((1f / frameRate));
                yield return new WaitForEndOfFrame();

                var rt = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
                ScreenCapture.CaptureScreenshotIntoRenderTexture(rt);
                AsyncGPUReadback.Request(rt, 0, TextureFormat.ARGB32, OnCompleteReadback);
                RenderTexture.ReleaseTemporary(rt);
            }
        }
    }
    void OnDisable()
    {
        if (stream!=null)
        {
            stream.Close();
            stream = null;
        }
    }

        void OnCompleteReadback(AsyncGPUReadbackRequest request)
    {
        if (stream == null) { return; }
        if (request.hasError) { Debug.Log("GPU readback error detected."); return; }
        Texture2D tex = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false); // creating here prevents errors on stop.
        tex.LoadRawTextureData(request.GetData<uint>());
        tex.Apply();
        // check stopwatch.
        if (!stopwatch.IsRunning) { stopwatch.Start(); }
        else {
            stream.Write(BitConverter.GetBytes(stopwatch.Elapsed.TotalSeconds));
            stopwatch.Restart();
        }
        // Writer raw data
        im = tex.EncodeToJPG(quality);
        stream.Write(BitConverter.GetBytes(im.Length));
        stream.Write(im);
        stream.Flush();

        DestroyImmediate(tex);
    }


}

[CustomEditor(typeof(VideoWriter))]
public class VideoWriterInspector : Editor
{
    private string _outputFolder;
    private string _newOutputFolder;
    public override void OnInspectorGUI()
    {
        if (_outputFolder == null) { _outputFolder = PlayerPrefs.GetString("unityvideowriter.outputfolder", "C:\\"); }
        _newOutputFolder = EditorGUILayout.TextField("Output Folder: ", _outputFolder);
        if (_newOutputFolder != _outputFolder)
        { PlayerPrefs.SetString("unityvideowriter.outputfolder", _newOutputFolder); _outputFolder = _newOutputFolder; }
        // Show default inspector property editor
        DrawDefaultInspector();
    }
}