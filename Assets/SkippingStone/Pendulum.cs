using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pendulum : MonoBehaviour
{
    //펜듈럼이 좌우로 랜덤하게 흔들리게 함

    public float speed = 1.5f; //움직임 속도를 제어하는 실수 변수
    public float limit = 40f; //회전 한계 각도를 제어하는 실수 변수
    public bool randomStart = false; //초기 각도를 랜덤하게 제어 여부를 나타내는 불린형 변수
    private float random = 0; //초기 랜덤 각도를 저장하는 변수
    // Start is called before the first frame update
    void Awake()
    {
        //값이 true일 경우
        if (randomStart)
            random = Random.Range(0f, 1f); //0부터 1 중 초기 각도를 랜덤하게 설정
    }

    //펜듈럼의 움직임 제어
    // Update is called once per frame
    void Update()
    {
        //현재 시간과 변수 값을 고려하여 펜듈럼의 현재 각도 계산
        float angel = limit * Mathf.Sin(Time.time + random * speed);
        //회전 설정, angel만큼 돌아감
        transform.localRotation = Quaternion.Euler(0, 0, angel);
    }
}