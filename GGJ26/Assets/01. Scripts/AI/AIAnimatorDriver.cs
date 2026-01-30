using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
public class AIAnimatorDriver : MonoBehaviour
{
    [SerializeField] private string speedParam = "Speed";
    [SerializeField] private string motionSpeedParam = "MotionSpeed";

    private Animator animator;
    private CharacterController characterController;
    private NavMeshAgent navMeshAgent;
    private int speedHash;
    private int motionSpeedHash;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        speedHash = Animator.StringToHash(speedParam);
        motionSpeedHash = Animator.StringToHash(motionSpeedParam);
    }

    private void Update()
    {
        Vector3 velocity = Vector3.zero;

        if (navMeshAgent != null)
        {
            velocity = navMeshAgent.velocity;
        }
        else if (characterController != null)
        {
            velocity = characterController.velocity;
        }

        float speed = new Vector3(velocity.x, 0f, velocity.z).magnitude;
        animator.SetFloat(speedHash, speed);
        animator.SetFloat(motionSpeedHash, speed > 0.01f ? 1f : 0f);
    }
}
