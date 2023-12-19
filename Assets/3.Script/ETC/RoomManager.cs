using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    // 플레이 가능한 아케이드 머신 색상 하이라이트
    [SerializeField] private Material color;
    private Color base_Color;
    private Color change_Color;
    private float timer = 0f;
    private Vector3 base_Color_Vector;
    private Vector3 change_Color_Vector;
    private float intensity;

    private void Start()
    {
        base_Color = color.GetColor("_EmissionColor");
        base_Color_Vector = new Vector3(base_Color.r, base_Color.g, base_Color.b);
        intensity = (base_Color.r + base_Color.g + base_Color.b) / 3f;
        intensity = 1f / intensity;
    }

    private void Update()
    {
        ChangeColor();
    }

    private void ChangeColor()
    {
        if (timer >= 3.0f)
        {
            int[] rand_Color = new int[3];

            for (int i = 0; i < rand_Color.Length; i++)
            {
                rand_Color[i] = Random.Range(0, 256);
            }

            change_Color_Vector = new Vector3(rand_Color[0], rand_Color[1], rand_Color[2]);

            timer = 0f;
        }

        if(change_Color != null)
        {
            base_Color_Vector = Vector3.Lerp(base_Color_Vector, change_Color_Vector, Time.deltaTime);

            change_Color.r = base_Color_Vector.x;
            change_Color.g = base_Color_Vector.y;
            change_Color.b = base_Color_Vector.z;

            intensity = (change_Color.r + change_Color.g + change_Color.b) / 3f;
            intensity = (1f / intensity) * 0.5f ;

            color.SetColor("_EmissionColor", change_Color * intensity);
        }
        
        timer += Time.deltaTime;

    }

}
