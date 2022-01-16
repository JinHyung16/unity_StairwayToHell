using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
    상황에 따른 GameObject 조절
    다른 script의 function를 이용 -> 세팅
 */
public class SetUp : MonoBehaviour
{
    public GameObject followingPrefab;
    public GameObject player;
    private UIScript UIsc;
    private CreateStairs stairsc;
    private TrapGenerater trapGensc;
    private CameraMovement camerasc;
    private TestPlayerScript playersc;
    private Animator playerAnimsc;
    private AudioManager audiosc;
    private Background backgroundsc;

    /*
     flow:
     화면의 상태
     0 = Main Menu Screen, 1 = Game Screen, 2 = GameOver Screen, 3 = In Game but player temporarily not moving

    조작:
    SetUp.MainMenu -> 0
    SetUp.StartGame -> 1
    SetUp.GameOver -> 2
    UIScript.playerPause -> 3,1 : Warning 보여줄 때 player 움직임 x
    TestPlayerScript -> 3 : 움직이지 않아서 Thunder 때
    DropObj -> 3 : Player가 잡혔을 때
     */
    public int flow; 
    
    /*
     angel:
     천사 효과가 적용 중일 때
     
     조작:
     SetUp -> false : 초기화
     UIScript.angelEffect -> true : 적용 중일 때
     UIScript -> false : 적용 중인 거 끌 때
     */
    public bool angel;

    /*
     succubus:
     서큐버스 효과가 적용 중일 때
     
     조작:
     SetUp -> false : 초기화
     Succubus-> true : 적용 중일 때
     Succubus -> false : 적용 중인 거 끌 때
     */
    public bool succubus;

    private void Awake()
    {
        stairsc = GameObject.Find("GameManager").GetComponent<CreateStairs>();
        UIsc = GameObject.Find("Canvas").GetComponent<UIScript>();
        player = GameObject.Find("Player");
        camerasc = GameObject.Find("Main Camera").GetComponent<CameraMovement>();
        trapGensc = GameObject.Find("GameManager").GetComponent<TrapGenerater>();
        playersc = player.GetComponent<TestPlayerScript>();
        playerAnimsc = player.GetComponent<Animator>();
        audiosc = GameObject.Find("GameManager").GetComponent<AudioManager>();
        backgroundsc = GameObject.Find("GameManager").GetComponent<Background>();

    }

    private void Start()
    {
        this.flow = 0;
        angel = false;
        succubus = false;
        MainMenu();
    }

    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
                return;
            }
        }
    }

    /*
     MainMenu:
        불필요 GameObj(Stair, ChaseNPC, Angel, Succubus) 삭제
        MainMenu로 세팅
        flow = 0
        
     사용:
        UIsc -> Home Event
        UIsc -> backHelp -> Home
    */
    public void MainMenu()
    {
        if(flow != 2) // GameOver의 Home버튼을 통해 오지 않음
        {
            DeleteGameObj();
            playersc.ResSetGame();
            player.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
            camerasc.Menue();
            stairsc.Menu();
            angel = false;
            succubus = false;
        }
        
        this.flow = 0;
        playerAnimsc.SetBool("Alive", true);
        UIsc.MainMenu();
        backgroundsc.Menu();
        audiosc.MainMenu();
    }
    /*
     StartGame:
        StartGame으로 세팅
        flow = 1
        
     사용: 
        UIScript -> startButton
        UIScript -> restart
    */
    public void StartGame()
    {
        audiosc.StartGame();
        trapGensc.StartGame();
        stairsc.StartGame();
        UIsc.StartGame();
        Instantiate(followingPrefab, new Vector3(-12,4,0), Quaternion.identity);
        playersc.StairsPassed = 0;
        playersc.alive = true;
        playerAnimsc.SetBool("Alive", true);
        this.flow = 1;
        
    }
    /*
     GameOver:
        불필요 GameObj(Stair, ChaseNPC, Angel, Succubus) 삭제
        GameOver으로 세팅
        flow = 2
     
     사용:
         TestPlayerScript.Death
    */
    public void GameOver()
    {
        this.flow = 2;
        DeleteGameObj();
        backgroundsc.Menu();
        player.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        camerasc.Menue();
        stairsc.Menu();
        UIsc.GameOver();
        audiosc.GameOver();
        angel = false;
        succubus = false;
    }

    private void DeleteGameObj() //Delete ChasingNPC, dropWarning, angel, succubus, stairs
    {
        GameObject[] allMapSetting = GameObject.FindGameObjectsWithTag("MapSetting");
        foreach(GameObject mapSetting in allMapSetting)
        {
            Destroy(mapSetting);
        }
    }
}
