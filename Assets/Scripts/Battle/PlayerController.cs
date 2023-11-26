using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator animator;
    private MonsterSettings monsterSettings;
    [HideInInspector]
    public int targerPosition = 0;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        BattleModeStart();
    }
    public void ResetAnimator()
    {
        animator = GetComponentInChildren<Animator>();
        BattleModeStart();
    }
    private void AttackTarget()
    {
        // 위치에 따른 공격
        GameObject monster = StageManager.instance.monsterGeneratePosition[targerPosition].GetChild(1).GetChild(targerPosition).gameObject;
        MonsterSettings targetInfo = monster.GetComponentInChildren<MonsterSettings>();
        if (targetInfo != null)
        {
            targetInfo.TakeDamage(PlayerStatManager.instance.playerPower);
        }
    }
    private void BossAttackTarget()
    {
        // 보스 몬스터 공격
        GameObject bossMonster = StageManager.instance.monsterGeneratePosition[4].GetChild(1).GetChild(4).GetChild(0).gameObject;
        BossMonsterController targetInfo = bossMonster.GetComponent<BossMonsterController>();
        if (targetInfo != null)
        {
            targetInfo.TakeDamage(PlayerStatManager.instance.playerPower);
        }
    }
    public void BattleModeStart()
    {
        // 몬스터 사냥 모드 시작
        StartCoroutine("PerformAttack");
    }
    public void BattleModeEnd()
    {
        // 몬스터 사냥 모드 종료
        StopCoroutine("PerformAttack");
    }
    public void BossBattleModeStart()
    {
        // 보스몬스터 사냥 모드 시작
        StartCoroutine("BossPerformAttack");
    }
    public void BossBattleModeEnd()
    {
        // 보스몬스터 사냥 모드 종료
        StopCoroutine("BossPerformAttack");
    }
    public void MoveModeStart()
    {
        // 이동 모드 시작
        animator.SetTrigger("MoveState");
    }
    public void MoveModeEnd()
    {
        // 이동 모드 종료
        animator.SetTrigger("IdleState");
    }
    IEnumerator PerformAttack()
    {
        yield return new WaitForSeconds(2f);
        while (true)
        {
            if (targerPosition > 4)
            {
                // 모든 몬스터를 처치 시

                StageManager.instance.bossMonsterAble = false;

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
    IEnumerator BossPerformAttack()
    {
        yield return new WaitForSeconds(2f);
        while (true)
        {
            // 공격 애니메이션 재생
            animator.SetTrigger("AttackState");
            yield return new WaitForSeconds(0.01f);
            animator.SetTrigger("IdleState");

            // 몬스터에게 데미지 입힘
            BossAttackTarget();


            yield return new WaitForSeconds(PlayerStatManager.instance.playerCoolDown); // 공격 속도에 따라 대기

        }

    }
}
