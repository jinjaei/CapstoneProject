using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*
 * Make a button holdeable.
 * You can change how often OnClick event will be generated when holding a button.
 * 
 */ 
namespace IV_Demo
{
    [RequireComponent(typeof(Button))]
    public class HoldableButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        // public fields
        [Tooltip("Minimum clicking time before we start calling the button OnClick event.")]
        public float minClickTime;
        [Tooltip("How many time per seconds will we invoke OnClick.")]
        public MinMax clickRateBounds = new MinMax(1, 1, false);
        [Tooltip("How quickly will the click rate increase per seconds.")]
        public float clickRateGain;
        [Space]
        [Tooltip("What color should we apply to the button target graphic when holding.")]
        public Color crossFadeColor = Color.white;

        // private fields
        Button button;
        ButtonSound sounds;

        bool startedClicking;
        float clickStartTime = -1;

        float currentClickRate = 0f;
        float bufferedTime = 0f;

        // private methods
        void StartHolding()
        {
            if (!button.interactable || !button.enabled)
                return;

            startedClicking = true;
            clickStartTime = Time.time;

            currentClickRate = 0;
            bufferedTime = 0;
        }

        void StopHolding()
        {
            if (!startedClicking)
                return;

            button.enabled = true;
            startedClicking = false;
        }

        // unity messages
        void Update()
        {
            // do nothing if we are not holding
            if (!startedClicking)
                return;

            // interrupt if the button becomes not interactable
            if (!button.interactable)
            {
                StopHolding();
                sounds?.IgnoreNext();
                return;
            }

            // wait for min click time
            if (clickStartTime >= 0 && Time.time - clickStartTime >= minClickTime)
            {
                clickStartTime = -1;
                currentClickRate = clickRateBounds.min;

                button.enabled = false;
                button.targetGraphic.CrossFadeColor(crossFadeColor, button.colors.fadeDuration, false, true);
            }

            // when holding
            if (clickStartTime < 0)
            {
                bool clickedOnce = false;

                bufferedTime += Time.deltaTime;

                while (bufferedTime >= 1f / currentClickRate)
                {
                    button.onClick.Invoke();
                    clickedOnce = true;

                    bufferedTime -= 1f / currentClickRate;
                }

                currentClickRate = clickRateBounds.Clamp(currentClickRate + Time.deltaTime * clickRateGain);

                if (clickedOnce && sounds != null)
                    Audio.PlaySound(sounds.onClickSound);
            }
        }

        void Awake()
        {
            button = GetComponent<Button>();
            sounds = GetComponent<ButtonSound>();
        }

        void OnValidate()
        {
            if (minClickTime < 0)
                minClickTime = 0;

            if (clickRateBounds.min < 0.0001f)
                clickRateBounds.min = 0.0001f;
            if (clickRateBounds.max < 0.0001f)
                clickRateBounds.max = 0.0001f;
        }

        // interface implementations
        public void OnPointerDown(PointerEventData eventData)
        {
            StartHolding();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            bool isStillHovering = startedClicking;

            StopHolding();

            if (isStillHovering)
            {
                button.targetGraphic.CrossFadeColor(button.colors.highlightedColor, button.colors.fadeDuration, false, true);

                if (clickStartTime < 0)
                    sounds?.IgnoreNext();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            StopHolding();
        }
    }
}