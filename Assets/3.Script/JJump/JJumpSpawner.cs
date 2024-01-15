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
            float random_YAxis = Random.Range(-2f, 2f); // y축 값 랜덤, 0이 기준
            int random_Prefab = Random.Range(0, island_Prefabs.Length); // Prefab들 중 하나 랜덤뽑기
            GameObject island = Instantiate(island_Prefabs[random_Prefab], Vector3.zero, Quaternion.identity);
            island.gameObject.name = $"island_{idx + 1}Line";
            island.gameObject.GetComponent<IslandController>().lineNumber = idx;
            island.gameObject.transform.SetParent(island_Parent.gameObject.transform);
        }

        // 해당 줄에 맞는 섬이 다 생성되면 섬 갯수 *2배, usedLineNumber true로 바꿈
        islandCount *= 2;
        manager.usedLineNumber[idx] = true;
    }

    private void InitiateSetting()
    {
        islandCount = Random.Range(2, 4);
    }


}
