using InfiniteValue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnhanceManager : MonoBehaviour
{
    private static object _lock = new object();
    private static EnhanceManager _instance = null;
    public static EnhanceManager instance
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
                    GameObject obj = new GameObject("EnhanceManager ");
                    obj.AddComponent<EnhanceManager>();
                    _instance = obj.GetComponent<EnhanceManager>();
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
    [HideInInspector] public InfVal powerResourceAmount = 100; //감소시킬 자원의 양 (파워)
    [HideInInspector] public InfVal cooldownResourceAmount = 100; //감소시킬 자원의 양 (공격속도)
    [HideInInspector] public int upgradeCount = 1; //업그레이드횟수  
    public void EnhancePower()
    {
        InfVal increaseAmount = powerResourceAmount;
        //공격력 증가 함수
        for(int i=0; i<upgradeCount-1; i++)
        {
            increaseAmount = increaseAmount + (increaseAmount * 0.01);
        }
        ResourceManager.instance.CheckResourceAmount(ResourceManager.ResourceType.Stone, increaseAmount);
        if (ResourceManager.instance.consumeAble == true)
        {
            powerResourceAmount = increaseAmount;
            ResourceManager.instance.RemoveResource(ResourceManager.ResourceType.Stone, powerResourceAmount);
            PlayerStatManager.instance.AddPower(upgradeCount);
            PowerResourceIncrease(powerResourceAmount);

            GameUI gameUI = FindObjectOfType<GameUI>();
            gameUI.SettingAPUpgradeText(); // UI 갱신
        }
    }
    public void Enhancecooldown()
    {
        InfVal increaseAmount = cooldownResourceAmount;
        //공격속도 증가 함수
        for (int i = 0; i < upgradeCount-1; i++)
        {
            increaseAmount = increaseAmount + (increaseAmount * 0.01);
        }
        ResourceManager.instance.CheckResourceAmount(ResourceManager.ResourceType.Stone, increaseAmount);
        if(ResourceManager.instance.consumeAble == true)
        {
            cooldownResourceAmount = increaseAmount;
            ResourceManager.instance.RemoveResource(ResourceManager.ResourceType.Stone, cooldownResourceAmount);
            PlayerStatManager.instance.AddCoolDown(upgradeCount);
            CooldownResourceIncrease(cooldownResourceAmount);

            GameUI gameUI = FindObjectOfType<GameUI>();
            gameUI.SettingASUpgradeText(); // UI 갱신
        }
    }
    public void PowerResourceIncrease(InfVal resourceAmount)
    {
        powerResourceAmount += resourceAmount;
    }
    public void CooldownResourceIncrease(InfVal resourceAmount)
    {
        cooldownResourceAmount += resourceAmount;
    }

    public void SetUpgradeCount1()
    {
        //업그레이드횟수를 1로 설정
        upgradeCount = 1;
    }
    public void SetUpgradeCount10() 
    {
        //업그레이드횟수를 10으로 설정
        upgradeCount = 10;
    }
    public void SetUpgradeCount100()     
    {
        //업그레이드횟수를 100으로 설정
        upgradeCount = 100;
    }
    public InfVal GetResourceIncrease(InfVal resourceType)
    {
        // 강화에 필요한 자원을 반환
        InfVal increaseAmount = resourceType;
        for (int i = 0; i < upgradeCount - 1; i++)
        {
            increaseAmount = increaseAmount + (increaseAmount * 0.01);
        }
        return increaseAmount;
    }

}
