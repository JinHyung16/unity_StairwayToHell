using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
    Succubus 제어
    
     Succubus 위치 == Player 위치
      ->succubus background 띄우기
      ->succubus warning 띄우기
      ->player 밑의 계단 12개 player의 x좌표 기준 y축 대칭이동
      ->Destroy
*/
public class Succubus : MonoBehaviour
{
    private CreateStairs stairsc;
    private GameObject player;
    private TestPlayerScript playersc;
    private UIScript UIsc;
    private Background backgroundsc;
    private TrapGenerater trapGensc;
    private CameraMovement camerasc;
    private SetUp setUpsc;

    const int MOVE_STAIR_NUM = 8; //2의 제곱수로 해주세요 아니면 버그 생깁니다
    const float MOVE_STAIR_TIME = 0.1f;
    const int STAIR_REPLACED = 16; //서큐버스로 인해 바뀌는 계단의 수 +1

    private int ReverseStairidx;
    private float SuccubusX;
    private int movingStair = 0;
    bool Changing = true;
    private bool movingStart = false;
    private float timer = 0;
    private float[] stairLen = new float[STAIR_REPLACED-1];

    private void Awake()
    {
        stairsc = GameObject.Find("GameManager").GetComponent<CreateStairs>();
        player = GameObject.Find("TestPlayer");
        playersc = player.GetComponent<TestPlayerScript>();
        UIsc = GameObject.Find("Canvas").GetComponent<UIScript>();
        backgroundsc = GameObject.Find("GameManager").GetComponent<Background>();
        trapGensc = GameObject.Find("GameManager").GetComponent<TrapGenerater>();
        camerasc = GameObject.Find("Main Camera").GetComponent<CameraMovement>();
        setUpsc = GameObject.Find("GameManager").GetComponent<SetUp>();
    }

    void Update()
    {
        ChasePlayer();
    }

    private void ChasePlayer()
    {
        if (gameObject.transform.position.y == player.transform.position.y && gameObject.transform.position.x == player.transform.position.x && Changing) //player위치 == 서큐버스 위치
        {
            setUpsc.succubus = true;
            UIsc.SuccubusEffect();
            backgroundsc.EffectSuccubus();
            Changing = false;
            ReverseStairidx = playersc.StairsPassed;
            SuccubusX = stairsc.stairPlace[ReverseStairidx % 25].transform.position.x;
            for (int i = 1; i < STAIR_REPLACED; i++)
            {
                stairsc.stairPlace[(ReverseStairidx + i) % 25].transform.GetChild(0).gameObject.SetActive(true);//moving 박쥐 보이기
                if ((SuccubusX - stairsc.stairPlace[(ReverseStairidx + i) % 25].transform.position.x) > 0) //방향에 따라 박쥐가 보는 방향 바꿔줌
                    stairsc.stairPlace[(ReverseStairidx + i) % 25].transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = true;
                else
                    stairsc.stairPlace[(ReverseStairidx + i) % 25].transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = false;
                stairLen[i - 1] = (2 * (SuccubusX - stairsc.stairPlace[(ReverseStairidx + i) % 25].transform.position.x));
                camerasc.CameraShake();
            }
            movingStair = 0;
            timer = 0;
            movingStart = true;
        }

        if (movingStart) //계단 이동 시작
        {
            if (movingStair < MOVE_STAIR_NUM) //이동 중
            {
                if (timer > MOVE_STAIR_TIME)
                {
                    for (int i = 1; i < STAIR_REPLACED; i++)
                    {
                        stairsc.stairPlace[(ReverseStairidx + i) % 25].transform.Translate(stairLen[i - 1] / MOVE_STAIR_NUM, 0, 0);
                        if (trapGensc.Trap[(ReverseStairidx + i) % 25] != null)
                            trapGensc.Trap[(ReverseStairidx + i) % 25].transform.Translate(stairLen[i - 1] / MOVE_STAIR_NUM, 0, 0);
                    }

                    movingStair += 1;
                    timer = 0;
                }
                timer += Time.deltaTime;

            }
            else // 이동 끝
            {
                for (int i = 1; i < STAIR_REPLACED; i++)
                    stairsc.stairPlace[(ReverseStairidx + i) % 25].transform.GetChild(0).gameObject.SetActive(false);
                stairsc.pos.x = stairsc.stairPlace[(ReverseStairidx + STAIR_REPLACED - 1) % 25].transform.position.x;
                trapGensc.succubusExist = false;
                setUpsc.succubus = false;
                movingStart = false;
                Destroy(gameObject);
            }
        }
    }
}
