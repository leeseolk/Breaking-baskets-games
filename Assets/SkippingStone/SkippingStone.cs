using UnityEngine;
using System.Collections;
using System;
using com.rfilkov.kinect;
using UnityEngine.UI;

//���ӽ����̽��� ���� ��θ� �����Ͽ� ������ �����ϰ� �ڵ� ����ȭ��
//�Ʒ� class���� �Ҽ� ����
namespace com.rfilkov.components
{
    /// <summary>
    /// JointOverlayer overlays the given body joint with the given virtual object.
    /// </summary>
    public class SkippingStone : MonoBehaviour
    {
        //�ν���Ʈ, ���� ���ǵ� 
        //���� ���� ����
        //tooltip: ���콺 ���� �� ���� ���� ����, �ѱ۷ε� ���� ����
        //�ν����� â �ȿ� ǥ����

        private Renderer objectRenderer;
        [Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
        public int playerIndex = 0;

        [Tooltip("������ ����")]
        public KinectInterop.JointType trackedJoint = KinectInterop.JointType.HandRight;

        [Tooltip("������ ��� ����")]
        public KinectInterop.JointType trackedJoint2 = KinectInterop.JointType.ShoulderRight;

        [Tooltip("Depth sensor index used for color camera overlay - 0 is the 1st one, 1 - the 2nd one, etc.")]
        public int sensorIndex = 0;

        [Tooltip("��")] // ���� ���� ��ġ ����
        public GameObject Ball;



        // reference to KM Ű��Ʈ ����ϴϱ� �־�� ��. Ű��Ʈ�Ŵ���
        private KinectManager kinectManager = null;

        // ���� �νĿ� ���� ��(�� �־����)
        float handShoulderGap; // �հ� ��� ���� ���� ��ġ ����
        float prevHandShoulderGap; // �հ� ��� ���� ���� ��ġ ����

        // ���� �������°� = false
        //�������� true�� �ٲ��
        bool isThrown = false;

        Vector3 handPos; // ���� ���� ��ġ
        Vector3 prevHandPos; // ���� ���� ��ġ

        private Rigidbody rb;



        public void Start()
        {

            // get reference to KM Ű��Ʈ ����ϴϱ� �־�� ��
            kinectManager = KinectManager.Instance;

            rb = Ball.GetComponent<Rigidbody>();
            objectRenderer = Ball.GetComponent<Renderer>();
            SetRandomColor();


        }




        void Update() // fixedUpdate���� ���� ����. 
        {
            //Ű��Ʈ ��� ��������
            if (kinectManager && kinectManager.IsInitialized())
            {
                // overlay the joint
                //���� id ��
                ulong userId = kinectManager.GetUserIdByIndex(playerIndex);

                //jointindex 0~24��
                int iJointIndex = (int)trackedJoint;
                int iJointIndex2 = (int)trackedJoint2;

                // �����հ� ������ ����� Ʈ��ŷ �ǰ� �ִ°�
                if (kinectManager.IsJointTracked(userId, iJointIndex) && kinectManager.IsJointTracked(userId, iJointIndex2))
                {
                    // int a = 1 > 0 ? 2 : -1; (����ǥ�� �����ϱ� ���� ����)
                    // Vector3 posJoint = foregroundCamera ? // posJoint �������� ��ġ
                    // GetJointPosColorOverlay ���� ��ġ�� ã�� �Լ��ε�.. �̰� �ƴ϶� GetJointPos(����ī�޶�� ����x, Ű��Ʈ ī�޶� �ٶ󺸴� ����)�� �ʿ�
                    // �������°�(������, ������ ��� ����)�� ���� Ű��Ʈ ������ �˸� ��. ���� ��ġ�� ������ �ʿ�x
                    // kinectManager.GetJointPosColorOverlay(userId, iJointIndex, sensorIndex, foregroundCamera, backgroundRect) :
                    // sensorTransform ? kinectManager.GetJointKinectPosition(userId, iJointIndex, true) : 
                    // kinectManager.GetJointPosition(userId, iJointIndex);

                    //��, ��� ��ġ�� vector3���� ����
                    Vector3 shoulderPos = kinectManager.GetJointPosition(userId, KinectInterop.JointType.ShoulderRight);
                    handPos = kinectManager.GetJointPosition(userId, KinectInterop.JointType.HandRight);

                    //��, ��� Z���� ��ġ ����
                    //z���� x��� y���� ���� ���̹Ƿ� handShoulderGap ������ �Ƿ��� shoulderPos.z���� handPos.z�� ���� ��
                    handShoulderGap = -handPos.x; // float handShoulderGap - class ������, �۷ι� ������ �����ؾ߰ڱ�..
                    if (handShoulderGap > 0) // ���� ������� ���� ��
                    {
                        // Ball.GetComponent<Renderer>().material.color = Color.red;
                        if (prevHandShoulderGap < 0) // ������ ���� ����
                        {
                            //�� �߷� �ֱ�
                            Ball.GetComponent<Rigidbody>().useGravity = true;

                            //�ٽ� ���� ���� ���� �⺻ �ӵ��� ����
                            Ball.GetComponent<Rigidbody>().velocity = Vector3.zero;

                            //* 700f = �� �������� ��Ȳ ����, ���� Ű����
                            Vector3 force = (handPos - prevHandPos) * 700f; // ���� ���� ��ġ - ���� ���� ��ġ * ��(������ ���� ����)
                            //force.z = 100;
                            //force.y += 300;

                            //���� �� �ֱ�
                            Ball.GetComponent<Rigidbody>().AddForce(force); // (force)��� (0, 200f, 0)ó�� ������ ���� ����

                            //�������ٴ� ���� �ǹ�
                            isThrown = true;

                        }

                    }
                    else
                    {
                        // Ball.GetComponent<Renderer>().material.color = Color.green;

                        //�������� ���� �����ؾ��ϱ� ������ �� ���� y�� �����ؾ� ��
                        if (Ball.transform.position.y < -1f) // ���� y���� -1 ������ ��������. ���� ����ġ �̵�
                        {
                            //�߷����
                            Ball.GetComponent<Rigidbody>().useGravity = false; // �߷��� ��

                            //��ġ���
                            isThrown = false; //���� �տ� �ٽ� �ٵ���

                            Ball.transform.position = new Vector3(0, 1, 2); // ���� ���� ���� ���� �⺻ ��ġ������ ����

                            //�ӵ� ���
                            Ball.GetComponent<Rigidbody>().velocity = Vector3.zero; //�ٽ� ���� ���� ���� �⺻ �ӵ��� ����

                            //�� ���� �� �Լ� ����
                            SetRandomColor();

                            //�߷� �ֱ�
                            Ball.GetComponent<Rigidbody>().useGravity = true;

                        }

                    }

                    //���Ű� = ���簪
                    prevHandShoulderGap = handShoulderGap;
                    prevHandPos = handPos;
                }
            }

        }

        //�� ������Ʈ�� ���� �����ϰ� �ϴ� �Լ�
        void SetRandomColor()
        {
            //ball ���� ������� �ʴٸ�
            if (Ball != null)
            {
                //0 �Ǵ� 1 �� �ϳ��� ������ ���� ���� �����ϰ� randomIndex�� ����
                int randomIndex = UnityEngine.Random.Range(0, 2);

                //randomIndex �� 0�̸� red�� �ƴϸ� blue��
                Color selectedColor = randomIndex == 0 ? Color.red : Color.blue;

                //���� ����� �����Ͽ� ���� ���� �����ϰ� ����
                objectRenderer.material.color = selectedColor;
            }
        }




    }

}
