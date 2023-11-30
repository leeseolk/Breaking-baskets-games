using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallControl : MonoBehaviour
{
    public float jumpForce = 7.0f; // ������ �ʿ��� ��
    public float forwardForce = 0.1f; // ���� �ӵ�
    private Rigidbody rb;
    // public GameObject Ball;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // �����̽��ٸ� ������ ���� �����ϰ� ����ϴ�.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        /*if (Ball.transform.position.y < -1f) // ���� y���� -1 ������ ��������. ���� ����ġ �̵�
        {
            // Ball.GetComponent<Rigidbody>().useGravity = false; // �߷��� ��
            // isThrown = false;
            Ball.transform.position = new Vector3(0, 1, 2); // ���� ���� ���� ���� �⺻ ��ġ������ ����
            Ball.GetComponent<Rigidbody>().velocity = Vector3.zero;

        }*/
    }

    /*void FixedUpdate()
    {
        // ���� ������ŵ�ϴ�.
        rb.AddForce(Vector3.forward * forwardForce);
    }*/
}
