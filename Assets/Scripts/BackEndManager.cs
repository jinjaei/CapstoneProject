using UnityEngine;
using BackEnd;
using TMPro;

public class BackEndManager : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField input;

    private void Awake()
    {
        // Update() 메소드의 Backend.AsyncPoll(); 호출을 위해
        DontDestroyOnLoad(gameObject);

        // 뒤끝 서버 초기화
        BackendSetup();
    }

    private void Update()
    {
    //서버의 비동기 메소드 호출을 위한 작성
         if ( Backend.IsInitialized )
        {
            Backend.AsyncPoll();
       }
    }

    private void BackendSetup()
    {
        //뒤끝 초기화
        var bro = Backend.Initialize(true);

        //뒤끝 초기화에 대한 응답값
        if (bro.IsSuccess())
        {
            //초기화 성공시 문자
            Debug.Log($"초기화 성공 : {bro}");
        }

        else
        {
            //초기화 실패시 문자
            Debug.LogError($"초기화 실패 : {bro}");
        }
    }

    public void GetGoogleHash()
    {
        // 구글 해시키 획득 
        string googlehash = Backend.Utils.GetGoogleHash();

        if(!string.IsNullOrEmpty(googlehash))
        {
            Debug.Log("구글 해시 키 : " + googlehash);
            if (input != null)
                input.text = googlehash;
        }
    }
}