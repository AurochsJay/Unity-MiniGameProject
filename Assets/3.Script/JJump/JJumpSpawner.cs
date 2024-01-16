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
    public float[] distanceBtwIslands = { 14.4f, 25.2f, 36f, 46.8f, 57.6f}; // 섬들 간의 거리
    private int islandCount; // Line별로 섬 갯수

    [Header("Coin")]
    [SerializeField] private GameObject coin_Prefab;

    private void Start()
    {
        InitiateSetting(); // 초기설정
        GenerateIslands(0); // 첫 라인 생성
    }

    // 실행될 때, 플레이어가 섬을 밟았을 때 섬 생성
    // 섬의 line index는 섬이 생성될 때 부여하고, 섬이 가지고 있는다.
    public void GenerateIslands(int idx)
    {
        if (idx >= 5) return; // 6번째 줄부터 return함

        if (manager.usedLineNumber[idx]) return; // 해당줄을 사용했다면 return함

        // 부모오브젝트 깔끔하게 보려고
        GameObject island_Parent = Instantiate(emptyObject, Vector3.zero, Quaternion.identity);
        island_Parent.gameObject.name = $"island_Parent : {idx + 1}Line";

        for (int i =0; i < islandCount; i++)
        {
            Vector3 spawnPos = CalculatePosition(idx);
            
            int random_Prefab = Random.Range(0, island_Prefabs.Length); // Prefab들 중 하나 랜덤뽑기
            GameObject island = Instantiate(island_Prefabs[random_Prefab], spawnPos, Quaternion.identity);
            island.gameObject.name = $"island_{idx + 1}Line";
            island.gameObject.GetComponent<IslandController>().lineNumber = idx;
            island.gameObject.transform.SetParent(island_Parent.gameObject.transform);

            GenerateCoins(spawnPos);

        }

        // 해당 줄에 맞는 섬이 다 생성되면 섬 갯수 *2배, usedLineNumber true로 바꿈
        islandCount *= 2;
        if (idx == 3) islandCount /= 2;
        manager.usedLineNumber[idx] = true;
    }

    private Vector3 CalculatePosition(int idx)
    {
        float random_YAxis = Random.Range(-1f, 1f); // y축 값 랜덤, 0이 기준
        float x = Random.Range(-distanceBtwIslands[idx], distanceBtwIslands[idx]);
        float z = Mathf.Sqrt(Mathf.Pow(distanceBtwIslands[idx],2) - Mathf.Pow(x,2));
        int signNumber = Random.Range(0, 2);
        if (signNumber == 0) z = -z;

        Vector3 spawnPos = new Vector3(x, random_YAxis, z);

        // 해당 위치에 섬이 있는지 없는지 체크
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
