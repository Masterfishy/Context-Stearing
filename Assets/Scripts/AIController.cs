using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : StateMachine<AIController>
{
    [Header("Context Steering Values")]
    public float Speed;
    public float Range;
    public int NumRays;
    public LayerMask InterestLayer;
    public LayerMask DangerLayer;

    [Header("Attacking Values")]
    [SerializeField] private float m_canAttackRange;
    [SerializeField] private float m_cooldown;
    public float AttackDistance;
    public float AttackSpeed;
    public float AttackEndAccuracy;
    [HideInInspector] public Vector2 AttackDirection;

    [HideInInspector] public Rigidbody2D Rigidbody;
    [HideInInspector] public AIMoveState MoveState;
    [HideInInspector] public AIAttackState AttackState;

    private float m_nextAttackTime;

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();

        MoveState = new AIMoveState(this, true);
        AttackState = new AIAttackState(this, true);

        m_nextAttackTime = 0f;
    }

    private void Start()
    {
        StartStateMachine(MoveState);
    }

    private void FixedUpdate()
    {
        if (m_nextAttackTime < Time.time)
        {
            Collider2D _hit = Physics2D.OverlapCircle(transform.position, m_canAttackRange, InterestLayer);
            if (_hit)
            {
                AttackDirection = _hit.transform.position - transform.position;

                SetState(AttackState);

                m_nextAttackTime = Time.time + m_cooldown;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, Range);
        Gizmos.DrawWireSphere(transform.position, m_canAttackRange);
        Gizmos.DrawWireSphere(transform.position, AttackDistance);
        Gizmos.DrawRay(transform.position, AttackDirection);
    }
}
 