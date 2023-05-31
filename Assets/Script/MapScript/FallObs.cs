using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 낙사 판정

    낙사
     ->TestPlayerScript.Death
*/


public class FallObs : MonoBehaviour 
{
    // Use this for initialization
    private TestPlayerScript playersc;
    private CreateStairs stairsc;
    private SetUp setUpsc;
    private GameObject player;

    private void Awake()
    {
        stairsc = GameObject.Find("GameManager").GetComponent<CreateStairs>();
        playersc = GameObject.Find("Player").GetComponent<TestPlayerScript>();
        setUpsc = GameObject.Find("GameManager").GetComponent<SetUp>();
        player = GameObject.Find("Player");
    }

    void Update () 
    {
        if (setUpsc.flow == 1 && playersc.alive) //게임 중 플레이어가 이동 가능하고 살아 있을 때
        {
            FallDownChar();
        }
    }

    private void FallDownChar()
    {
        //gamemanager.createstairs 의 정보와 player의 x좌표를 비교해 낙사판정
        if ((stairsc.stairPlace[playersc.StairsPassed % 25].transform.position.x) != (player.transform.position.x))
        {
            Debug.Log("낙사");
            playersc.Death();
        }
    }
}
