using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


namespace WasaaMP {
    public class Navigation : MonoBehaviourPunCallbacks {
     
        #region Public Fields

        // to be able to manage the offset of the camera
        public Vector3 cameraPositionOffset = new Vector3 (0, 1.6f, 0) ;
        public Quaternion cameraOrientationOffset = new Quaternion () ;
 
        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;

        #endregion
        void Awake () {
            // #Important
            // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
            if (photonView.IsMine) {
                LocalPlayerInstance = this.gameObject;
            }
            // #Critical
            // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
            //DontDestroyOnLoad (this.gameObject) ;
        }

        void Start () {
            if (photonView.IsMine) {
                // attach the camera to the navigation rig
                Camera theCamera = (Camera)GameObject.FindObjectOfType (typeof(Camera)) ;
                Transform cameraTransform = theCamera.transform ;
                cameraTransform.SetParent (transform) ;
                cameraTransform.localPosition = cameraPositionOffset ;
                cameraTransform.localRotation = cameraOrientationOffset ;
            }
        }

        void Update () {
            if (photonView.IsMine) {
                var x = Input.
                
                GetAxis ("Horizontal") * Time.deltaTime * 150.0f ;
                var z = Input.GetAxis ("Vertical") * Time.deltaTime * 3.0f ;
                transform.Rotate (0, x, 0) ;
                transform.Translate (0, 0, z) ;      
            }
        }

    }

}