using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Destroy : MonoBehaviour
{

    public ParticleSystem destroyParticle; //destroy될 때 터지는 파티클 넣을 파티클시스템
    private int collisionCount = 0; //충돌 횟수 초기화
    public GameObject destroySoundPrefab; //destroy 시 생기는 효과음 넣을 게임 오브젝트
    public GameObject collisionSoundPrefab; //destroy 시 생기는 효과음 넣을 게임 오브젝트
    private bool hasCollided = false; // 충돌 여부를 추적
    private float delayTime = 4f; // 씬 전환을 위한 대기 시간


    //두 오브젝트가 충돌 시 실행하는 함수
    private void OnCollisionEnter(Collision collision)
    {
        if (hasCollided) return; // 이미 충돌 처리됨
        //collisionSoundPrefab 값이 존재한다면
        if (collisionSoundPrefab != null)
        {
            GameObject soundObject = Instantiate(collisionSoundPrefab, transform.position, Quaternion.identity); //collisionSoundPrefab 같은 위치에 복제
            AudioSource audioSource = soundObject.GetComponent<AudioSource>(); //소리를 재생하는 컴포넌트 불러오기

            //audioSource 값이 존재한다면
            if (audioSource != null)
            {
                audioSource.Play(); //소리 재생
            }
        }

        collisionCount++; //충돌 횟수 증가

        //공과 박이 일정 횟수 충돌되면 파티클이 나오며 박은 destroy되고 효과음이 나오도록 함

        //충돌 횟수가 3번 이상일 때
        if (collisionCount >= 10)
        {
            Instantiate(destroyParticle, transform.position, transform.rotation); //destroyParticle 같은 위치에 복제
            
            //destroySoundPrefab 값이 존재한다면
            if (destroySoundPrefab != null)
            {
                GameObject soundObject = Instantiate(destroySoundPrefab, transform.position, Quaternion.identity); //destroySoundPrefab 같은 위치에 복제
                AudioSource audioSource = soundObject.GetComponent<AudioSource>(); //소리를 재생하는 컴포넌트 불러오기

                //audioSource 값이 존재한다면
                if (audioSource != null)
                {
                    audioSource.Play(); //소리 재생
                }

             
            }

            Destroy(gameObject);
        }


    }
}