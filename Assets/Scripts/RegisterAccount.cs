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
    private TMP_InputField inputFieldConfirmPW; // 필두 택수투 정보 추출

    [SerializeField]
    private Button btnRegisterAccount; // "계정 생성" 버튼 ( 상호 작용 가능/ 불가능)
    [SerializeField]
    private Button CancelButton; //뒤로가기

    // "계정 생성" 버튼을 눌렀을 때 호출
    public void OnClickRegisterAccount()
    {
        // 매개 변수수로 입력한 색상과 내용 초기화
        ResetUI(imageID, imagePW);

        // 필드 값이 비어있는지 체크
        if (IsFieldDataEmpty(imageID, inputFieldID.text, "아이디")) return;
        if (IsFieldDataEmpty(imagePW, inputFieldPW.text, "비밀번호")) return;
        if (IsFieldDataEmpty(imageConfirmPW, inputFieldConfirmPW.text, "비밀번호")) return;

        // 비밀번호와 비밀번호 확인의 내용이 다를때 
        if (!inputFieldPW.text.Equals(inputFieldConfirmPW.text))
        {
            GuideForIncorrectlyEnteredData(imageConfirmPW, "비밀번호가 일치하지 않습니다.");
            return;
        }

        // 버튼 비활성화
        btnRegisterAccount.interactable = false;
        SetMesssage("계정 생성중입니다..");

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
                SetMesssage($"계정 생성 성공. {inputFieldID.text}님 환영합니다.");

                BackendGameData.Instance.GameDataInsert();
            }

            // 계정 생성 실패
            else
            {
                string message = string.Empty;

                switch(int.Parse(callback.GetStatusCode()) )
                {
                    case 409: // 중복된 ID가 존재하는 겨웅
                        message = "이미 존재하는 아이디입니다.";
                        break;
                    case 401: //프로젝트 상태가 '점검'일 경우
                    case 400: //디바이스 정보가 null인경우
                    default:
                        message = callback.GetMessage();
                        break;
                }

                if (message.Contains("아이디") )
                {
                    GuideForIncorrectlyEnteredData(imageID, message);
                }
                else
                {
                    SetMesssage(message);
                }    
            }
        });
    }
}
