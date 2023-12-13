using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class StoneUpgrade : MonoBehaviour
{
    public List<Button> upgradeButtons; 
    public List<Button> purchasedButtons; 
    private Button currentButton; 

    public GameObject[] stonePrefab; 

    public int[] upgradePrices; 
    public int currentUpgradeLevel = 0; 

    public Transform player; 
    private static object _lock = new object();
    private static StoneUpgrade _instance = null;
    public static StoneUpgrade instance
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
                    GameObject obj = new GameObject("StoneUpgrade");
                    obj.AddComponent<StoneUpgrade>();
                    _instance = obj.GetComponent<StoneUpgrade>();
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

    private void Awake()
    {
        _instance = this;
    }
    private void OnDestroy()
    {
        applicationQuitting = true;
    }
    private void Start()
    {
        upgradeButtons[0].interactable = true;
        StartCoroutine(SetValue());
    }

    public void PurchaseButton()
    {
        currentButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>(); // Áö±Ý Å¬¸¯µÈ ¹öÆ° °¡Á®¿À±â

        if ((upgradeButtons.Count > 0) && upgradeButtons.Contains(currentButton)) // ¿ÜÇüÀ» Ã³À½ ±¸¸ÅÇÒ ¶§¸¸ ½ÇÇà
        {
            ResourceManager.instance.CheckResourceAmount(ResourceManager.ResourceType.Diamond, upgradePrices[currentUpgradeLevel+1]);
            if (ResourceManager.instance.consumeAble)
            {
                purchasedButtons.Add(upgradeButtons[0]); // ±¸¸ÅÇÑ ¹öÆ° ¸®½ºÆ®·Î ÀÌµ¿
                ChangeStone();

                ResourceManager.instance.RemoveResource(ResourceManager.ResourceType.Diamond, upgradePrices[currentUpgradeLevel]); // ÀÚ¿ø °¨¼Ò
                TextMeshProUGUI upgradeText = upgradeButtons[0].GetComponentInChildren<TextMeshProUGUI>();
                upgradeText.text = "Àû¿ëÁß"; // ÅØ½ºÆ® "Àû¿ëÁß" º¯È¯
                upgradeText.rectTransform.anchoredPosition = new Vector3(upgradeText.rectTransform.anchoredPosition.x - 40, 0, 0); // ÅØ½ºÆ® °¡¿îµ¥·Î ¸ÂÃß±â

                Image[] removeImages = upgradeText.GetComponentsInChildren<Image>();
                removeImages[0].gameObject.SetActive(false); // ´ÙÀÌ¾Æ ÀÌ¹ÌÁö ºñÈ°¼ºÈ­
                removeImages[1].gameObject.SetActive(false); // ½Ç·ç¿§ ÀÌ¹ÌÁö ºñÈ°¼ºÈ­

                upgradeButtons.Remove(upgradeButtons[0]);
                upgradePrices[currentUpgradeLevel] = 0; // ±¸¸ÅÇÑ ¿ÜÇü °¡°ÝÀº ¹«·á·Î º¯°æ

                if (upgradeButtons.Count != 0)
                    upgradeButtons[0].interactable = true; // ´ÙÀ½ ¿ÜÇü ¹öÆ° È°¼ºÈ­
            }
            else
                return; // ±¸¸Å ºÒ°¡¸é ÇÔ¼ö Á¾·á
        }
        else // ÀÌ¹Ì ±¸¸ÅÇÑ ¿ÜÇüÀÏ ¶§
            ChangeStone();

        ChangeText(purchasedButtons.Count);
        PlayerStoneUpgrade();
        // 외형 업그레이드 레벨을 업데이트
    }
    public void ChangeStone() // º¯°æÇÒ ¿ÜÇü °áÁ¤
    {
        switch (purchasedButtons.IndexOf(currentButton))
        {
            case var _ when currentButton == purchasedButtons[0]:
                currentUpgradeLevel = 0;
                    break;
            case var _ when currentButton == purchasedButtons[1]:
                currentUpgradeLevel = 1;
                break;
            case var _ when currentButton == purchasedButtons[2]:
                currentUpgradeLevel = 2;
                break;
            case var _ when currentButton == purchasedButtons[3]:
                currentUpgradeLevel = 3;
                break;
            case var _ when currentButton == purchasedButtons[4]:
                currentUpgradeLevel = 4;
                break;
            case var _ when currentButton == purchasedButtons[5]:
                currentUpgradeLevel = 5;
                break;
            case var _ when currentButton == purchasedButtons[6]:
                currentUpgradeLevel = 6;
                break;
            default:
                return;
        }
    }

    public void ChangeText(int count) // ±¸¸ÅµÈ ¹öÆ°ÀÇ ÅØ½ºÆ® º¯°æ
    {
        for (int i = 0; i < count; i++)
        {
            purchasedButtons[i].interactable = true; // Àû¿ëÁßÀÌ ¾Æ´Ñ ¹öÆ° ¸ðµÎ È°¼ºÈ­
            purchasedButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = "적용";
        }
        if (currentButton != null)
        {
            currentButton.interactable = false; // Àû¿ëÁßÀÎ ¹öÆ°Àº ºñÈ°¼ºÈ­
            currentButton.GetComponentInChildren<TextMeshProUGUI>().text = "적용중";

        }
        else
        {
            purchasedButtons[count - 1].interactable = false;
            upgradeButtons[0].interactable = true;
            purchasedButtons[count-1].GetComponentInChildren<TextMeshProUGUI>().text = "적용중";
        }
    }

    public void PlayerStoneUpgrade()
    {
        // ½ºÅæ ¾÷±×·¹ÀÌµå
        int childCount = player.childCount;
        if (childCount > 0)
        {
            Transform childToBeDeleted = player.GetChild(0);
            Destroy(childToBeDeleted.gameObject);

            GameObject newChild = Instantiate(stonePrefab[currentUpgradeLevel], player);
            newChild.transform.localPosition = Vector3.zero;

            Animator newAnimator = newChild.GetComponent<Animator>();
            PlayerController playerController = FindObjectOfType<PlayerController>();
            playerController.animator = newAnimator;
        }
        else
        {
            Debug.Log("ÀÚ½Ä ¿ÀºêÁ§Æ®°¡ ¾ø½À´Ï´Ù.");
        }
    }
    private void SetPurchasedButtons(int count)
    {
        if (count == 1) return;
        for(int i=0; i<count-1; i++)
        {
            purchasedButtons.Add(upgradeButtons[i]);

        }
        for (int i = 0; i < count-1; i++)
        {
            upgradeButtons.Remove(upgradeButtons[0]);
        }
        for (int i = 1; i < count; i++)
        {
            TextMeshProUGUI upgradeText = purchasedButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            upgradeText.rectTransform.anchoredPosition = new Vector3(upgradeText.rectTransform.anchoredPosition.x - 40, 0, 0);

            Image[] removeImages = upgradeText.GetComponentsInChildren<Image>();
            removeImages[0].gameObject.SetActive(false);
            removeImages[1].gameObject.SetActive(false);
        }
        
        ChangeText(count);
    }
    private IEnumerator SetValue()
    {
        yield return new WaitForSeconds(0.3f);
        currentUpgradeLevel = BackendGameData.Instance.UserGameData.PlayerLevel;
        int count = BackendGameData.Instance.UserGameData.CurrentPlayerUpgrade;
        SetPurchasedButtons(count);
        PlayerStoneUpgrade();
        // 외형 설정
    }


}
