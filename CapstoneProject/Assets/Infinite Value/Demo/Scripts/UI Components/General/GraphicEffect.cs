using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/*
 * Apply effects to a UI.Graphic component.
 * The effects are made using the CrossFadeColor method.
 * They can be a simple color change, a singular flash, or repeating flashes (blink).
 * Their will be a smooth transition between effects.
 * 
 */
namespace IV_Demo
{
    public class GraphicEffect : MonoBehaviour
    {
        const float invalidTimeDefault = 1;

        // editor
        public float defaultTransitionTime = 0.15f;
        public float defaultColoredStandByTime = 0.1f;
        public float defaultOffStandByTime = 0.05f;
        [Space]
        public Color clearColor = Color.white;

        // public access
        public void Clear(float transitionTime = -1)
        {
            if (!Application.isPlaying)
                Debug.LogError("Cannot Clear in editor");

            GetUsedTimes(ref transitionTime);

            StopAllCoroutines();
            graphic.CrossFadeColor(clearColor, transitionTime, false, true);
        }

        public void Cover(Color color, bool useAlpha = true, bool useRGB = true, float transitionTime = -1)
        {
            if (!Application.isPlaying)
                Debug.LogError("Cannot Cover in editor");

            GetUsedTimes(ref transitionTime);

            StopAllCoroutines();
            graphic.CrossFadeColor(color, transitionTime, false, useAlpha, useRGB);
        }

        public void Blink(Color color, bool useAlpha = true, bool useRGB = true, int maxCycle = 0,
            float transitionTime = -1, float standbyColoredTime = -1, float standbyOffTime = -1)
        {
            if (!Application.isPlaying)
                Debug.LogError("Cannot Blink in editor");

            GetUsedTimes(ref transitionTime, ref standbyColoredTime, ref standbyOffTime);

            StopAllCoroutines();
            StartCoroutine(BlinkRoutine(color, useAlpha, useRGB, maxCycle, transitionTime, standbyColoredTime, standbyOffTime));
        }

        public void Flash(Color color, bool useAlpha = true, bool useRGB = true, float transitionTime = -1, float standbyColoredTime = -1)
        {
            if (!Application.isPlaying)
                Debug.LogError("Cannot Flash in editor");

            GetUsedTimes(ref transitionTime, ref standbyColoredTime);

            StopAllCoroutines();
            StartCoroutine(FlashRoutine(color, useAlpha, useRGB, transitionTime, standbyColoredTime));
        }

        // internal logic
        Graphic _graphic;
        Graphic graphic
        {
            get
            {
                if (_graphic == null)
                    _graphic = GetComponent<Graphic>();
                return _graphic;
            }
        }

        IEnumerator BlinkRoutine(Color color, bool useAlpha, bool useRGB, int maxCycle,
            float transitionTime, float standbyColoredTime, float standbyOffTime)
        {
            int cycle = 0;
            while (maxCycle <= 0 || cycle < maxCycle)
            {
                graphic.CrossFadeColor(color, transitionTime, false, useAlpha, useRGB);
                yield return new WaitForSeconds(transitionTime + standbyColoredTime);
                graphic.CrossFadeColor(clearColor, transitionTime, false, useAlpha, useRGB);
                yield return new WaitForSeconds(transitionTime + standbyOffTime);
                cycle++;
            }
        }

        IEnumerator FlashRoutine(Color color, bool useAlpha, bool useRGB, float transitionTime, float standbyColoredTime)
        {
            graphic.CrossFadeColor(color, transitionTime, false, useAlpha, useRGB);
            yield return new WaitForSeconds(transitionTime + standbyColoredTime);
            graphic.CrossFadeColor(clearColor, transitionTime, false, useAlpha, useRGB);
        }

        void GetUsedTimes(ref float transitionTime)
        {
            SetOneTime(ref transitionTime, defaultTransitionTime);
        }

        void GetUsedTimes(ref float transitionTime, ref float standbyColoredTime)
        {
            SetOneTime(ref transitionTime, defaultTransitionTime);
            SetOneTime(ref standbyColoredTime, defaultColoredStandByTime);
        }

        void GetUsedTimes(ref float transitionTime, ref float standbyColoredTime, ref float standbyOffTime)
        {
            SetOneTime(ref transitionTime, defaultTransitionTime);
            SetOneTime(ref standbyColoredTime, defaultColoredStandByTime);
            SetOneTime(ref standbyOffTime, defaultOffStandByTime);
        }

        void SetOneTime(ref float time, float defaultTime)
        {
            time = time >= 0 ? time : (defaultTime >= 0 ? defaultTime : invalidTimeDefault);
        }

        // unity
        void OnEnable()
        {
            if (graphic != null)
                graphic.CrossFadeColor(clearColor, 0, false, true);
        }

        void Start()
        {
            graphic.CrossFadeColor(clearColor, 0, false, true);
        }

        void OnValidate()
        {
            graphic.CrossFadeColor(clearColor, 0, false, true);
        }
    }
}