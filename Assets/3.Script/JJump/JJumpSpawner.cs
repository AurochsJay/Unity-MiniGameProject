using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JJumpSpawner : MonoBehaviour
{
    [SerializeField] private JJumpManager manager;
    [SerializeField] private IslandController islandController;
    
    [Space(10)]
    
    [Header("Island")]
    [SerializeField] private GameObject[] island_Prefabs;
    [SerializeField] private GameObject emptyObject;
    public float[] distanceBtwIslands = { 14.4f, 25.2f, 36f, 46.8f, 57.6f}; // ���� ���� �Ÿ�
    private int islandCount; // Line���� �� ����

    [Header("Coin")]
    [SerializeField] private GameObject coin_Prefab;

    private void Start()
    {
        InitiateSetting(); // �ʱ⼳��
        GenerateIslands(0); // ù ���� ����
    }

    // ����� ��, �÷��̾ ���� ����� �� �� ����
    // ���� line index�� ���� ������ �� �ο��ϰ�, ���� ������ �ִ´�.
    public void GenerateIslands(int idx)
    {
        if (idx >= 5) return; // 6��° �ٺ��� return��

        if (manager.usedLineNumber[idx]) return; // �ش����� ����ߴٸ� return��

        // �θ������Ʈ ����ϰ� ������
        GameObject island_Parent = Instantiate(emptyObject, Vector3.zero, Quaternion.identity);
        island_Parent.gameObject.name = $"island_Parent : {idx + 1}Line";

        for (int i =0; i < islandCount; i++)
        {
            float random_YAxis = Random.Range(-2f, 2f); // y�� �� ����, 0�� ����
            int random_Prefab = Random.Range(0, island_Prefabs.Length); // Prefab�� �� �ϳ� �����̱�
            GameObject island = Instantiate(island_Prefabs[random_Prefab], Vector3.zero, Quaternion.identity);
            island.gameObject.name = $"island_{idx + 1}Line";
            island.gameObject.GetComponent<IslandController>().lineNumber = idx;
            island.gameObject.transform.SetParent(island_Parent.gameObject.transform);
        }

        // �ش� �ٿ� �´� ���� �� �����Ǹ� �� ���� *2��, usedLineNumber true�� �ٲ�
        islandCount *= 2;
        manager.usedLineNumber[idx] = true;
    }

    private void InitiateSetting()
    {
        islandCount = Random.Range(2, 4);
    }


}
