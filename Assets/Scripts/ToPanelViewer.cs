using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToPanelViewer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textNickname;
    [SerializeField]
    private TextMeshProUGUI textStone;          //돌가루
    [SerializeField]
    private TextMeshProUGUI textDiamond;	    // 현제 보유한 보석
    [SerializeField]
    private TextMeshProUGUI textSpeed;		    // 공격속도
    [SerializeField]
    private TextMeshProUGUI textPower;			// 공격력
    [SerializeField]
    private TextMeshProUGUI textSpeedLevel;		// 공격속도 레벨
    [SerializeField]
    private TextMeshProUGUI textPowerLevel;     // 공격력 레벨
    [SerializeField]
    private TextMeshProUGUI textPRA;            // 공격력 강화시 감소시킬 자원량
    [SerializeField]
    private TextMeshProUGUI textSPA;           // 공격속도 강화시 감소시킬 자원량


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
        textStone.text = $"{ BackendGameData.Instance.UserGameData.Stone}";                 // 돌가루
        textDiamond.text = $"{ BackendGameData.Instance.UserGameData.Diamond}";             // 다이아
        textSpeed.text = $"{ BackendGameData.Instance.UserGameData.Speed}";                 // 공격속도
        textPower.text = $"{ BackendGameData.Instance.UserGameData.Power}";                 // 공격력
        textSpeedLevel.text = $"{ BackendGameData.Instance.UserGameData.SpeedLevel}";       // 공격속도 레벨
        textPowerLevel.text = $"{ BackendGameData.Instance.UserGameData.PowerLevel}";       // 공격력 래밸
        textPRA.text = $"{ BackendGameData.Instance.UserGameData.PRA}";                     //강화시 감소시킬 자원량
        textSPA.text = $"{ BackendGameData.Instance.UserGameData.SPA}";                     //강화시 감소시킬 자원량
    }
}