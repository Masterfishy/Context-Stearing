using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAttackState : State<AIController>
{
    private Vector2 m_attackDestination;

    public AIAttackState(AIController actor, bool interrupt) : base(actor, interrupt) 
    {
    }

    public override void Enter()
    {
        base.Enter();

        RaycastHit2D _ray = Physics2D.Raycast(m_actor.transform.position, m_actor.AttackDirection, m_actor.AttackDistance, m_actor.DangerLayer);
        m_attackDestination = (Vector2)m_actor.transform.position + m_actor.AttackDirection * (_ray ? _ray.distance : m_actor.AttackDistance);

        m_actor.StartCoroutine(Tick());
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override IEnumerator Tick()
    {
        Vector2 _startPos = m_actor.transform.position;
        Vector2 _endPos = m_attackDestination;

        float _totalDistance = Vector2.Distance(_startPos, _endPos);
        float _traveledDistance = 0f;
        float _expectedFinishTime = Time.time + 0.2f; // Please ignore this magic number, thank you

        while (m_isActiveState && _expectedFinishTime > Time.time && _traveledDistance < (_totalDistance - m_actor.AttackEndAccuracy))
        {
            m_actor.transform.position = Vector2.Lerp(m_actor.transform.position, _endPos, m_actor.AttackSpeed);

            _traveledDistance = Vector2.Distance(_startPos, m_actor.transform.position);

            yield return new WaitForFixedUpdate();
        }

        m_actor.SetState(m_actor.MoveState);
    }

    //Collider2D[] _hitEnemies = Physics2D.OverlapCircleAll(m_actor.transform.position, m_actor.AttackDamageRadius);
    //        foreach (Collider2D _col in _hitEnemies)
    //        {
    //            // TODO Do damage to player
    //        }
}
