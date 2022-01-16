using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
    Angel 제어:
    
    Angel 위치 == player 위치
     ->angel background 띄우기
     ->UIsc의 angelEffect
     ->Destroy

    Angel 효과 지속 시간: UIScript에서 조정
 */
public class Angel : MonoBehaviour
{
    private TestPlayerScript playersc;
    private UIScript UIsc;

    private bool trapEnabled = true;

    private void Awake()
    {
        playersc = GameObject.Find("Player").GetComponent<TestPlayerScript>();
        UIsc = GameObject.Find("Canvas").GetComponent<UIScript>();
    }

    // Update is called once per frame
    private void Update()
    {
        AngelEffect();
    }

    private void AngelEffect()
    {
        if (gameObject.transform.position.y == playersc.transform.position.y && gameObject.transform.position.x == playersc.transform.position.x && trapEnabled && playersc.alive)//player 위치 == angel 위치
        {
            trapEnabled = false;
            UIsc.AngelEffect();
            Destroy(gameObject);
        }
    }
}
