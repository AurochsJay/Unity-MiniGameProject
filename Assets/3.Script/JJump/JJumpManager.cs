using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JJumpManager : MonoBehaviour
{
    // 해당 번호의 라인을 사용했는지. 라인마다 섬들을 한번만 생성
    public bool[] usedLineNumber = new bool[5];  

    private void Start()
    {
        InitiateSetting();
    }

    private void InitiateSetting()
    {
        for(int i =0; i<usedLineNumber.Length; i++)
        {
            usedLineNumber[i] = false;
        }
    }

}
