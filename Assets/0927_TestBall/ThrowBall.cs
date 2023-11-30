using UnityEngine;
using System.Collections;
using System;
using com.rfilkov.kinect;


namespace com.rfilkov.components
{
    /// <summary>
    /// JointOverlayer overlays the given body joint with the given virtual object.
    /// </summary>
    public class ThrowBall : MonoBehaviour
    {
        [Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
        public int playerIndex = 0;

        //오른손 관절
        [Tooltip("Kinect joint that is going to be overlayed.")]
        public KinectInterop.JointType trackedJoint = KinectInterop.JointType.HandRight;

        //오른쪽 어깨 관절
        [Tooltip("Kinect joint that is going to be overlayed.")]
        public KinectInterop.JointType trackedJoint2 = KinectInterop.JointType.HandRight;

        
        [Tooltip("Game object used to overlay the joint.")]
        public Transform overlayObject;

        [Tooltip("Whether to rotate the overlay object, according to the joint rotation.")]
        public bool rotateObject = true;

        [Tooltip("Smooth factor used for joint rotation.")]
        [Range(0f, 10f)]
        public float rotationSmoothFactor = 10f;

        [Tooltip("Depth sensor index used for color camera overlay - 0 is the 1st one, 1 - the 2nd one, etc.")]
        public int sensorIndex = 0;

        [Tooltip("Camera that will be used to overlay the 3D-objects over the background.")]
        public Camera foregroundCamera;

        [Tooltip("Scene object that will be used to represent the sensor's position and rotation in the scene.")]
        public Transform sensorTransform;

        [Tooltip("Horizontal offset in the object's position with respect to the object's x-axis.")]
        [Range(-0.5f, 0.5f)]
        public float horizontalOffset = 0f;

        [Tooltip("Vertical offset in the object's position with respect to the object's y-axis.")]
        [Range(-0.5f, 0.5f)]
        public float verticalOffset = 0f;

        [Tooltip("Forward offset in the object's position with respect to the object's z-axis.")]
        [Range(-0.5f, 0.5f)]
        public float forwardOffset = 0f;

        //public UnityEngine.UI.Text debugText;

        [NonSerialized]
        public Quaternion initialRotation = Quaternion.identity;
        private bool objMirrored = false;

        // reference to KM
        private KinectManager kinectManager = null;

        // background rectangle
        private Rect backgroundRect = Rect.zero;

        float handShoulderGop;
        float prevHandShoulderGop;
        //공이 던져졌는가(변수)
        //스위치
        bool isThrown = false;
        Vector3 handPos;
        Vector3 prevHandPos;

        public void Start()
        {
            // get reference to KM
            //키넥트 매니저가 없으면 만들어준다.
            //Instance값이 키넥트매니저안에 만들어진다.
            kinectManager = KinectManager.Instance;

            //if (!foregroundCamera)
            //{
            //    // by default - the main camera
            //    foregroundCamera = Camera.main;
            //}

            if(overlayObject == null)
            {
                // by default - the current object
                overlayObject = transform;
            }
            else
            {
                transform.position = Vector3.zero;
                transform.rotation = Quaternion.identity;
            }

            if (rotateObject && overlayObject)
            {
                // always mirrored
                initialRotation = overlayObject.rotation; // Quaternion.Euler(new Vector3(0f, 180f, 0f));

                Vector3 vForward = foregroundCamera ? foregroundCamera.transform.forward : Vector3.forward;
                objMirrored = (Vector3.Dot(overlayObject.forward, vForward) < 0);

                overlayObject.rotation = Quaternion.identity;
            }
        }

        //프레임마다 반복진행
        void Update()
        {
            //키넥트가 작동되기위한 인스턴스가 올바르게 만들어졌는지 확인
            if (kinectManager && kinectManager.IsInitialized())
            {
                //foregroundCamera > 유니티에 기본으로 만들어진 메인카메라
                //메인카메라가 지정이 되어있는가 (메인카메라의 기능이 절대적)
                if (foregroundCamera)
                {
                    // get the background rectangle (use the portrait background, if available)
                    backgroundRect = foregroundCamera.pixelRect;
                    PortraitBackground portraitBack = PortraitBackground.Instance;

                    if (portraitBack && portraitBack.enabled)
                    {
                        backgroundRect = portraitBack.GetBackgroundRect();
                    }
                }

                // overlay the joint
                //최초 사용자의 아이디값
                ulong userId = kinectManager.GetUserIdByIndex(playerIndex);

                //손
                int iJointIndex = (int)trackedJoint;
                //관절마다 아이디 값 
                //IsJointTracked > 관절들이 온전하게 프레임에 들어왔는지

                //어깨
                int iJointIndex2 = (int)trackedJoint2;
                //손과 어깨 모두 추적
                if (kinectManager.IsJointTracked(userId, iJointIndex) && kinectManager.IsJointTracked(userId, iJointIndex))
                {
                    
                    //if else문을 한줄로 쓴것
                    //이 조건문은 변수안에 함수를 넣을 때 쓴다.
                    //true면 첫번째 값 false면 두번 째 값
                    // int a = 1 > 0 ? 2 : -1;

                    //?는 무엇인가.
                    //이중 조건문을 사용하고 있다.
                    //3가지의 조건을 만족
                    Vector3 posJoint = foregroundCamera ?
                        //GetJointPosColorOverlay로 관절의 위치값 확인
                        //(userId, iJointIndex, sensorIndex, foregroundCamera, backgroundRect)필요정보
                        //유저정보, 관절, 키넥트의 인덱스 값, 카메라, backgroundRect는 카메라에서 나온정보
                        //backgroundRect는 게임화면의 정보
                        //참이면 이 조건문의 값을 받아오고
                        //GetJointPosColorOverlay은 화면상에서의 위치(실제위치x)
                        kinectManager.GetJointPosColorOverlay(userId, iJointIndex, sensorIndex, foregroundCamera, backgroundRect) :
                        //증강현실 상황에서 내 위치 정보를 계산
                        //위의 5가지의 정보를 넣으면 좌표정보를 가져온다.
                        //좌표정보는 posJoint에 저장
                        //거짓이면 이 조건문의 값을 받아온다.
                        sensorTransform ? kinectManager.GetJointKinectPosition(userId, iJointIndex, true) : 
                        kinectManager.GetJointPosition(userId, iJointIndex);

                    //부수적인 기능
                    //sensorTransform은 키넥트의 각도와 물리적위치를 받고 값을 받아오는 것
                    //별로 쓸일 x
                    if (sensorTransform)
                    {
                        posJoint = sensorTransform.TransformPoint(posJoint);
                    }
                    //두가지 조건을 확인한다.
                    //실패하지 않았고, overlayObject가 존재하면
                    //overlayObject > 그린볼
                    if (posJoint != Vector3.zero && overlayObject)
                    {
                        //Offset > 원래 위치에서 벗어나는 것(여백)
                        //프로그램에서의 Offset은 벗어나는 정도 즉 떨어지는 정도의 값을 조절
                        //horizontalOffset 가로 방향
                        //0이 아닐경우 
                        if (horizontalOffset != 0f)
                        {
                            // add the horizontal offset
                            //Vector3에 뺄셈이 들어가는걸 확인할 수 있다.
                            //양쪽 어깨를 찾아서 원점을 찾는다.
                            //오른쪽 어깨 - 왼쪽 어깨 > 오른쪽 어깨 원점, 왼쪽 어깨 원점백터 (x축)
                            //사람을 기준으로 어디 서있는지 확인(카메라 앞에서있는 나의 어깨 방향): dirShoulders
                            //GetJointPosition함수 카메라를 원점으로 내 관절의 위치를 x,y,z값으로 확인한다.
                            //사람의 기준으로 따지고자 했을 때 x축 확인
                            //GetJointPosition(실제위치 물리세계에서의 위치)
                            Vector3 dirShoulders = kinectManager.GetJointPosition(userId, KinectInterop.JointType.ShoulderRight) -
                                kinectManager.GetJointPosition(userId, KinectInterop.JointType.ShoulderLeft);
                            //Vector3 dirHorizOfs = overlayObject.InverseTransformDirection(new Vector3(horizontalOffset, 0, 0));
                            //dirForward의 백터값을 구하고 옵셋값을 주고싶다.
                            //normalized 방향을 똑같이 만들고 길이를 1로 만든다.(백터를 노말라이징한다)
                            //horizontalOffset 실수값을 곱한다. = 즉 내가 원하는 길이의 값을 구할 수 있다.
                            // += > 누적덧셈
                            posJoint += dirShoulders.normalized * horizontalOffset;  // dirHorizOfs;
                        }

                        //verticalOffset 세로방향
                        //Pelvis에서 Neck으로 (y축 방향)
                        //
                        if (verticalOffset != 0f)
                        {
                            // add the vertical offset
                            //
                            Vector3 dirSpine = kinectManager.GetJointPosition(userId, KinectInterop.JointType.Neck) -
                                kinectManager.GetJointPosition(userId, KinectInterop.JointType.Pelvis);
                            //Vector3 dirVertOfs = overlayObject.InverseTransformDirection(new Vector3(0, verticalOffset, 0));
                            posJoint += dirSpine.normalized * verticalOffset;  // dirVertOfs;
                        }

                        //forwardOffset 앞뒤 z축
                        //관절점은 2D , 사람의 몸통은 평면 > 이것의 앞의 방향
                        //백터의 곱(X와 Y를 곱하면 Z방향이 나온다) >X,Y를 알면 Z를 알 수 있다.
                        //어깨방향(dirShoulders)과 척추방향(dirSpine)을 알면 앞(dirForward)를 알 수 있다.
                        if (forwardOffset != 0f)
                        {
                            // add the forward offset
                            Vector3 dirShoulders = (kinectManager.GetJointPosition(userId, KinectInterop.JointType.ShoulderRight) -
                                kinectManager.GetJointPosition(userId, KinectInterop.JointType.ShoulderLeft)).normalized;
                            Vector3 dirSpine = (kinectManager.GetJointPosition(userId, KinectInterop.JointType.Neck) -
                                kinectManager.GetJointPosition(userId, KinectInterop.JointType.Pelvis)).normalized;
                            //Cross> 백터 곱을 수행하는 함수
                            //어깨x척주 = z방향
                            Vector3 dirForward = Vector3.Cross(dirShoulders, dirSpine).normalized;
                            //Vector3 dirFwdOfs = overlayObject.InverseTransformDirection(new Vector3(0, 0, forwardOffset));
                            //내가 원하는 만큼 옵셋 값을 준다.
                            posJoint += dirForward * forwardOffset;  // dirFwdOfs;
                        }

                        //오버레이의 포지션에 조인트 값 적용
                        //isThrown값이 flase일때만 overlayObject.position = posJoint이 값을 실행하라
                        if (!isThrown)
                        overlayObject.position = posJoint;

                        //관절의 회전
                        if(rotateObject)
                        {
                            //GetJointOrientation을 이용해 관절의 회전값 적용
                            Quaternion rotJoint = kinectManager.GetJointOrientation(userId, iJointIndex, !objMirrored);
                            rotJoint = initialRotation * rotJoint;

                            overlayObject.rotation = rotationSmoothFactor > 0f ?
                                Quaternion.Slerp(overlayObject.rotation, rotJoint, rotationSmoothFactor * Time.deltaTime) : rotJoint;
                        }
                        Vector3 ShoulderPos = kinectManager.GetJointPosition(userId, KinectInterop.JointType.ShoulderRight);
                        Vector3 handPos = kinectManager.GetJointPosition(userId, KinectInterop.JointType.HandRight);
                        //손위치에서 어깨값을 뺏을 때 0보다 크면 손이 어깨위로 올라갔는가
                        float handShoulderGop = handPos.y - ShoulderPos.y;
                        if(handShoulderGop > 0)
                        {
                            overlayObject.GetComponent<Renderer>().material.color = Color.red;
                            if(prevHandShoulderGop < 0)
                            {
                                //공이 날라간 순간
                                //날라가기 직전 기본 속도값을 0로 만들었다.
                                overlayObject.GetComponent<Rigidbody>().velocity = Vector3.zero;  //던져지기 직전 0세팅
                                Vector3 force = (handPos - prevHandPos) * 600f;
                                overlayObject.GetComponent<Rigidbody>().AddForce(force);
                                isThrown = true;
                            }
                           
                        }
                        //그렇지 않으면 녹색으로 변해라
                        //손이 어깨보다 내려갔을 때
                        else
                        {
                            overlayObject.GetComponent<Renderer>().material.color = Color.green;
                            if(overlayObject.position.y  < -10f)
                                {
                                isThrown = false;
                                }
                        }
                        //과거값은 언제 저장해야하는가(그 과거 때 저장해야한다)>업데이트의 마지막
                        //현재 값이 과거값이 된다.
                        prevHandShoulderGop = handShoulderGop;
                        prevHandPos = handPos;
                    }
                }
                else
                {
                    // make the overlay object invisible
                    //관절 인식이 안되면
                    //녹색볼이 안보이게 처리
                    //오브젝트를 카메라 뒤로위치시키게 한다.
                    if (overlayObject && overlayObject.position.z > 0f)
                    {
                        Vector3 posJoint = overlayObject.position;
                        posJoint.z = -10f;
                        overlayObject.position = posJoint;
                    }
                }

            }
        }
    }
}
