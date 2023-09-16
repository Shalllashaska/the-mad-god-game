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
    public PlayerLook playerLookScript;
    
    [Header("---Crouch---")]
    public Transform ceilingCheck;
    public float crouchSpeed = 3f;

    [Header("---Control Drag---")]
    public float groundDrag = 6f;
    public float airDrag = 2f;
    
    [Header("---Ground---")]
    public LayerMask groundedMask;
    public Transform groundCheck;

    [Header("---Keybinds---")]
    [SerializeField]
    private KeyCode jumpKey = KeyCode.Space;
    [SerializeField]
    private KeyCode crouchKey = KeyCode.C; 
    [SerializeField]
    private KeyCode interactKey = KeyCode.E;

    [Header("--Weapon--")] 
    public Transform weaponTransform;

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
    
    private bool _ceiling = false;
    private bool _grounded;
    
    private float _groundDist = 0.4f;
    private RaycastHit _slopeHit;
    private float _currentTimeFootsteps;
    private Camera _camera;
    
    private Vector3 _targetWeaponBobPosition;
    private Vector3 _weaponOrigin;
    private float _movementCounter;
    private float _idleCounter;
    
    
#endregion

#region SystemFunctions

private void Start()
{
    _rb = GetComponent<Rigidbody>();
    _rb.freezeRotation = true;
    _weaponOrigin = weaponTransform.localPosition;
}

private void Update()
{
    MyInput();
    ControlSpeed();
    WeaponBobing();
}

private void FixedUpdate()
{
    Movement();
    ControlDrag();
}

#endregion

#region MyPrivateMethods


    private void WeaponBobing()
    {
        if (_horizontalMovement == 0 && _verticalMovement == 0)
        {
            HeadBob(_idleCounter, 0.005f, 0.015f);
            _idleCounter += Time.deltaTime;
            weaponTransform.localPosition = Vector3.Lerp(weaponTransform.localPosition, _targetWeaponBobPosition, Time.deltaTime * 2f);
        }
        else
        {
            HeadBob(_movementCounter, 0.045f, 0.045f);
            _movementCounter += Time.deltaTime * walkSpeed;
            weaponTransform.localPosition =
                Vector3.Lerp(weaponTransform.localPosition, _targetWeaponBobPosition, Time.deltaTime * 10f);
        }
    }
        
    private void HeadBob(float parZ, float parX_intens, float parY_intens)
    {
        _targetWeaponBobPosition = _weaponOrigin + new Vector3(Mathf.Cos(parZ)*parX_intens, Mathf.Sin(parZ * 2) * parY_intens, 0);
    }

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
        if (Input.GetKey(crouchKey) && _grounded)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, crouchSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration * Time.deltaTime);
        }
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
        Crouching();
        Interact();
        
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
    
    private void Crouching()
    {
        RaycastHit hit;
        Ray ray = new Ray(ceilingCheck.position, Vector3.up);
        Debug.DrawRay(ceilingCheck.position, Vector3.up * 2, Color.green);
        _ceiling = Physics.Raycast(ray, out hit, 2f, groundedMask);
        if (Input.GetKey(crouchKey))
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1.3f, _playerCrouchHeight, 1.3f), Time.deltaTime  * acceleration);
            
        }
        else if(!_ceiling && !Input.GetKey(crouchKey))
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1.3f, _playerHeight, 1.3f),Time.deltaTime  * acceleration);
        }
    }

    private void Interact()
    {
        if (Input.GetKeyDown(interactKey))
        {
            playerLookScript.CanInteract(true);
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