using UnityEngine;

public class CameraPlayerFollow : MonoBehaviour
{
    [Header("Camera Options")]
    public Transform player;
    public Vector3 offset;
    public float rotationSpeed = 5.0f;
    [Space]

    [Header("Limitations")]
    public float minYAngle = -20.0f;
    public float maxYAngle = 60.0f;

    private float mouseX, mouseY;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void LateUpdate()
    {
        mouseX += Input.GetAxis("Mouse X") * rotationSpeed;
        mouseY -= Input.GetAxis("Mouse Y") * rotationSpeed;

        mouseY = Mathf.Clamp(mouseY, minYAngle, maxYAngle);

        transform.position = DesiredPosition();
        transform.LookAt(player.position);
    }

    Vector3 DesiredPosition()
    {
        Quaternion rotation = Quaternion.Euler(mouseY, mouseX, 0);

        Vector3 desiredPosition = player.position - (rotation * offset);

        return desiredPosition;
    }
}
