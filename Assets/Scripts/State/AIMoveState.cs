using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMoveState : State<AIController>
{
    private float[] m_interests;
    private Collider2D[] m_interestResults;

    private float[] m_dangers;
    private Collider2D[] m_dangerResults; // Currently unused because danger detection is done by raycast and not overlap.  May be necessary later.

    private Vector2 m_chosenDirection;
    private Vector2[] m_rayDirections;

    public AIMoveState(AIController actor, bool interrupt) : base(actor, interrupt) 
    {
        m_interests = new float[m_actor.NumRays];
        m_interestResults = new Collider2D[1];

        m_dangers = new float[m_actor.NumRays];
        m_dangerResults = new Collider2D[m_actor.NumRays];

        m_chosenDirection = Vector2.zero;
        m_rayDirections = new Vector2[m_actor.NumRays];

        for (int _i = 0; _i < m_actor.NumRays; _i++)
        {
            float _angle = _i * 2 * Mathf.PI / m_actor.NumRays;
            m_rayDirections[_i] = new Vector2(Mathf.Cos(_angle), Mathf.Sin(_angle)).normalized;
        }
    }

    public override void Enter()
    {
        base.Enter();

        m_actor.StartCoroutine(Tick());
        m_actor.StartCoroutine(SetInterest());
        m_actor.StartCoroutine(SetDanger());
        m_actor.StartCoroutine(SetDirection());

        m_actor.StartCoroutine(DebugDraw()); // For debugging
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override IEnumerator Tick()
    {
        while (m_isActiveState)
        {
            Vector2 _vel = m_actor.Rigidbody.velocity;
            _vel.x = m_chosenDirection.x * m_actor.Speed;
            _vel.y = m_chosenDirection.y * m_actor.Speed;
            m_actor.Rigidbody.velocity = _vel;

            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator DebugDraw()
    {
        while (m_isActiveState)
        {
            foreach (Vector2 _ray in m_rayDirections)
            {
                Debug.DrawRay(m_actor.transform.position, _ray, Color.yellow);
            }

            for (int _i = 0; _i < m_actor.NumRays; _i++)
            {
                Vector2 _interest = m_rayDirections[_i] * m_interests[_i];
                Debug.DrawRay(m_actor.transform.position, _interest, Color.green);

                Vector2 _danger = m_rayDirections[_i] * m_dangers[_i];
                Debug.DrawRay(m_actor.transform.position, _danger, Color.red);
            }

            Debug.DrawRay(m_actor.transform.position, m_chosenDirection, Color.blue);

            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator SetInterest()
    {
        while (m_isActiveState)
        {
            Physics2D.OverlapCircleNonAlloc(m_actor.transform.position, m_actor.Range, m_interestResults, m_actor.InterestLayer);

            // Forgive me for the O(n^2)
            foreach (Collider2D _col in m_interestResults)
            {
                if (_col)
                {
                    for (int _i = 0; _i < m_actor.NumRays; _i++)
                    {
                        Vector2 _direction = _col.transform.position - m_actor.transform.position;

                        float _d = Vector2.Dot(m_rayDirections[_i], _direction) / _direction.magnitude;
                        m_interests[_i] = Mathf.Max(0.1f, _d);
                    }
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator SetDanger()
    {
        while (m_isActiveState)
        {
            for (int _i = 0; _i < m_actor.NumRays; _i++)
            {
                RaycastHit2D _result = Physics2D.Raycast(m_actor.transform.position, m_rayDirections[_i], m_actor.Range, m_actor.DangerLayer);
                m_dangers[_i] = _result ? 1f : 0f;
            }

            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator SetDirection()
    {
        while (m_isActiveState)
        {
            m_chosenDirection = Vector2.zero;
            for (int _i = 0; _i < m_actor.NumRays; _i++)
            {
                if (m_dangers[_i] > 0f)
                {
                    m_interests[_i] = 0f;
                }

                m_chosenDirection += m_rayDirections[_i] * m_interests[_i];
            }

            m_chosenDirection.Normalize();

            yield return new WaitForFixedUpdate();
        }
    }
}
