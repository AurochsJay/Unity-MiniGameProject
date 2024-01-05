using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    private float rotate = 0f;
    private float rotateSpeed = 100f;

    private void Update()
    {
        if(rotate > 360)
        {
            rotate = 0f;
        }
        
        rotate += rotateSpeed * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(0, 0, rotate);
    }
}
