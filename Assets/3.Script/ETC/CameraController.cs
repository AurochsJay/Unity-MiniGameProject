using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private InputManager input;
    private Vector3 rotation;

    private void Start()
    {
        rotation = transform.rotation.eulerAngles;
    }

    private void Update()
    {
        CameraRotation();
    }

    private void CameraRotation()
    {
        rotation.x -= input.mouse_Rotate_X;
        rotation.y += input.mouse_Rotate_Y;
        transform.rotation = Quaternion.Euler(rotation);
    }
}
