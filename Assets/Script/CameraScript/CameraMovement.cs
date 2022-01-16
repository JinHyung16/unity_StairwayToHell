using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 카메라 움직임:

    카메라의 x = Player의 x
    카메라의 y= Player의 y - PLAYER_OFFSET
*/

public class CameraMovement : MonoBehaviour {

    private GameObject player;
    private TestPlayerScript playersc;

    const float playerOffset = 2.0f; //게임 중에 화면에서 player의 y위치
    const float shakeMove = 0.01f;
    const float shakeWait = 0.05f;//shake 중 대기 시간
    const int shakeTime = 3; //몇번 shake 하는지

    private bool succubusShake;

    private void Awake()
    {
        player = GameObject.Find("Player");
        playersc = GameObject.Find("Player").GetComponent<TestPlayerScript>();
    }

    private void LateUpdate(){
        if (playersc.alive && !succubusShake){//게임 중일 때의 세팅
            transform.position = new Vector3(player.transform.position.x, player.transform.position.y- playerOffset, -1);
        }
    }
    /*
     Menu:
     카메라를 메뉴창을 띄울 때의 위치로 이동
    
     사용:
     SetUp.MainMenu
     SetUp.GameOver
     */
    public void Menue()//게임 중이지 않을 때의 세팅
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y - playerOffset, -1);
    }

    /*
     CameraShake:
        카메라 좌우로 진동

    사용:
    Succubus
     */
    public void CameraShake()
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y - playerOffset, -1);
        succubusShake = true;
        StartCoroutine("cameraShake");
    }

    private IEnumerator cameraShake()
    {
        for (int i = 0; i<shakeTime; i++)
        {
            transform.Translate(shakeMove, 0.0f, 0.0f);
            yield return new WaitForSeconds(shakeWait);
            transform.Translate(-shakeMove, 0.0f, 0.0f);
            yield return new WaitForSeconds(shakeWait);
            transform.Translate(-shakeMove, 0.0f, 0.0f);
            yield return new WaitForSeconds(shakeWait);
            transform.Translate(shakeMove, 0.0f, 0.0f);
            yield return new WaitForSeconds(shakeWait);
            if (!playersc.alive)
                break;
        }
        succubusShake = false;
    }
}
