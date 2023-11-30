using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    private int Timer = 0; //�ð��� ��Ÿ���� Timer ���� ���� ����

    public GameObject CD_One; //�ؽ�Ʈ 1 ���� ���� ������Ʈ
    public GameObject CD_Two; //�ؽ�Ʈ 2 ���� ���� ������Ʈ
    public GameObject CD_Three; //�ؽ�Ʈ 3 ���� ���� ������Ʈ
    public GameObject CD_Start; //�ؽ�Ʈ ��ŸƮ ���� ���� ������Ʈ

    public GameObject cheerSoundPrefab; //bgm �Ҹ� ���� ���� ������Ʈ
    public GameObject startSoundPrefab; //���� �� �Ҹ� ���� ���� ������Ʈ
    public GameObject bgSoundPrefab; //bgm �Ҹ� ���� ���� ������Ʈ2



    // Start is called before the first frame update
    void Start()
    {
        Timer = 0; //�ð� �ʱ�ȭ

        //���� ������Ʈ�� ��Ȱ��ȭ
        CD_One.SetActive(false);
        CD_Two.SetActive(false);
        CD_Three.SetActive(false);
        CD_Start.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Timer == 0) //�ð��� 0�� ��
        {
            //���� �� �ð��� �帧 ����, 0.0�� ��� �ð� ����
            Time.timeScale = 0.0f; //������ ���۵��� �ʵ��� ��
        }

        //Timer�� 120���� ���� ���
        //90���� �����ϴ� ���� ������ �����ϴ� ���� �־� 120���� �ø�
        if (Timer <= 120)
        {
            Timer++; //Timer 1�� ����

            // 3
            if (Timer < 40) // Timer�� 40���� ���� ��� 3���ѱ�
            {
                CD_Three.SetActive(true); //�ؽ�Ʈ 3 ��Ÿ����
            }

            // 2
            if (Timer > 40) // Timer�� 40���� Ŭ ��� 3������ 2���ѱ�
            {
                CD_Three.SetActive(false); //�ؽ�Ʈ 3 ���ֱ�
                CD_Two.SetActive(true); //�ؽ�Ʈ 2 ��Ÿ����
            }

            // 1
            if (Timer > 80) // Timer�� 80���� ���� ��� 2������ 1���ѱ�
            {
                CD_Two.SetActive(false); //�ؽ�Ʈ 2 ���ֱ�
                CD_One.SetActive(true); //�ؽ�Ʈ 1 ��Ÿ����
            }

            // START
            if (Timer >= 120) //Timer �� 120���� ũ�ų� ���� ��� 1������ GO �ѱ�, LoadingEnd () �ڷ�ƾȣ��
            {
                CD_One.SetActive(false); //�ؽ�Ʈ 2 ���ֱ�
                CD_Start.SetActive(true); //�ؽ�Ʈ START ��Ÿ����
                StartCoroutine(this.LoadingEnd()); //�ڷ�ƾ ȣ��
                                                   //1.0�� ��� ���� �ð��� ���� �ð��� �����ϰ� �帧
                Time.timeScale = 1.0f; //���� �簳


                //������ �簳�Ǹ�(ī��Ʈ�ٿ��� ������!) �� �ؿ� �Ҹ����� ����ǵ���

                //startSoundPrefab ���� �����Ѵٸ�
                if (startSoundPrefab != null)
                {
                    GameObject soundObject = Instantiate(startSoundPrefab, transform.position, Quaternion.identity); //startSoundPrefab ���� ��ġ�� ����
                    AudioSource audioSource = soundObject.GetComponent<AudioSource>(); //�Ҹ��� ����� �� �ִ� ������Ʈ �ҷ�����

                    //audioSource ���� �����Ѵٸ�
                    if (audioSource != null)
                    {
                        audioSource.Play(); //�Ҹ� ���
                    }
                }


                //������ �����Ǿ��µ� ��������� ������ ���� ����� �� �� ������� �������� ���� �簳 �� �����ϴ� if�� �ȿ� �־� ������ �����ϸ� ��������� ����ǵ��� ��

                //cheerSoundPrefab ���� �����Ѵٸ�
                if (cheerSoundPrefab != null)
                {
                    GameObject soundObject = Instantiate(cheerSoundPrefab, transform.position, Quaternion.identity); //cheerSoundPrefab ���� ��ġ�� ����
                    AudioSource audioSource = soundObject.GetComponent<AudioSource>(); //�Ҹ��� ����� �� �ִ� ������Ʈ �ҷ�����

                    //audioSource ���� �����Ѵٸ�
                    if (audioSource != null)
                    {
                        audioSource.Play(); //�Ҹ� ���
                    }
                }

                //bgSoundPrefab ���� �����Ѵٸ�
                if (bgSoundPrefab != null)
                {
                    GameObject soundObject = Instantiate(bgSoundPrefab, transform.position, Quaternion.identity); //bgSoundPrefab ���� ��ġ�� ����
                    AudioSource audioSource = soundObject.GetComponent<AudioSource>(); //�Ҹ��� ����� �� �ִ� ������Ʈ �ҷ�����

                    //audioSource ���� �����Ѵٸ�
                    if (audioSource != null)
                    {
                        audioSource.Play(); //�Ҹ� ���
                    }
                }
            }
        }

    }
    //�Ͻ������� ���ߴ� �Լ��� �ڷ�ƾ �ҷ�����
    IEnumerator LoadingEnd()
    {

        //1�ʵ��� �����Ǵ� �ð� = 1�� ��ٸ� �Ŀ� ����
        yield return new WaitForSeconds(1.0f);
        CD_Start.SetActive(false); //�ؽ�Ʈ START ���ֱ�
    }

    //��� �ؽ�Ʈ ������Ʈ(3,2,1,START)�� ���ְ� ������ ������ �� �ֵ���
}

