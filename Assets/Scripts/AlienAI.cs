using UnityEngine;
using System.Collections;

public class AlienAI : MonoBehaviour
{
    [Header("Target References (auto-assigned)")]
    public Transform entranceTrigger;
    public Transform room1Trigger;
    public Transform room2Trigger;
    public Transform room3Trigger;
    public Transform room4Trigger;

    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float stopDistance = 0.3f;
    public float groundOffset = 0.05f; // small offset above ground

    private Transform currentTarget;
    private Transform finalRoomTarget;
    private bool reachedEntrance = false;
    private bool reachedRoom1 = false;
    private bool reachedFinal = false;
    private bool isMoving = true;

    private Animator anim;
    private Rigidbody rb;

    // Cached ground Y used for all movement (newPos.y = groundY)
    private float groundY;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        // Rigidbody setup for pushable, physics-driven movement
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

        // Auto-assign triggers by name if not already assigned in Inspector
        if (entranceTrigger == null) entranceTrigger = GameObject.Find("EntranceTrigger")?.transform;
        if (room1Trigger == null) room1Trigger = GameObject.Find("Room1Trigger")?.transform;
        if (room2Trigger == null) room2Trigger = GameObject.Find("Room2Trigger")?.transform;
        if (room3Trigger == null) room3Trigger = GameObject.Find("Room3Trigger")?.transform;
        if (room4Trigger == null) room4Trigger = GameObject.Find("Room4Trigger")?.transform;

        // Initialize current target
        currentTarget = entranceTrigger;

        // Determine initial groundY by raycasting down from the alien.
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, 5f))
            groundY = hit.point.y + groundOffset;
        else
            groundY = transform.position.y; // fallback

        PlayWalkAnimation();
    }

    void FixedUpdate()
    {
        if (isMoving)
            MoveToTarget();
    }

    void MoveToTarget()
    {
        if (currentTarget == null || rb == null) return;

        // Flat direction ignoring Y differences
        Vector3 targetPos = new Vector3(currentTarget.position.x, transform.position.y, currentTarget.position.z);
        Vector3 direction = (targetPos - transform.position).normalized;

        // Compute new position using Rigidbody so collisions apply
        Vector3 newPos = rb.position + direction * moveSpeed * Time.fixedDeltaTime;

        // IMPORTANT: use cached groundY (user requested)
        newPos.y = groundY;

        // Apply movement
        rb.MovePosition(newPos);

        // Smooth facing
        if (direction.magnitude > 0.01f)
            transform.forward = Vector3.Lerp(transform.forward, direction, 0.15f);

        // Horizontal distance check only
        float distanceXZ = Vector3.Distance(
            new Vector3(transform.position.x, 0f, transform.position.z),
            new Vector3(currentTarget.position.x, 0f, currentTarget.position.z)
        );

        if (distanceXZ <= stopDistance)
            HandleReachedTarget();
    }

    void HandleReachedTarget()
    {
        if (!reachedEntrance)
        {
            reachedEntrance = true;
            currentTarget = room1Trigger;
        }
        else if (!reachedRoom1)
        {
            reachedRoom1 = true;
            Transform[] finals = { room2Trigger, room3Trigger, room4Trigger };
            finalRoomTarget = finals[Random.Range(0, finals.Length)];
            currentTarget = finalRoomTarget;
        }
        else if (!reachedFinal)
        {
            reachedFinal = true;
            isMoving = false;
            PlayStopAnimation();

            // When we arrive at the final room, re-sample actual ground Y once
            // and then freeze Y so the alien stays grounded and pushable.
            StartCoroutine(FinalizeGroundAndFreezeY());

            Debug.Log($"{gameObject.name} reached {currentTarget.name} and is now pushable");
        }
    }

    IEnumerator FinalizeGroundAndFreezeY()
    {
        // wait one physics frame so the transform settles
        yield return new WaitForFixedUpdate();

        if (rb != null)
        {
            // Re-sample ground below the alien (final alignment)
            if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, 5f))
            {
                groundY = hit.point.y + groundOffset;
                Vector3 pos = transform.position;
                pos.y = groundY;
                rb.MovePosition(pos); // snap to final ground level
            }

            // Lock Y so it doesn't float, but keep X/Z movement and block tipping
            rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            rb.useGravity = true;

            // Ensure no residual velocities
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    void PlayWalkAnimation()
    {
        if (anim)
            anim.Play("Alien Walk");
    }

    void PlayStopAnimation()
    {
        if (anim)
            anim.Play("Alien Stop");
    }
}
