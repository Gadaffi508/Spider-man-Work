using System.Collections;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Grappling : MonoBehaviour
{
    [Header("Options")] public LayerMask whatIsGrappleable;

    public Transform player;

    public GameObject line;

    //<----------------------------------------------------------------->

    private float maxDistance = 100;

    private Vector3 grapplePoint;

    private TwoBoneIKConstraint constraint;

    private LineRenderer lineRenderer;

    private PlayerMovement pm;

    private SpringJoint joint;

    private void Start()
    {
        constraint = GetComponentInParent<TwoBoneIKConstraint>();

        pm = GetComponentInParent<PlayerMovement>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !pm.wallRunning)
            AnimationLine();

        if (Input.GetMouseButtonUp(0))
            StopGrapple();

        if (Input.GetMouseButton(0))
            lineRenderer?.SetPosition(0, transform.position);
    }

    void StartGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;
            pm.anim.SetBool("spider", true);

            joint = player.gameObject.AddComponent<SpringJoint>();

            SpringJointOptions();
        }
    }

    void StopGrapple()
    {
        constraint.weight = 0;

        pm.anim.SetBool("spider", false);

        Destroy(joint);
        joint = null;
    }

    void SpringJointOptions()
    {
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = grapplePoint;

        float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

        joint.maxDistance = distanceFromPoint * 0.8f;
        joint.minDistance = distanceFromPoint * 0.25f;

        joint.spring = 4.5f;
        joint.damper = 7f;
        joint.massScale = 4.5f;
    }

    void AnimationLine()
    {
        lineRenderer = null;

        lineRenderer = SpiderWeb();

        constraint.weight = 1;
        
        lineRenderer.SetPosition(1, AnimationLinerenderer());

        StartGrapple();
    }

    Vector3 AnimationLinerenderer()
    {
        return Vector3.Lerp(transform.position, grapplePoint, 1);
    }

    LineRenderer SpiderWeb()
    {
        GameObject _line = Instantiate(line);
        return _line.GetComponent<LineRenderer>();
    }
}