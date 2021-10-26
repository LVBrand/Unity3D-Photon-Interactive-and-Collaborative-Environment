using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace WasaaMP {
	public class CursorDriver : MonoBehaviourPun {
		private bool active ;
		private CursorTool cursor ;
		private Camera theCamera ;

		void Start () {
			if (photonView.IsMine || ! PhotonNetwork.IsConnected) {
				// get the camera
				theCamera = (Camera)GameObject.FindObjectOfType (typeof(Camera)) ;
				active = false ;
				cursor = GetComponent<CursorTool> () ;
			}
		}
		
		void Update () {
			if (photonView.IsMine  || ! PhotonNetwork.IsConnected) {
				if (Input.GetButtonDown ("Fire1"))  {
					cursor.Catch () ;
				}
				if (Input.GetButtonUp ("Fire1")) {
					cursor.Release () ;
				}
				//if (Input.GetKeyDown (KeyCode.C)) {
				//	cursor.CreateInteractiveCube () ;
				//}
				if (Input.mousePosition != null) {
					Vector3 point = new Vector3 () ;
					Vector3 mousePos = Input.mousePosition ;
					float deltaZ = Input.mouseScrollDelta.y / 10.0f ;
					cursor.transform.Translate (0, 0, deltaZ) ;
					// Note that the y position from Event should be inverted, but maybe it is not true any longer...
					// mousePos.y = myCamera.pixelHeight - mousePos.y ;
					point = theCamera.ScreenToWorldPoint (new Vector3 (mousePos.x, mousePos.y, cursor.transform.localPosition.z)) ;
					cursor.transform.position = point ;
				}

				
			}
		}

	}

}