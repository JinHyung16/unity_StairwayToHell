using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropObj : MonoBehaviour
{
    public Sprite warning;
    public Sprite danger;
    public GameObject cagePrefab;
    private GameObject m_camera;
    private GameObject player;
    private TrapGenerater trapGensc;
    private TestPlayerScript playersc;
    private Animator animator;
    private SetUp setUpsc;
    private CreateStairs stairsc;

    const float lightTime = 2.0f; //그냥 빛인 시간
    const float blinkPreiod = 0.2f;//깜빡이는 주기
    const float blinkTime = 2.0f; // 깜빡이는 시간
    const float cageCatch = 2.0f;// Cage가 player위 CAGE_CAUGHT에 있으면 잡힌 것이라고 감지

    private float timer = 0;
    private float blinkTimer = 0;
    private bool trapEnabled; // cage가 떠러질 때
    private bool caught; // player가 잡혔을 때

    private void Awake()
    {
        m_camera = GameObject.Find("Main Camera");
        player = GameObject.Find("Player");
        trapGensc = GameObject.Find("GameManager").GetComponent<TrapGenerater>();
        playersc = player.GetComponent<TestPlayerScript>();
        setUpsc = GameObject.Find("GameManager").GetComponent<SetUp>();
        stairsc = GameObject.Find("GameManager").GetComponent<CreateStairs>();
    }

    private void Start()// TrapGenerater로 생성
    {
        GetComponent<SpriteRenderer>().sprite = warning;
        transform.position = new Vector3(transform.position.x, m_camera.transform.position.y - 5.5f, 0.0f);
        trapEnabled = false;
        caught = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (playersc.alive)
        {
            DoTrap();
        }
    }

    private void DoTrap()
    {
        if (!caught)
        {
            transform.position = new Vector3(transform.position.x, m_camera.transform.position.y - 5.5f, 0.0f);
        }
        if (lightTime <= timer && timer <= lightTime + blinkTime) //깜빡이는 시간
        {
            if (blinkTimer > blinkPreiod)
            {
                if (GetComponent<SpriteRenderer>().sprite == warning)
                    GetComponent<SpriteRenderer>().sprite = null;
                else if (GetComponent<SpriteRenderer>().sprite == null)
                    GetComponent<SpriteRenderer>().sprite = warning;
                blinkTimer = 0.0f;
            }
            blinkTimer += Time.deltaTime;
        }
        else if (timer >= lightTime + blinkTime && !trapEnabled) //cage 떨어지는 시간
        {
            trapEnabled = true;
            StartCoroutine("TrapDrop", Instantiate(cagePrefab, new Vector3(transform.position.x, transform.position.y + 10.5f, 0.0f), Quaternion.identity));
        }
        timer += Time.deltaTime;

    }

    private IEnumerator TrapDrop(GameObject cage)
    {
        GetComponent<SpriteRenderer>().sprite = danger;
        while (cage.transform.position.y > m_camera.transform.position.y - 8)//cage가 화면 안에 있을 때
        {
            cage.transform.Translate(0, -0.5f, 0);//cage 이동
            if (cage.transform.position.x == player.transform.position.x && player.transform.position.y <= cage.transform.position.y && cage.transform.position.y <= player.transform.position.y + cageCatch && !caught && playersc.alive) //잡힐 상황
            {
                if(setUpsc.flow == 1)
                    setUpsc.flow = 3;
                caught = true;
                transform.localScale = new Vector3(0.75f, 1.0f, 1.0f);
                transform.position = stairsc.stairPlace[playersc.StairsPassed%25].transform.position;
            }
            if (player.transform.position.y <= cage.transform.position.y && caught) //잡힌 후 상황
            {
                if (playersc.alive)
                {
                    playersc.Death();
                    cage.GetComponent<Animator>().SetTrigger("Crash");
                }
                cage.transform.position = player.transform.position;
            }
            yield return new WaitForSeconds(0.05f);
        }
        trapGensc.dropExist = false;
        Destroy(cage);
        Destroy(this.gameObject);
    }
}