using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class SealifeController : MonoBehaviour
{
    public enum sealifeForwardDirection
    {
        up,
        ahead,
    }

    [Header("Sealife")]
    public Rigidbody sealifeRB;
    public float forwardSpeed = 3f;
    public float turnSpeed = .3f;

    [Header("Free Float")]
    public Transform rayOrigin;
    public sealifeForwardDirection SealifeForwardDirection;
    public float avoidanceDistance;
    public float minimumSpeed = .03f;

    [Header("Or Follow The Player")]
    public bool followThatPlayer = false;
    public Transform target;
    public float stopDistance = 1f;

    Vector3 forwardVector;
    Vector3 rotationAxis;
    Vector3 pitchAxis;

    RaycastHit hitData;
    RaycastHit leftHitData;
    RaycastHit rightHitData;
    RaycastHit topHitData;
    RaycastHit bottomHitData;
    Quaternion q;

    bool avoidLeft = false;
    bool avoidRight = false;
    bool turningAway = false;

    float horizMultiplier = 1.25f;
    float vertMultiplier = .4f;

    [Header("Debug")]
    [SerializeField] float velocity;

    void Start()
    {
        //These vector checks are to create appropriate movement depending on the type of sealife
        //For example, sealife that moves like salmon sealife or sealife that moves like octopi
        switch (SealifeForwardDirection)
        {
            case sealifeForwardDirection.up:
                forwardVector = Vector3.up;
                rotationAxis = -transform.forward;
                pitchAxis = transform.right;
                break;
            case sealifeForwardDirection.ahead:
                forwardVector = Vector3.forward;
                rotationAxis = transform.up;
                pitchAxis = transform.right;
                break;
            default:
                forwardVector = Vector3.forward;
                rotationAxis = transform.up;
                pitchAxis = transform.right;
                break;
        }
        float randomMultiplier = Random.Range(.75f, 1.25f);
        forwardSpeed = forwardSpeed * randomMultiplier;
        turnSpeed = turnSpeed * randomMultiplier;
    }

    void Update()
    {
        velocity = sealifeRB.velocity.magnitude;
    }

    void FixedUpdate()
    {
        if (followThatPlayer)
        {
            if (target != null)
            {
                if (Vector3.Distance(transform.position, target.position) > stopDistance)//Stop at a certain distance
                {
                    q = Quaternion.LookRotation(target.position - transform.position); //Rotate towards player
                    transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * forwardSpeed);
                    sealifeRB.AddForce(transform.forward * forwardSpeed, ForceMode.Force); //Move towards player
                }
            }

        }
        else if (rayOrigin != null)
        {
            int layerMask = 1 << 12;
            layerMask = ~layerMask;

            var raycastDirection = CheckForwardVectorForRaycast();
            var raycastUpDirection = CheckUpVectorForRaycast();
            var correctionVector = CheckForwardVectorForCorrection();

            //Check left and right raycasts for upcoming collisions
            if (Physics.SphereCast(rayOrigin.transform.position, .1f, (raycastDirection - transform.right), out leftHitData, avoidanceDistance * horizMultiplier, layerMask)) { avoidLeft = true; } else { avoidLeft = false; }
            if (Physics.SphereCast(rayOrigin.transform.position, .1f, (raycastDirection + transform.right), out rightHitData, avoidanceDistance * horizMultiplier, layerMask)) { avoidRight = true; } else { avoidRight = false; }

            //Check forward raycast for upcoming collisions. Move to the side if forward raycast sees something ahead. Turn around if stuck.
            if (Physics.SphereCast(rayOrigin.transform.position, .05f, raycastDirection, out hitData, avoidanceDistance, layerMask))
            {
                if (avoidLeft && !avoidRight) //Turn/scramble right
                {
                    if (sealifeRB.velocity.magnitude > minimumSpeed) //Turn right
                    {
                        sealifeRB.AddRelativeForce(forwardVector * forwardSpeed * .5f, ForceMode.Force);
                        sealifeRB.AddTorque(rotationAxis * turnSpeed, ForceMode.Force);
                    }
                    else { if (!turningAway) { turningAway = true; StartCoroutine(TurnAway("right")); } } //Scramble right
                }
                else if (avoidRight && !avoidLeft) //Turn/scramble left
                {
                    if (sealifeRB.velocity.magnitude > minimumSpeed) //Turn left
                    {
                        sealifeRB.AddRelativeForce(forwardVector * forwardSpeed * .5f, ForceMode.Force);
                        sealifeRB.AddTorque(-rotationAxis * forwardSpeed * turnSpeed, ForceMode.Force);
                    }
                    else { if (!turningAway) { turningAway = true; StartCoroutine(TurnAway("left")); } } //Scramble left
                }
                else if (!avoidLeft && !avoidRight) //Turn away from incoming ahead collision
                {
                    if (!turningAway)
                    {
                        if (sealifeRB.velocity.magnitude > minimumSpeed)
                        {
                            turningAway = true; StartCoroutine("TurnAround"); //Turn around
                        }
                        else { turningAway = true; StartCoroutine("PanicTurn"); } //Panic and scramble!
                    }
                }
                else if (avoidLeft && avoidRight) //Trapped? Rotate or scramble
                {
                    if (!turningAway)
                    {
                        if (sealifeRB.velocity.magnitude > minimumSpeed)
                        {
                            if (leftHitData.distance < rightHitData.distance) { turningAway = true; StartCoroutine(TurnAway("right")); } //Attempt escape right
                            else if (leftHitData.distance > rightHitData.distance) { turningAway = true; StartCoroutine(TurnAway("left")); } //Attempt escape left
                        }
                        else { turningAway = true; StartCoroutine("PanicTurn"); } //Panic and scramble!
                    }
                }
            }

            else
            { //Move forward if forward raycast doesn't see anything ahead, but turn a little if side raycasts do. This is the usual movement.
                if (avoidLeft && !avoidRight) //Turn right a little
                {
                    sealifeRB.AddRelativeForce(forwardVector * forwardSpeed, ForceMode.Force);
                    sealifeRB.AddTorque(rotationAxis * turnSpeed, ForceMode.Acceleration);
                }
                else if (avoidRight && !avoidLeft) //Turn left a little
                {
                    sealifeRB.AddRelativeForce(forwardVector * forwardSpeed, ForceMode.Force);
                    sealifeRB.AddTorque(-rotationAxis * turnSpeed, ForceMode.Acceleration);
                }
                else if (!avoidLeft && !avoidRight) //Full speed ahead
                {
                    sealifeRB.AddRelativeForce(forwardVector * forwardSpeed, ForceMode.Force);
                }
                else if (avoidLeft && avoidRight) //Still full speed ahead
                {
                    sealifeRB.AddRelativeForce(forwardVector * forwardSpeed, ForceMode.Force);
                }
                //Panic and scramble if stuck!
                if (sealifeRB.velocity.magnitude < minimumSpeed) { turningAway = true; StartCoroutine("PanicTurn"); } 
            }

            //Nudge the sealife a little up or down if it gets stuck vertically
            if (Physics.SphereCast(rayOrigin.transform.position, .05f, (raycastDirection + raycastUpDirection), out topHitData, avoidanceDistance * vertMultiplier, layerMask))
            {
                sealifeRB.AddTorque(pitchAxis * turnSpeed, ForceMode.Force);
            }
            if (Physics.SphereCast(rayOrigin.transform.position, .05f, (raycastDirection - raycastUpDirection), out bottomHitData, avoidanceDistance * vertMultiplier, layerMask))
            {
                sealifeRB.AddTorque(-pitchAxis * turnSpeed, ForceMode.Force);
            }

            // X- & Z-axis correction to keep sealife upright
            q = Quaternion.FromToRotation(raycastUpDirection, correctionVector) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * forwardSpeed * .5f);
        }
    }

    IEnumerator TurnAround()
    {
        sealifeRB.AddTorque(rotationAxis * forwardSpeed * Random.Range(-3f, 3f), ForceMode.Impulse);
        yield return new WaitForSeconds(2f);
        turningAway = false;
    }

    IEnumerator TurnAway(string direction)
    {
        if (direction == "right") { sealifeRB.AddTorque(rotationAxis * forwardSpeed * Random.Range(4f, 6f), ForceMode.Impulse); }
        else { sealifeRB.AddTorque(-rotationAxis * forwardSpeed * Random.Range(4f, 6f), ForceMode.Impulse); }
        yield return new WaitForSeconds(2f);
        turningAway = false;
    }

    IEnumerator PanicTurn()
    {
        sealifeRB.AddTorque(rotationAxis * forwardSpeed * Random.Range(-8f, 8f), ForceMode.Impulse);
        yield return new WaitForSeconds(2f);
        turningAway = false;
    }

    Vector3 CheckForwardVectorForRaycast()
    {
        if (SealifeForwardDirection == sealifeForwardDirection.ahead) { return transform.forward; }
        else if (SealifeForwardDirection == sealifeForwardDirection.up) { return transform.up; }
        else { return transform.forward; }
    }

    Vector3 CheckUpVectorForRaycast()
    {
        if (SealifeForwardDirection == sealifeForwardDirection.ahead) { return transform.up; }
        else if (SealifeForwardDirection == sealifeForwardDirection.up) { return transform.forward; }
        else { return transform.up; }
    }

    Vector3 CheckForwardVectorForCorrection()
    {
        if (SealifeForwardDirection == sealifeForwardDirection.ahead) { return Vector3.up; }
        else if (SealifeForwardDirection == sealifeForwardDirection.up) { return -Vector3.up; }
        else { return Vector3.up; }
    }

    private void OnDrawGizmosSelected()
    {
        var raycastDirection = CheckForwardVectorForRaycast();
        var raycastUpDirection = CheckUpVectorForRaycast();

        float horizontal = avoidanceDistance * horizMultiplier;
        float vertical = avoidanceDistance * vertMultiplier;

        Debug.DrawRay(rayOrigin.transform.position, raycastDirection * avoidanceDistance, Color.red, .1f, true);
        Debug.DrawRay(rayOrigin.transform.position, (raycastDirection * horizontal + transform.right * horizontal), Color.red, .1f, true);
        Debug.DrawRay(rayOrigin.transform.position, (raycastDirection * horizontal - transform.right * horizontal), Color.red, .1f, true);
        Debug.DrawRay(rayOrigin.transform.position, (raycastDirection * vertical - raycastUpDirection * vertical), Color.red, .1f, true);
        Debug.DrawRay(rayOrigin.transform.position, (raycastDirection * vertical + raycastUpDirection * vertical), Color.red, .1f, true);

        Gizmos.DrawWireSphere((rayOrigin.transform.position + (raycastDirection * avoidanceDistance)), .025f);
        Gizmos.DrawWireSphere((rayOrigin.transform.position + (raycastDirection * horizontal) - (transform.right * horizontal)), .025f);
        Gizmos.DrawWireSphere((rayOrigin.transform.position + (raycastDirection * horizontal) + (transform.right * horizontal)), .025f);
        Gizmos.DrawWireSphere((rayOrigin.transform.position + (raycastDirection * vertical) - (raycastUpDirection * vertical)), .0125f);
        Gizmos.DrawWireSphere((rayOrigin.transform.position + (raycastDirection * vertical) + (raycastUpDirection * vertical)), .0125f);
    }

}
