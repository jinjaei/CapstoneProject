using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatManager : MonoBehaviour
{
    private static object _lock = new object();
    private static PlayerStatManager _instance = null;
    public static PlayerStatManager instance
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
                    GameObject obj = new GameObject("PlayerStatManager ");
                    obj.AddComponent<PlayerStatManager>();
                    _instance = obj.GetComponent<PlayerStatManager>();
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

    //public int playerLevel { get; set; }
    public float playerPower = 10;
    public float playerCoolDown = 3f;
    public int PowerLevel = 1; // 공격력 레벨
    public int CoolDownLevel = 1; // 공격속도 레벨

    /*public void AddLevel(int amount)
    {
        playerLevel += amount;
    }*/

    public void AddPower(float count)
    {
        // 공격력 업그레이드
        for(int i=0; i < count; i++)
        {
            playerPower += playerPower * 0.001f;

        }
    }

    public void AddCoolDown(float count)
    {
        // 공격속도 업그레이드
        for (int i = 0; i < count; i++)
        {
            playerCoolDown -= playerCoolDown * 0.0005f;
        }
    }

    public int GetPowerLevelAmount()
    {
        // 공격력 레벨 반환
        if (ResourceManager.instance.consumeAble == true)
            PowerLevel = PowerLevel + EnhanceManager.instance.upgradeCount;
        else
            return PowerLevel;
        return PowerLevel;
    }
    public int GetCoolDownLevelAmount()
    {
        // 공격속도 레벨 반환
        if (ResourceManager.instance.consumeAble == true)
            CoolDownLevel = CoolDownLevel + EnhanceManager.instance.upgradeCount;
        else
            return CoolDownLevel;
        return CoolDownLevel;
    }

    public float GetPowerAmount()
    {
        // 강화 될 스탯을 반환
        float statAmount = playerPower;
        for (int i = 0; i < EnhanceManager.instance.upgradeCount - 1; i++)
        {
            statAmount = statAmount + (statAmount * 0.001f);
        }
        return statAmount;
    }
    public float GetCooldownAmount()
    {
        // 강화 될 스탯을 반환
        float statAmount = playerCoolDown;
        for (int i = 0; i < EnhanceManager.instance.upgradeCount - 1; i++)
        {
            statAmount = statAmount - (statAmount * 0.0005f);
        }
        return statAmount;
    }
}
