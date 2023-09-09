using UnityEngine;
using UnityEngine.EventSystems;

/*
 * Add a tooltip to an UI Object.
 * This Component is used by the TooltipWindow to create tooltips appearing when hovering over the Object.
 * 
 */ 
namespace IV_Demo
{
    public class ToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // public fields
        [TextArea(2, 10)] public string text;
        public bool defaultToUnder = true;

        // editor only
        [SerializeField] RectTransform _doNotCoverRectTransform;

        // public properties
        public RectTransform doNotCoverRectTransform
        {
            get
            {
                if (_doNotCoverRectTransform != null)
                    return _doNotCoverRectTransform;
                return transform as RectTransform;
            }
            set { _doNotCoverRectTransform = value; }
        }

        // events
        public void OnPointerEnter(PointerEventData eventData)
        {
            tooltipWindow.Open(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            tooltipWindow.Close(this);
        }

        // internal logic
        TooltipWindow tooltipWindow;

        // unity
        void Awake()
        {
            tooltipWindow = FindObjectOfType<TooltipWindow>();
        }

        void OnDisable()
        {
            tooltipWindow.Close(this);
        }
    }
}