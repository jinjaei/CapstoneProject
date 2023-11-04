using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StageManager : MonoBehaviour
{
    private static object _lock = new object();
    private static StageManager _instance = null;
    public static StageManager instance
    {
        get
        {
            if (applicationQuitting)
            {
                return null;
            }
            lock (_lock)
            {
                if (_instance == null)
                {
                    GameObject obj = new GameObject("StageManager ");
                    obj.AddComponent<StageManager>();
                    _instance = obj.GetComponent<StageManager>();
                }
                return _instance;
            }
        }
        set
        {
            _instance = value;
        }
    }
    private static bool applicationQuitting = false;
    // 싱글톤
    private void Awake()
    {
        _instance = this;
        // 싱글톤 인스턴스
    }
    private void OnDestroy()
    {
        applicationQuitting = true;
        // 인스턴스 삭제
    }

    public Transform[] monsterGeneratePosition = new Transform[5]; // 몬스터 포지션
    public SpriteRenderer[] stageBackgroundMap; // 스테이지 맵 배경이미지
    public StageDataTable[] stageDataTables; // 스테이지 데이터테이블 변수

    public MonsterSettings[] currentMonsterData = new MonsterSettings[5]; // 현재 몬스터의 데이터

    private int stageLevel = 1; // 스테이지 레벨
    //private int mapLevel = 0; // 맵 레벨


    public TextMeshProUGUI stageLevelText;
    public Transform mainMap;

    void Start()
    {
        for (int i = 0; i < monsterGeneratePosition.Length; i++)
        {
            // monsterGeneratePosition에 Transform컴포넌트 할당
            monsterGeneratePosition[i] = GetComponent<Transform>();
        }

        SetStage(); // 스테이지 구성
    }

    public void SetStage()
    {
        // 스테이지 구성
        DisplayStageLevelText();
        SetMonster();
    }
    public void MovingNextStage()
    {
        // 다음 스테이지로 이동
        
    }
    private void DisplayStageLevelText()
    {
        // 스테이지에 맞는 텍스트를 표시
        stageLevelText.text = "STAGE "+stageDataTables[stageLevel].mainStageNumber.ToString() + "-" + stageDataTables[stageLevel].subStageNumber.ToString();
    }

    private void SetMonster()
    {
        // 스테이지에 맞는 몬스터를 배치 후 데이터 세팅
        for (int i = 0; i < monsterGeneratePosition.Length; i++)
        {
            GameObject monster = Instantiate(stageDataTables[stageLevel].monsterPrefab[i], monsterGeneratePosition[i].GetChild(1).GetChild(i));
        }
    }

    private void DeleteMonster()
    {
        // 현재 스테이지의 몬스터 전부 삭제
        for (int i = 0; i < monsterGeneratePosition.Length; i++)
        {
            GameObject monster = monsterGeneratePosition[i].GetChild(1).GetChild(i).gameObject;
            Destroy(monster);
        }
    }

    public void EndBattleMode()
    {
        PlayerController playerController = FindObjectOfType<PlayerController>();
        playerController.BattleModeEnd();
    }

}
