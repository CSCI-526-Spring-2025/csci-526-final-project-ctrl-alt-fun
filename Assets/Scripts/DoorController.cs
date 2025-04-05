using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour
{
    // Option 1: Use an Animator to play "Open" and "Close" animations.
    public Animator doorAnimator;
    
    // Rotation settings: maximum rotation angle (in degrees) and the speed (degrees per second).
    public float openAngle = 90f;
    public float openSpeed = 90f;  // degrees per second
    
    // The door's width. Set this in the Inspector to match your door's width.
    public float doorWidth = 2f;
    
    // Internal flag to track the door state.
    private bool isOpen = false;
    
    // The hinge pivot in world space.
    private Vector3 hingePivot;
    
    // Track the current rotation angle around the hinge (0 means closed, openAngle means fully open).
    private float currentAngle = 0f;

    void Start()
    {
        // Calculate the hinge pivot assuming the door's pivot is at its center.
        // For a left-hinged door, the hinge is to the left of the center.
        hingePivot = transform.position - transform.right * (doorWidth / 2f);
        currentAngle = 0f;
    }

    public void OpenDoor()
    {
        if (isOpen) return;
        isOpen = true;
        Debug.Log("Door opening...");
        
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("Open");
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(OpenDoorCoroutine());
        }
    }

    private IEnumerator OpenDoorCoroutine()
    {
        // Rotate until we have reached the desired open angle.
        while (currentAngle < openAngle)
        {
            // Calculate the incremental angle for this frame.
            float angleDelta = openSpeed * Time.deltaTime;
            // Clamp so we don't exceed the target openAngle.
            if (currentAngle + angleDelta > openAngle)
            {
                angleDelta = openAngle - currentAngle;
            }
            // Rotate around the hinge pivot along the z-axis (assuming z-axis points out of screen).
            transform.RotateAround(hingePivot, Vector3.forward, angleDelta);
            currentAngle += angleDelta;
            yield return null;
        }
    }

    public void CloseDoor()
    {
        if (!isOpen) return;
        isOpen = false;
        Debug.Log("Door closing...");
        
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("Close");
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(CloseDoorCoroutine());
        }
    }

    private IEnumerator CloseDoorCoroutine()
    {
        // Rotate until we return to a 0° rotation relative to the hinge.
        while (currentAngle > 0f)
        {
            float angleDelta = openSpeed * Time.deltaTime;
            if (currentAngle - angleDelta < 0f)
            {
                angleDelta = currentAngle;
            }
            // Rotate in the opposite direction (negative angleDelta).
            transform.RotateAround(hingePivot, Vector3.forward, -angleDelta);
            currentAngle -= angleDelta;
            yield return null;
        }
    }
}
