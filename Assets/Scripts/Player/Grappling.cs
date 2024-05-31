using UnityEngine;

public class Grappling : MonoBehaviour
{
    public LayerMask whatIsGrappleable;

    public Transform player;

    private float maxDistance = 100;

    private LineRenderer lineRenderer;

    private Vector3 grapplePoint;

    private SpringJoint joint;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            StartGrapple();
        }
        else if(Input.GetMouseButtonUp(0))
        {

        }
    }

    private void LateUpdate() => DrawRope();

    void StartGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward,out hit,maxDistance,whatIsGrappleable))
        {
            grapplePoint = hit.point;

            joint = player.gameObject.AddComponent<SpringJoint>();

            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position,grapplePoint);

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;

            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;
        }
    }

    void StopGrapple()
    {

    }

    void DrawRope()
    {
        lineRenderer.SetPosition(0,transform.position);
        lineRenderer.SetPosition(1,grapplePoint);
    }
}
