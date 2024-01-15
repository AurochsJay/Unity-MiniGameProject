using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandController : MonoBehaviour
{
    [SerializeField] private JJumpManager manager;
    [SerializeField] private JJumpSpawner spawner;

    // 자기 라인 번호
    public int lineNumber;

    private void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            spawner.GenerateIslands(lineNumber+1);
        }
    }

}
