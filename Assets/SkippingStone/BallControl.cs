using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallControl : MonoBehaviour
{
    public float jumpForce = 7.0f; // 점프에 필요한 힘
    public float forwardForce = 0.1f; // 전진 속도
    private Rigidbody rb;
    // public GameObject Ball;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 스페이스바를 누르면 공을 점프하게 만듭니다.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        /*if (Ball.transform.position.y < -1f) // 공의 y값이 -1 밑으로 내려가면. 공의 원위치 이동
        {
            // Ball.GetComponent<Rigidbody>().useGravity = false; // 중력을 끔
            // isThrown = false;
            Ball.transform.position = new Vector3(0, 1, 2); // 벡터 안의 값은 공의 기본 위치값으로 설정
            Ball.GetComponent<Rigidbody>().velocity = Vector3.zero;

        }*/
    }

    /*void FixedUpdate()
    {
        // 공을 전진시킵니다.
        rb.AddForce(Vector3.forward * forwardForce);
    }*/
}
