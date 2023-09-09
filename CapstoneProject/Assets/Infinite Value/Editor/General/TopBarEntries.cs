using UnityEditor;
using UnityEngine;

namespace InfiniteValue
{
    /// Add all the entries in the top bar menu related to the tool.
    class TopBarEntries
    {
        [MenuItem("Tools/Infinite Value/Configuration", priority = 0)]
        static void FocusConfiguration()
        {
            Selection.activeObject = Configuration.asset;
        }

        [MenuItem("Tools/Infinite Value/Documentation", priority = 20)]
        static void OpenDocumentation()
        {
            Application.OpenURL($"file:{Application.dataPath}{Configuration.folderPath.Substring("Assets".Length)}/Documentation.pdf");
        }
        
        [MenuItem("Tools/Infinite Value/Public API", priority = 21)]
        static void OpenPublicAPI()
        {
            Application.OpenURL("https://justetools.com/infinite-value/public-api/");
        }
        
        [MenuItem("Tools/Infinite Value/Unit Tests", priority = 40)]
        static void OpenUnitTestsWindow()
        {
            UnitTestsWindow window = (UnitTestsWindow)EditorWindow.GetWindow(typeof(UnitTestsWindow), true);
            window.Show();
        }
    }
}