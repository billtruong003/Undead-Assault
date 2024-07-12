using System;
using Photon.Pun;
using Photon.Realtime;
using Sirenix.OdinInspector;
using UnityEngine;

public class Movement : MonoBehaviourPunCallbacks, IPunObservable
{
    [TitleGroup("Movement Settings")]
    public float walkSpeed;
    public float sprintSpeed;
    public float maxVelChange;

    [Space]
    [TitleGroup("Jump Settings")]
    public float jumpHeight;
    public float airControl;

    [Space]
    [TitleGroup("Dash Settings")]
    public float dashForce;
    public float dashCooldown;
    private float nextDashTime;

    [Space]
    [FoldoutGroup("Detail Info (Read Only)"), ReadOnly, SerializeField]
    private Vector2 input;
    [FoldoutGroup("Detail Info (Read Only)"), ReadOnly, SerializeField]
    private Rigidbody rb;
    [FoldoutGroup("Detail Info (Read Only)"), ReadOnly, SerializeField]
    private float xInput;
    [FoldoutGroup("Detail Info (Read Only)"), ReadOnly, SerializeField]
    private float yInput;

    [Space]
    [TitleGroup("Player State")]
    [ReadOnly, SerializeField]
    private bool grounded;
    [ReadOnly, SerializeField]
    private bool sprinting;
    [ReadOnly, SerializeField]
    private bool jumping;
    [ReadOnly, SerializeField]
    private float playerSpeed;
    [ReadOnly, SerializeField]
    private bool sitting;
    [ReadOnly, SerializeField]
    private bool dashing;

    [SerializeField] private float camSit = 0.4f;
    [SerializeField] private CapsuleCollider capsuleCol;
    [SerializeField] private Transform camControl;
    private Vector3 originalCam;

    [Space]
    [TitleGroup("Custom Gravity")]
    public float customGravity = -20f;

    [SerializeField] private PhotonView photonView;
    private Vector3 networkPosition;
    private Quaternion networkRotation;

    void Start()
    {
        originalCam = camControl.localPosition;
        rb = GetComponent<Rigidbody>();

        if (!photonView.IsMine)
        {
            rb.isKinematic = true;
        }
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        HandleInput();
        HandleSitting();
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            SmoothMovement();
            return;
        }

        ApplyCustomGravity();
        if (grounded)
        {
            HandleMovementOnGround();
        }
        else
        {
            HandleAirControl();
        }
    }

    private void HandleInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");
        input = new Vector2(xInput, yInput).normalized;
        sprinting = Input.GetButton("Sprint");
        jumping = Input.GetButton("Jump");
        sitting = Input.GetButton("Sit");
        dashing = Input.GetButtonDown("Dash") && Time.time >= nextDashTime;

    }

    private void HandleSitting()
    {
        if (sitting)
        {
            SitDown();
        }
        else
        {
            StandUp();
        }
    }

    private void HandleMovementOnGround()
    {
        if (jumping)
        {
            photonView.RPC("HandleJump", RpcTarget.All);
            grounded = false;
        }
        else if (dashing)
        {
            photonView.RPC("HandleDash", RpcTarget.All);
            nextDashTime = Time.time + dashCooldown;
        }
        else
        {
            HandleWalking();
        }
    }

    private void HandleAirControl()
    {
        if (input.magnitude > 0.5f)
        {
            playerSpeed = sprinting ? sprintSpeed : walkSpeed;
            playerSpeed *= airControl;
            rb.AddForce(CalculateMovement(playerSpeed), ForceMode.VelocityChange);
        }
    }

    private void HandleWalking()
    {
        if (input.magnitude > 0.5f && !sitting)
        {
            playerSpeed = sprinting ? sprintSpeed : walkSpeed;
            rb.AddForce(CalculateMovement(playerSpeed), ForceMode.VelocityChange);
        }
        else if (input.magnitude > 0.5f && sitting)
        {
            playerSpeed = walkSpeed * 0.25f;
            rb.AddForce(CalculateMovement(playerSpeed), ForceMode.VelocityChange);
        }
        else
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }

    [PunRPC]
    private void HandleJump()
    {
        rb.velocity = new Vector3(rb.velocity.x, Mathf.Sqrt(2 * jumpHeight * Physics.gravity.magnitude), rb.velocity.z);
    }

    [PunRPC]
    private void HandleDash()
    {
        Vector3 dashDirection = new Vector3(input.x, 0, input.y).normalized;
        rb.AddForce(dashDirection * dashForce, ForceMode.Impulse);
    }

    private Vector3 CalculateMovement(float speed)
    {
        Vector3 targetVelocity = new Vector3(input.x, 0, input.y);
        targetVelocity = transform.TransformDirection(targetVelocity) * speed;

        Vector3 velocity = rb.velocity;
        if (input.magnitude > 0.5f)
        {
            Vector3 velocityChange = targetVelocity - velocity;
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelChange, maxVelChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelChange, maxVelChange);
            velocityChange.y = 0;
            return velocityChange;
        }
        return Vector3.zero;
    }

    private void ApplyCustomGravity()
    {
        rb.AddForce(Vector3.up * customGravity, ForceMode.Acceleration);
    }

    private void SitDown()
    {
        camControl.localPosition = new Vector3(originalCam.x, camSit, originalCam.z);
        capsuleCol.height = 1.5f;
        capsuleCol.center = new Vector3(0, -0.25f, 0);
    }

    private void StandUp()
    {
        camControl.localPosition = originalCam;
        capsuleCol.height = 2;
        capsuleCol.center = Vector3.zero;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(rb.position);
            stream.SendNext(rb.rotation);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }

    private void SmoothMovement()
    {
        rb.position = Vector3.MoveTowards(rb.position, networkPosition, Time.fixedDeltaTime);
        rb.rotation = Quaternion.RotateTowards(rb.rotation, networkRotation, Time.fixedDeltaTime * 100);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!photonView.IsMine) return;
        grounded = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!photonView.IsMine) return;
        grounded = false;
    }
}
