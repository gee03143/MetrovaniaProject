using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState<T>
{
    void OperateEnter(T actor);
    void OperateUpdate(T actor);
    void OperateExit(T actor);
}

public class StateMachine<T>
{
    private T actor;
    public IState<T> CurrentState;

    //기본 상태
    public StateMachine(IState<T> defaultState, T who)
    {
        actor = who;
        CurrentState = defaultState;
        CurrentState.OperateEnter(actor);
    }

    public void SetState(IState<T> state)
    {
        //같은 행동을 연이어서 세팅하지 못하도록 예외처리.
        if (CurrentState == state)
        {
            //Debug.Log("현재 이미 해당 상태입니다.");
            return;
        }

        CurrentState.OperateExit(actor);
        Debug.Log(CurrentState + " changed to " + state);
        CurrentState = state;
        CurrentState.OperateEnter(actor);
    }

    public void DoOperateUpdate(T actor)
    {
        CurrentState.OperateUpdate(actor);
    }
}
