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
    // ½Ì±ÛÅæ
    private void Awake()
    {
        _instance = this;
        // ½Ì±ÛÅæ ÀÎ½ºÅÏ½º
    }
    private void OnDestroy()
    {
        applicationQuitting = true;
        // ÀÎ½ºÅÏ½º »èÁ¦
    }

    public Transform[] monsterGeneratePosition = new Transform[5]; // ¸ó½ºÅÍ Æ÷Áö¼Ç
    public SpriteRenderer[] stageBackgroundMap; // ½ºÅ×ÀÌÁö ¸Ê ¹è°æÀÌ¹ÌÁö
    public StageDataTable[] stageDataTables; // ½ºÅ×ÀÌÁö µ¥ÀÌÅÍÅ×ÀÌºí º¯¼ö

    public MonsterSettings[] currentMonsterData = new MonsterSettings[5]; // ÇöÀç ¸ó½ºÅÍÀÇ µ¥ÀÌÅÍ

    private int stageLevel = 1; // ½ºÅ×ÀÌÁö ·¹º§
    //private int mapLevel = 0; // ¸Ê ·¹º§


    public TextMeshProUGUI stageLevelText;
    public Transform mainMap;

    void Start()
    {
        for (int i = 0; i < monsterGeneratePosition.Length; i++)
        {
            // monsterGeneratePosition¿¡ TransformÄÄÆ÷³ÍÆ® ÇÒ´ç
            monsterGeneratePosition[i] = GetComponent<Transform>();
        }

        BackendGameData.Instance.onGameDataLoadEvent.AddListener(OnGameDataLoad); // 리스너 추가
        BackendGameData.Instance.GameDataLoad(); // 뒤끝 서버에서 게임 데이터 로드
    }

    private void OnGameDataLoad()
    {
        SetStage();
    }

    public void SetStage()
    {
        // ½ºÅ×ÀÌÁö ±¸¼º
        DisplayStageLevelText();
        SetMonster();
    }
    public void MovingNextStage()
    {
        // ´ÙÀ½ ½ºÅ×ÀÌÁö·Î ÀÌµ¿
        
    }
    private void DisplayStageLevelText()
    {
        // ½ºÅ×ÀÌÁö¿¡ ¸Â´Â ÅØ½ºÆ®¸¦ Ç¥½Ã
        stageLevelText.text = "STAGE "+stageDataTables[stageLevel].mainStageNumber.ToString() + "-" + stageDataTables[stageLevel].subStageNumber.ToString();
    }

    private void SetMonster()
    {
        // ½ºÅ×ÀÌÁö¿¡ ¸Â´Â ¸ó½ºÅÍ¸¦ ¹èÄ¡ ÈÄ µ¥ÀÌÅÍ ¼¼ÆÃ
        for (int i = 0; i < monsterGeneratePosition.Length; i++)
        {
            GameObject monster = Instantiate(stageDataTables[stageLevel].monsterPrefab[i], monsterGeneratePosition[i].GetChild(1).GetChild(i));
        }
    }

    private void DeleteMonster()
    {
        // ÇöÀç ½ºÅ×ÀÌÁöÀÇ ¸ó½ºÅÍ ÀüºÎ »èÁ¦
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
