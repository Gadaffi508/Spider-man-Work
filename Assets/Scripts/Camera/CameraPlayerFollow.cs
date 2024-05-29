using UnityEngine;

public class CameraPlayerFollow : MonoBehaviour
{
    public Transform player;
    public Quaternion playerRotation;
    public Vector3 offsetVector;

    void Start()
    {
        offsetVector = transform.position - player.position;
    }

    void Update()
    {
        playerRotation = player.transform.rotation;
        Vector3 offsetRotated = playerRotation * offsetVector;

        transform.position = player.position + offsetRotated;
        transform.rotation = player.rotation;
    }
}
