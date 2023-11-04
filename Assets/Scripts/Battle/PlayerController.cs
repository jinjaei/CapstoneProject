using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    private MonsterSettings monsterSettings;
    [HideInInspector]
    public int targerPosition = 0;


    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        BattleModeStart();
    }
    private void AttackTarget()
    {
        GameObject monster = StageManager.instance.monsterGeneratePosition[targerPosition].GetChild(1).GetChild(targerPosition).gameObject;
        MonsterSettings targetInfo = monster.GetComponentInChildren<MonsterSettings>();
        if (targetInfo != null)
        {
            targetInfo.TakeDamage(PlayerStatManager.instance.playerPower);
        }
    }
    public void BattleModeStart()
    {
        StartCoroutine("PerformAttack");
    }
    public void BattleModeEnd()
    {
        StopCoroutine("PerformAttack");
    }
    IEnumerator PerformAttack()
    {
        yield return new WaitForSeconds(2f);
        while (true)
        {
            if (targerPosition > 4)
            {
                targerPosition = 0;
                yield return new WaitForSeconds(0.5f);
                StageManager.instance.SetStage();
                yield return new WaitForSeconds(2f);
            }
            // 공격 애니메이션 재생
            animator.SetTrigger("AttackState");
            yield return new WaitForSeconds(0.01f);
            animator.SetTrigger("IdleState");

            // 몬스터에게 데미지 입힘
            AttackTarget();


            yield return new WaitForSeconds(PlayerStatManager.instance.playerCoolDown); // 공격 속도에 따라 대기

        }

    }
}
