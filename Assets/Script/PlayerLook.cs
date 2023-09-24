using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLook : MonoBehaviour
{
    #region Variables

    public float aimingMult = 0.7f;
    [Header("---Camera Settings---")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Transform orientation;
    [SerializeField] private float aimFov = 45;
    
    [Header("---Mouse Settings---")]
    public float sensX = 100f;
    public float sensY = 100f;
    
    [Header("--Interact--")]
    public LayerMask canTakeLayers;
    public float distanceToInteract = 1.5f;
    public GameObject interactIcon;

    [Header("--Rifle--")] 
    public Transform rifleTransform;
    public Transform hipPositionTransform;
    public Transform aimingPositionTransform;
    public float speedOfAiming = 0.5f;
    
    public bool pause = false;
    
    private float _mouseY;
    private float _mouseX;
    
    private float _mult = 0.01f;
    private float _startFov;
    
    private float _xRotation;
    private float _yRotation;
    private Camera _camera;

    #endregion

    #region SystemMethods

    private void Start()
    {
        interactIcon.SetActive(false);
        _yRotation = transform.rotation.y;
        if (playerCamera == null)
        {
            playerCamera = GameObject.Find("Camera").transform;
        }
        rifleTransform.position = hipPositionTransform.position;
        rifleTransform.rotation = hipPositionTransform.rotation;
        _camera = GameObject.Find("Camera/MainCamera").GetComponent<Camera>();
        _startFov = _camera.fieldOfView;
        ToggleCursorMode();
    }

    private void Update()
    {
        if (pause) return;
        
        MyInput();
        
        playerCamera.transform.localRotation = Quaternion.Euler(_xRotation,  _yRotation, 0);
        orientation.transform.rotation = Quaternion.Euler(0, _yRotation,0);
        
    }

    private void FixedUpdate()
    {
        CanInteract();
    }

    #endregion

    #region MyPrivateMethods

    private void MyInput()
    {
        _mouseX = Input.GetAxisRaw("Mouse X");
        _mouseY = Input.GetAxisRaw("Mouse Y");
       
        _yRotation += _mouseX * sensX * _mult;
        _xRotation -= _mouseY * sensY * _mult;

        _xRotation = Mathf.Clamp(_xRotation, -80, 85);

        if (Input.GetMouseButton(1))
        {
            Aiming();
        }
        else
        {
            DisAiming();
        }
    }

    private void Aiming()
    {
        rifleTransform.position = Vector3.Lerp(
            rifleTransform.position, 
            aimingPositionTransform.position,
            speedOfAiming * Time.deltaTime
            );
        rifleTransform.rotation = Quaternion.Lerp(
            rifleTransform.rotation,
            aimingPositionTransform.rotation,
            speedOfAiming * Time.deltaTime
        );
        _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, aimFov, speedOfAiming * Time.deltaTime);
    }

    private void DisAiming()
    {
        rifleTransform.position = Vector3.Lerp(
            rifleTransform.position,
            hipPositionTransform.position, 
            speedOfAiming * Time.deltaTime
        );
        rifleTransform.rotation = Quaternion.Lerp(
            rifleTransform.rotation,
            hipPositionTransform.rotation,
            speedOfAiming * Time.deltaTime
        );
        _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, _startFov, speedOfAiming * Time.deltaTime);
    }

    private void ToggleCursorMode()
    {
        if (Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
        Cursor.visible = !Cursor.visible;
    }

    public void ToggleCursorModeMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public bool CanInteract(bool interactWithObject = false)
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, distanceToInteract, canTakeLayers))
        {
            interactIcon.SetActive(true);
            if (interactWithObject)
            {
                // Interact with object
                Destroy(hit.collider.gameObject);    
            }

            return true;
        }
        interactIcon.SetActive(false);
        return false;
    }
    #endregion
    
}