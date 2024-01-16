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

    // ������� �������� ������� ���� alpha��
    private float[] alphaValue = { 0.8f, 0.6f, 0.4f, 0.2f, 0f};

    // �ڱ� ���� ��ȣ
    public int lineNumber;

    // �ѹ� �ε����� �� �ķ� return��Ų��.
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

    // 5�ʰ���� ���� �����, 0.5�ʸ��� ������� �������� ���ƿԴٰ� �ð��� ǥ��
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
