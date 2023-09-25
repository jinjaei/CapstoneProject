using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Slider EnemyHPBar; // 일반 적 체력 바
    [SerializeField] UnityEngine.UI.Slider BossHPBar; // 보스 체력 바
    [SerializeField] UnityEngine.UI.Slider TimerBar; // 타이머 바
    [SerializeField] UnityEngine.UI.Button BossTryButton; // 보스 시도 버튼
    [SerializeField] GameObject StoneUpgradeUI; // 광석 버튼
    [SerializeField] GameObject LevelUpgradeUI; // 강화 버튼

    public static float time = 30; // 보스 제한 시간 30초
    bool TimerStart = false;

    // Start is called before the first frame update
    void Awake()
    {
        NormalEnemyHunting();
        LevelUpgrade();
    }

    // Update is called once per frame
    void Update()
    {
        if(TimerStart == true)
            Timer();
    }

    public void NormalEnemyHunting()
    {
        BossTryButton.gameObject.SetActive(true);
        EnemyHPBar.gameObject.SetActive(true);
        BossHPBar.gameObject.SetActive(false);
        TimerBar.gameObject.SetActive(false);
    }

    public void BossTry()
    {
        BossTryButton.gameObject.SetActive(false);
        EnemyHPBar.gameObject.SetActive(false);
        BossHPBar.gameObject.SetActive(true);
        TimerBar.gameObject.SetActive(true);
        TimerStart = true;
    }

    public void Timer()
    {
        if (time > 0)
        {
            time -= Time.deltaTime;
            TimerBar.value = time;
        }
    }
    public void StoneUpgrade()
    {
        LevelUpgradeUI.SetActive(false);
        StoneUpgradeUI.SetActive(true);
    }
    public void LevelUpgrade()
    {
        StoneUpgradeUI.SetActive(false);
        LevelUpgradeUI.SetActive(true);
    }
}
