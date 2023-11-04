using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI StoneText; // 스톤 텍스트
    [SerializeField] TextMeshProUGUI DiaText; // 다이아 텍스트
    [SerializeField] TextMeshProUGUI powerEnhanceText; // 공격력 강화 텍스트
    [SerializeField] TextMeshProUGUI cooldownEnhanceText; // 공격속도 강화 텍스트
    [SerializeField] TextMeshProUGUI powerResourceText; // 공격력 필요자원 텍스트
    [SerializeField] TextMeshProUGUI cooldownResourceText; // 공격속도 필요자원 텍스트



    [SerializeField] Slider BossHPBar; // 보스 체력 바
    [SerializeField] Slider TimerBar; // 타이머 바
    [SerializeField] Button BossTryButton; // 보스 시도 버튼
    [SerializeField] Image SettingWindow; // 설정 창
    [SerializeField] Image BossFailWindow; // 보스 사냥 실패 창
    [SerializeField] Image GameExitWindow; // 게임 종료 창
    [SerializeField] GameObject StoneUI; // 광석 UI
    [SerializeField] GameObject EnhanceUI; // 강화 UI

    [SerializeField] Button StoneLv2; // 광석 2레벨(수정석)

    [SerializeField] Button Level1Upgrade; // 1 레벨씩 강화
    [SerializeField] Button Level10Upgrade; // 10 레벨씩 강화
    [SerializeField] Button Level100Upgrade; // 100 레벨씩 강화

    public static float time = 30; // 보스 제한 시간 30초
    bool TimerStart = false;

    private void Start()
    {
        SettingAPUpgradeText();
        SettingASUpgradeText();
    }
    // Update is called once per frame
    void Update()
    {
        if (TimerStart == true)
        {
            Timer();
            if (TimerBar.value == 0)
                BossFailWindow.gameObject.SetActive(true);
        }

        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                GameExitWindow.gameObject.SetActive(true);
        }

        //StoneUpgrade();
    }

    public void StoneUpgrade() // 광석 외형 업그레이드
    {
        if (ResourceManager.instance.Diamond >= 100)
            StoneLv2.interactable = true;
    }

    public void NormalEnemyHunting() // 일반 몬스터 사냥 상태
    {
        TimerStart = false;
        BossTryButton.gameObject.SetActive(true);
        BossHPBar.gameObject.SetActive(false);
        TimerBar.gameObject.SetActive(false);
        BossFailWindow.gameObject.SetActive(false);
    }

    public void BossTry() // 보스 트라이 상태
    {
        time = 30;
        TimerBar.value = float.MaxValue;
        TimerStart = true;
        BossTryButton.gameObject.SetActive(false);
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

    public void StoneUIEnabled() // 광석 UI 활성화
    {
        EnhanceUI.SetActive(false);
        StoneUI.SetActive(true);
    }
    public void EnhanceUIEnabled() // 강화 UI 활성화
    {
        StoneUI.SetActive(false);
        EnhanceUI.SetActive(true);
    }
    public void SettingWindowEnabled() // 설정 창 활성화
    {
        SettingWindow.gameObject.SetActive(true);
    }

    public void Level1UpgradeEnabled() // 1 레벨씩 강화 버튼 활성화
    {
        Level1Upgrade.interactable = false;
        Level10Upgrade.interactable = true;
        Level100Upgrade.interactable = true;


        EnhanceManager.instance.SetUpgradeCount1(); // 1 강화모드
        SettingAPUpgradeText();
        SettingASUpgradeText();

        SoundManager.instance.PlaySound("ButtonClick");

    }
    public void Level10UpgradeEnabled() // 10 레벨씩 강화 버튼 활성화
    {
        Level1Upgrade.interactable = true;
        Level10Upgrade.interactable = false;
        Level100Upgrade.interactable = true;


        EnhanceManager.instance.SetUpgradeCount10(); // 10 강화모드
        SettingAPUpgradeText();
        SettingASUpgradeText();


        SoundManager.instance.PlaySound("ButtonClick");

    }
    public void Level100UpgradeEnabled() // 100 레벨씩 강화 버튼 활성화
    {
        Level1Upgrade.interactable = true;
        Level10Upgrade.interactable = true;
        Level100Upgrade.interactable = false;


        EnhanceManager.instance.SetUpgradeCount100(); // 100 강화모드
        SettingAPUpgradeText();
        SettingASUpgradeText();


        SoundManager.instance.PlaySound("ButtonClick");


    }
    public void SettingAPUpgradeText()
    {
        powerEnhanceText.text = PlayerStatManager.instance.GetPowerAmount().ToString("F2");
        powerResourceText.text = EnhanceManager.instance.GetResourceIncrease(EnhanceManager.instance.powerResourceAmount).ToString();
    }
    public void SettingASUpgradeText()
    {
        cooldownEnhanceText.text = PlayerStatManager.instance.GetCooldownAmount().ToString("F2");
        cooldownResourceText.text = EnhanceManager.instance.GetResourceIncrease(EnhanceManager.instance.cooldownResourceAmount).ToString();
    }

    public void GameExit() // 게임 나가기 확인
    {
        Application.Quit();
    }
    public void WindowExit() // 설정 창 끄기 및 게임 나가기 취소
    {
        SettingWindow.gameObject.SetActive(false);
        GameExitWindow.gameObject.SetActive(false);
    }
}
