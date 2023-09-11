using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*
 * Component that will play a sound when a button is clicked.
 * The sound is different if the button is interactable or not.
 * No sound will be played if the button is disabled.
 * 
 */
namespace IV_Demo
{
    public class ButtonSound : MonoBehaviour, IPointerClickHandler
    {
        public Button targetButton;

        public Audio.Sound onClickSound;
        public Audio.Sound offClickSound;

        bool isInteractable = true;
        bool ignoreNext = false;

        public void IgnoreNext() => ignoreNext = true;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (ignoreNext)
            {
                ignoreNext = false;
                return;
            }

            if (targetButton == null || !targetButton.enabled)
                return;

            Audio.PlaySound(isInteractable ? onClickSound : offClickSound);
        }

        void LateUpdate()
        {
            isInteractable = targetButton.interactable;
        }

        void Reset()
        {
            targetButton = GetComponent<Button>();
        }

        void Awake()
        {
            if (targetButton == null)
                targetButton = GetComponent<Button>();
        }
    }
}