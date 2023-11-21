using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BackEnd;

public class Login : LoginBase
{
    [SerializeField]
    private Image imageID; //ID필드 색상 변경
    [SerializeField]
    private TMP_InputField inputFieldID; //ID필드 텍스트 정보 추출
    [SerializeField]
    private Image imagePW; //ID필드 색상 변경
    [SerializeField]
    private TMP_InputField inputFieldPW; //ID필드 텍스트 정보 추출

    [SerializeField]
    private Button btnLogin; //로그인 버튼(상호작용 가능/불가능)

    //"로그인"버튼을 눌렀을 때 호출
    public void OnClickLogin()
    {
        //매개 변수로 입력한 InputField UI의 색상과 Message 내용 초기화
        ResetUI(imageID, imagePW);

        //필드값이 비어있는 지 체크
        if (IsFieldDataEmpty(imageID, inputFieldID.text, "아이디")) return;
        if (IsFieldDataEmpty(imagePW, inputFieldPW.text, "비밀번호  ")) return;

        //로그인 버튼을 연타하지 못하도록 상호작용 비활성화
        btnLogin.interactable = false;

        // 서버에 로그인을 요청하는 동안 화면에 출력하는 내용 업데이트
        StartCoroutine(nameof(LoginProgress));

        // 뒤끝 서버 로그인 시도
        ResponseToLogin(inputFieldID.text, inputFieldPW.text);
    }


    //로그인 시도후 서버로부터 전달받은 message를 기반으로 로직 처리
    private void ResponseToLogin(string ID, string PW)
    {
        // 서버에 로그인 요청(비동기)
        Backend.BMember.CustomLogin(ID, PW, callback =>
        {
            StopCoroutine(nameof(LoginProgress));

            //로그인 성공
            if (callback.IsSuccess() )
            {
                SetMesssage($"{inputFieldID.text}님 환영합니다.");
            }

            //로그인 실패
            else
            {
                //로그인 버튼 상호작용 활성화
                btnLogin.interactable = true;

                string message = string.Empty;

                switch ( int.Parse(callback.GetStatusCode()) )
                {
                    case 401: //존재하지 않는 아이디. 잘못된 비밀번호
                        message = callback.GetMessage().Contains("customid") ? "존재하지않은 아이디입니다." : "잘못된 비밀번호입니다.";
                        break;
                    default:
                        message = callback.GetMessage();
                        break;
                }

                if ( message.Contains("비밀번호") )
                {
                    GuideForIncorrectlyEnteredData(imagePW, message);
                }
                else
                {
                    GuideForIncorrectlyEnteredData(imageID, message);
                }
            }
        });
    }

    private IEnumerator LoginProgress()
    {
        float time = 0;

        while (true)
        {
            time += Time.deltaTime;

            SetMesssage($"로그인 중입니다...{time:F1}");

            yield return null;
        }
    }
}
