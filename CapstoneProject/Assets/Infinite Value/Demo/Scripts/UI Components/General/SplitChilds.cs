using UnityEngine;
using InspectorAttribute;

/*
 * Split 2 or 3 childs of an object automatically.
 * It is to be used on the parent of UI objects set to full stretch.
 * It is similar to a LayoutGroup but doesn't consider the content of the objects.
 * This will work in edit mode.
 * 
 */
namespace IV_Demo
{
    [ExecuteAlways]
    public class SplitChilds : MonoBehaviour
    {
        enum Orientation
        {
            Horizontal,
            Vertical,
        }

        enum Mode
        {
            Duo,
            SquareFirst,
            SquareLast,
            Trio,
        }

        [SerializeField] Orientation orientation = Orientation.Horizontal;
        [SerializeField] Mode mode = Mode.Duo;
        [SerializeField] float spacing = 0;
        [SerializeField] string firstName = "First";
        [ConditionalHide("!mode", (int)Mode.Trio, HideType.Hide)]
        [SerializeField] string midName = "Mid";
        [SerializeField] string lastName = "Last";

        RectTransform rectTransform => transform as RectTransform;

        RectTransform firstRect => transform.Find(firstName) as RectTransform;
        RectTransform midRect => transform.Find(midName) as RectTransform;
        RectTransform lastRect => transform.Find(lastName) as RectTransform;

        void OnRectTransformDimensionsChange()
        {
            if (orientation == Orientation.Horizontal)
            {
                if (mode == Mode.Duo)
                {
                    firstRect.offsetMax = new Vector2(-rectTransform.rect.width / 2f - spacing / 2f, firstRect.offsetMax.y);

                    lastRect.offsetMin = new Vector2(rectTransform.rect.width / 2f + spacing / 2f, lastRect.offsetMin.y);
                }
                else if (mode == Mode.SquareFirst)
                {
                    firstRect.offsetMax = new Vector2(-rectTransform.rect.width + firstRect.rect.height, firstRect.offsetMax.y);

                    lastRect.offsetMin = new Vector2(firstRect.rect.height + spacing / 2f, lastRect.offsetMin.y);
                }
                else if (mode == Mode.SquareLast)
                {
                    firstRect.offsetMax = new Vector2(-lastRect.rect.height - spacing / 2f, firstRect.offsetMax.y);

                    lastRect.offsetMin = new Vector2(rectTransform.rect.width - lastRect.rect.height, lastRect.offsetMin.y);
                }
                else // (mode == Mode.Trio)
                {
                    firstRect.offsetMax = new Vector2(-rectTransform.rect.width * 2f / 3f - spacing * 2f / 3f, firstRect.offsetMax.y);

                    midRect.offsetMax = new Vector2(-rectTransform.rect.width / 3f - spacing / 3f, midRect.offsetMax.y);
                    midRect.offsetMin = new Vector2(rectTransform.rect.width / 3f + spacing / 3f, midRect.offsetMin.y);

                    lastRect.offsetMin = new Vector2(rectTransform.rect.width * 2f / 3f + spacing * 2f / 3f, lastRect.offsetMin.y);
                }
            }
            else // (orientation == Orientation.Vertical)
            {
                if (mode == Mode.Duo)
                {
                    firstRect.offsetMin = new Vector2(firstRect.offsetMin.x, rectTransform.rect.height / 2f + spacing / 2f);

                    lastRect.offsetMax = new Vector2(lastRect.offsetMax.x, -rectTransform.rect.height / 2f - spacing / 2f);
                }
                else if (mode == Mode.SquareFirst)
                {
                    firstRect.offsetMin = new Vector2(firstRect.offsetMin.x, rectTransform.rect.height - firstRect.rect.width);

                    lastRect.offsetMax = new Vector2(lastRect.offsetMax.x, -firstRect.rect.width - spacing / 2f);
                }
                else if (mode == Mode.SquareLast)
                {
                    firstRect.offsetMin = new Vector2(firstRect.offsetMin.x, lastRect.rect.width + spacing / 2f);

                    lastRect.offsetMax = new Vector2(lastRect.offsetMax.x, -rectTransform.rect.height + lastRect.rect.width);
                }
                else // (mode == Mode.Trio)
                {
                    firstRect.offsetMin = new Vector2(firstRect.offsetMin.x, rectTransform.rect.height * 2f / 3f + spacing * 2f / 3f);

                    midRect.offsetMin = new Vector2(midRect.offsetMin.x, rectTransform.rect.height / 3f + spacing / 3f);
                    midRect.offsetMax = new Vector2(midRect.offsetMax.x, -rectTransform.rect.height / 3f - spacing / 3f);

                    lastRect.offsetMax = new Vector2(lastRect.offsetMax.x, -rectTransform.rect.height * 2f / 3f - spacing * 2f / 3f);
                }
            }
        }

        void OnValidate()
        {
            OnRectTransformDimensionsChange();
        }
    }
}