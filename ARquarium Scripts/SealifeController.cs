using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SealifeController : MonoBehaviour
{
    public enum sealifeForwardDirection
    {
        up,
        ahead,
    }

    [Header("Sealife Movement")]
    public sealifeForwardDirection SealifeForwardDirection;
    public Rigidbody sealifeRB;
    public Transform rayOrigin;
    public float Speed = 3f;
    public float minimumSpeed = .01f;
    public float avoidanceDistance;

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

    float horizMultiplier = .8f;
    float vertMultiplier = .4f;

    void Start()
    {
        if (intermediate != null) { intermRB = intermediate.GetComponent<Rigidbody>(); }
        switch (SealifeForwardDirection)
        {
            case sealifeForwardDirection.up:
                forwardVector = Vector3.up;
                rotationAxis = -transform.forward;
                pitchAxis =  transform.right;
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
    }

    void FixedUpdate()
    {
        if (rayOrigin != null)
        {
            int layerMask = 1 << 12;
            layerMask = ~layerMask;

            //These vector checks are to create appropriate movement depending on the type of sealife
            //For example, sealife that moves like salmon sealife or sealife that moves like octopi
            var raycastDirection = CheckForwardVectorForRaycast();
            var raycastUpDirection = CheckUpVectorForRaycast();

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
                        sealifeRB.AddRelativeForce(forwardVector * Speed * .5f, ForceMode.Force);
                        sealifeRB.AddTorque(rotationAxis * Speed * .025f, ForceMode.Force);
                    }
                    else { if (!turningAway) { turningAway = true; StartCoroutine(TurnAway("right")); } } //Scramble right
                }
                else if (avoidRight && !avoidLeft) //Turn/scramble left
                {
                    if (sealifeRB.velocity.magnitude > minimumSpeed) //Turn left
                    {
                        sealifeRB.AddRelativeForce(forwardVector * Speed * .5f, ForceMode.Force);
                        sealifeRB.AddTorque(-rotationAxis * Speed * .025f, ForceMode.Force);
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
            { //Move forward if forward raycast doesn't see anything ahead, but turn a little if side raycasts do
                if (sealifeRB.velocity.magnitude < .01f) { sealifeRB.AddTorque(rotationAxis * Speed * Random.Range(-10f, 10f), ForceMode.Impulse); } //Stuck? Scramble
                if (avoidLeft && !avoidRight) //Turn right a little
                {
                    sealifeRB.AddRelativeForce(forwardVector * Speed, ForceMode.Force);
                    sealifeRB.AddTorque(rotationAxis * Speed * .025f, ForceMode.Acceleration);
                }
                else if (avoidRight && !avoidLeft) //Turn left a little
                {
                    sealifeRB.AddRelativeForce(forwardVector * Speed, ForceMode.Force);
                    sealifeRB.AddTorque(-rotationAxis * Speed * .025f, ForceMode.Acceleration);
                }
                else if (!avoidLeft && !avoidRight) //Full speed ahead
                {
                    sealifeRB.AddRelativeForce(forwardVector * Speed, ForceMode.Force);
                }
                else if (avoidLeft && avoidRight) //Still full speed ahead
                {
                    sealifeRB.AddRelativeForce(forwardVector * Speed, ForceMode.Force);
                }
            }

            //Nudge the sealife a little up or down if it gets stuck vertically
            if (Physics.SphereCast(rayOrigin.transform.position, .05f, (raycastDirection + raycastUpDirection), out topHitData, avoidanceDistance * vertMultiplier, layerMask))
            {
                sealifeRB.AddTorque(pitchAxis * Speed * .125f, ForceMode.Force);
            }
            if (Physics.SphereCast(rayOrigin.transform.position, .05f, (raycastDirection - raycastUpDirection), out bottomHitData, avoidanceDistance * vertMultiplier, layerMask))
            {
                sealifeRB.AddTorque(-pitchAxis * Speed * .125f, ForceMode.Force);
            }
        }

        // X- & Z-axis correction to keep sealife upright
        q = Quaternion.FromToRotation(CheckUpVectorForRaycast(), CheckForwardVectorForCorrection()) * transform.rotation; 
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * Speed * .5f);
        
        }

    IEnumerator TurnAround() //Turn around in a random direction
    {
        sealifeRB.AddTorque(rotationAxis * Speed * Random.Range(-3f, 3f), ForceMode.Impulse);
        yield return new WaitForSeconds(2f);
        turningAway = false;
    }

    IEnumerator TurnAway(string direction) //Turn away with more force in a specific direction
    {
        if (direction == "right") { sealifeRB.AddTorque(rotationAxis * Speed * Random.Range(4f, 6f), ForceMode.Impulse); }
        else { sealifeRB.AddTorque(-rotationAxis * Speed * Random.Range(4f, 6f), ForceMode.Impulse); }
        yield return new WaitForSeconds(2f);
        turningAway = false;
    }

    IEnumerator PanicTurn() //Turn around forcefully in a random direction
    {
        sealifeRB.AddTorque(rotationAxis * Speed * Random.Range(-8f, 8f), ForceMode.Impulse);
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

    private void OnDrawGizmosSelected() //For visual debugging purposes
    {
        var raycastDirection = CheckForwardVectorForRaycast();
        var raycastUpDirection = CheckUpVectorForRaycast();

        float horizontal = avoidanceDistance * horizMultiplier;
        float vertical = avoidanceDistance * vertMultiplier;

        Debug.DrawRay(rayOrigin.transform.position, raycastDirection  * avoidanceDistance, Color.red, .1f, true);
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
