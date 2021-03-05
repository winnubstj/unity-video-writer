using UnityEditor;
public class VideoWriterEditor
{
    [MenuItem("VideoWriter/CreateVideoWriter")]
    static void Create()
    {
         for (int x=0; x!=10; x++)
         {
              GameObject go = new GameObject("MyCreatedGO"+i);
              go.transform.position = new Vector3(x,0,0);
         }
    }
}