using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine<A> : MonoBehaviour where A : MonoBehaviour
{
    protected State<A> m_state;

    private bool m_started = false;

    public void StartStateMachine(State<A> state)
    {
        m_started = true;
        m_state = state;
        m_state.Enter();
    }

    public void SetState(State<A> state)
    {
        if (!m_started)
        {
            StartStateMachine(state);
            return;
        }

        if (!m_state.IsInteruptable)
        {
            return;
        }

        m_state.Exit();

        m_state = state;
        m_state.Enter();
    }
}
