using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class LoginBase : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textMessage;

    // aptlwl sodyd, InputField 색상 초기화
    protected void ResetUI(params Image[] images)
    {
        textMessage.text = string.Empty;

        for (int i = 0; i < images.Length; ++i)
        {
            images[i].color = Color.white;
        }
    }

    //매개변수에 있느 내용을 출력
    protected void SetMesssage(string msg)
    {
        textMessage.text = msg;
    }

    /**
    * 입력오류가 있는 INputField의 색상 변경
    * 오류에 대한 메시지 출력
    **/
    protected void GuideForIncorrectlyEnteredData(Image image, string msg)
    {
        textMessage.text = msg;
        image.color = Color.red;
    }

    //필드가 비어있는 지확인
    protected bool IsFieldDataEmpty(Image image, string field, string result)
    {
        if ( field.Trim().Equals("") )
        {
            GuideForIncorrectlyEnteredData(image, $"\"{result}\" 필드를 채워주세요.");

            return true;
        }

        return false;
    }
}
