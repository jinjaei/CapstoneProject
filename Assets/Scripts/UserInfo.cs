using UnityEngine;
using BackEnd;
using LitJson;

public class UserInfo : MonoBehaviour
{
    [System.Serializable]
    public class UserInfoEvent : UnityEngine.Events.UnityEvent { }
    public UserInfoEvent onUserInfoEvent = new UserInfoEvent();

    private static UserInfoData data = new UserInfoData();
    public static UserInfoData Data => data;

    public void GetUserInfoFromBackend()
    {
        //현재 로그인한 사용자 정보 불러오기
        Backend.BMember.GetUserInfo(callback =>
        {
            // 정보 불러오기 성공
            if (callback.IsSuccess())
            {
                // Json 데이터 파싱 성공
                try
                {
                    JsonData json = callback.GetReturnValuetoJSON()["row"];

                    //// "gamerId" 키가 존재하는지 확인
                    //if (json.Keys.Contains("gamerId"))
                    //{
                    //    // 존재하는 경우에만 값을 가져옴
                    //    data.gamerId = json["gamerId"].ToString();
                    //}
                    //else
                    //{
                    //    // 키가 존재하지 않는 경우 처리
                    //    Debug.LogError("Key 'gamerId' not found in JSON data");
                    //}

                    data.gamerId = json["gamerId"].ToString();
                    data.inData = json["inDate"].ToString();
                    data.subscriptionType = json["subscriptionType"].ToString();
                    //data.federationId = json["federationId"].ToString();
                }

                //json 데이터 파싱 실패
                catch (System.Exception e)
                {
                    //유저 정보를 기본 상태로 설정
                    data.Reset();
                    //try-catach 에러 출력
                    Debug.LogError(e);
                }
            }
            // 정보 불러오기 실패
            else
            {
                //유저 정보를 기보 상태로 설정
                // Tip 일반적으로 오프라인 상태를 대비해 기본적인 정보를 저장해두고 오프라인일때 불러와서 사용
                data.Reset();
                Debug.LogError(callback.GetMessage());
            }

            //유저 정보 부러오기가 완료되었을 때 onUserInFoEvent에 등록되어있는 이벤트메소드 호출
            onUserInfoEvent?.Invoke();
        });
    }
}

public class UserInfoData
{
    public string gamerId;              //유저의 ID
    public string inData;               //유저의 데이터
    public string federationId;         //커스텀, 페더레이션 타입
    public string subscriptionType;     //구글, 에플, 페이스북 패더레이션 ID, 커스텀 계정은 null

    public void Reset()
    {
        gamerId = "Offline";
        inData = string.Empty;
        subscriptionType = string.Empty;
        //federationId = string.Empty;
    }
}