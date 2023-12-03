using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BackEnd;
using UnityEngine.SceneManagement;

public class RegisterAccount : LoginBase
{
    [SerializeField]
    private Image imageID; // ID 필드 색상변경
    [SerializeField]
    private TMP_InputField inputFieldID; // ID 필드 텍스트 정보 추출
    [SerializeField]
    private Image imagePW; // PW 필드 색상변경
    [SerializeField]
    private TMP_InputField inputFieldPW; // PW 필드 텍스트 정보 추출
    [SerializeField]
    private Image imageConfirmPW; // 필드 색상 변경
    [SerializeField]
    private TMP_InputField inputFieldConfirmPW; // 필두 택스트 정보 추출

    [SerializeField]
    private Button btnRegisterAccount; // "계정 생성" 버튼 (상호 작용 가능/ 불가능)
    [SerializeField]
    private Button CancelButton; //뒤로가기

    // "계정 생성" 버튼을 눌렀을 때 호출
    public void OnClickRegisterAccount()
    {
        // 필드 값이 비어있는지 체크
        if (IsFieldDataEmpty(imageID, inputFieldID.text, "아이디")) return;
        if (IsFieldDataEmpty(imagePW, inputFieldPW.text, "비밀번호")) return;
        if (IsFieldDataEmpty(imageConfirmPW, inputFieldConfirmPW.text, "비밀번호")) return;

        // 비밀번호와 비밀번호 확인의 내용이 다를때 
        if (!inputFieldPW.text.Equals(inputFieldConfirmPW.text))
        {
            GuideForIncorrectlyEnteredData(imageConfirmPW);
            ToastMessage.I.ShowToastMessage("비밀번호가 일치하지 않습니다.", ToastMessage.ToastLength.Short);
            return;
        }

        // 버튼 비활성화
        btnRegisterAccount.interactable = false;
        ToastMessage.I.ShowToastMessage("계정 생성중..", ToastMessage.ToastLength.Short);

        //뒤끝 서버 계정 생성 시도
        CustomSignUp();
    }

    // 계정 생성 시도후 서버로부터 전달 받은 message를 기반으로 로직 처리
    private void CustomSignUp()
    {
        Backend.BMember.CustomSignUp(inputFieldID.text, inputFieldPW.text, callback =>
        {
            btnRegisterAccount.interactable = true;

            // 계정 생성 성공
            if ( callback.IsSuccess() )
            {
                ToastMessage.I.ShowToastMessage("회원가입 완료", ToastMessage.ToastLength.Short);

                BackendGameData.Instance.GameDataInsert();
            }

            // 계정 생성 실패
            else
            {
                string message = string.Empty;

                switch(int.Parse(callback.GetStatusCode()) )
                {
                    case 409: // 중복된 ID가 존재하는 경우
                        message = "이미 존재하는 아이디입니다.";
                        GuideForIncorrectlyEnteredData(imageID);
                        ToastMessage.I.ShowToastMessage(message, ToastMessage.ToastLength.Short);
                        break;
                    case 401: //프로젝트 상태가 '점검'일 경우
                    case 400: //디바이스 정보가 null인경우
                    default:
                        message = callback.GetMessage();
                        ToastMessage.I.ShowToastMessage(message, ToastMessage.ToastLength.Short);
                        break;
                }
            }
        });
    }
}
