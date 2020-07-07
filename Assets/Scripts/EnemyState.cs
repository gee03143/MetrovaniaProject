using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    Idle,
    Chase,
    Attack,
    Damaged
}

public class StateEIdle : IState<EnemyBase>
{

    public void OperateEnter(EnemyBase enemyRef)
    {
        enemyRef.anim.Play("crab_walk");
    }

    public void OperateUpdate(EnemyBase enemyRef)
    {
        if (enemyRef.TheresLedge())
        {
            enemyRef.direction *= -1;
        }
        if(Mathf.Abs(enemyRef.playerDifference) < enemyRef.followRange)
        {
            enemyRef.ChangeState(EnemyState.Chase);
        }

        enemyRef.move.x = 1;
        enemyRef.move *= enemyRef.direction;
    }

    public void OperateExit(EnemyBase enemyRef)
    {

    }
}

public class StateEChase : IState<EnemyBase>
{

    public void OperateEnter(EnemyBase enemyRef)
    {
        enemyRef.anim.Play("crab_walk");
    }

    public void OperateUpdate(EnemyBase enemyRef)
    {
        enemyRef.direction = (enemyRef.playerDifference < 0) ? -1 : 1;

        if (enemyRef.TheresLedge())
        {
            enemyRef.direction *= -1;
        }

        if (Mathf.Abs(enemyRef.playerDifference) > enemyRef.followRange)
        {
            enemyRef.ChangeState(EnemyState.Idle);
        }

        enemyRef.move.x = 1;
        enemyRef.move *= enemyRef.direction;
    }

    public void OperateExit(EnemyBase enemyRef)
    {
    }
}


public class StateEAttack : IState<EnemyBase>
{
    public void OperateEnter(EnemyBase enemyRef)
    {

    }

    public void OperateUpdate(EnemyBase enemyRef)
    {

    }

    public void OperateExit(EnemyBase enemyRef)
    {
    }
}

public class StateEDamaged : IState<EnemyBase>
{
    float timer = 0;

    public void OperateEnter(EnemyBase enemyRef)
    {
        enemyRef.velocity.y = enemyRef.launchPower;
        enemyRef.anim.Play("crab_damaged");
        timer = 0;
    }

    public void OperateUpdate(EnemyBase enemyRef)
    {
        if (timer < enemyRef.launchTime)
        {
            timer += Time.deltaTime;
            enemyRef.move.x = 0f;
            if (enemyRef.launchDirection == -1)   //왼쪽으로 날아가야 한다면
                enemyRef.move.x = -enemyRef.launchDistance / enemyRef.launchTime;
            else
                enemyRef.move.x = enemyRef.launchDistance / enemyRef.launchTime;
        }
        else
        {
            enemyRef.ChangeState(EnemyState.Idle);
        }
    }

    public void OperateExit(EnemyBase enemyRef)
    {
    }


}
