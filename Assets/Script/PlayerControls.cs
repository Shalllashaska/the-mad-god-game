using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class PlayerControls : MonoBehaviour
{
#region Variables

    [Header("---Movement---")] 
    public bool lockMovement = false;
    public float moveSpeed = 10f;
    public float acceleration = 15f;
    public float moveMultiplier = 10f;
    public float airMultiplier = 0.1f;
    public float jumpForce = 5f;
    public Transform orientation;
    //public AudioSource footsteps;
    public float timeBetweenFootsteps;
    
    [Header("---Walk---")]
    public float walkSpeed = 7f;

    [Header("---Control Drag---")]
    public float groundDrag = 6f;
    public float airDrag = 2f;
    
    [Header("---Ground---")]
    public LayerMask groundedMask;
    public Transform groundCheck;

    [Header("---Keybinds---")]
    [SerializeField]
    private KeyCode jumpKey = KeyCode.Space;

    private float _playerHeight = 1f;
    private float _playerCrouchHeight = 0.5f;
    private float _G = 9.8f;
    [HideInInspector]
    public float _horizontalMovement;
    [HideInInspector]
    public float _verticalMovement;
    private Vector3 _moveDirection;
    private Vector3 _slopeMoveDirection;
    private Rigidbody _rb;
    
    private bool _grounded;
    
    private float _groundDist = 0.4f;
    private RaycastHit _slopeHit;
    private float _currentTimeFootsteps;
    private Camera _camera;
    
    
#endregion

#region SystemFunctions

private void Start()
{
    _rb = GetComponent<Rigidbody>();
    _rb.freezeRotation = true;
}

private void Update()
{
    MyInput();
    ControlSpeed();
}

private void FixedUpdate()
{
    Movement();
    ControlDrag();
}

#endregion

#region MyPrivateMethods

    private void ControlDrag()
    {
        if (_grounded)
        {
            _rb.drag = groundDrag;
        }
        else if (!_grounded)
        {
            _rb.drag = airDrag;
        }
    }

    private void ControlSpeed()
    {
        moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration * Time.deltaTime);
    }

    private void MyInput()
    {
        _grounded =
            Physics
                .CheckSphere(groundCheck.position, _groundDist, groundedMask);

        if (!lockMovement)
        {
            _horizontalMovement = Input.GetAxisRaw("Horizontal");
            _verticalMovement = Input.GetAxisRaw("Vertical");
            _moveDirection =
                orientation.forward * _verticalMovement +
                orientation.right * _horizontalMovement;

            if (_verticalMovement != 0 || _horizontalMovement != 0)
            {
                if (_grounded && _currentTimeFootsteps <= 0)
                {

                    //footsteps.pitch = Random.Range(0.9f, 1.1f);
                    //footsteps.panStereo = -footsteps.panStereo;
                    //footsteps.Play();
                    _currentTimeFootsteps = timeBetweenFootsteps / moveSpeed;
                }
            }
        }
        else
        {
            _moveDirection = transform.right * _horizontalMovement;
            _verticalMovement = 0;
        }

        _currentTimeFootsteps -= Time.deltaTime;


        _slopeMoveDirection =
            Vector3.ProjectOnPlane(_moveDirection, _slopeHit.normal);
        
        if (Input.GetKeyDown(jumpKey) && _grounded)
        {
            Jump();
        }
    }

    private void Movement()
    {
        _rb.AddForce(-transform.up.normalized * _G, ForceMode.Acceleration);
        if (_grounded && !OnSlope())
        {
            _rb
                .AddForce(_moveDirection.normalized *
                moveSpeed *
                moveMultiplier,
                ForceMode.Acceleration);
        }
        else if (_grounded && OnSlope())
        {
            _rb
                .AddForce(_slopeMoveDirection.normalized *
                moveSpeed *
                moveMultiplier,
                ForceMode.Acceleration);
        }
        else if (!_grounded)
        {
            _rb
                .AddForce(_moveDirection.normalized *
                moveSpeed *
                moveMultiplier *
                airMultiplier,
                ForceMode.Acceleration);
        }
    }

    private void Jump()
    {
        _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
        _rb.AddForce(transform.up.normalized * jumpForce, ForceMode.Impulse);
    }

    private bool OnSlope()
    {
        if (
            Physics
                .Raycast(groundCheck.position,
                Vector3.down,
                out _slopeHit,
                0.5f, groundedMask)
        )
        {
            if (_slopeHit.normal != Vector3.up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    #endregion
}