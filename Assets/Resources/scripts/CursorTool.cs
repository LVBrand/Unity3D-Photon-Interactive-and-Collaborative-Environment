using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


namespace WasaaMP {
	public class CursorTool : MonoBehaviourPun {
		private bool caught ;
		public bool catching;
		public CollaborativeObject target ;
		public int strength = 50;

		void Start () {
			caught = false ;
			catching = false;
		}
		
		public void Catch () 
		{
			print ("Catch ?");
			if (target != null) 
			{
				print ("Catch :");
				if ((! caught) && (transform != target.transform))
				{
					target.Catch(this);
					catching = true;
					caught = true;
					print ("Catch !");
				}
				else
				{
					catching = false;
					print ("Catch failed.");
				}
			}
		}

		public void Release () {
			if (caught) {
				print ("Release !") ;
				target.Release(this);
				caught = false ;
				catching = false;
			}
		}

		void OnTriggerEnter (Collider other) {
			if (! caught) {
				print (name + " : CursorTool OnTriggerEnter") ;
				target = other.gameObject.GetComponent<CollaborativeObject> () ;
				if (target != null) {
					target.photonView.RPC ("ShowCatchable", RpcTarget.All) ;
					PhotonNetwork.SendAllOutgoingCommands () ;
				}
			}
		}

		void OnTriggerExit (Collider other) {
			if (! caught) {
				print (name + " : CursorTool OnTriggerExit") ;
				if (target != null) {
					target.photonView.RPC ("HideCatchable", RpcTarget.All) ;
					PhotonNetwork.SendAllOutgoingCommands () ;
					target = null ;
				}
			}
		}

	}

}