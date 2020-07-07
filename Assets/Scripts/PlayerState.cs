using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Run,
    Attack,
    Jump,
    Dash,
    DashAttack,
    Fall,
    Shoot,
    Damaged
}

public class StateRun : IState<Player>
{

    public void OperateEnter(Player playerRef)
    {
        playerRef.anim.Play("player_idle");
    }

    public void OperateUpdate(Player playerRef)
    {
        playerRef.GetHorizontalMovement();
    }

    public void OperateExit(Player playerRef)
    {

    }
}

public class StateJump : IState<Player>
{
    public void OperateEnter(Player playerRef)
    {
        playerRef.anim.Play("player_jump");
        playerRef.velocity.y += playerRef.jumpTakeOffSpeed;
    }

    public void OperateUpdate(Player playerRef)
    {
        playerRef.GetHorizontalMovement();
        if (Input.GetButtonUp("Jump"))    //점프 캔슬, 점프 버튼에서 손을 떼면 더 적게 뜀
        {
            if (playerRef.velocity.y > 0)
                playerRef.velocity.y *= 0.5f;
        }
    }

    public void OperateExit(Player playerRef)
    {
    }
}


public class StateDash : IState<Player>
{
    public void OperateEnter(Player playerRef)
    {
        playerRef.anim.Play("player_dash");
        playerRef.ToggleUndamagable();
    }

    public void OperateUpdate(Player playerRef)
    {
        playerRef.DashMovement();
    }

    public void OperateExit(Player playerRef)
    {
        playerRef.ToggleUndamagable();
    }
}

public class StateAttack : IState<Player>
{
    public void OperateEnter(Player playerRef)
    {
        playerRef.anim.Play("player_attack1");
    }

    public void OperateUpdate(Player playerRef)
    {
        //공격 중에는 이동 불가
    }

    public void OperateExit(Player playerRef)
    {
    }
}

public class StateDashAttack : IState<Player>
{
    public void OperateEnter(Player playerRef)
    {
        playerRef.anim.Play("player_attack3");
    }

    public void OperateUpdate(Player playerRef)
    {
        //공격 중에는 이동 불가
    }

    public void OperateExit(Player playerRef)
    {
    }
}

public class StateFall : IState<Player>
{
    public void OperateEnter(Player playerRef)
    {
        playerRef.anim.Play("player_fall");
    }

    public void OperateUpdate(Player playerRef)
    {
        playerRef.GetHorizontalMovement();
    }

    public void OperateExit(Player playerRef)
    {

    }
}

public class StateShoot : IState<Player>
{
    public void OperateEnter(Player playerRef)
    {
        playerRef.anim.Play("player_shoot");
    }

    public void OperateUpdate(Player playerRef)
    {
        //공격 중에는 이동 불가
    }

    public void OperateExit(Player playerRef)
    {

    }
}

public class StateDamaged : IState<Player>
{
    float timer = 0;

    public void OperateEnter(Player playerRef)
    {
        playerRef.velocity.y = playerRef.launchPower;
        playerRef.anim.Play("player_damaged");
        timer = 0;
        playerRef.ToggleUndamagable();
        playerRef.Invoke("ToggleUndamagable", playerRef.undamagableTime);
    }

    public void OperateUpdate(Player playerRef)
    {
        if (timer < playerRef.launchTime)
        {
            timer += Time.deltaTime;
            playerRef.HitReaction();
        }
        else
        {
            playerRef.ChangeState(PlayerState.Run);
        }
    }

    public void OperateExit(Player playerRef)
    {
    }
}