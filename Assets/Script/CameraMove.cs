using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private Transform cameraPosition;

    private void Start()
    {
        cameraPosition = GameObject.Find("Player/CameraPosition").transform;
    }

    private void Update()
    {
        transform.position = cameraPosition.position;
    }
}