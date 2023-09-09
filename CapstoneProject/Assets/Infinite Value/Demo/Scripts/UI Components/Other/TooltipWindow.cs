using UnityEngine;
using UnityEngine.UI;

/*
 * Show a tooltip when hovering over an object.
 * This script is called by Tooltip components to display a window with a text.
 * 
 */ 
namespace IV_Demo
{
    public class TooltipWindow : MonoBehaviour
    {
        // editor
        [SerializeField] RectTransform backgroundRect = default;
        [SerializeField] Text text = default;
        [Space]
        [SerializeField] float appearDelay = 0.15f;

        // public access
        public void Open(ToolTip toolTip)
        {
            if (currentToolTip == toolTip)
                return;

            // base init
            backgroundRect.gameObject.SetActive(true);
            backgroundRect.localScale = Vector3.zero;

            appearTime = Time.time;
            text.text = toolTip.text;

            // set box size
            backgroundRect.sizeDelta = new Vector2(
                Mathf.Min(rectTransform.rect.width, toolTip.doNotCoverRectTransform.rect.width, text.preferredWidth + 40),
                backgroundRect.sizeDelta.y);

            // set currentTooltip
            currentToolTip = toolTip;
        }

        public void Close(ToolTip toolTip)
        {
            if (currentToolTip != toolTip)
                return;

            backgroundRect.gameObject.SetActive(false);
            appearTime = -1;

            currentToolTip = null;
        }

        // internal logic
        RectTransform rectTransform;

        ToolTip currentToolTip = null;

        float appearTime = -1;

        void SetPosition()
        {
            backgroundRect.position = currentToolTip.doNotCoverRectTransform.position;

            Vector2 anchorPos = backgroundRect.anchoredPosition;

            // if aff over (si pas possible aff under)
            if (!currentToolTip.defaultToUnder)
            {
                if (anchorPos.y + (currentToolTip.doNotCoverRectTransform.sizeDelta.y / 2 + backgroundRect.sizeDelta.y) > rectTransform.rect.height / 2)
                    anchorPos += Vector2.down * (currentToolTip.doNotCoverRectTransform.sizeDelta.y + backgroundRect.sizeDelta.y) / 2;
                else
                    anchorPos += Vector2.up * (currentToolTip.doNotCoverRectTransform.sizeDelta.y + backgroundRect.sizeDelta.y) / 2;
            }
            // if aff under (si pas possible aff over)
            else
            {
                if (anchorPos.y - (currentToolTip.doNotCoverRectTransform.sizeDelta.y / 2 + backgroundRect.sizeDelta.y) < -rectTransform.rect.height / 2)
                    anchorPos += Vector2.up * (currentToolTip.doNotCoverRectTransform.sizeDelta.y + backgroundRect.sizeDelta.y) / 2;
                else
                    anchorPos += Vector2.down * (currentToolTip.doNotCoverRectTransform.sizeDelta.y + backgroundRect.sizeDelta.y) / 2;
            }

            anchorPos.x = Mathf.Clamp(anchorPos.x, (-rectTransform.rect.width + backgroundRect.sizeDelta.x) / 2,
                                                   (rectTransform.rect.width - backgroundRect.sizeDelta.x) / 2);
            anchorPos.y = Mathf.Clamp(anchorPos.y, (-rectTransform.rect.height + backgroundRect.sizeDelta.y) / 2,
                                                   (rectTransform.rect.height - backgroundRect.sizeDelta.y) / 2);
            backgroundRect.anchoredPosition = anchorPos;
        }

        // unity
        void Awake()
        {
            rectTransform = transform as RectTransform;

            Close(null);
        }

        void Update()
        {
            if (currentToolTip != null)
            {
                if (Input.mousePosition.x < 0 || Input.mousePosition.y < 0 || Input.mousePosition.x > Screen.width || Input.mousePosition.y > Screen.height)
                    Close(currentToolTip);
                else if (appearTime >= 0)
                {
                    if (Time.time - appearTime >= appearDelay)
                    {
                        appearTime = -1;
                        backgroundRect.localScale = Vector3.one;
                        SetPosition();
                    }
                }
                else
                    SetPosition();
            }
        }

        void OnApplicationFocus(bool focus)
        {
            if (!focus && currentToolTip != null)
                Close(currentToolTip);
        }
    }
}