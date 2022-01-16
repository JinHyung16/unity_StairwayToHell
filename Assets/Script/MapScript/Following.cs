using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    ChaseNPC 제어

    생성 후
     - START_TIME 지난 후 추격 시작
     - MoveDelay만큼 시간이 지나면 이동

    MoveDelay 조작: 미완성
*/

public class Following : MonoBehaviour
{
    private Animator animator;
    private CreateStairs stairsc;
    private TestPlayerScript playersc;

    const float startTime = 3; // 이동하기 전에 대기 시간
    const float moveDelayMax = 1; // 최대 이동 간격 시간
    const float moveDealyMin = 0.4f; //최소 이동 간격 시간
    const float moveDealyDiff = 0.15f; //이동 간경 시간이 줄어드는 정도
    const int moveDelayCond = 10; //ChaseNPC와 player의 이동 계단이 moveDelayCond 만큼 차이 -> moveDealyDiff 뺌

    private float MoveDelay;
    private float Starttime = 0;
    private int step = 0;
    private float Timer = 0;
    private bool firstTrans = true;

    //startGame 때 생성
    private void Awake()
    {
        playersc = GameObject.Find("TestPlayer").GetComponent<TestPlayerScript>();
        stairsc = GameObject.Find("GameManager").GetComponent<CreateStairs>();
        animator = GetComponent<Animator>();
    }

    private void LateUpdate()
    {
        ChasePlayer();
    }

    private void ChasePlayer()
    {
        if (Starttime >= startTime && playersc.alive)//Start Game 후 START_TIME만큼 시간 지나면 추격시작
        {
            if (firstTrans)//첫 추격시작시 위치
            {
                animator.ResetTrigger("Move");
                transform.position = new Vector3(0, 0, 0);
                firstTrans = false;
                MoveDelay = moveDelayMax;
            }
            if (Timer >= MoveDelay)//이동
            {
                animator.SetTrigger("Move");
                Timer = 0;
                step += 1;
                //ChaseNPC 바라보는 방향 조절
                if (stairsc.stairPlace[step % 25].transform.position.x - transform.position.x > 0) //오른쪽 이동
                    GetComponent<SpriteRenderer>().flipX = false;
                else //왼쪽 이동
                    GetComponent<SpriteRenderer>().flipX = true;
                transform.Translate(stairsc.stairPlace[step % 25].transform.position.x - transform.position.x, -0.5f, 0); //이동
            }
            if (playersc.StairsPassed == step)//플레이어를 잡았을 때 죽음
                playersc.Death();

            //MoveDelay 조정
            MoveDelay = moveDelayMax - ((playersc.StairsPassed - step) / moveDelayCond) * moveDealyDiff;
            if (MoveDelay < moveDealyMin)
                MoveDelay = moveDealyMin;

            Timer += Time.deltaTime; //Timer 증가
        }
        else
        {
            Starttime += Time.deltaTime;
        }
    }
}
