using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CubeGameUI : MonoBehaviour
{
    public TextMeshProUGUI TimerText;                           //UI 선언
    public float Timer;                                         //타이머 선언

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime;                                        //타이머 시간이 늘어난다.
        TimerText.text = "생존 시간 : " + Timer.ToString("0.00");       //문자열 형태로 변환하여 보여준다. 
    }
}
