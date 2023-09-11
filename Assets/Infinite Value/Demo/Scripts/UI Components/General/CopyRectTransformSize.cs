using UnityEngine;

/*
 * Copy another RectTransform size.
 * This will work in edit mode.
 * 
 */ 
namespace IV_Demo
{
    [ExecuteInEditMode]
    public class CopyRectTransformSize : MonoBehaviour
    {
        public RectTransform toCopy;
        public bool copyWidth;
        public bool copyHeight;
        public Vector2 extraSize;

        RectTransform _myRect;
        RectTransform myRect
        {
            get
            {
                if (_myRect == null)
                    _myRect = transform as RectTransform;
                return _myRect;
            }
        }

        void Update()
        {
            if (toCopy == null)
                return;

            if (copyWidth)
                myRect.sizeDelta = new Vector2(toCopy.rect.width + extraSize.x, myRect.sizeDelta.y);
            if (copyHeight)
                myRect.sizeDelta = new Vector2(myRect.sizeDelta.x, toCopy.rect.height + extraSize.y);
        }
    }
}