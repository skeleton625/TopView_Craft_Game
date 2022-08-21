using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

[RequireComponent(typeof(NavMeshAgent)), RequireComponent(typeof(Animator))]
public class Human : MonoBehaviour
{
    private readonly string SPEED = "_Speed";

    private float preSpeed = 0f;
    private NavMeshAgent humanAgent = null;
    private Animator humanAnimator = null;

    private void Start()
    {
        humanAgent = GetComponent<NavMeshAgent>();
        humanAnimator = GetComponent<Animator>();
        preSpeed = humanAgent.speed;
    }

    private void Update()
    {
        HandleAnimation();
    }

    private void HandleAnimation()
    {
        var speed = Mathf.Clamp01(humanAgent.velocity.sqrMagnitude / preSpeed);
        humanAnimator.SetFloat(SPEED, speed);
    }

    public void MovePosition(Vector3 position)
    {
        var path = new NavMeshPath();
        humanAgent.CalculatePath(position, path);
        if (path.status == NavMeshPathStatus.PathComplete)
        {
            position.y = transform.position.y;
            humanAgent.SetPath(path);
            transform.LookAt(position);
        }
    }
}
