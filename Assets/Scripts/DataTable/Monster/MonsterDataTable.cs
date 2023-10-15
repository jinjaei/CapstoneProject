using UnityEngine;
using InfiniteValue;

[CreateAssetMenu(menuName = "ScriptableObjects/MonsterDataTable")]
public class MonsterDataTable : ScriptableObject
{
    public string monsterName; // 몬스터 이름
    public float monsterHP; // 몬스터 체력
    public InfVal dropResourceAmount; // 드랍되는 자원의 양 
}
