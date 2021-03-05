using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;
using UnityEngine.Rendering;

public class VideoWriter : MonoBehaviour
{
    public float frameRate = 24;
    // The Encoder Thread
    private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
    private byte[] im;
    private BinaryWriter stream;
    IEnumerator Start()
    {
        stream = new BinaryWriter(File.Open("C:\\temp\\video.bin", FileMode.Create));

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
    void OnDisable()
    {
        stream.Close();
        stream = null;
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
        im = tex.EncodeToJPG(75);
        stream.Write(BitConverter.GetBytes(im.Length));
        stream.Write(im);
        stream.Flush();

        DestroyImmediate(tex);
    }

}