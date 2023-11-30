using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    private int Timer = 0; //시간을 나타내는 Timer 정수 변수 선언

    public GameObject CD_One; //텍스트 1 넣을 게임 오브젝트
    public GameObject CD_Two; //텍스트 2 넣을 게임 오브젝트
    public GameObject CD_Three; //텍스트 3 넣을 게임 오브젝트
    public GameObject CD_Start; //텍스트 스타트 넣을 게임 오브젝트

    public GameObject cheerSoundPrefab; //bgm 소리 넣을 게임 오브젝트
    public GameObject startSoundPrefab; //시작 시 소리 넣을 게임 오브젝트
    public GameObject bgSoundPrefab; //bgm 소리 넣을 게임 오브젝트2



    // Start is called before the first frame update
    void Start()
    {
        Timer = 0; //시간 초기화

        //게임 오브젝트들 비활성화
        CD_One.SetActive(false);
        CD_Two.SetActive(false);
        CD_Three.SetActive(false);
        CD_Start.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Timer == 0) //시간이 0일 때
        {
            //게임 시 시간의 흐름 제어, 0.0일 경우 시간 정지
            Time.timeScale = 0.0f; //게임이 시작되지 않도록 함
        }

        //Timer가 120보다 작을 경우
        //90으로 설정하니 조금 빠르게 시작하는 감이 있어 120으로 늘림
        if (Timer <= 120)
        {
            Timer++; //Timer 1씩 증가

            // 3
            if (Timer < 40) // Timer가 40보다 작을 경우 3번켜기
            {
                CD_Three.SetActive(true); //텍스트 3 나타내기
            }

            // 2
            if (Timer > 40) // Timer가 40보다 클 경우 3번끄고 2번켜기
            {
                CD_Three.SetActive(false); //텍스트 3 없애기
                CD_Two.SetActive(true); //텍스트 2 나타내기
            }

            // 1
            if (Timer > 80) // Timer가 80보다 작을 경우 2번끄고 1번켜기
            {
                CD_Two.SetActive(false); //텍스트 2 없애기
                CD_One.SetActive(true); //텍스트 1 나타내기
            }

            // START
            if (Timer >= 120) //Timer 가 120보다 크거나 같을 경우 1번끄고 GO 켜기, LoadingEnd () 코루틴호출
            {
                CD_One.SetActive(false); //텍스트 2 없애기
                CD_Start.SetActive(true); //텍스트 START 나타내기
                StartCoroutine(this.LoadingEnd()); //코루틴 호출
                                                   //1.0일 경우 게임 시간이 실제 시간과 동일하게 흐름
                Time.timeScale = 1.0f; //게임 재개


                //게임이 재개되면(카운트다운이 끝나면!) 이 밑에 소리들이 재생되도록

                //startSoundPrefab 값이 존재한다면
                if (startSoundPrefab != null)
                {
                    GameObject soundObject = Instantiate(startSoundPrefab, transform.position, Quaternion.identity); //startSoundPrefab 같은 위치에 복제
                    AudioSource audioSource = soundObject.GetComponent<AudioSource>(); //소리를 재생할 수 있는 컴포넌트 불러오기

                    //audioSource 값이 존재한다면
                    if (audioSource != null)
                    {
                        audioSource.Play(); //소리 재생
                    }
                }


                //게임은 정지되었는데 배경음악이 나오는 것이 어색해 이 두 배경음악 프리팹을 게임 재개 시 동작하는 if문 안에 넣어 게임이 시작하면 배경음악이 재생되도록 함

                //cheerSoundPrefab 값이 존재한다면
                if (cheerSoundPrefab != null)
                {
                    GameObject soundObject = Instantiate(cheerSoundPrefab, transform.position, Quaternion.identity); //cheerSoundPrefab 같은 위치에 복제
                    AudioSource audioSource = soundObject.GetComponent<AudioSource>(); //소리를 재생할 수 있는 컴포넌트 불러오기

                    //audioSource 값이 존재한다면
                    if (audioSource != null)
                    {
                        audioSource.Play(); //소리 재생
                    }
                }

                //bgSoundPrefab 값이 존재한다면
                if (bgSoundPrefab != null)
                {
                    GameObject soundObject = Instantiate(bgSoundPrefab, transform.position, Quaternion.identity); //bgSoundPrefab 같은 위치에 복제
                    AudioSource audioSource = soundObject.GetComponent<AudioSource>(); //소리를 재생할 수 있는 컴포넌트 불러오기

                    //audioSource 값이 존재한다면
                    if (audioSource != null)
                    {
                        audioSource.Play(); //소리 재생
                    }
                }
            }
        }

    }
    //일시적으로 멈추는 함수인 코루틴 불러오기
    IEnumerator LoadingEnd()
    {

        //1초동안 지연되는 시간 = 1초 기다린 후에 실행
        yield return new WaitForSeconds(1.0f);
        CD_Start.SetActive(false); //텍스트 START 없애기
    }

    //모든 텍스트 오브젝트(3,2,1,START)를 없애고 게임을 시작할 수 있도록
}

