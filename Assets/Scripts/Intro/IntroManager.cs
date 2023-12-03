using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    [SerializeField] AudioSource IntroBGM; // 인트로 배경 음악
    [SerializeField] AudioSource ButtonSFX; // 버튼 클릭 효과음
    [SerializeField] TextMeshProUGUI Logo;
    [SerializeField] TextMeshProUGUI GameStartText;
    [SerializeField] Image LogoBackground;
    [SerializeField] Image GameIntroImage;
    [SerializeField] Image SignInWindow;
    [SerializeField] Image SignUpWindow;
    bool TouchAble = false;

    private void Awake()
    {
        GameIntroImage.enabled = false;
        GameStartText.gameObject.SetActive(false);
    }

    void Start()
    {
        StartCoroutine(FadeTextToFullAlpha(2f, LogoBackground, Logo));
    }

    private void Update()
    {
        if ((Application.platform == RuntimePlatform.Android))
        {
            if (Input.touchCount > 0 && (TouchAble == true))
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    Vector3 pos = Input.GetTouch(0).position;

                    if (pos.y <= Screen.height / 2)
                        SignInWindow.gameObject.SetActive(true);
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
        GameStartText.gameObject.SetActive(true);
        TouchAble = true;
        IntroBGM.Play();
    }
    public void ButtonSFXPlay()
    {
        ButtonSFX.Play();
    }

    public void SignUpSuccess()
    {
        ToastMessage.I.ShowToastMessage("회원가입 완료", ToastMessage.ToastLength.Short);
    }

    public void SignUpCancel()
    {
        SignUpWindow.gameObject.SetActive(false);
    }

    public void SignUpButtonClick()
    {
        SignUpWindow.gameObject.SetActive(true);
    }

    public void SignInSuccess()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void SignInFail()
    {
        ToastMessage.I.ShowToastMessage("아이디 또는 비밀번호가 틀렸습니다", ToastMessage.ToastLength.Short);
    }

    public void WindowExit()
    {
        SignInWindow.gameObject.SetActive(false);
    }
}
