using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChange : MonoBehaviour
{
    // Material 색상 변경
    [SerializeField] private Material baseColor; // UI Canvas Material에 적용
    [SerializeField] private Material emissionColor; // Object Material에 적용

    //Base , rgba
    private Color origin_Color_Base;
    private Color change_Color_Base;
    //private float timer = 0f;
    private Vector4 origin_Color_Vector_Base;
    private Vector4 change_Color_Vector_Base;

    //Emission , rgb + intensity
    private Color origin_Color_Emission;
    private Color change_Color_Emission;
    private float timer = 0f;
    private Vector3 origin_Color_Vector_Emission;
    private Vector3 change_Color_Vector_Emission;
    private float intensity;

    private void Start()
    {
        SetBaseColor();
        SetEmissionColor();
        
    }

    private void Update()
    {
        ChangeColor(baseColor, emissionColor);
    }

    private void SetBaseColor()
    {
        origin_Color_Base = emissionColor.GetColor("_BaseColor");
        origin_Color_Vector_Base = new Vector4(origin_Color_Base.r, origin_Color_Base.g, origin_Color_Base.b, origin_Color_Base.a);
        intensity = (origin_Color_Base.r + origin_Color_Base.g + origin_Color_Base.b) / 3f;
    }

    private void SetEmissionColor()
    {
        origin_Color_Emission = emissionColor.GetColor("_EmissionColor");
        origin_Color_Vector_Emission = new Vector3(origin_Color_Emission.r, origin_Color_Emission.g, origin_Color_Emission.b);
        intensity = (origin_Color_Emission.r + origin_Color_Emission.g + origin_Color_Emission.b) / 3f;
        intensity = 1f / intensity;
    }

    private void ChangeColor(Material baseColor, Material emissionColor)
    {
        if (timer >= 3.0f)
        {
            int[] rand_Color = new int[3];

            for (int i = 0; i < rand_Color.Length; i++)
            {
                rand_Color[i] = Random.Range(0, 256);
            }

            change_Color_Vector_Emission = new Vector3(rand_Color[0], rand_Color[1], rand_Color[2]);

            timer = 0f;
        }

        if(change_Color_Emission != null)
        {
            origin_Color_Vector_Emission = Vector3.Lerp(origin_Color_Vector_Emission, change_Color_Vector_Emission, Time.deltaTime);

            change_Color_Emission.r = origin_Color_Vector_Emission.x;
            change_Color_Emission.g = origin_Color_Vector_Emission.y;
            change_Color_Emission.b = origin_Color_Vector_Emission.z;

            intensity = (change_Color_Emission.r + change_Color_Emission.g + change_Color_Emission.b) / 3f;
            intensity = (1f / intensity) * 0.5f ;

            emissionColor.SetColor("_EmissionColor", change_Color_Emission * intensity);
        }
        
        timer += Time.deltaTime;

    }

}
