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
    // ΩÃ±€≈Ê

    private void Awake()
    {
        _instance = this;
        // ΩÃ±€≈Ê ¿ŒΩ∫≈œΩ∫
    }

    private void OnDestroy()
    {
        applicationQuitting = true;
        // ¿ŒΩ∫≈œΩ∫ ªË¡¶
    }

    private int playerLevel { get; set; }
    private float playerPower { get; set; }
    private float playerCoolDown { get; set; }

    public void AddLevel(int amount)
    {
        playerLevel += amount;
    }

    public void AddPower(float count)
    {
        for(int i=0; i < count; i++)
        {
            playerPower += playerPower * 0.25f;

        }
    }

    public void AddCoolDown(float count)
    {
        for (int i = 0; i < count; i++)
        {
            playerCoolDown -= playerCoolDown * 0.25f;
        }
    }
}
