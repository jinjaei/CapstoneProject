using UnityEngine;
using BackEnd;

public class BackendGameData
{
    [System.Serializable]
    public class GameDataLoadEvent : UnityEngine.Events.UnityEvent { }
    public GameDataLoadEvent onGameDataLoadEvent = new GameDataLoadEvent();

    private static BackendGameData instance = null;
    public static BackendGameData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new BackendGameData();
            }

            return instance;
        }
    }

    private UserGameData userGameData = new UserGameData();
    public UserGameData  UserGameData=> userGameData;

    private string gameDataRowInDate = string.Empty;

    // 뒤끝 콘솔 테이블에 새로운 유저 정보 추가
    public void GameDataInsert()
    {
        // 유저 정보를 초기값으로 설정
        userGameData.Reset();

        // 테이블에 추가할 데이터로 가공
        Param param = new Param()
        {
            //{"", userGameData.}, // 공격 속도
            //{"", userGameData.}, // 보석
            //{"", userGameData.}, // 돌가루
            //{"", userGameData.}, // 스테이지
            //{"", userGameData.}  // 비슷하게 부족한 데이터값 설정
        };

        // 첫번 째 매개변수는 뒤끝 콘솔의 게임 정보 관리 탭에 생선한 테이블 이름
        Backend.GameData.Insert("USER_DATA", param, callback =>
        {
            //게임 정보 추가에 성송했을 때
            if ( callback.IsSuccess() )
            {
                // 게임 정보의 고유값
                gameDataRowInDate = callback.GetInDate();
                
                Debug.Log($"게임 정보 데이터 삽입에 성공했습니다. : {callback}");
            }
            //실패했을때
            else
            {
                Debug.LogError($"게임 정보 데이터 삽입에 실패했습니다. : {callback}");
            }
        });
    }

    //뒤끝 콘솔 테이블에서 유저 정보를 불러올 때 호출
    public void GameDataLoad()
    {
        Backend.GameData.GetMyData("USER_DATA", new Where(), callback =>
        {
            // 게임 정보 불러오기에 성공했을 때
            if ( callback.IsSuccess() )
            {
                Debug.Log($"게임 정보 데이터 불러오기에 성공했습니다. : {callback}");

                // JSON 데이터 파싱 성공
                try
                {
                    LitJson.JsonData gameDataJson = callback.FlattenRows();

                    // 받아온 데이터의 개수가 0이면 데이터가 없는 것
                    if ( gameDataJson.Count <= 0 )
                    {
                        Debug.LogWarning("데이터가 존재하지 않습니다.");
                    }
                    else
                    {
                        // 불러온 게임 정보의 고유값
                        gameDataRowInDate = gameDataJson[0]["inDate"].ToString();
                        //불러온 게임 정보를 userGameData 변수에 저장
                        //UserGameData.__ = int.Parse(gameDataJson[0][""].ToString()); //돌가루
                        //UserGameData.__ = float.Parse(gameDataJson[0][""].ToString()); //공격속도
                        //UserGameData.__ = int.Parse(gameDataJson[0][""].ToString()); //보석
                        //UserGameData.__ = int.Parse(gameDataJson[0][""].ToString()); // 공격력
                        //UserGameData.__ = int.Parse(gameDataJson[0][""].ToString()); // ?

                        onGameDataLoadEvent?.Invoke();
                    }
                }

                // JSON 데이터 파싱 실패
                catch ( System.Exception e )
                {
                    // 유저 정보를 초기값으로 설정
                    userGameData.Reset();

                    // try-catch 에러 출력
                    Debug.LogError(e);
                }
            }

            // 실패했을 때
            else
            {
                Debug.LogError($"게임 정보 데이터 불러오기에 실패했습니다. : {callback}");
            }
        });
    }
}