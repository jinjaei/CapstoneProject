using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/StageDataTable")]
public class StageDataTable : ScriptableObject
{
    public int mainStageNumber; // 메인 스테이지 번호
    public int subStageNumber; // 서브 스테이지 번호
    public GameObject[] monsterPrefab = new GameObject[5]; // 몬스터 위치에 따른 프리팹
}
