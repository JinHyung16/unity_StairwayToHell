using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
    상황에 따른 UI 조절
 */
public class UIScript : MonoBehaviour
{
    public GameObject startButton;
    public GameObject leftButton;
    public GameObject rightButton;
    public GameObject title;
    public GameObject count;
    public GameObject mute;
    public GameObject gameOver;
    public GameObject backHome;
    public GameObject backHelp;
    public GameObject introduce;
    public GameObject introHelp;

    private SetUp setUpsc;
    private TestPlayerScript playersc;
    private TrapGenerater trapGensc;
    private Background backgroundsc;

    const float ANGEL_TIME = 5.0f; //angel 효과 지속 시간
    const float ALERT_TIME = 1.0f; //장애물 조심 표기 시간 (이 시간 동안 움직일 수 없습니다)
    const float BLINK_TIME = 1.0f; //Start to Play (깜빡임 주기)/2
    const float GAMEOVER_SHOW_TIME = 0.05f; //gameOver 창이 올라오는 데 걸리는 시간/10

    private float angelTime;

    private void Awake()
    {
        setUpsc = GameObject.Find("GameManager").GetComponent<SetUp>();
        playersc = GameObject.Find("Player").GetComponent<TestPlayerScript>();
        trapGensc = GameObject.Find("GameManager").GetComponent<TrapGenerater>();
        backgroundsc = GameObject.Find("GameManager").GetComponent<Background>();

        startButton.GetComponent<Button>().onClick.AddListener(setUpsc.StartGame);
        gameOver.transform.GetChild(1).gameObject.GetComponent<Button>().onClick.AddListener(setUpsc.StartGame);//restart
        gameOver.transform.GetChild(2).gameObject.GetComponent<Button>().onClick.AddListener(setUpsc.MainMenu);//home
        backHome.GetComponent<Button>().onClick.AddListener(delegate {
            Time.timeScale = 0;
            leftButton.SetActive(false);
            rightButton.SetActive(false);
            backHome.SetActive(false);
            backHelp.SetActive(true);
        });//open backHelp
        backHelp.transform.GetChild(1).gameObject.GetComponent<Button>().onClick.AddListener(delegate {
            Time.timeScale = 1;
            leftButton.SetActive(true);
            rightButton.SetActive(true);
            backHome.SetActive(true);
            backHelp.SetActive(false);
        }); //backHelp exit
        backHelp.transform.GetChild(2).gameObject.GetComponent<Button>().onClick.AddListener(delegate {
            Time.timeScale = 1;
            backHelp.SetActive(false);
            setUpsc.MainMenu(); //여기에 필요한 것 다 나옴
        }); //backHelp -> go to main menu
        introduce.GetComponent<Button>().onClick.AddListener(delegate {
            introHelp.SetActive(true);
            introduce.SetActive(false);
        });
        introHelp.transform.GetChild(2).gameObject.GetComponent<Button>().onClick.AddListener(delegate {
            introHelp.SetActive(false);
            introduce.SetActive(true);
        }); //introHelp exit

        startButton.SetActive(false);
        leftButton.SetActive(false);
        rightButton.SetActive(false);
        title.SetActive(false);
        count.SetActive(false);
        mute.SetActive(false);
        gameOver.SetActive(false);
        backHome.SetActive(false);
        backHelp.SetActive(false);
        introduce.SetActive(false);
        introHelp.SetActive(false);
    }

    private void Update() //angel 효과 푸는 코드
    {
        AngelEffectSolve();
        
    }

    private void AngelEffectSolve()
    {
        if (playersc.alive)
        {
            if (setUpsc.angel)
            {
                if (angelTime <= 0)
                {
                    backgroundsc.EffectAngel(1);
                    StartCoroutine("playerPause");
                    setUpsc.angel = false;
                    trapGensc.angelExist = false;
                    angelTime = 0.0f;
                }
                angelTime -= Time.deltaTime;
            }
        }
    }
    /*
     MainMenu:
        StartButton 깜빡임
        Title, Mute 띄우기
        
     사용:
        SetUp.MainMenu 
    */
    public void MainMenu()
    {
        startButton.SetActive(true);
        leftButton.SetActive(false);
        rightButton.SetActive(false);
        title.SetActive(true);
        count.SetActive(false);
        mute.SetActive(true);
        gameOver.SetActive(false);
        backHome.SetActive(false);
        introduce.SetActive(true);
        StartCoroutine("Blink");
    }

    /*
     StartGame:
        조작키 띄우기
        count 띄우기

     사용:
        SetUp.StartGame
    */
    public void StartGame()
    {
        startButton.SetActive(false);
        leftButton.SetActive(true);
        rightButton.SetActive(true);
        title.SetActive(false);
        count.SetActive(true);
        mute.SetActive(false);
        gameOver.SetActive(false);
        backHome.SetActive(true);
        introduce.SetActive(false);
        introHelp.SetActive(false);
        ChangeArrows(1);
        angelTime = 0.0f;
    }

    /*
     GameOver:
        GameOver 창 띄우기
        최종 Score 표시
    
     사용:
        SetUp.GameOver
    */
    public void GameOver()
    {
        gameOver.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Score\n" + playersc.StairsPassed.ToString();
        backHome.SetActive(false);
        leftButton.SetActive(false);
        rightButton.SetActive(false);
        for(int i = 0; i<4; i++)
        {
            gameOver.transform.GetChild(i).gameObject.SetActive(false);
        }
        gameOver.SetActive(true);
        gameOver.GetComponent<RectTransform>().localPosition = new Vector3(0.0f, -1750.0f, 0.0f);
        StartCoroutine("gameOverShow");
    }

    /*
     AngelEffect:
        조작키 바뀜/원위치 warning (이 때 이동 x)
        조작키 좌우 바뀜/원위치

     사용:
        Angel
    */
    public void AngelEffect()
    {
        if(playersc.alive)
        {
            if(setUpsc.angel == false) // angel 효과 적용 중이지 않음
            {
                backgroundsc.EffectAngel(-1);
                StartCoroutine("playerPause");
                setUpsc.angel = true;
            }
            angelTime = ANGEL_TIME; //angel 효과가 원래 있었을 때 효과 지속시간을 다시 ANGEL_TIME으로 바꿈
        }
        
    }

    /*
     succubusEffect:
        맵 좌우반전 warning(이 때 이동 x)

     사용:
        Succubus
    */
    public void SuccubusEffect()
    {
        StartCoroutine("playerPause");
    }

    private IEnumerator Blink()
    {
        while (setUpsc.flow == 0)
        {
            if (setUpsc.flow != 0) break;
            startButton.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Press to Play";
            yield return new WaitForSeconds(BLINK_TIME);
            if (setUpsc.flow != 0) break;
            startButton.transform.GetChild(0).gameObject.GetComponent<Text>().text = "";
            yield return new WaitForSeconds(BLINK_TIME);
        }
    }

    private IEnumerator playerPause()
    {
        if(setUpsc.flow == 1 || setUpsc.flow == 3)
            setUpsc.flow = 3; //player not moving
        yield return new WaitForSeconds(ALERT_TIME); //효과 후 움직이지 않는 시간
        if (setUpsc.flow == 1 || setUpsc.flow == 3)
            setUpsc.flow = 1; //player moving
        
    }
    /*
     changeArrows:
     화살표의 위치를 서로 바꿈

     사용:
     Background.EffectAngel -> 1, -1
     UIsc.StartGame -> 1
     */
    public void ChangeArrows(int dir)//1: 원래 상태 -1: 천사 상태
    {
        leftButton.GetComponent<RectTransform>().localPosition = new Vector3(-300.0f * dir, -750.0f, 0.0f);
        rightButton.GetComponent<RectTransform>().localPosition = new Vector3(300.0f * dir, -750.0f, 0.0f);
    }

    private IEnumerator gameOverShow()
    {
        for (int i = 1; i < 11; i++)
        {
            gameOver.GetComponent<RectTransform>().localPosition = new Vector3(0.0f, (-1750.0f) + (185.0f*i), 0.0f);
            yield return new WaitForSeconds(GAMEOVER_SHOW_TIME);
        }
        yield return new WaitForSeconds(1.0f);
        gameOver.transform.GetChild(0).gameObject.SetActive(true);
        gameOver.transform.GetChild(3).gameObject.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        gameOver.transform.GetChild(1).gameObject.SetActive(true);
        gameOver.transform.GetChild(2).gameObject.SetActive(true);
    }
}