using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.rfilkov.kinect;

namespace com.rfilkov.components
{
    /// <summary>
    /// SkeletonOverlayer overlays the the user's body joints and bones with spheres and lines.
    /// </summary>
    public class SkeletonOverlayer : MonoBehaviour
    {
        // ���� ���� �����ϸ� inspectorâ���� ���� ���� ���콺 ȣ���� �ÿ� ������ �����.
        // �ڸ�Ʈ�� �����ʾƵ� ������ �ǰ�, inspector���� ��Ÿ���� ����(��� inspectorâ�� ��Ÿ���� ������) 
        [Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
        public int playerIndex = 0;
        // ������ ��� ��
        
        [Tooltip("Game object used to overlay the joints.")]
        public GameObject jointPrefab;

        [Tooltip("Line object used to overlay the bones.")]
        public LineRenderer linePrefab;
        //public float smoothFactor = 10f;

        [Tooltip("Depth sensor index used for color frame overlay - 0 is the 1st one, 1 - the 2nd one, etc.")]
        public int sensorIndex = 0;

        // Main Camera ����ī�޶��� ��ġ ����
        [Tooltip("Camera that will be used to overlay the 3D-objects over the background.")]
        public Camera foregroundCamera;

        // ���� ī�޶�(Ű��Ʈ)�� ��ġ ����
        [Tooltip("Scene object that will be used to represent the sensor's position and rotation in the scene.")]
        public Transform sensorTransform;

        //public UnityEngine.UI.Text debugText;

        // list of filtered-out joints
        // array �迭�� �ϳ��ϳ� �� ����������� list�� ���� �����ϸ� �ڵ����� �迭�� ��. ���� ũ�� ���� ����
        // inspecter���� ������ ����(=private)
        protected List<int> filteredOutJoints = new List<int>();

        // joints & lines
        protected GameObject[] joints = null;
        protected LineRenderer[] lines = null;

        // initial body rotation
        protected Quaternion initialRotation = Quaternion.identity;

        // reference to KM
        // �ٽ�. ��ǥ���� class > KinectManager / namespace 4 ����(using com.rfilkov.kinect;)
        protected KinectManager kinectManager = null;

        // background rectangle
        protected Rect backgroundRect = Rect.zero;



        // unity script reference ����Ʈ���� Ŭ���� ������ ã�� ���ؼ�.. �� ã�ƺ��� �����ϵ��� �ϰڽ��ϴ� �� ��




        protected virtual void Start()
        {
            kinectManager = KinectManager.Instance;
            
            // if (kinectManager!=null && kinectManager.IsInitialized())
            if (kinectManager && kinectManager.IsInitialized())
            {
                int jointsCount = kinectManager.GetJointCount();

                if (jointPrefab)
                {
                    // array holding the skeleton joints
                    joints = new GameObject[jointsCount];

                    for (int i = 0; i < joints.Length; i++)
                    {
                        joints[i] = Instantiate(jointPrefab) as GameObject;
                        joints[i].transform.parent = transform;
                        joints[i].name = ((KinectInterop.JointType)i).ToString();
                        joints[i].SetActive(false);
                    }
                }

                // array holding the skeleton lines
                lines = new LineRenderer[jointsCount];
            }

            // always mirrored
            initialRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));

            //if (!foregroundCamera)
            //{
            //    // by default - the main camera
            //    foregroundCamera = Camera.main;
            //}
        }

        protected virtual void Update()
        {
            if (kinectManager && kinectManager.IsInitialized())
            {
                if(foregroundCamera)
                {
                    // get the background rectangle (use the portrait background, if available)
                    backgroundRect = foregroundCamera.pixelRect;
                    PortraitBackground portraitBack = PortraitBackground.Instance;

                    if (portraitBack && portraitBack.enabled)
                    {
                        backgroundRect = portraitBack.GetBackgroundRect();
                    }
                }

                // overlay all joints in the skeleton
                if (kinectManager.IsUserDetected(playerIndex))
                {
                    ulong userId = kinectManager.GetUserIdByIndex(playerIndex);
                    int jointsCount = kinectManager.GetJointCount();

                    //Debug.Log("Displaying user " + playerIndex + ", ID: " + userId + 
                    //    ", body: " + kinectManager.GetBodyIndexByUserId(userId) + ", pos: " + kinectManager.GetJointKinectPosition(userId, 0));

                    for (int i = 0; i < jointsCount; i++)
                    {
                        int joint = i;
                        // ������ ��. �����ڰ� ���� ����(?)
                        if (filteredOutJoints.Contains(joint))
                        {
                            continue;
                        }

                        if (kinectManager.GetJointTrackingState(userId, joint) >= KinectInterop.TrackingState.Tracked)
                        {
                            Vector3 posJoint = GetJointPosition(userId, joint);
                            //Debug.Log("U " + userId + " " + (KinectInterop.JointType)joint + " - pos: " + posJoint);

                            if (sensorTransform)
                            {
                                posJoint = sensorTransform.TransformPoint(posJoint);
                            }

                            if (joints != null)
                            {
                                // overlay the joint
                                if (posJoint != Vector3.zero)
                                {
                                    joints[i].SetActive(true);
                                    joints[i].transform.position = posJoint;

                                    Quaternion rotJoint = kinectManager.GetJointOrientation(userId, joint, false);
                                    rotJoint = initialRotation * rotJoint;
                                    joints[i].transform.rotation = rotJoint;

                                    //if(i == (int)KinectInterop.JointType.WristLeft)
                                    //{
                                    //    Debug.Log(string.Format("USO {0:F3} {1} user: {2}, state: {3}\npos: {4}, rot: {5}", Time.time, (KinectInterop.JointType)i,
                                    //        userId, kinectManager.GetJointTrackingState(userId, joint),
                                    //        kinectManager.GetJointPosition(userId, joint), kinectManager.GetJointOrientation(userId, joint, false).eulerAngles));
                                    //}
                                }
                                else
                                {
                                    joints[i].SetActive(false);
                                }
                            }

                            if (lines[i] == null && linePrefab != null)
                            {
                                lines[i] = Instantiate(linePrefab) as LineRenderer;
                                lines[i].transform.parent = transform;
                                lines[i].gameObject.SetActive(false);
                            }

                            if (lines[i] != null)
                            {
                                // overlay the line to the parent joint
                                int jointParent = (int)kinectManager.GetParentJoint((KinectInterop.JointType)joint);
                                Vector3 posParent = GetJointPosition(userId, jointParent);

                                if (sensorTransform)
                                {
                                    posParent = sensorTransform.TransformPoint(posParent);
                                }

                                if (posJoint != Vector3.zero && posParent != Vector3.zero &&
                                    kinectManager.GetJointTrackingState(userId, jointParent) >= KinectInterop.TrackingState.Tracked)
                                {
                                    lines[i].gameObject.SetActive(true);

                                    //lines[i].SetVertexCount(2);
                                    lines[i].SetPosition(0, posParent);
                                    lines[i].SetPosition(1, posJoint);
                                }
                                else
                                {
                                    lines[i].gameObject.SetActive(false);
                                }
                            }
                        }
                        else
                        {
                            if (joints[i] != null)
                            {
                                joints[i].SetActive(false);
                            }

                            if (lines[i] != null)
                            {
                                lines[i].gameObject.SetActive(false);
                            }
                        }
                    }

                }
                else
                {
                    // disable the skeleton
                    int jointsCount = kinectManager.GetJointCount();

                    for (int i = 0; i < jointsCount; i++)
                    {
                        if (joints[i] != null)
                        {
                            joints[i].SetActive(false);
                        }

                        if (lines[i] != null)
                        {
                            lines[i].gameObject.SetActive(false);
                        }
                    }
                }
            }
        }

        // returns body joint position
        private Vector3 GetJointPosition(ulong userId, int joint)
        {
            Vector3 posJoint = Vector3.zero;

            if (foregroundCamera)
            {
                // GetJointPosColorOverlay�� ���� �߿��� �Լ�!!!!! userId���� �˾ƾ��ϰ�, ���� ��ġã�� �Լ�
                posJoint = kinectManager.GetJointPosColorOverlay(userId, joint, sensorIndex, foregroundCamera, backgroundRect);
            }
            else if (sensorTransform)
            {
                posJoint = kinectManager.GetJointKinectPosition(userId, joint, true);
            }
            else
            {
                posJoint = kinectManager.GetJointPosition(userId, joint);
            }

            return posJoint;
        }

    }
}
