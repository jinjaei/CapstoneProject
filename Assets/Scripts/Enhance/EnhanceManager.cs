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
    public InfVal powerResourceAmount = 100; //감소시킬 자원의 양 (파워)
    public InfVal cooldownResourceAmount = 100; //감소시킬 자원의 양 (공격속도)
    public int upgradeCount = 1; //업그레이드횟수  
    public void EnhancePower()
    {
        //공격력 증가 함수
        ResourceManager.instance.CheckResourceAmount(ResourceManager.ResourceType.Stone, powerResourceAmount);
        if (ResourceManager.instance.consumeAble == true)
        {
            ResourceManager.instance.RemoveResource(ResourceManager.ResourceType.Stone, powerResourceAmount);
            PlayerStatManager.instance.AddPower(upgradeCount);
            ResourceIncrease(powerResourceAmount);
        }
    }
    public void Enhancecooldown()
    {
        //공격속도 증가 함수
        ResourceManager.instance.CheckResourceAmount(ResourceManager.ResourceType.Stone, cooldownResourceAmount);
        if(ResourceManager.instance.consumeAble == true)
        {
            ResourceManager.instance.RemoveResource(ResourceManager.ResourceType.Stone, cooldownResourceAmount);
            PlayerStatManager.instance.AddCoolDown(upgradeCount);
            ResourceIncrease(cooldownResourceAmount);
        }
    }
    public void ResourceIncrease(InfVal resourceAmount)
    {
        resourceAmount = resourceAmount + (resourceAmount*0.1*upgradeCount);
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

}
