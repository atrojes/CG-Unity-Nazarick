using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Patrol : MonoBehaviour
{
    public Transform[] points;
    public float waitTime = 2f;

    NavMeshAgent agent;
    Animator anim;
    int index;
    float waitTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        anim.SetFloat("MotionSpeed", 1f);
        if (points.Length > 0) agent.SetDestination(points[0].position);
    }

    void Update()
    {
        anim.SetFloat("Speed", agent.velocity.magnitude);

        if (points.Length == 0 || agent.pathPending) return;

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime)
            {
                waitTimer = 0;
                index = (index + 1) % points.Length;
                agent.SetDestination(points[index].position);
            }
        }
    }
}