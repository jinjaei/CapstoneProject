using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterSettings : MonoBehaviour
{
    public Slider monsterHPBar; // 체력 바
    public MonsterDataTable monsterDataTable;
    private Animator animator;

    private float currentHealth; // 현재 체력바
    private float maxHealth; //최대 체력

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        maxHealth = monsterDataTable.monsterHP;
        currentHealth = maxHealth;
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
        PlayerController playerController = FindObjectOfType<PlayerController>();
        playerController.targerPosition++;
        
        // 다음 타겟으로 변경
        StartCoroutine(DeadMotion());

        ResourceManager.instance.AddResource(ResourceManager.ResourceType.Stone, monsterDataTable.dropResorceAmount);
        // 드랍 양 만큼 자원추가
    }

    private void UpdateHealthUI()
    {
        if (monsterHPBar != null)
        {
            // 슬라이더의 값을 업데이트하여 현재 체력을 반영합니다.
            monsterHPBar.value = (float)(currentHealth / maxHealth)*100f;
        }
    }
    IEnumerator DamageMotion()
    {
        // 공격 애니메이션 재생
        animator.SetTrigger("Damage");
        yield return new WaitForSeconds(0.01f); // 공격 속도에 따라 대기
        UpdateHealthUI();
        animator.SetTrigger("Idle");
    }
    IEnumerator DeadMotion()
    {
        // 공격 애니메이션 재생
        animator.SetTrigger("Dead");
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
