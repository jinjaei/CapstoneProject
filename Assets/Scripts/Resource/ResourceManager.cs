using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using InfiniteValue;


public class ResourceManager : MonoBehaviour
{
    private static object _lock = new object();
    private static ResourceManager _instance = null;
    public static ResourceManager instance
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
                    GameObject obj = new GameObject("ResourceManager ");
                    obj.AddComponent<ResourceManager>();
                    _instance = obj.GetComponent<ResourceManager>();
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


    [SerializeField]
    private InfVal currentStone; // 현재 골드 값

    [SerializeField]
    private InfVal currentDiamond; // 현재 다이아 값

    private InfVal startStone = 0; // 시작 골드 값
    private InfVal startDiamond = 0; // 시작 다이아 값

    public bool consumeAble = false;

    public event Action<ResourceType, InfVal> OnResourceChanged; // 값 변환 이벤트

    public enum ResourceType
    {
        Stone,
        Diamond
    }
    // 자원 타입
    private void Awake()
    {
        _instance = this;
        // 싱글톤 인스턴스

        startStone = InfVal.Parse("9999M");
        startDiamond = 9999;
        currentStone = startStone;
        currentDiamond = startDiamond;
        // 자원 시작 값 설정
    }

    private void OnDestroy()
    {
        applicationQuitting = true;
        // 인스턴스 삭제
    }

    public InfVal Stone
    {
        get { return currentStone; }
        set
        {
            // 값이 변경된 경우에만 값 변환 애니메이션 실행
            if (value != currentStone)
            {
                StartCoroutine(IncreaseResourceValue(ResourceType.Stone, currentStone, value));
            }
            currentStone = value;

        }
    }
    // 스톤 프로퍼티


    public InfVal Diamond
    {
        get { return currentDiamond; }
        set
        {
            if (value != currentDiamond) // 값이 변경된 경우에만 애니메이션 실행
            {
                StartCoroutine(IncreaseResourceValue(ResourceType.Diamond, currentDiamond, value));
            }
            currentDiamond = value;

        }
    }
    // 다이아 프로퍼티

    public InfVal GetResourceValue(ResourceType resourceType)
    {
        // 자원 값 읽기 메서드
        switch (resourceType)
        {
            case ResourceType.Stone:
                return Stone;

            case ResourceType.Diamond:
                return Diamond;

            default:
                return 0;
        }
    }
    public void AddResource(ResourceType resourceType, InfVal amount)
    {
        // 자원 증가 메서드
        switch (resourceType)
        {
            case ResourceType.Stone:
                Stone += amount;
                break;

            case ResourceType.Diamond:
                Diamond += amount;
                break;
        }
    }

    public void RemoveResource(ResourceType resourceType, InfVal amount)
    {
        // 자원 감소 메서드
        switch (resourceType)
        {
            case ResourceType.Stone:
                if (consumeAble)
                {
                    Stone -= amount;
                }
                break;

            case ResourceType.Diamond:
                if (consumeAble)
                {
                    Diamond -= amount;
                }
                break;
        }
    }

    public void CheckResourceAmount(ResourceType resourceType, InfVal amount)
    {
        // 자원 감소 시 확인 메서드
        switch (resourceType)
        {
            case ResourceType.Stone:
                if (currentStone >= amount)
                {
                    consumeAble = true;
                }
                else 
                {
                    consumeAble = false;
                    ToastMessage.I.ShowToastMessage("스톤이 부족합니다!", ToastMessage.ToastLength.Short);
                    Debug.Log($"{resourceType}이 부족합니다.");
                } 
                break;

            case ResourceType.Diamond:
                if (currentDiamond >= amount)
                {
                    consumeAble = true;
                }
                else
                {
                    consumeAble = false;
                    ToastMessage.I.ShowToastMessage("보석이 부족합니다!", ToastMessage.ToastLength.Short);
                    Debug.Log($"{resourceType}이 부족합니다.");
                }
                break;
        }
    }

    private IEnumerator IncreaseResourceValue(ResourceType resourceType, InfVal startValue, InfVal endValue)
    {
        float duration = 1.0f; // 애니메이션 지속 시간 (초)
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            InfVal newValue = MathInfVal.Round(Mathf.Lerp((float)startValue, (float)endValue, t));
            OnResourceChanged?.Invoke(resourceType, newValue);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        OnResourceChanged?.Invoke(resourceType, endValue);
    }
}

