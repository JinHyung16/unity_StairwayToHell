using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 Player 제어

    조작키 클릭시
     ->이동 + 애니메이션
    오랜 시간 동안 이동 x
     -> 애니메이션: PlayerThunder -> 죽음

    죽을 때 떨어지는 속도, 거리 등 수정하고 싶으면 카톡에 올려주세요
*/
public class TestPlayerScript : MonoBehaviour
{
    public GameObject leftButton;
    public GameObject rightButton;
    private GameObject m_camera;
    private Background backgroundsc;
    private Animator animator;
    private SetUp setUpsc;
    private CreateStairs stairsc;
    private AudioManager audiosc;

    const float idleDeadTime = 5.0f; // idleDeadTime 동안 움직임 x -> 죽음
    const float thunderTime = 0.2f; // THUNDER 애니메이션 지속 시간
    const float fastMusic = 0.25f; // 빠른 노래 나올려면 이동 시간이 fastMusic보다 짧아야함
    const float slowMusic = 1.0f; //느린 노래 나올려면 이동 시간이 slowMusic보다 느려야함
    const int fastMusicStair = 100; // fastMusicStair 이동했을 때 무조건 fastMusic
    const float dropTime = 4.0f; // cage가 떨어지는데 걸리는 시간보다 긴 시간
    const int fastCount = 3; // FAST_COUNT 만큼 FAST_MUSIC보다 빨리 움직이면 Fast Music

    /*
     StairsPassed:
     Player가 이동한 계단의 계수

    조작:
    SetUp.StartGame -> 0 : 게임 초기화
     */
    public int StairsPassed = 0;

    /*
     alive:
     Player가 살아있는지 여부

    조작:
    TestPlayerScript -> false : 초기화
    SetUp.StartGame -> true : 게임 초기화
    TestPlayerScript.reset -> false : 죽었을 때 , Main으로 갈 때
     */
    public bool alive;

    private float idleTime = 0.0f;

    private int fast = 0;
    
    // Use this for initialization

    private void Awake()
    {
        m_camera = GameObject.Find("Main Camera");
        setUpsc = GameObject.Find("GameManager").GetComponent<SetUp>();
        stairsc = GameObject.Find("GameManager").GetComponent<CreateStairs>();
        backgroundsc = GameObject.Find("GameManager").GetComponent<Background>();
        animator = GetComponent<Animator>();
        audiosc = GameObject.Find("GameManager").GetComponent<AudioManager>();
    }

    private void Start()
    {
        alive = false;
        leftButton.GetComponent<Button>().onClick.AddListener(delegate { MovePlayer('L'); });
        rightButton.GetComponent<Button>().onClick.AddListener(delegate { MovePlayer('R'); });
    }

    private void Update()
    {
        if (setUpsc.flow == 1 || setUpsc.flow == 3)
        {
            if (setUpsc.succubus || setUpsc.angel)
                animator.SetBool("Trap", true);
            else
                animator.SetBool("Trap", false);
            if (idleTime >= idleDeadTime)// 일정 시간 지나면 Thunder
                StartCoroutine("Thunder");
            else if (StairsPassed > fastMusicStair && alive) //어느 정도 이상 움직였을 때
                audiosc.SpeedUp();
            else if (idleTime > slowMusic && alive) //느리게 움직이면
            {
                audiosc.StartGame();
                fast = 0;
            }
            idleTime += Time.deltaTime;
        }       
    }

    /*
     Death:
        animation -> death
        0.5초 동안 아래로 이동
        GameOver로 세팅
        alive = false
        
     사용: 
        FallObs
        Following
        DropObj
    */
    public void Death()
    {
        if (alive)
        {
            ResSetGame();
            animator.SetBool("Alive", false);
            animator.SetBool("Trap", false);
            StartCoroutine("Drop");
        }
    }

    /*
    
    사용:
        TestPlayerScript.Death
        SetUp -> MainMenu (게임 중 나왔을 때)
    */
    public void ResSetGame()
    {
        animator.ResetTrigger("Move");
        animator.ResetTrigger("Thunder");
        animator.SetBool("Trap", false);
        alive = false;
        idleTime = 0.0f;
        fast = 0;
        GetComponent<SpriteRenderer>().flipX = false;
    }
/*
     movePlayer:
        좌우로 이동
        animation -> move

        angel일 때:
            잘못 클릭 시 이동하지 않음
        
     사용:
     조작키 버튼 
    */
    private void MovePlayer(char dir) // L = left, R = right
    {
        if (alive && setUpsc.flow == 1)
        {
            if (setUpsc.angel) //angel 효과 있을때
            {
                if (dir == 'L' && (stairsc.stairPlace[(StairsPassed+1) % 25].transform.position.x) == (transform.position.x - 1.0f))
                    Move('L');
                else if (dir == 'R' && (stairsc.stairPlace[(StairsPassed+1) % 25].transform.position.x) == (transform.position.x + 1.0f))
                    Move('R');
            }
            else //angel 효과 없을때
            {
                Move(dir);
            }
        }
    }

    private void Move(char dir)
    {
        if (dir == 'L')
        {
            transform.Translate(Vector3.left * 1.0f);
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (dir == 'R')
        {
            transform.Translate(Vector3.right * 1.0f);
            GetComponent<SpriteRenderer>().flipX = false;
        }
        transform.Translate(Vector3.down * 0.5f);
        backgroundsc.backgroundOffset(dir);
        animator.SetTrigger("Move");
        if (idleTime <= fastMusic)
            fast++;
        if(fast >= fastCount) //빨리 움직이고 있을 때
        {
            audiosc.SpeedUp();
        }
        idleTime = 0.0f;
        StairsPassed++;
    }

    private IEnumerator Drop()
    {
        while(setUpsc.flow ==1 || setUpsc.flow == 3){
            transform.Translate(0, -0.5f, 0);
            yield return new WaitForSeconds(0.05f);
            if(m_camera.transform.position.y - 6.5f > transform.position.y)
            {
                setUpsc.GameOver();
                break;
            }
        }
    }
    private IEnumerator Thunder()
    {
        if (alive)
        {
            setUpsc.flow = 3;
            animator.SetTrigger("Thunder");
            yield return new WaitForSeconds(0.5f);
            Death();
        }
    }
}
    