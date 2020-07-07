using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//다음에 다시 시도해볼 코드

public abstract class InputInterface
{

    public abstract bool Support();
    public abstract void Execute();
}

public class JumpInput : InputInterface
{
    public override bool Support()
    {
        return Input.GetButtonDown("Jump");
    }

    public override void Execute()
    {
        if (Player.Instance.stateMachine.CurrentState == Player.Instance.dicState[PlayerState.Run])
        {
            Player.Instance.stateMachine.SetState(Player.Instance.dicState[PlayerState.Jump]);
        }
    }
}

public class AttackInput : InputInterface
{
    public override bool Support()
    {
        return Input.GetButton("Attack");
    }

    public override void Execute()
    {
        if (Player.Instance.stateMachine.CurrentState == Player.Instance.dicState[PlayerState.Run])
        {
            Player.Instance.stateMachine.SetState(Player.Instance.dicState[PlayerState.Attack]);
        }
        else if (Player.Instance.stateMachine.CurrentState == Player.Instance.dicState[PlayerState.Attack] && Player.Instance.waitingNextInput)
        {
            Player.Instance.doNextAttack = true;
        }
        else if (Player.Instance.stateMachine.CurrentState == Player.Instance.dicState[PlayerState.Dash])
        {
            Player.Instance.stateMachine.SetState(Player.Instance.dicState[PlayerState.DashAttack]);
        }
    }
}

public class DashInput : InputInterface
{
    public override bool Support()
    {
        return Input.GetButtonDown("Dash");
    }

    public override void Execute()
    {
        if (Player.Instance.stateMachine.CurrentState == Player.Instance.dicState[PlayerState.Run])
        {
            Player.Instance.stateMachine.SetState(Player.Instance.dicState[PlayerState.Dash]);
        }
    }
}

public class ShootInput : InputInterface
{
    public override bool Support()
    {
        return Input.GetButtonDown("Shoot");
    }

    public override void Execute()
    {
        if (Player.Instance.stateMachine.CurrentState == Player.Instance.dicState[PlayerState.Run])
        {
            Player.Instance.stateMachine.SetState(Player.Instance.dicState[PlayerState.Shoot]);
        }
    }
}
