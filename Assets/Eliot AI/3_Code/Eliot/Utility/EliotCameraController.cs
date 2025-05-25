using UnityEngine;

namespace Eliot.Utility
{
    /// <summary>
    /// Gives control over a camera's position and rotation in space.
    /// </summary>
    public class EliotCameraController : MonoBehaviour
    {
        public const string VerticalAxisMouse = "Mouse Y";
        public const string HorizontalAxisMouse = "Mouse X";

        /// <summary>
        /// Target to follow and look at.
        /// </summary>
        public Transform target;
        
        /// <summary>
        /// Rotation pivot.
        /// </summary>
        public Transform pivot;
        
        /// <summary>
        /// If true, pivot will be created and initialized automatically.
        /// </summary>
        public bool autoPivot = true;

        /// <summary>
        /// Position offset of the pivot.
        /// </summary>
        public Vector3 pivotOffset;
        
        /// <summary>
        /// Position offset of the camera.
        /// </summary>
        public Vector3 cameraOffset;

        /// <summary>
        /// If true, pivotOffset and cameraOffset will be initialized automatically from the initial position set in the Editor.
        /// </summary>
        public bool initializeFromCamera = true;
        
        /// <summary>
        /// If true, the camera will follow the target smoothly.
        /// </summary>
        public bool smoothFollow = true;
        
        /// <summary>
        /// Smoothness of the camera's motion while following the target.
        /// </summary>
        public float followSpeed = 1.0f;
        
        /// <summary>
        /// Distance to the target.
        /// </summary>
        public float distance = 15.0f;

        /// <summary>
        /// If true, the camera can change its distance to the target based on player input.
        /// </summary>
        public bool canZoom = false;
        
        /// <summary>
        /// Minimum distance to which camera can zoom in.
        /// </summary>
        public float minDistance = 5f;
        
        /// <summary>
        /// Maximum distance to which camera can zoom out.
        /// </summary>
        public float maxDistance = 35f;
        
        /// <summary>
        /// Speed of changing the distance while zooming.
        /// </summary>
        public float zoomSpeed = 1f;

        #region Input Info

        public float mouseHorizontal = 0f;
        public float mouseVertical = 0f;

        #endregion
        
        /// <summary>
        /// Speed of horizontal rotation.
        /// </summary>
        public float horizontalSpeed = 0f;
        
        /// <summary>
        /// Speed of vertical rotation.
        /// </summary>
        public float verticalSpeed = 0f;


        /// <summary>
        /// Create and initialize pivot.
        /// </summary>
        public void InitPivot()
        {
            var pivotGo = new GameObject("_cameraPivot");
            pivot = pivotGo.transform;
            pivot.position = target.position;
            if (initializeFromCamera)
            {
                pivot.rotation = transform.rotation;
            }
            else pivot.rotation = target.rotation;
            transform.parent = pivot;
        }

        /// <summary>
        /// Updates the position of the camera.
        /// </summary>
        /// <param name="playerTransform"></param>
        /// <param name="cameraTransform"></param>
        public void UpdateCameraPosition(Transform playerTransform, Transform cameraTransform)
        {
            if (!playerTransform) return;
            if (!smoothFollow)
            {
                cameraTransform.position = playerTransform.position + pivotOffset;
            }
            else
            {
                cameraTransform.position = Vector3.Lerp(cameraTransform.position, playerTransform.position+ pivotOffset, followSpeed * Time.deltaTime) ;
            }
        }

        /// <summary>
        /// Updates the rotation of the pivot.
        /// </summary>
        public void UpdatePivotRotation()
        {
            pivot.eulerAngles = new Vector3(-mouseVertical, mouseHorizontal, 0.0f);
        }

        /// <summary>
        /// Initialize the component.
        /// </summary>
        private void Start()
        {
            if(autoPivot) InitPivot();
            if (initializeFromCamera)
            {
                distance = Vector3.Distance(pivot.position, transform.position);
                if (minDistance < distance) minDistance = distance;
            }
        }

        /// <summary>
        /// Called every frame.
        /// </summary>
        private void Update()
        {
            mouseHorizontal += Input.GetAxis(HorizontalAxisMouse) * horizontalSpeed;
            mouseVertical += Input.GetAxis(VerticalAxisMouse) * verticalSpeed;

            UpdateCameraPosition(target, pivot);
            if(horizontalSpeed > 0 || verticalSpeed > 0)
                UpdatePivotRotation();
            
            transform.LookAt(pivot);
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(0, 0, -distance)+ cameraOffset, zoomSpeed * Time.deltaTime) ;
            if(canZoom)
             distance = Mathf.Clamp(distance - Input.mouseScrollDelta.y * zoomSpeed, minDistance, maxDistance);
        }
    }
}

