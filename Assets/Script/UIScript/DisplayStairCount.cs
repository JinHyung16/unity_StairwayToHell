using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
    이동 계단 곗수 표시
 */
public class DisplayStairCount : MonoBehaviour {

    public Text countText;
    private TestPlayerScript playersc;

    private void Awake()
    {
        playersc = GameObject.Find("Player").GetComponent<TestPlayerScript>();
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateUI();
    }
    
    private void UpdateUI()
    {
        countText.text = playersc.StairsPassed.ToString();
    }
}
