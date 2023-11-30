using UnityEngine;
using System.Collections;
using System;
using com.rfilkov.kinect;

//네임스페이스로 절대 경로를 설정하여 오류를 방지하고 코드 조직화함
//아래 class들의 소속 정리
//kinect 관련 기능을 사용하기 위해사용
namespace com.rfilkov.components
{
    /// <summary>
    /// JointOverlayer overlays the given body joint with the given virtual object.
    /// </summary>
    public class skippingStone : MonoBehaviour
    {
        //인스턴트, 변수 정의들 

        //변수 역할 설명
        //tooltip: 마우스 오버 시 툴팁 설명 나옴, 한글로도 설명 가능
        //인스펙터 창 안에 표현됨

        [Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
        public int playerIndex = 0;

        [Tooltip("오른손 관절")]
        public KinectInterop.JointType trackedJoint = KinectInterop.JointType.HandRight;

        [Tooltip("오른쪽 어깨 관절")]
        public KinectInterop.JointType trackedJoint2 = KinectInterop.JointType.ShoulderRight;

        //색상카메라 오버레이에 사용할 깊이 센서 설정
        [Tooltip("Depth sensor index used for color camera overlay - 0 is the 1st one, 1 - the 2nd one, etc.")]
        public int sensorIndex = 0;

        [Tooltip("공")]
        public GameObject Ball;



        //public UnityEngine.UI.Text debugText;

        // reference to KM
        private KinectManager kinectManager = null;

        //현재 위치 차이값
        float handShoulderGap;
        //과거 위치 차이값
        float PrevHandShoulderGap;

        //공이 던져졌냐 = false
        //던져지면 true로 바뀌도록
        bool isThrown = false;

        //현재 손 위치
        Vector3 handPos;
        //과거 손 위치
        Vector3 prevHandPos;


        public void Start()
        {
            //**instance값이 kinectmanager 변수 값에 복사
            //키넥트 사용을 위한 준비
            // get reference to KM
            kinectManager = KinectManager.Instance;
        }

        void Update()
        {
            //kinectManager가 초기화
            //키넥트 사용 가능한지
            if (kinectManager && kinectManager.IsInitialized())
            {
                // overlay the joint
                //고유 id값
                ulong userId = kinectManager.GetUserIdByIndex(playerIndex);

                //jointindex 0~24번
                int iJointIndex = (int)trackedJoint; //오른손
                int iJointIndex2 = (int)trackedJoint2; //오른쪽어깨
                //손과 어깨 모두 추적 중이면
                if (kinectManager.IsJointTracked(userId, iJointIndex) && kinectManager.IsJointTracked(userId, iJointIndex2))
                {
                    //손, 어깨 위치를 구했으니 vector3 값에 저장
                    Vector3 handPos = kinectManager.GetJointPosition(userId, KinectInterop.JointType.HandRight);
                    Vector3 shoulderPos = kinectManager.GetJointPosition(userId, KinectInterop.JointType.ShoulderRight);
                    //손, 어깨 Z축의 위치 차이
                    //z축은 x축과 y축을 곱한 값이므로 handShoulderGap가 음수가 되려면 shoulderPos.z에서 handPos.z를 빼야 함
                    handShoulderGap = shoulderPos.z - handPos.z;

                    //위치 차이가 0보다 크다면? = 손의 위치가 어깨 위치보다 앞에 있다면! = 손이 어깨를 넘어섰을 때
                    if (handShoulderGap > 0)
                    {
                        Ball.GetComponent<Renderer>().material.color = Color.red;

                        //던져짐 감지 순간
                        //과거값이 0보다 작다면
                        if (PrevHandShoulderGap < 0)
                        {
                            //공 중력 주기
                            Ball.GetComponent<Rigidbody>().useGravity = true;

                            //다시 던질 때를 위해 기본 속도값 리셋
                            Ball.GetComponent<Rigidbody>().velocity = Vector3.zero;

                            //* 1000f = 공 떨어지는 상황 방지, 힘을 키워줌
                            Vector3 force = (handPos - prevHandPos) * 700f;

                            //-값을 주어 부호 바꾸기 = 방향 바꾸기
                            force.z = -force.z;

                            //공에 힘 주기
                            Ball.GetComponent<Rigidbody>().AddForce(force);

                            //던져졌다는 것을 의미
                            isThrown = true;

                            /*                                


                                //handPos - prevHandPos = 거리 차이
                                //* 1000f = 공 떨어지는 상황 방지, 힘을 키워줌
                                Vector3 force = (handPos - prevHandPos) * 1000f;

                                //공에 힘을 줌 그러나 이렇게 작성했을 때 손이 어깨 위에있다면 힘이 계속 들어가게 됨
                                //손이어깨 위로 올라간 그 한순간만 힘을 주도록 수정해야함
                                overlayObject.GetComponent<Rigidbody>().AddForce(force);

                                //던져졌다는 것을 의미
                                isThrown = true;
*/
                        }

                    }
                    //손의 위치가 어깨보다 내려갔을 때
                    else
                    {
                        //공의 색을 green으로 유지함
                        Ball.GetComponent<Renderer>().material.color = Color.green;

                        //-10f로 했을 때 손에 공이 잘 붙지 않아 값을 조정함
                        if (Ball.transform.position.y < 0.1f) //공 원위치 이동
                        {
                            //중력 잡기
                            //공 중력 없애기
                            Ball.GetComponent<Rigidbody>().useGravity = false;

                            //위치 잡기
                            //공이 손에 다시 붙도록
                            isThrown = false;
                            Ball.transform.position = new Vector3(0, 1, 2);

                            //속도 잡기
                            //다시 던질 때를 위해 기본 속도값 리셋
                            Ball.GetComponent<Rigidbody>().velocity = Vector3.zero;

                        }

                    }

                    //과거값 = 현재값
                    PrevHandShoulderGap = handShoulderGap;
                    prevHandPos = handPos;

                }
                else
                {
                    // make the overlay object invisible
                }

            }
        }
    }
}