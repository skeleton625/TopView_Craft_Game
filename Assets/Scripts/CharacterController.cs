using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [Header("Human Setting"), Space(10)]
    [SerializeField] private Human HumanPrefab = null;

    private Camera mainCamera = null;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000, -1))
                HumanPrefab.MovePosition(hit.point);
        }
    }
}
