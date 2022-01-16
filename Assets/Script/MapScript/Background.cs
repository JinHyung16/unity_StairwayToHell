using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
    Background 제어

    Angel/Succubus 배경을 EFFECT_TIME동안 보여주기
    Player가 이동할 때 배경이 OFFSET만큼 이동
 */
public class Background : MonoBehaviour
{
    public GameObject angelOb;
    public GameObject succubusOb;
    public GameObject background;
    public GameObject m_camera;
    private UIScript UIsc;
    private TestPlayerScript playersc;

    const float backOffset = 0.01f; // player가 1번 이동할 때 배경이 이동하는 정도
    const float effectTime = 1.0f; // trap에 해당하는 배경이 보여지는 시간

    private float offset;

    private void Awake()
    {
        UIsc = GameObject.Find("Canvas").GetComponent<UIScript>();
        playersc = GameObject.Find("Player").GetComponent<TestPlayerScript>();
    }

    private void Start()
    {
        angelOb.SetActive(false);
        succubusOb.SetActive(false);
        offset = 0;
    }

    private void LateUpdate()//background의 위치를 카메라랑 맞게 하기 (EffectAngel과 EffectSuccubus는 Canvas에 소속되어 있으므로 필요 x)
    {
        background.gameObject.transform.position = new Vector3(m_camera.transform.position.x, m_camera.transform.position.y,100.0f);
    }

    /*
     EffectAngel:
        Angel Background 2초 보여주기
        
     사용: 
        UIsc.angelEffect -> -1
        UIsc -> 1
    */
    public void EffectAngel(int dir)// 1: 원위치 -1: 천사 위치
    {
        if(playersc.alive)
        {
            StartCoroutine(TrapEffect(angelOb,2.0f));
            StartCoroutine("FeatherMovement", dir);
        }
    }

    /*
     EffectSuccubus:
        Succubus Background 1초 보여주기
        
     사용: 
        Succubus
    */
    public void EffectSuccubus()
    {
        StartCoroutine(TrapEffect(succubusOb,1.0f));
    }

    /*
     backgroundOffset:
        Player의 이동에 따라 background를 보는 시점을 다르게 하기
        
     사용: 
        Background.Menu -> S
        TestPlayerScript -> L, R
    */

    public void backgroundOffset(char control) // L = left, R = right, S = reset
    {
        if (control == 'L')
            offset -= backOffset;
        else if (control == 'R')
            offset += backOffset;
        else if (control == 'S')
            offset = 0.0f;
        background.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(offset, 0);
    }

    /*
     Menu:
     게임 종료후 초기화

     사용:
     SetUp.MainMenu
     SetUp.GameOver
    */
    public void Menu()
    {
        backgroundOffset('S');
        angelOb.transform.localPosition = new Vector3(0.0f, -864.0f, 0.0f);
    }

    private IEnumerator TrapEffect(GameObject effect,float time)
    {
        if (playersc.alive)
        {
            effect.SetActive(true);
            yield return new WaitForSeconds(time);
            effect.SetActive(false);
        }
        else
            effect.SetActive(false);

    }

    private IEnumerator FeatherMovement(int dir)
    {
        for (int i = 0; i < 12; i++)
        {
            angelOb.transform.GetChild(0).localPosition = new Vector3(-7.0f + 0.5f*i, 0.0f, 0.0f);
            angelOb.transform.GetChild(1).localPosition = new Vector3(7.0f - 0.5f * i, 0.0f, 0.0f);
            yield return new WaitForSeconds(0.05f);
        }
        UIsc.ChangeArrows(dir);
        for (int i = 0; i < 12; i++)
        {
            angelOb.transform.GetChild(0).localPosition = new Vector3(-1.5f - 0.5f * i, 0.0f, 0.0f);
            angelOb.transform.GetChild(1).localPosition = new Vector3(1.5f + 0.5f * i, 0.0f, 0.0f);
            yield return new WaitForSeconds(0.05f);
        }
    }
}
