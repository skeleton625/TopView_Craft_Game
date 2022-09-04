using UnityEngine.AI;
using UnityEngine;

[RequireComponent(typeof(NavMeshAgent)), RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(Animator))]
public class HumanController : MonoBehaviour
{
    #region Animator Variables
    private enum Movement { Idle = 0, Run = 1, Turn = 2 }
    private readonly string RUN = "_Running";
    private readonly string TURN = "_Turn";
    private Movement MoveStatus = default;
    private Animator charAnimator = null;
    #endregion

    #region NavMeshAgent Variables
    private bool isTurn = false;
    private bool isArrived = false;
    private Vector3 agentPosition = Vector3.zero;

    private Camera mainCamera = null;
    private NavMeshAgent charAgent = null;
    private NavMeshPath charMeshPath = null;
    #endregion

    #region Rigidbody Variables
    [Header("Rigidbody Setting"), Space(10)]
    [SerializeField] private Vector3 UpVector = default;
    [SerializeField] private Vector3 RightVector = default;
    [SerializeField] private float RigidbodySpeed = 0f;

    private Vector3 preDirection = default;
    private Rigidbody charRigidbody = null;
    #endregion

    private void Start()
    {
        mainCamera = Camera.main;

        MoveStatus = Movement.Idle;

        charAnimator = GetComponent<Animator>();
        charRigidbody = GetComponent<Rigidbody>();

        charAgent = GetComponent<NavMeshAgent>();
        charMeshPath = new NavMeshPath();

        agentPosition = transform.position;
    }

    private void Update()
    {
        //HandleMovement_Agent();
        HandleMovement_Rigidbody();
    }

    private void HandleMovement_Agent()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000, -1)) MovePosition(hit.point);
        }

        if (isArrived || isTurn) return;

        if (charAgent.remainingDistance < charAgent.stoppingDistance)
        {
            isArrived = true;
            charAnimator.SetBool(RUN, false);
        }
    }

    private void HandleMovement_Rigidbody()
    {
        var xAxis = Input.GetAxisRaw("Horizontal");
        var zAxis = Input.GetAxisRaw("Vertical");

        var direction = RightVector * xAxis + UpVector * zAxis;
        var velocity = direction * RigidbodySpeed;

        charRigidbody.AddForce(velocity);
        if (preDirection != direction)
        {
            preDirection = direction;
            preDirection.y = transform.position.y;
            transform.LookAt(transform.position + preDirection);
            preDirection = direction;
        }

        var status = direction.Equals(Vector3.zero) ? Movement.Idle : Movement.Run;
        if (MoveStatus != status)
        {
            charAnimator.SetBool(RUN, status == Movement.Run);
            MoveStatus = status;
        }
    }

    private void MovePosition(Vector3 position)
    {
        if (isTurn || agentPosition.Equals(position)) return;

        charAgent.CalculatePath(position, charMeshPath);
        if (charMeshPath.status == NavMeshPathStatus.PathComplete)
        {
            var speed = Mathf.Clamp01(charAgent.velocity.sqrMagnitude / charAgent.speed);
            if (speed > 0 && Vector3.Angle(position = transform.position, agentPosition - transform.position) > 100f)
            {
                isTurn = true;
                charAnimator.SetBool(TURN, true);
                charAgent.isStopped = true;
            }
            else
            {
                position.y = transform.position.y;
                transform.LookAt(position);
            }

            isArrived = false;
            charAnimator.SetBool(RUN, true);
            charAgent.SetPath(charMeshPath);
            agentPosition = position;
        }
    }
}
