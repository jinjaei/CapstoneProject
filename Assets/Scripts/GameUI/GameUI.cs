using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI StoneText; // 스톤 텍스트
    [SerializeField] TextMeshProUGUI DiaText; // 다이아 텍스트
    [SerializeField] UnityEngine.UI.Slider EnemyHPBar; // 일반 적 체력 바
    [SerializeField] UnityEngine.UI.Slider BossHPBar; // 보스 체력 바
    [SerializeField] UnityEngine.UI.Slider TimerBar; // 타이머 바
    [SerializeField] UnityEngine.UI.Button BossTryButton; // 보스 시도 버튼
    [SerializeField] UnityEngine.UI.Image BossFail; // 보스 사냥 실패 창
    [SerializeField] UnityEngine.UI.Image GameExit; // 게임 종료 창
    [SerializeField] GameObject StoneUpgradeUI; // 광석 버튼
    [SerializeField] GameObject LevelUpgradeUI; // 강화 버튼

    [SerializeField] UnityEngine.UI.Button StoneLv2; // 광석 2레벨(수정석)

    [SerializeField] UnityEngine.UI.Button Level1Upgrade; // 1 레벨씩 강화
    [SerializeField] UnityEngine.UI.Button Level10Upgrade; // 10 레벨씩 강화
    [SerializeField] UnityEngine.UI.Button Level100Upgrade; // 100 레벨씩 강화

    public static float time = 30; // 보스 제한 시간 30초
    bool TimerStart = false;

    // Start is called before the first frame update
    void Awake()
    {
        NormalEnemyHunting();
        LevelUpgradeEnabled();
        GameExit.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (TimerStart == true)
        {
            Timer();
            if (TimerBar.value == 0)
                BossFail.gameObject.SetActive(true);
        }

        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                GameExit.gameObject.SetActive(true);
        }

        StoneUpgrade();
    }

    public void StoneUpgrade() // 광석 외형 업그레이드
    {
        if (ResourceManager.instance.Diamond >= 100)
            StoneLv2.interactable = true;
    }

    public void NormalEnemyHunting() // 일반 적 사냥 상태
    {
        TimerStart = false;
        BossTryButton.gameObject.SetActive(true);
        EnemyHPBar.gameObject.SetActive(true);
        BossHPBar.gameObject.SetActive(false);
        TimerBar.gameObject.SetActive(false);
        BossFail.gameObject.SetActive(false);
    }

    public void BossTry() // 보스 트라이 상태
    {
        time = 30;
        TimerBar.value = float.MaxValue;
        TimerStart = true;
        BossTryButton.gameObject.SetActive(false);
        EnemyHPBar.gameObject.SetActive(false);
        BossHPBar.gameObject.SetActive(true);
        TimerBar.gameObject.SetActive(true);
    }

    public void Timer() // 타이머
    {
        if (time > 0)
        {
            time -= Time.deltaTime;
            TimerBar.value = time;
        }
    }

    public void StoneUpgradeEnabled() // 광석 업그레이드 창 활성화
    {
        LevelUpgradeUI.SetActive(false);
        StoneUpgradeUI.SetActive(true);
    }
    public void LevelUpgradeEnabled() // 강화 창 활성화
    {
        StoneUpgradeUI.SetActive(false);
        LevelUpgradeUI.SetActive(true);
    }

    public void Level1UpgradeEnabled() // 1 레벨씩 강화 버튼 활성화
    {
        Level1Upgrade.interactable = false;
        Level10Upgrade.interactable = true;
        Level100Upgrade.interactable = true;
    }
    public void Level10UpgradeEnabled() // 10 레벨씩 강화 버튼 활성화
    {
        Level1Upgrade.interactable = true;
        Level10Upgrade.interactable = false;
        Level100Upgrade.interactable = true;
    }
    public void Level100UpgradeEnabled() // 100 레벨씩 강화 버튼 활성화
    {
        Level1Upgrade.interactable = true;
        Level10Upgrade.interactable = true;
        Level100Upgrade.interactable = false;
    }

    public void GameExitOK() // 게임 나가기 확인
    {
        Application.Quit();
    }
    public void GameExitCancel() // 게임 나가기 취소
    {
        GameExit.gameObject.SetActive(false);
    }
}
