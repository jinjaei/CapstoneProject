using System.IO;
using UnityEngine;

namespace InfiniteValue
{
    /// Automatically move the Gizmos folder at the root of the project assets.
    static class GizmosFolderMover
    {
        [UnityEditor.Callbacks.DidReloadScripts]
        static void OnScriptReload()
        {
            if (Directory.Exists($"{Configuration.folderPath}/Gizmos"))
            {
                if (!Directory.Exists($"{Application.dataPath}/Gizmos"))
                {
                    Directory.Move($"{Configuration.folderPath}/Gizmos", $"{Application.dataPath}/Gizmos");
                    File.Move($"{Configuration.folderPath}/Gizmos.meta", $"{Application.dataPath}/Gizmos.meta");
                }
                else
                {
                    Directory.Move($"{Configuration.folderPath}/Gizmos/InfiniteValue", $"{Application.dataPath}/Gizmos/InfiniteValue");
                    Directory.Delete($"{Configuration.folderPath}/Gizmos", true);
                    File.Delete($"{Configuration.folderPath}/Gizmos.meta");
                }
            }
        }
    }
}

