using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Destroy : MonoBehaviour
{

    public ParticleSystem destroyParticle; //destroy�� �� ������ ��ƼŬ ���� ��ƼŬ�ý���
    private int collisionCount = 0; //�浹 Ƚ�� �ʱ�ȭ
    public GameObject destroySoundPrefab; //destroy �� ����� ȿ���� ���� ���� ������Ʈ
    public GameObject collisionSoundPrefab; //destroy �� ����� ȿ���� ���� ���� ������Ʈ
    private bool hasCollided = false; // �浹 ���θ� ����
    private float delayTime = 4f; // �� ��ȯ�� ���� ��� �ð�


    //�� ������Ʈ�� �浹 �� �����ϴ� �Լ�
    private void OnCollisionEnter(Collision collision)
    {
        if (hasCollided) return; // �̹� �浹 ó����
        //collisionSoundPrefab ���� �����Ѵٸ�
        if (collisionSoundPrefab != null)
        {
            GameObject soundObject = Instantiate(collisionSoundPrefab, transform.position, Quaternion.identity); //collisionSoundPrefab ���� ��ġ�� ����
            AudioSource audioSource = soundObject.GetComponent<AudioSource>(); //�Ҹ��� ����ϴ� ������Ʈ �ҷ�����

            //audioSource ���� �����Ѵٸ�
            if (audioSource != null)
            {
                audioSource.Play(); //�Ҹ� ���
            }
        }

        collisionCount++; //�浹 Ƚ�� ����

        //���� ���� ���� Ƚ�� �浹�Ǹ� ��ƼŬ�� ������ ���� destroy�ǰ� ȿ������ �������� ��

        //�浹 Ƚ���� 3�� �̻��� ��
        if (collisionCount >= 10)
        {
            Instantiate(destroyParticle, transform.position, transform.rotation); //destroyParticle ���� ��ġ�� ����
            
            //destroySoundPrefab ���� �����Ѵٸ�
            if (destroySoundPrefab != null)
            {
                GameObject soundObject = Instantiate(destroySoundPrefab, transform.position, Quaternion.identity); //destroySoundPrefab ���� ��ġ�� ����
                AudioSource audioSource = soundObject.GetComponent<AudioSource>(); //�Ҹ��� ����ϴ� ������Ʈ �ҷ�����

                //audioSource ���� �����Ѵٸ�
                if (audioSource != null)
                {
                    audioSource.Play(); //�Ҹ� ���
                }

             
            }

            Destroy(gameObject);
        }


    }
}