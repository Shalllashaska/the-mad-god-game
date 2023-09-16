using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sway : MonoBehaviour
{
    public float intensity;
    public float smooth;
    
    private Quaternion _originRotation;
    private void Start()
    {
        _originRotation = transform.localRotation;
    }

    private void Update()
    {
        UpdateSway();
    }

    private void UpdateSway()
    {

        //Controls
        float xMouse = Input.GetAxis("Mouse X");
        float yMouse = Input.GetAxis("Mouse Y");

        //calculate target rotation
        Quaternion t_x_adj = Quaternion.AngleAxis(-intensity * xMouse, Vector3.up);
        Quaternion t_y_adj = Quaternion.AngleAxis(intensity * yMouse, Vector3.right);
        Quaternion targetRotation = _originRotation * t_x_adj * t_y_adj;

        //Rotate towards target rotation
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * smooth);
    }
}
