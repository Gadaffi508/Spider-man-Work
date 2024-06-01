using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Grappling : MonoBehaviour
{
    public LayerMask whatIsGrappleable;

    public Transform player;

    private float maxDistance = 100;

    private LineRenderer lineRenderer;

    private Vector3 grapplePoint;

    private TwoBoneIKConstraint constraint;

    private PlayerMovement pm;

    private SpringJoint joint;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        constraint = GetComponentInParent<TwoBoneIKConstraint>();

        pm = GetComponentInParent<PlayerMovement>();

        lineRenderer.positionCount = 0;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !pm.wallRunning)
        {
            StartGrapple();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopGrapple();
        }
    }

    private void LateUpdate() => DrawRope();

    void StartGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance, whatIsGrappleable))
        {
            pm.anim.SetBool("spider",true);

            grapplePoint = hit.point;

            joint = player.gameObject.AddComponent<SpringJoint>();

            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;

            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

            constraint.weight = 1;

            lineRenderer.positionCount = 2;
        }
    }

    void StopGrapple()
    {
        constraint.weight = 0;

        pm.anim.SetBool("spider", false);

        lineRenderer.positionCount = 0;
        Destroy(joint);
    }

    void DrawRope()
    {
        if(!joint) return;

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, grapplePoint);
    }
}
