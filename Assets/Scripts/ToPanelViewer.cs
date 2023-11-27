using UnityEngine;
using TMPro;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textNickname;
    [SerializeField]
    private TextMeshProUGUI textehfrkfn; //돌가루
    [SerializeField]
    private TextMeshProUGUI textrhdrurfur; //스테이지
    [SerializeField]
    private TextMeshProUGUI textrhdrurthrhe; //공격속도
    [SerializeField]
    private TextMeshProUGUI textqhtjr; //보석


    private void Awake()
    {
        BackendGameData.Instance.onGameDataLoadEvent.AddListener(UpdateGameData);
    }
    public void UpdateNicname()
    {
        textNickname.text = /*UserInfo.Data.nickname == null ?*/
            UserInfo.Data.gamerId; /*: UserInfo.Data.nickname;*/
    }

    public void UpdateGameData()
    {
        //textehfrkfn.text = $"{ BackendGameData.Instance.UserGameData.ehfrkfn}"; // 돌가루
        //textehfrkfn.text = $"{ BackendGameData.Instance.UserGameData.ehfrkfn}"; // 공격속도
        //textehfrkfn.text = $"{ BackendGameData.Instance.UserGameData.ehfrkfn}"; // 보석
        //textehfrkfn.text = $"{ BackendGameData.Instance.UserGameData.ehfrkfn}"; // 필요한 데이터 값 추가
    }
}
