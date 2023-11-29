using System.Collections.Generic;

[System.Serializable]
public class UserGameData
{
	public int Stone;           // 현재 보유한 돌가루
	public int Diamond;			// 현제 보유한 보석
	public int Power;			// 공격력
	public double  Speed;		// 공격속도
	public int SpeedLevel;		// 공격속도 레벨
	public int PowerLevel;    // 공격력 레벨
	public int PRA;				// 공격력 강화시 감소시킬 자원량
	public int SPA;             // 공격속도 강화시 감소시킬 자원량
	public List<int> stoneUpgradeLevels = new List<int>();
	

	public void Reset()
	{
		Stone = 0;
		Diamond = 0;
		Power = 10;
		Speed = 0.15;
		SpeedLevel = 1;
		PowerLevel = 1;
		SPA = 100;
		PRA = 100;
	}
}