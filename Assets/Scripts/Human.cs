using UnityEngine.AI;
using UnityEngine;

[RequireComponent(typeof(NavMeshAgent)), RequireComponent(typeof(Animator))]
public class Human : MonoBehaviour
{
    private readonly string Run = "_Running";
    private readonly string TURN = "_Turn";

    private bool isTurn = false;
    private bool isArrived = false;

    private float preSpeed = 0f;
    private Vector3 prePosition = Vector3.zero;

    private Animator humanAnimator = null;
    private NavMeshAgent humanAgent = null;
    private NavMeshPath humanPath = null;

    private void Start()
    {
        humanAgent = GetComponent<NavMeshAgent>();
        humanAnimator = GetComponent<Animator>();
        preSpeed = humanAgent.speed;

        prePosition = transform.position;
    }

    private void LateUpdate()
    {
        HandleAnimation();
    }

    private void HandleAnimation()
    {
        if (isArrived) return;

        if (isTurn)
        {
            if (humanAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= .9f)
            {
                isTurn = false;
                humanAnimator.SetBool(TURN, false);

                humanAgent.isStopped = false;
            }
        }
        else if (humanAgent.remainingDistance < humanAgent.stoppingDistance)
        {
            isArrived = true;
            humanAnimator.SetBool(Run, false);
        }
    }

    public void MovePosition(Vector3 position)
    {
        if (isTurn) return;

        humanPath = new NavMeshPath();
        humanAgent.CalculatePath(position, humanPath);
        if (humanPath.status == NavMeshPathStatus.PathComplete)
        {
            var speed = Mathf.Clamp01(humanAgent.velocity.sqrMagnitude / preSpeed);
            if (speed > 0 && Vector3.Angle(position - transform.position, prePosition - transform.position) > 120f)
            {
                isTurn = true;
                humanAnimator.SetBool(TURN, true);

                humanAgent.isStopped = true;
            }
            else
            {
                position.y = transform.position.y;
                transform.LookAt(position);
            }

            isArrived = false;
            humanAnimator.SetBool(Run, true);
            humanAgent.SetPath(humanPath);
            prePosition = position;
        }
    }
}
