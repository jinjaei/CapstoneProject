using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    [SerializeField] Image GameIntroImage = null;
    [SerializeField] Image LogoBackground = null;
    [SerializeField] TextMeshProUGUI Logo = null;
    bool TouchAble = false;

    private void Awake()
    {
        GameIntroImage.enabled = false;
    }

    void Start()
    {
        StartCoroutine(FadeTextToFullAlpha(2f, LogoBackground, Logo));
    }

    private void Update()
    {
        if((Application.platform == RuntimePlatform.Android))
        {
            if (Input.touchCount > 0 && (TouchAble == true))
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    Vector3 pos = Input.GetTouch(0).position;

                    if (pos.y <= Screen.height / 2)
                        SceneManager.LoadScene("GameScene");
                }
            }
        }
    }

    public IEnumerator FadeTextToFullAlpha(float time, Image image, TextMeshProUGUI text)
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);

        while (image.color.a < 1.0f)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a + (Time.deltaTime / time));
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + (Time.deltaTime / time));
            yield return null;
        }

        image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);

        while (image.color.a > 0.0f)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a - (Time.deltaTime / time));
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - (Time.deltaTime / time));
            yield return null;
        }

        GameIntroImage.enabled = true;
        TouchAble = true;
    }
}
