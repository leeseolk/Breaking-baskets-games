using UnityEngine;
using System.Collections;
using System;
using com.rfilkov.kinect;


namespace com.rfilkov.components
{
    /// <summary>
    /// JointOverlayer overlays the given body joint with the given virtual object.
    /// </summary>
    public class ThrowBall2 : MonoBehaviour
    {
        [Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
        public int playerIndex = 0;

        [Tooltip("Kinect joint that is going to be overlayed. 오른손 관절")]
        public KinectInterop.JointType trackedJoint = KinectInterop.JointType.HandRight;

        [Tooltip("Kinect joint that is going to be overlayed. 오른쪽 어깨 관절")]
        public KinectInterop.JointType trackedJoint2 = KinectInterop.JointType.ShoulderRight;

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

        float handShoulderGap;
        float prevHandShoulderGap;
        bool isThrown = false;
        Vector3 handPos;
        Vector3 prevHandPos;

        public void Start()
        {
            // get reference to KM
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

        void Update() // fixedUpdate와의 차이 공부. 
        {
            if (kinectManager && kinectManager.IsInitialized())
            {
                if (foregroundCamera) // = Main Camera
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
                ulong userId = kinectManager.GetUserIdByIndex(playerIndex);

                 int iJointIndex = (int)trackedJoint;
                int iJointIndex2 = (int)trackedJoint2;
                if (kinectManager.IsJointTracked(userId, iJointIndex) && kinectManager.IsJointTracked(userId, iJointIndex2))
                {
                    // int a = 1 > 0 ? 2 : -1;
                    Vector3 posJoint = foregroundCamera ? // posJoint 오른손의 위치
                        kinectManager.GetJointPosColorOverlay(userId, iJointIndex, sensorIndex, foregroundCamera, backgroundRect) :
                        sensorTransform ? kinectManager.GetJointKinectPosition(userId, iJointIndex, true) : 
                        kinectManager.GetJointPosition(userId, iJointIndex);

                    if (sensorTransform)
                    {
                        posJoint = sensorTransform.TransformPoint(posJoint);
                    }

                    if (posJoint != Vector3.zero && overlayObject) // 실패(Vector3.zero)하지 않았을 경우
                    {
                        if (horizontalOffset != 0f) // offset ex옵셋인쇄.. vertical-y forward-z
                        {
                            // add the horizontal offset
                            Vector3 dirShoulders = kinectManager.GetJointPosition(userId, KinectInterop.JointType.ShoulderRight) -
                                kinectManager.GetJointPosition(userId, KinectInterop.JointType.ShoulderLeft);
                            //Vector3 dirHorizOfs = overlayObject.InverseTransformDirection(new Vector3(horizontalOffset, 0, 0));
                            posJoint += dirShoulders.normalized * horizontalOffset;  // dirHorizOfs;
                        }

                        if (verticalOffset != 0f)
                        {
                            // add the vertical offset
                            Vector3 dirSpine = kinectManager.GetJointPosition(userId, KinectInterop.JointType.Neck) -
                                kinectManager.GetJointPosition(userId, KinectInterop.JointType.Pelvis);
                            //Vector3 dirVertOfs = overlayObject.InverseTransformDirection(new Vector3(0, verticalOffset, 0));
                            posJoint += dirSpine.normalized * verticalOffset;  // dirVertOfs;
                        }

                        if (forwardOffset != 0f)
                        {
                            // add the forward offset
                            Vector3 dirShoulders = (kinectManager.GetJointPosition(userId, KinectInterop.JointType.ShoulderRight) -
                                kinectManager.GetJointPosition(userId, KinectInterop.JointType.ShoulderLeft)).normalized;
                            Vector3 dirSpine = (kinectManager.GetJointPosition(userId, KinectInterop.JointType.Neck) -
                                kinectManager.GetJointPosition(userId, KinectInterop.JointType.Pelvis)).normalized;
                            Vector3 dirForward = Vector3.Cross(dirShoulders, dirSpine).normalized;
                            //Vector3 dirFwdOfs = overlayObject.InverseTransformDirection(new Vector3(0, 0, forwardOffset));
                            posJoint += dirForward * forwardOffset;  // dirFwdOfs;
                        }
                        if(!isThrown)
                            overlayObject.position = posJoint;
              
                        if(rotateObject) // 회전값 적용?
                        {
                            Quaternion rotJoint = kinectManager.GetJointOrientation(userId, iJointIndex, !objMirrored);
                            rotJoint = initialRotation * rotJoint;

                            overlayObject.rotation = rotationSmoothFactor > 0f ?
                                Quaternion.Slerp(overlayObject.rotation, rotJoint, rotationSmoothFactor * Time.deltaTime) : rotJoint;
                        }

                        Vector3 shoulderPos = kinectManager.GetJointPosition(userId, KinectInterop.JointType.ShoulderRight);
                        Vector3 handPos = kinectManager.GetJointPosition(userId, KinectInterop.JointType.HandRight);
                        handShoulderGap = shoulderPos.z - handPos.z; // float handShoulderGap - class 변수로, 글로벌 변수로 지정해야겠군..
                        if (handShoulderGap > 0)
                        {
                            overlayObject.GetComponent<Renderer>().material.color = Color.red;
                            if (prevHandShoulderGap < 0)
                            {
                                overlayObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                                Vector3 force = (handPos - prevHandPos) * 600f;
                                overlayObject.GetComponent<Rigidbody>().AddForce(force); // (force)대신 (0, 200f, 0)처럼 고정된 힘도 가능
                                isThrown = true;
                            }

                        }
                        else
                        {
                            overlayObject.GetComponent<Renderer>().material.color = Color.green;
                            if(overlayObject.position.z < -10f)
                            {
                                isThrown = false;
                            }

                        }

                        prevHandShoulderGap = handShoulderGap;
                        prevHandPos = handPos;
                    }
                }
                else
                {
                    // make the overlay object invisible
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
