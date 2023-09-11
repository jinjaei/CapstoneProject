using UnityEngine;
using UnityEngine.UI;

/*
 * Import and export save menus.
 * This Component will call the SaveAndLoad manager.
 * 
 */
namespace IV_Demo
{
    public class SaveWindows : MonoBehaviour
    {
        // editor
        public string copyToClipBoardStr = "Copy to clipboard";
        public string copiedStr = "Copied !";
        [Space]
        public string resetSaveStr = "Reset the save";
        public string importStr = "Import this save";
        [Space]
        [TextArea(2, 10)]
        public string confirmationFormat = "Are you sure you want to {0} ? " +
            "This is undoable, make sure to get a copy of the current save first.";

        // public methods
        public void OpenExport()
        {
            gameObject.SetActive(true);
            confirmationObj.SetActive(false);

            isInExportMode = true;
            coverTransform.SetSiblingIndex(0);

            inputField.readOnly = true;
            saveAtStart = SaveAndLoad.Export();
            inputField.text = saveAtStart;

#if UNITY_WEBGL && !UNITY_EDITOR
            mainButton.gameObject.SetActive(false);
            (inputField.transform as RectTransform).offsetMin = new Vector2((inputField.transform as RectTransform).offsetMin.x, 10);
#endif

            mainButton.onClick.RemoveAllListeners();
            mainButton.onClick.AddListener(() =>
            {
                GUIUtility.systemCopyBuffer = saveAtStart;
            });
        }

        public void OpenImport()
        {
            gameObject.SetActive(true);
            confirmationObj.SetActive(false);

            isInExportMode = false;
            coverTransform.SetSiblingIndex(0);

            inputField.readOnly = false;
            inputField.text = string.Empty;

            buttonText.text = resetSaveStr;

#if UNITY_WEBGL && !UNITY_EDITOR
            mainButton.gameObject.SetActive(true);
            (inputField.transform as RectTransform).offsetMin = new Vector2((inputField.transform as RectTransform).offsetMin.x, 70);
#endif

            mainButton.onClick.RemoveAllListeners();
            mainButton.onClick.AddListener(() =>
            {
                confirmationText.text = string.Format(confirmationFormat, buttonText.text.ToLower());

                coverTransform.SetSiblingIndex(1);
                confirmationObj.SetActive(true);
            });
        }

        // private fields
        Transform coverTransform;
        GameObject confirmationObj;
        Text confirmationText;

        InputField inputField;
        Button mainButton;
        Text buttonText;
        bool isInExportMode = false;

        string saveAtStart;

        // unity
        void Update()
        {
            if (!isInExportMode)
                return;

            buttonText.text = (GUIUtility.systemCopyBuffer == saveAtStart ? copiedStr : copyToClipBoardStr);
        }

        void Awake()
        {
            coverTransform = transform.Find("Back Cover");
            confirmationObj = transform.Find("Confirmation Window").gameObject;
            confirmationText = transform.Find("Confirmation Window/Text").GetComponent<Text>();

            inputField = transform.Find("Import-Export Window/InputField").GetComponent<InputField>();
            mainButton = transform.Find("Import-Export Window/Button").GetComponent<Button>();
            buttonText = transform.Find("Import-Export Window/Button/Text").GetComponent<Text>();

            coverTransform.GetComponent<Button>().onClick.AddListener(() => gameObject.SetActive(false));
            inputField.onValueChanged.AddListener(OnValueChanged);

            transform.Find("Confirmation Window/Buttons/Confirm").GetComponent<Button>().onClick.AddListener(() =>
            {
                if (string.IsNullOrEmpty(inputField.text))
                    SaveAndLoad.Reset();
                else
                {
                    try { SaveAndLoad.Import(inputField.text); }
                    catch { SaveAndLoad.Reset(); }
                }
                gameObject.SetActive(false);
            });
            transform.Find("Confirmation Window/Buttons/Cancel").GetComponent<Button>().onClick.AddListener(() =>
            {
                coverTransform.SetSiblingIndex(0);
                confirmationObj.SetActive(false);
            });

            confirmationObj.SetActive(false);
            gameObject.SetActive(false);
        }

        // private methods
        void OnValueChanged(string text)
        {
            if (isInExportMode)
                return;

            buttonText.text = (string.IsNullOrEmpty(text) ? resetSaveStr : importStr);
        }
    }
}