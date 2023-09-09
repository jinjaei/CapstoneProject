using UnityEngine;
using UnityEngine.UI;

/*
 * Simple Component that will move the center border to the correct position.
 * 
 */ 
namespace IV_Demo
{
    [ExecuteInEditMode]
    public class Borders : MonoBehaviour
    {
        public LayoutElement layoutElement;
        public RectTransform midSepRect;
        public float decalX;

        RectTransform myRect => transform as RectTransform;

        void OnRectTransformDimensionsChange()
        {
            midSepRect.anchoredPosition = new Vector3(myRect.rect.width * (layoutElement.flexibleWidth / (layoutElement.flexibleWidth + 1)) + decalX, 
                midSepRect.anchoredPosition.y);
        }
    }
}