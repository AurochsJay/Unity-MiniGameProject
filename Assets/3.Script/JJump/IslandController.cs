using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandController : MonoBehaviour
{
    [SerializeField] private JJumpManager manager;
    [SerializeField] public JJumpSpawner spawner;
    [SerializeField] private Renderer renderer;
    private Material material;

    //Base , rgba
    private Color originColor;
    private Color changeColor;

    // 사라지는 과정에서 흐려지는 정도 alpha값
    private float[] alphaValue = { 0.8f, 0.6f, 0.4f, 0.2f, 0f};

    // 자기 라인 번호
    public int lineNumber;

    // 한번 부딪히면 그 후로 return시킨다.
    public  bool wasHit = false;

    private void Start()
    {
        manager = GameObject.Find("JJumpManager").GetComponent<JJumpManager>();
        spawner = GameObject.Find("Spawner").GetComponent<JJumpSpawner>();
        renderer = GetComponent<Renderer>();
        material = renderer.material;
        originColor = material.GetColor("_BaseColor");
        changeColor = material.GetColor("_BaseColor");
    }

    // 5초경과시 섬이 사라짐, 0.5초마다 흐려졌다 정상으로 돌아왔다가 시각적 표현
    public IEnumerator Disappear()
    {
        WaitForSeconds wfs = new WaitForSeconds(0.5f);

        for(int i = 0; i < alphaValue.Length; i++)
        {
            material.SetColor("_BaseColor", originColor);
            yield return wfs;

            changeColor.a = alphaValue[i];
            material.SetColor("_BaseColor", changeColor);
            yield return wfs;
        }

        Destroy(this.gameObject);
    }
}
