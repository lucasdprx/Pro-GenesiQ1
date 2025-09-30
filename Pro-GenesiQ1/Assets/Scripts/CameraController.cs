using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float scrollSpeed = 5f;
    [SerializeField] private float lookSpeed = 5f;
    
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Transform cameraPivotTransform;
    private Transform cameraTransform;
    
    private void Start()
    {
        cameraPivotTransform = transform;
        cameraTransform = Camera.main?.transform;
        InputManager.OnMoveInput += SetMoveInput;
        InputManager.OnLookInput += SetLookInput;
    }

    private void Update()
    {
        Move();

        if (Input.GetMouseButton(1))
        {
            cameraTransform.eulerAngles += new Vector3(-lookInput.y, 0, 0) * (lookSpeed * Time.deltaTime);
            cameraPivotTransform.eulerAngles += new Vector3(0, lookInput.x, 0) * (lookSpeed * Time.deltaTime);
        }
    }

    private void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    private void SetLookInput(Vector2 input)
    {
        lookInput = input;
    }
    
    private void Move()
    {
        Vector3 move = cameraTransform.forward * moveInput.y + cameraTransform.right * moveInput.x;
        cameraPivotTransform.position += move * (moveSpeed * Time.deltaTime);
    }
}
