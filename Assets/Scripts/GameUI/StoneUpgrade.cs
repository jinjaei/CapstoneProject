using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoneUpgrade : MonoBehaviour
{
    public List<Button> upgradeButtons; // 업그레이드 할 버튼
    public List<Button> purchasedButtons; // 구매한 버튼

    public GameObject[] stonePrefab; // 스톤 프리팹

    public int[] upgradePrices; // 업그레이드 가격
    private int currentUpgradeLevel=0; // 현재 업그레이드 레벨

    public Transform player; // 플레이어

    private void Start()
    {
        upgradeButtons[0].interactable = true;
    }
    public void PurchasedButtonRemove()
    {
        // 구매한 버튼을 삭제
        purchasedButtons[0].gameObject.SetActive(false);
        purchasedButtons.Remove(purchasedButtons[0]);

    }

    public void PurchaseButton()
    {
        ResourceManager.instance.CheckResourceAmount(ResourceManager.ResourceType.Diamond, upgradePrices[currentUpgradeLevel]);
        if (ResourceManager.instance.consumeAble)
        {
            ResourceManager.instance.RemoveResource(ResourceManager.ResourceType.Diamond, upgradePrices[currentUpgradeLevel]); // 자원 감소
            TextMeshProUGUI stoneText = upgradeButtons[0].GetComponentInChildren<TextMeshProUGUI>();
            stoneText.text = "적용중"; // 텍스트 "적용중" 변환

            Image stoneImage = stoneText.GetComponentInChildren<Image>();
            stoneImage.gameObject.SetActive(false); // 다이아 이미지 비활성화

            purchasedButtons.Add(upgradeButtons[0]);
            upgradeButtons.Remove(upgradeButtons[0]); // 구매한 버튼 리스트로 이동

            if (upgradeButtons.Count > 0)
            {
                upgradeButtons[0].interactable = true;
            }
            PurchasedButtonRemove();
            purchasedButtons[0].interactable = false;


            PlayerStoneUpgrade();

            currentUpgradeLevel++;

        }
    }

    public void PlayerStoneUpgrade()
    {
        // 스톤 업그레이드
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
            Debug.Log("자식 오브젝트가 없습니다.");
        }
    }


}
