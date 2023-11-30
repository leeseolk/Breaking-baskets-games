using UnityEngine;
using System.Collections;
using System;
using com.rfilkov.kinect;
using UnityEngine.UI;

//네임스페이스로 절대 경로를 설정하여 오류를 방지하고 코드 조직화함
//아래 class들의 소속 정리
namespace com.rfilkov.components
{
    /// <summary>
    /// JointOverlayer overlays the given body joint with the given virtual object.
    /// </summary>
    public class SkippingStone : MonoBehaviour
    {
        //인스턴트, 변수 정의들 
        //변수 역할 설명
        //tooltip: 마우스 오버 시 툴팁 설명 나옴, 한글로도 설명 가능
        //인스펙터 창 안에 표현됨

        private Renderer objectRenderer;
        [Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
        public int playerIndex = 0;

        [Tooltip("오른손 관절")]
        public KinectInterop.JointType trackedJoint = KinectInterop.JointType.HandRight;

        [Tooltip("오른쪽 어깨 관절")]
        public KinectInterop.JointType trackedJoint2 = KinectInterop.JointType.ShoulderRight;

        [Tooltip("Depth sensor index used for color camera overlay - 0 is the 1st one, 1 - the 2nd one, etc.")]
        public int sensorIndex = 0;

        [Tooltip("공")] // 공에 대한 위치 정보
        public GameObject Ball;



        // reference to KM 키넥트 사용하니까 있어야 함. 키넥트매니저
        private KinectManager kinectManager = null;

        // 동작 인식에 대한 것(꼭 있어야함)
        float handShoulderGap; // 손과 어깨 사이 현재 위치 차이
        float prevHandShoulderGap; // 손과 어깨 사이 이전 위치 차이

        // 공이 던져졌는가 = false
        //던져지면 true로 바뀌도록
        bool isThrown = false;

        Vector3 handPos; // 현재 손의 위치
        Vector3 prevHandPos; // 이전 손의 위치

        private Rigidbody rb;



        public void Start()
        {

            // get reference to KM 키넥트 사용하니까 있어야 함
            kinectManager = KinectManager.Instance;

            rb = Ball.GetComponent<Rigidbody>();
            objectRenderer = Ball.GetComponent<Renderer>();
            SetRandomColor();


        }




        void Update() // fixedUpdate와의 차이 공부. 
        {
            //키넥트 사용 가능한지
            if (kinectManager && kinectManager.IsInitialized())
            {
                // overlay the joint
                //고유 id 값
                ulong userId = kinectManager.GetUserIdByIndex(playerIndex);

                //jointindex 0~24번
                int iJointIndex = (int)trackedJoint;
                int iJointIndex2 = (int)trackedJoint2;

                // 오른손과 오른쪽 어깨가 트래킹 되고 있는가
                if (kinectManager.IsJointTracked(userId, iJointIndex) && kinectManager.IsJointTracked(userId, iJointIndex2))
                {
                    // int a = 1 > 0 ? 2 : -1; (물음표를 설명하기 위한 예제)
                    // Vector3 posJoint = foregroundCamera ? // posJoint 오른손의 위치
                    // GetJointPosColorOverlay 공의 위치를 찾는 함수인데.. 이게 아니라 GetJointPos(메인카메라와 관계x, 키넥트 카메라가 바라보는 관점)가 필요
                    // 던져졌는가(오른손, 오른쪽 어깨 관계)에 대한 키넥트 정보만 알면 됨. 공의 위치를 저장할 필요x
                    // kinectManager.GetJointPosColorOverlay(userId, iJointIndex, sensorIndex, foregroundCamera, backgroundRect) :
                    // sensorTransform ? kinectManager.GetJointKinectPosition(userId, iJointIndex, true) : 
                    // kinectManager.GetJointPosition(userId, iJointIndex);

                    //손, 어깨 위치를 vector3값에 저장
                    Vector3 shoulderPos = kinectManager.GetJointPosition(userId, KinectInterop.JointType.ShoulderRight);
                    handPos = kinectManager.GetJointPosition(userId, KinectInterop.JointType.HandRight);

                    //손, 어깨 Z축의 위치 차이
                    //z축은 x축과 y축을 곱한 값이므로 handShoulderGap 음수가 되려면 shoulderPos.z에서 handPos.z를 빼야 함
                    handShoulderGap = -handPos.x; // float handShoulderGap - class 변수로, 글로벌 변수로 지정해야겠군..
                    if (handShoulderGap > 0) // 손이 어깨보다 높을 때
                    {
                        // Ball.GetComponent<Renderer>().material.color = Color.red;
                        if (prevHandShoulderGap < 0) // 던져짐 감지 순간
                        {
                            //공 중력 주기
                            Ball.GetComponent<Rigidbody>().useGravity = true;

                            //다시 던질 때를 위해 기본 속도값 리셋
                            Ball.GetComponent<Rigidbody>().velocity = Vector3.zero;

                            //* 700f = 공 떨어지는 상황 방지, 힘을 키워줌
                            Vector3 force = (handPos - prevHandPos) * 700f; // 손의 현재 위치 - 손의 이전 위치 * 힘(던지는 힘의 정도)
                            //force.z = 100;
                            //force.y += 300;

                            //공에 힘 주기
                            Ball.GetComponent<Rigidbody>().AddForce(force); // (force)대신 (0, 200f, 0)처럼 고정된 힘도 가능

                            //던져졌다는 것을 의미
                            isThrown = true;

                        }

                    }
                    else
                    {
                        // Ball.GetComponent<Renderer>().material.color = Color.green;

                        //떨어지면 공을 리셋해야하기 때문에 이 값은 y로 고정해야 함
                        if (Ball.transform.position.y < -1f) // 공의 y값이 -1 밑으로 내려가면. 공의 원위치 이동
                        {
                            //중력잡기
                            Ball.GetComponent<Rigidbody>().useGravity = false; // 중력을 끔

                            //위치잡기
                            isThrown = false; //공이 손에 다시 붙도록

                            Ball.transform.position = new Vector3(0, 1, 2); // 벡터 안의 값은 공의 기본 위치값으로 설정

                            //속도 잡기
                            Ball.GetComponent<Rigidbody>().velocity = Vector3.zero; //다시 던질 때를 위해 기본 속도값 리셋

                            //공 랜덤 색 함수 실행
                            SetRandomColor();

                            //중력 주기
                            Ball.GetComponent<Rigidbody>().useGravity = true;

                        }

                    }

                    //과거값 = 현재값
                    prevHandShoulderGap = handShoulderGap;
                    prevHandPos = handPos;
                }
            }

        }

        //공 오브젝트의 색을 랜덤하게 하는 함수
        void SetRandomColor()
        {
            //ball 값이 비어있지 않다면
            if (Ball != null)
            {
                //0 또는 1 중 하나의 임의의 정수 값을 생성하고 randomIndex에 저장
                int randomIndex = UnityEngine.Random.Range(0, 2);

                //randomIndex 이 0이면 red로 아니면 blue로
                Color selectedColor = randomIndex == 0 ? Color.red : Color.blue;

                //공의 색상과 연결하여 공의 색을 랜덤하게 설정
                objectRenderer.material.color = selectedColor;
            }
        }




    }

}
