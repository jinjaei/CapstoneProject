using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class LoginBase : MonoBehaviour
{
    /**
    * 입력오류가 있는 InputField의 색상 변경
    * 오류에 대한 메시지 출력
    **/
    protected void GuideForIncorrectlyEnteredData(Image image)
    {
        image.color = new Color(1, 0.4f, 0.4f);
    }

    //필드가 비어있는지 확인
    protected bool IsFieldDataEmpty(Image image, string field, string result)
    {
        if ( field.Trim().Equals("") )
        {
            GuideForIncorrectlyEnteredData(image);
            ToastMessage.I.ShowToastMessage($"\"{result}\"를 입력해 주세요.", ToastMessage.ToastLength.Short);
            return true;
        }

        return false;
    }
}
