using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State<A> where A : MonoBehaviour
{
    public bool IsInteruptable;

    protected A m_actor;
    protected bool m_isActiveState;

    public State(A actor, bool interrupt)
    {
        IsInteruptable = interrupt;

        m_actor = actor;
        m_isActiveState = false;
    }

    public virtual void Enter() 
    {
        m_isActiveState = true;
    }
    public virtual void Exit()
    {
        m_isActiveState = false;
    }
    public virtual IEnumerator Tick() 
    {
        yield break;
    }
}
