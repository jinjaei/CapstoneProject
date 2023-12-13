using System.Collections.Generic;
using InfiniteValue;


[System.Serializable]
public class UserGameData
{
	public string Stone;           // 현재 보유한 돌가루
	public string Diamond;			// 현제 보유한 보석
	public float Power;			// 공격력
	public float  Speed;		// 공격속도
	public int SpeedLevel;		// 공격속도 레벨
	public int PowerLevel;    // 공격력 레벨
	public string PRA;				// 공격력 강화시 감소시킬 자원량
	public string SPA;             // 공격속도 강화시 감소시킬 자원량
	public int StageLevel;
	public int PlayerLevel;
	public int CurrentPlayerUpgrade;
	

	public void Reset()
	{
		Stone = "1.2G";
		Diamond = "4500";
		Power = 10f;
		Speed = 3f;
		SpeedLevel = 1;
		PowerLevel = 1;
		SPA = "100";
		PRA = "100";
		StageLevel = 0;
		PlayerLevel = 0;
		CurrentPlayerUpgrade = 1;
	}
}