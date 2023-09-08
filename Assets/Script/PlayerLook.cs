using UnityEngine;
using UnityEngine.UI;

public class PlayerLook : MonoBehaviour
{
    #region Variables

    public float aimingMult = 0.7f;
    [Header("---Camera Settings---")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Transform orientation;
    
    [Header("---Mouse Settings---")]
    public float sensX = 100f;
    public float sensY = 100f;

    public bool pause = false;
    
    private float mouseY;
    private float mouseX;
    
    private float mult = 0.01f;
    
    private float xRotation;
    private float yRotation;
    private Camera _camera;
    private float _startFov;

    #endregion

    #region SystemMethods

    private void Start()
    {
        yRotation = transform.rotation.y;
        if (playerCamera == null)
        {
            playerCamera = GameObject.Find("Camera").transform;
        }
        _camera = GameObject.Find("Camera/MainCamera").GetComponent<Camera>();
        _startFov = _camera.fieldOfView;
        ToggleCursorMode();
    }

    private void Update()
    {
        if (pause) return;
        
        MyInput();
        
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation,  yRotation, 0);
        orientation.transform.rotation = Quaternion.Euler(0, yRotation,0);
    }

    #endregion

    #region MyPrivateMethods

    private void MyInput()
    {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");
       
        yRotation += mouseX * sensX * mult;
        xRotation -= mouseY * sensY * mult;

        xRotation = Mathf.Clamp(xRotation, -80, 85);

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
        _camera.fieldOfView = _startFov * aimingMult;
    }

    private void DisAiming()
    {
        _camera.fieldOfView = _startFov;
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
    #endregion
    
}