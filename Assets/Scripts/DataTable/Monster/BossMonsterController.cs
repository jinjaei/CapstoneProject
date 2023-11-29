using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossMonsterController : MonoBehaviour
{
    private Animator animator;
    private GameUI gameUI;
    private PlayerController playerController;

    private float currentHealth; // 현재 체력바
    private float maxHealth; //최대 체력

    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
        gameUI = FindObjectOfType<GameUI>();
        animator = GetComponent<Animator>();
        maxHealth = StageManager.instance.bossMonsterHP;
        currentHealth = maxHealth;

        gameUI.BossHPBar.value = gameUI.BossHPBar.maxValue;
    }
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        StartCoroutine(DamageMotion());
        

        UpdateHealthUI();
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        StartCoroutine(DeadMotion());
        ResourceManager.instance.AddResource(ResourceManager.ResourceType.Stone, StageManager.instance.bossMonsterDropResource);
        // 드랍 양 만큼 자원추가

        BackgroundScrolling backgroundScrolling = FindObjectOfType<BackgroundScrolling>();
        backgroundScrolling.StartScrolling();
        gameUI.MovingStage();
        StageManager.instance.stageLevel++;
        playerController.targerPosition = 0;
        playerController.BossBattleModeEnd();

    }

    private void UpdateHealthUI()
    {
        if (gameUI.BossHPBar != null)
        {
            // 슬라이더의 값을 업데이트하여 현재 체력을 반영합니다.
            gameUI.BossHPBar.value = (float)(currentHealth / maxHealth)*100f;
        }
    }
    IEnumerator DamageMotion()
    {
        // 피격 애니메이션 재생
        animator.SetTrigger("Damage");
        yield return new WaitForSeconds(0.01f); // 공격 속도에 따라 대기
        UpdateHealthUI();
        animator.SetTrigger("Idle");
    }
    IEnumerator DeadMotion()
    {
        // 죽음 애니메이션 재생
        animator.SetTrigger("Dead");
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
