using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class StoneUpgrade : MonoBehaviour
{
    public List<Button> upgradeButtons; // 업그레이드 할 버튼
    public List<Button> purchasedButtons; // 구매한 버튼
    private Button currentButton; // 현재 적용중인 버튼

    public GameObject[] stonePrefab; // 스톤 프리팹

    public int[] upgradePrices; // 업그레이드 가격
    private int currentUpgradeLevel = 0; // 현재 업그레이드 레벨

    public Transform player; // 플레이어

    private void Start()
    {
        upgradeButtons[0].interactable = true;
    }

    public void PurchaseButton()
    {
        currentButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>(); // 지금 클릭된 버튼 가져오기

        if ((upgradeButtons.Count > 0) && upgradeButtons.Contains(currentButton)) // 외형을 처음 구매할 때만 실행
        {
            ResourceManager.instance.CheckResourceAmount(ResourceManager.ResourceType.Diamond, upgradePrices[currentUpgradeLevel+1]);
            if (ResourceManager.instance.consumeAble)
            {
                purchasedButtons.Add(upgradeButtons[0]); // 구매한 버튼 리스트로 이동
                ChangeStone();

                ResourceManager.instance.RemoveResource(ResourceManager.ResourceType.Diamond, upgradePrices[currentUpgradeLevel]); // 자원 감소
                TextMeshProUGUI upgradeText = upgradeButtons[0].GetComponentInChildren<TextMeshProUGUI>();
                upgradeText.text = "적용중"; // 텍스트 "적용중" 변환
                upgradeText.rectTransform.anchoredPosition = new Vector3(upgradeText.rectTransform.anchoredPosition.x - 40, 0, 0); // 텍스트 가운데로 맞추기

                Image[] removeImages = upgradeText.GetComponentsInChildren<Image>();
                removeImages[0].gameObject.SetActive(false); // 다이아 이미지 비활성화
                removeImages[1].gameObject.SetActive(false); // 실루엣 이미지 비활성화

                upgradeButtons.Remove(upgradeButtons[0]);
                upgradePrices[currentUpgradeLevel] = 0; // 구매한 외형 가격은 무료로 변경

                if (upgradeButtons.Count != 0)
                    upgradeButtons[0].interactable = true; // 다음 외형 버튼 활성화
            }
            else
                return; // 구매 불가면 함수 종료
        }
        else // 이미 구매한 외형일 때
            ChangeStone();

        ChangeText();
        PlayerStoneUpgrade();
    }
    public void ChangeStone() // 변경할 외형 결정
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

    public void ChangeText() // 구매된 버튼의 텍스트 변경
    {
        for (int i = 0; i < purchasedButtons.Count; i++)
        {
            purchasedButtons[i].interactable = true; // 적용중이 아닌 버튼 모두 활성화
            purchasedButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = "적용";
        }
        currentButton.interactable = false; // 적용중인 버튼은 비활성화
        currentButton.GetComponentInChildren<TextMeshProUGUI>().text = "적용중";
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
