using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pendulum : MonoBehaviour
{
    //��ⷳ�� �¿�� �����ϰ� ��鸮�� ��

    public float speed = 1.5f; //������ �ӵ��� �����ϴ� �Ǽ� ����
    public float limit = 40f; //ȸ�� �Ѱ� ������ �����ϴ� �Ǽ� ����
    public bool randomStart = false; //�ʱ� ������ �����ϰ� ���� ���θ� ��Ÿ���� �Ҹ��� ����
    private float random = 0; //�ʱ� ���� ������ �����ϴ� ����
    // Start is called before the first frame update
    void Awake()
    {
        //���� true�� ���
        if (randomStart)
            random = Random.Range(0f, 1f); //0���� 1 �� �ʱ� ������ �����ϰ� ����
    }

    //��ⷳ�� ������ ����
    // Update is called once per frame
    void Update()
    {
        //���� �ð��� ���� ���� ����Ͽ� ��ⷳ�� ���� ���� ���
        float angel = limit * Mathf.Sin(Time.time + random * speed);
        //ȸ�� ����, angel��ŭ ���ư�
        transform.localRotation = Quaternion.Euler(0, 0, angel);
    }
}