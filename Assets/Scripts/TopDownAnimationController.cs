using UnityEngine;

public class TopDownAnimationController : MonoBehaviour
{
    public GameObject spriteBody;
    private Animator animator;
    private TopDownController controller;

    private Vector3 lastPosition;

    void Start()
    {
        animator = spriteBody.GetComponent<Animator>();
        controller = GetComponent<TopDownController>();
        lastPosition = transform.position;
    }

    void Update()
    {
        if (controller == null || animator == null) return;

        bool isWalking = (transform.position != lastPosition);
        animator.SetBool("isWalking", isWalking);

        lastPosition = transform.position;
    }
}
