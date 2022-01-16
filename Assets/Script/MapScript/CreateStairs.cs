using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 계단 생성
*/

public class CreateStairs : MonoBehaviour
{

    public GameObject stairPrefab;
    private GameObject m_camera;
    private SetUp setUpsc;
    private TrapGenerater trapGensc;

    /*
    stairPlace:
    화면에 있는 stair gameObject의 배열
    stair의 위치를 알기 위해서는 stairPlace[StairsPassed%25] 사용

    조작:
    CreateStairs.MainMenu -> 계단[0]을 (0,0,1)에 생성 : MainMenu 초기화
    CreateStairs -> 카메라 위치에 따라 계단 제거 + 생성
    Succubus -> 서큐버스 효과 적용
    */
    public GameObject[] stairPlace= new GameObject[25];

    /*
    pos:
    마지막으로 만든 계단의 위치

    조작:
    CreateStairs.MainMenu -> (0,0,1) : 초기화
    CreateStairs -> 카메라 위치에 따라 계단 제거 + 생성하면서 바꿔줌
    Succubus -> 서큐버스 효과 적용 후 마지막 위치 저장
    */ 
    public Vector3 pos;

    public int stairIndex; // trap generater에서 사용
    private Quaternion rot = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f); //stair start rotation, doesn't change
    private float deltaY = 0.5f;
    private float deltaX = 1.0f;
    private float createHeight; //creating condition

    private void Awake()
    {
        setUpsc = GameObject.Find("GameManager").GetComponent<SetUp>();
        m_camera = GameObject.Find("Main Camera");
        trapGensc = GameObject.Find("GameManager").GetComponent<TrapGenerater>();
    }

    private void LateUpdate()
    {
        if(setUpsc.flow == 1 || setUpsc.flow == 3)
        {
            if (m_camera.transform.position.y <= createHeight + 5 && !setUpsc.succubus)
                {
                    createHeight -= 0.5f;
                    while (pos.y >= createHeight)
                        CreateStair();
                }
        }
        
    }

    /*
     MainMenu:
        pos,stairIndex,createHeight 초기화
        첫 stair 생성

     사용: 
        SetUp.MainMenu
        SetUp.GameOver
    */
    public void Menu()
    {
        pos = new Vector3(0.0f, 0.0f, 1.0f);
        if (stairPlace[0] != null)
            Destroy(stairPlace[0]);
        stairPlace[0]= Instantiate(stairPrefab, pos, rot);
        stairIndex = 1;
        createHeight = -8.0f;
    }

    /*
     StartGame:
         게임 첫 화면에 필요한 계단 생성
        
     사용: 
        SetUp.StartGame
    */
    public void StartGame() //Create Stairs Beginning of Game
    {
        while (pos.y >= createHeight)
            CreateStair();
    }

    private void CreateStair()
    {
        pos.y -= deltaY;
        if (Random.Range(0, 2) == 1)
        {
            pos.x += deltaX;
        }
        else
        {
            pos.x -= deltaX;
        }
        if (stairPlace[stairIndex] != null)
            Destroy(stairPlace[stairIndex]);
        stairPlace[stairIndex] = Instantiate(stairPrefab, pos, rot);
        trapGensc.CreateTrap();
        stairIndex = (stairIndex + 1) % 25;
    }
}