using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScrolling : MonoBehaviour
{
    private float scrollSpeed = 1.5f;
    public GameObject backgroundPrefab;

    private Transform[] backgrounds;
    private Coroutine scrollingCoroutine;


    void Start()
    {
        // 두 개의 배경을 생성하고 배열에 저장
        backgrounds = new Transform[2];
        backgrounds[0] = Instantiate(backgroundPrefab, new Vector3(0, 2.16f, 2), Quaternion.identity).transform;
        backgrounds[1] = Instantiate(backgroundPrefab, new Vector3(backgroundPrefab.GetComponent<SpriteRenderer>().bounds.size.x, 2.16f, 2), Quaternion.identity).transform;
    }

    // 스테이지 변경 이벤트가 발생할 때 호출될 메서드
    public void StartScrolling()
    {
        scrollingCoroutine = StartCoroutine(ScrollBackground());
    }

    IEnumerator ScrollBackground()
    {
        PlayerController playerController = FindObjectOfType<PlayerController>();
        playerController.MoveModeStart();
        while (true)
        {
            for (int i = 0; i < backgrounds.Length; i++)
            {
                backgrounds[i].position -= new Vector3(scrollSpeed * Time.deltaTime, 0, 0);

                if (backgrounds[i].position.x < -backgroundPrefab.GetComponent<SpriteRenderer>().bounds.size.x)
                {
                    backgrounds[i].position = new Vector3(backgroundPrefab.GetComponent<SpriteRenderer>().bounds.size.x, 2.16f, 2);

                    playerController.MoveModeEnd();
                    GameUI gameUI=FindObjectOfType<GameUI>();
                    StageManager.instance.SetStage();

                    playerController.BattleModeStart();
                    gameUI.NormalEnemyHunting();

                    StopCoroutine(scrollingCoroutine);
                }
            }

            yield return null; // 한 프레임 대기
        }
    }
}
