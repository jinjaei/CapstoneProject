using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToastMessage : MonoBehaviour
{
    public static ToastMessage I
    {
        get
        {
            if (instance == null) // Ã¼Å© 1 : ÀÎ½ºÅÏ½º°¡ ¾ø´Â °æ¿ì
                CheckExsistence();

            return instance;
        }
    }

    // ½Ì±ÛÅæ ÀÎ½ºÅÏ½º
    private static ToastMessage instance;

    // ½Ì±ÛÅæ ÀÎ½ºÅÏ½º Á¸Àç ¿©ºÎ È®ÀÎ (Ã¼Å© 2)
    private static void CheckExsistence()
    {
        // ½Ì±ÛÅæ °Ë»ö
        instance = FindObjectOfType<ToastMessage>();

        // ÀÎ½ºÅÏ½º °¡Áø ¿ÀºêÁ§Æ®°¡ Á¸ÀçÇÏÁö ¾ÊÀ» °æ¿ì, ºó ¿ÀºêÁ§Æ®¸¦ ÀÓÀÇ·Î »ý¼ºÇÏ¿© ÀÎ½ºÅÏ½º ÇÒ´ç
        if (instance == null)
        {
            // ºó °ÔÀÓ ¿ÀºêÁ§Æ® »ý¼º
            GameObject container = new GameObject("AndroidToast Singleton Container");

            // °ÔÀÓ ¿ÀºêÁ§Æ®¿¡ Å¬·¡½º ÄÄÆ÷³ÍÆ® Ãß°¡ ÈÄ ÀÎ½ºÅÏ½º ÇÒ´ç
            instance = container.AddComponent<ToastMessage>();
        }
    }

    private void CheckInstance()
    {
        // ½Ì±ÛÅæ ÀÎ½ºÅÏ½º°¡ Á¸ÀçÇÏÁö ¾Ê¾ÒÀ» °æ¿ì, º»ÀÎÀ¸·Î ÃÊ±âÈ­
        if (instance == null)
            instance = this;

        // ½Ì±ÛÅæ ÀÎ½ºÅÏ½º°¡ Á¸ÀçÇÏ´Âµ¥, º»ÀÎÀÌ ¾Æ´Ò °æ¿ì, ½º½º·Î(ÄÄÆ÷³ÍÆ®)¸¦ ÆÄ±«
        if (instance != null && instance != this)
        {
            Debug.Log("ÀÌ¹Ì AndroidToast ½Ì±ÛÅæÀÌ Á¸ÀçÇÏ¹Ç·Î ¿ÀºêÁ§Æ®¸¦ ÆÄ±«ÇÕ´Ï´Ù.");
            Destroy(this);

            // ¸¸¾à °ÔÀÓ ¿ÀºêÁ§Æ®¿¡ ÄÄÆ÷³ÍÆ®°¡ ÀÚ½Å¸¸ ÀÖ¾ú´Ù¸é, °ÔÀÓ ¿ÀºêÁ§Æ®µµ ÆÄ±«
            var components = gameObject.GetComponents<Component>();

            if (components.Length <= 2)
                Destroy(gameObject);
        }
    }

    private void Awake()
    {
        CheckInstance();
    }

    public enum ToastLength
    {
        /// <summary> ¾à 2.5ÃÊ </summary>
        Short,
        /// <summary> ¾à 4ÃÊ </summary>
        Long
    };

#if UNITY_EDITOR
    private float __editorGuiTime = 0f;
    private string __editorGuiMessage;

#elif UNITY_ANDROID

    private AndroidJavaClass _unityPlayer;
    private AndroidJavaObject _unityActivity;
    private AndroidJavaClass _toastClass;

    private void Start()
    {
        _unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        _unityActivity = _unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        _toastClass = new AndroidJavaClass("android.widget.Toast");
    }
#endif

    /// <summary> ¾Èµå·ÎÀÌµå Åä½ºÆ® ¸Þ½ÃÁö Ç¥½ÃÇÏ±â </summary>
    [System.Diagnostics.Conditional("UNITY_ANDROID")]
    public void ShowToastMessage(string message, ToastLength length = ToastLength.Short)
    {
#if UNITY_EDITOR
        __editorGuiTime = length == ToastLength.Short ? 2.5f : 4f;
        __editorGuiMessage = message;

#elif UNITY_ANDROID
        if (_unityActivity != null)
        {
            _unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = _toastClass.CallStatic<AndroidJavaObject>("makeText", _unityActivity, message, (int)length);
                toastObject.Call("show");
            }));
        }
#endif
    }

#if UNITY_EDITOR
    /* À¯´ÏÆ¼ ¿¡µðÅÍ IMGUI¸¦ ÅëÇØ Åä½ºÆ® ¸Þ½ÃÁö Ç¥½Ã ¸ð¹æÇÏ±â */

    private GUIStyle toastStyle;
    private void OnGUI()
    {
        if (__editorGuiTime <= 0f) return;

        float width = Screen.width * 0.5f;
        float height = Screen.height * 0.08f;
        Rect rect = new Rect((Screen.width - width) * 0.5f, Screen.height * 0.8f, width, height);

        if (toastStyle == null)
        {
            toastStyle = new GUIStyle(GUI.skin.box);
            toastStyle.fontSize = 36;
            toastStyle.fontStyle = FontStyle.Bold;
            toastStyle.alignment = TextAnchor.MiddleCenter;
            toastStyle.normal.textColor = Color.white;
        }

        GUI.Box(rect, __editorGuiMessage, toastStyle);
    }
    private void Update()
    {
        if (__editorGuiTime > 0f)
            __editorGuiTime -= Time.unscaledDeltaTime;
    }
#endif
}
