using UnityEngine;
using InspectorAttribute;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
#endif

/*
 * 
 * Apply global modifications to every scene Objects.
 * This Manager is useful for making global changes to the scene before building the game.
 * Thing that apply to a lot of objects or that you could forget to toggle when working on the scene.
 * 
 */
namespace IV_Demo
{
    public class GlobalModifier : AManager<GlobalModifier>
    {
        [Header("On/Off GameObjects")]
        public GameObject[] onGameObjects;
        public GameObject[] offGameObjects;
        [ButtonParameter("SetObjects", "Apply")] public InspectorTrigger setObjectsState;

        [Header("Components Parameters")]
        [ButtonParameter("SetScroll", "Apply")] public float scrollSensitivity = 16;
        [ButtonParameter("SetNavigation", "Apply")] public InspectorTrigger clearNavigation;

#if UNITY_EDITOR
        void SetObjects()
        {
            foreach (GameObject go in onGameObjects)
                if (go != null)
                {
                    Undo.RecordObject(go, "SetObjects");
                    go.SetActive(true);
                }

            foreach (GameObject go in offGameObjects)
                if (go != null)
                {
                    Undo.RecordObject(go, "SetObjects");
                    go.SetActive(false);
                }

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        void SetScroll(float scrollSensitivity)
        {
            ScrollRect[] scrolls = GameObject.Find("= UI").GetComponentsInChildren<ScrollRect>(true); 

            foreach (ScrollRect s in scrolls)
            {
                Undo.RecordObject(s, "SetScroll");
                s.scrollSensitivity = scrollSensitivity;
            }

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        void SetNavigation()
        {
            Selectable[] selectables = GameObject.Find("= UI").GetComponentsInChildren<Selectable>(true);

            foreach (Selectable s in selectables)
            {
                Undo.RecordObject(s, "SetNavigation");
                s.navigation = new Navigation() { mode = Navigation.Mode.None };
            }

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
#endif
    }
}