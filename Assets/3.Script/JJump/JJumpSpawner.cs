using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JJumpSpawner : MonoBehaviour
{
    [SerializeField] private JJumpManager manager;
    //[SerializeField] private IslandController islandController;
    
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
            Vector3 spawnPos = CalculatePosition(idx);
            
            int random_Prefab = Random.Range(0, island_Prefabs.Length); // Prefab�� �� �ϳ� �����̱�
            GameObject island = Instantiate(island_Prefabs[random_Prefab], spawnPos, Quaternion.identity);
            island.gameObject.name = $"island_{idx + 1}Line";
            island.gameObject.GetComponent<IslandController>().lineNumber = idx;
            island.gameObject.transform.SetParent(island_Parent.gameObject.transform);

            GenerateCoins(spawnPos);

        }

        // �ش� �ٿ� �´� ���� �� �����Ǹ� �� ���� *2��, usedLineNumber true�� �ٲ�
        islandCount *= 2;
        if (idx == 3) islandCount /= 2;
        manager.usedLineNumber[idx] = true;
    }

    private Vector3 CalculatePosition(int idx)
    {
        float random_YAxis = Random.Range(-1f, 1f); // y�� �� ����, 0�� ����
        float x = Random.Range(-distanceBtwIslands[idx], distanceBtwIslands[idx]);
        float z = Mathf.Sqrt(Mathf.Pow(distanceBtwIslands[idx],2) - Mathf.Pow(x,2));
        int signNumber = Random.Range(0, 2);
        if (signNumber == 0) z = -z;

        Vector3 spawnPos = new Vector3(x, random_YAxis, z);

        // �ش� ��ġ�� ���� �ִ��� ������ üũ
        bool isEmptySpace = CheckPosition(spawnPos);

        while (!isEmptySpace)
        {
            random_YAxis = Random.Range(-1f, 1f);
            x = Random.Range(-distanceBtwIslands[idx], distanceBtwIslands[idx]);
            z = Mathf.Sqrt(Mathf.Pow(distanceBtwIslands[idx], 2) - Mathf.Pow(x, 2));
            signNumber = Random.Range(0, 2);
            if (signNumber == 0) z = -z;
            spawnPos = new Vector3(x, random_YAxis, z);
            isEmptySpace = CheckPosition(spawnPos);
        }

        return spawnPos;
    }

    private bool CheckPosition(Vector3 spawnPos)
    {
        bool isEmptySpace = true;
        float straightOffset = 3.5f;

        Collider[] colliders = Physics.OverlapSphere(spawnPos, straightOffset);
        
        if(colliders.Length >= 1)
        {
            isEmptySpace = false;
        }
        else
        {
            isEmptySpace = true;
        }

        return isEmptySpace;
    }
    
    private void GenerateCoins(Vector3 spawnPos)
    {
        GameObject coin = Instantiate(coin_Prefab, spawnPos + Vector3.up*2, Quaternion.identity);
    }

    private void InitiateSetting()
    {
        islandCount = 3;
    }


}
