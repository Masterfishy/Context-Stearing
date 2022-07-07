using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextSteering : MonoBehaviour
{
    public LayerMask m_interestLayer;
    public LayerMask m_dangerLayer;

    [SerializeField]
    private float m_speed;

    [SerializeField]
    private float m_interestRange;

    [SerializeField]
    private float m_dangerRange;

    [SerializeField]
    private int m_numRays;

    private Rigidbody2D rb;

    private float[] m_interests;
    private Collider2D[] m_interestResults;

    private float[] m_dangers;
    private Collider2D[] m_dangerResults;
    
    private Vector2 m_chosenDirection;
    private Vector2[] m_rayDirections;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        m_interests = new float[m_numRays];
        m_interestResults = new Collider2D[1];

        m_dangers = new float[m_numRays];
        m_dangerResults = new Collider2D[m_numRays];
        
        m_chosenDirection = Vector2.zero;
        m_rayDirections = new Vector2[m_numRays];
    }

    private void Start()
    {
        for (int _i = 0; _i < m_numRays; _i++)
        {
            float _angle = _i * 2 * Mathf.PI / m_numRays;
            m_rayDirections[_i] = new Vector2(Mathf.Cos(_angle), Mathf.Sin(_angle)).normalized;
        }

        StartCoroutine(SetInterest());
        StartCoroutine(SetDanger());
        StartCoroutine(SetDirection());

        StartCoroutine(DebugDraw()); // For debugging
    }

    private void Update()
    {
        // Interest directions
        //foreach (Vector2 entry in m_interestMap)
        //{
        //    Debug.DrawLine(transform.position, entry + (Vector2)transform.position, Color.green);
        //}

        for (int _i = 0; _i < m_numRays; _i++)
        {
            Vector2 _interest = m_rayDirections[_i] * m_interests[_i];
            Debug.DrawRay(transform.position, _interest, Color.green);

            Vector2 _danger = m_rayDirections[_i] * m_dangers[_i];
            Debug.DrawRay(transform.position, _danger, Color.red);
        }

        Debug.DrawRay(transform.position, m_chosenDirection, Color.blue);
    }

    private void FixedUpdate()
    {
        // Movement
        Vector2 _vel = rb.velocity;
        _vel.x = m_chosenDirection.x * m_speed;
        _vel.y = m_chosenDirection.y * m_speed;
        rb.velocity = _vel;
    }

    private IEnumerator SetInterest()
    {
        while (isActiveAndEnabled)
        {
            Physics2D.OverlapCircleNonAlloc(transform.position, m_interestRange, m_interestResults, m_interestLayer);

            // For give me for the O(n^2) 2022 Note: not really n^2 its dim(m_interestsResults) * m_numRays
            foreach (Collider2D _col in m_interestResults)
            {
                if (_col)
                {
                    for (int _i = 0; _i < m_numRays; _i++)
                    {
                        Vector2 _direction = _col.transform.position - transform.position;

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
        while (isActiveAndEnabled)
        {
            for (int _i = 0; _i < m_numRays; _i++)
            {
                RaycastHit2D _result = Physics2D.Raycast(transform.position, m_rayDirections[_i], m_dangerRange, m_dangerLayer);
                if (_result)
                {
                    float _weight = 1f;
                    float _distanceToDanger = _result.distance;

                    SteeringObject _so = _result.collider.gameObject.GetComponent<SteeringObject>();
                    if (_so != null)
                    {
                        _weight = 1 - (_distanceToDanger / m_dangerRange);
                    }

                    m_dangers[_i] = _weight;
                }
                else
                {
                    m_dangers[_i] = 0f;
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator SetDirection()
    {
        while (isActiveAndEnabled)
        {
            for (int _i = 0; _i < m_numRays; _i++)
            {
                if (m_dangers[_i] > 0f)
                {
                    m_interests[_i] = 0f;
                }
            }

            m_chosenDirection = Vector2.zero;
            for (int _i = 0; _i < m_numRays; _i++)
            {
                m_chosenDirection += m_rayDirections[_i] * (m_interests[_i] - m_dangers[_i]);
            }

            m_chosenDirection.Normalize();

            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator DebugDraw()
    {
        while (isActiveAndEnabled)
        {
            for (int _i = 0; _i < m_numRays; _i++)
            {
                Vector2 _default = m_rayDirections[_i].normalized;
                Debug.DrawRay(transform.position, _default, Color.yellow);

                Vector2 _interest = m_rayDirections[_i] * m_interests[_i];
                Debug.DrawRay(transform.position, _interest, Color.green);

                Vector2 _danger = m_rayDirections[_i] * m_dangers[_i];
                Debug.DrawRay(transform.position, _danger, Color.red);
            }

            Debug.DrawRay(transform.position, m_chosenDirection, Color.blue);

            yield return new WaitForFixedUpdate();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, m_interestRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_dangerRange);
    }
}
