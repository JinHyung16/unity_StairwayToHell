using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Trap 생성
 
     player 이동량 > 마지막 trap 생성 때 player의 위치 + TRAP_INTERVAL
        ->랜덤으로 Succubus 생성, Angel 생성, Drop 생성, trap 생성 제지 중 1개 함

    trap 생성
        -> player 위치에서 (1 ~ TRAP_MAX-1)만큼 앞에 생성
    
    INCREASE_LAST_TRAP : 마지막 trap 생성 때 player의 위치를 증가
        -> 그만큼 trap이 생성이 안됨

    주의사항:
        TRAP_INTERVAL+1 >= TRAP_MAX
            -> 안 지켜질 시 trap이 같은 곳에 2개씩 생성 될 수 있음
*/

public class TrapGenerater : MonoBehaviour
{
    public GameObject SuccubusPrefab;
    public GameObject AngelPrefab;
    public GameObject DropPrefab;
    public GameObject m_camera;
    private CreateStairs stairsc;
    private TestPlayerScript playersc;
    //private SetUp setUpsc;

    const int DROP_MIN = 1;
    const int DROP_MAX = 6; //현재 위치에서 +DROP_MIN ~ DROP_MAX-1 앞에 있는 자리에 drop 생성
    const int TRAP_INTERVAL = 5; //두 trap 간의 최소 간격
    //const int INCREASE_LAST_TRAP = 1; //클 수록 trap 생성 제지의 효과 큼

    /*
     succubusExist:
     서큐버스가 게임 안에 적용되는지
      -> 캐릭터 존재
      -> 효과 작동중
     
     조작:
     TrapGenereter.StartGame -> false : 초기화
     TrapGenerater -> true : 서큐버스 생성
     Succubus -> false : 서큐버스 제거 및 효과 끝
     */
    public bool succubusExist;

    /*
    angelExist:
    천사가 게임 안에 적용되는지
     -> 캐릭터 존재
     -> 효과 작동중(setUp.angel)

    조작:
    TrapGenereter.StartGame -> false : 초기화
    TrapGenerater -> true : 천사 생성
    UIScript -> false : 천사 효과 끝날 때
    */
    public bool angelExist;

    /*
    dropExist:
    Drop가 게임 안에 적용되는지
     -> 캐릭터 존재
     -> 효과 작동중

    조작:
    TrapGenereter.StartGame -> false : 초기화
    TrapGenerater -> true : Drop 생성
    DropObj -> false : DropObj가 끝났을 때
    */
    public bool dropExist;
    public GameObject[] Trap = new GameObject[25];
    
    private int TrapInterval;


    private void Awake()
    {
        playersc = GameObject.Find("Player").GetComponent<TestPlayerScript>();
        stairsc = GameObject.Find("GameManager").GetComponent<CreateStairs>();
    }

    /*
     CreateTrap:
      Trap 생성
     사용: 
        CreateStairs -> 계단 1개 생성될 때 사용
    */
    public void CreateTrap()
    {
        if(TrapInterval >= TRAP_INTERVAL)
        {
            switch (Random.Range(1, 4))
            {
                case 1:
                    if (!succubusExist)
                    {
                        if (Trap[stairsc.stairIndex] != null)
                            Destroy(Trap[stairsc.stairIndex]);
                        Trap[stairsc.stairIndex] = Instantiate(SuccubusPrefab, stairsc.stairPlace[stairsc.stairIndex].transform.position, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
                        succubusExist = true;
                        TrapInterval = 0;
                    }
                    break;
                case 2:
                    if (!angelExist)
                    {
                        if (Trap[stairsc.stairIndex] != null)
                            Destroy(Trap[stairsc.stairIndex]);
                        Trap[stairsc.stairIndex] = Instantiate(AngelPrefab, stairsc.stairPlace[stairsc.stairIndex].transform.position, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
                        angelExist = true;
                        TrapInterval = 0;
                    }
                    break;
                case 3:
                    if (!dropExist)
                    {
                        int place = Random.Range(DROP_MIN, DROP_MAX);
                        if (stairsc.stairPlace[(playersc.StairsPassed + place) % 25] != null)
                        {
                            Instantiate(DropPrefab, stairsc.stairPlace[(playersc.StairsPassed + place) % 25].transform.position, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
                            dropExist = true;
                            TrapInterval = 0;
                        }
                    }
                    break;
                case 4:// Trap 생성 하지 않음
                    break;
            }
            
        }else
        {
            TrapInterval++;
        }

        
    }
    /*
     StartGame:
     게임 시작 전에 변수들 초기화

     사용:
     SetUp.StartGame
     */

    public void StartGame() {
        succubusExist = false;
        angelExist = false;
        dropExist = false;
        TrapInterval = 0;
    }
}
