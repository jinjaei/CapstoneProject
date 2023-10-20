using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StageManager : MonoBehaviour
{
    public Transform[] monsterGeneratePosition = new Transform[5]; // 몬스터 포지션
    public SpriteRenderer[] stageBackgroundMap; // 스테이지 맵 배경이미지
    public StageDataTable[] stageDataTables; // 스테이지 데이터테이블 변수

    private MonsterDataTable[] currentMonsterData = new MonsterDataTable[5]; // 현재 몬스터의 데이터

    private int stageLevel = 0; // 스테이지 레벨
    private int mapLevel = 0; // 맵 레벨

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
        // 스테이지에 맞는 몬스터를 배치
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

    public void AccessMonster()
    {
        // 현재 몬스터 정보가져오기
        for (int i = 0; i < monsterGeneratePosition.Length; i++)
        {
            GameObject monster = Instantiate(stageDataTables[stageLevel].monsterPrefab[i], monsterGeneratePosition[i].GetChild(1).GetChild(i));
            currentMonsterData[i] = monster.GetComponent<MonsterSettings>().monsterDataTable;
        }
    }
}
