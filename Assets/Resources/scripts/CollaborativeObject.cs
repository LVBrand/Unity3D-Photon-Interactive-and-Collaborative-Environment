using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

namespace WasaaMP {
    public class CollaborativeObject : MonoBehaviourPun
    {  
        public CursorTool cursor = null;
        private int weight = 50;

        private bool catchable = false;
        private bool caught = false;

        private Color colorBeforeHighlight ;
        private Color oldColor ;
        private float oldAlpha ;

        public Dictionary<int, Vector3> positions;
        public Dictionary<int, int> strengths;

        
        void Start()
        {
            // On met dans un dictionnaire la position de chaque curseur des clients distants
            positions = new Dictionary<int, Vector3>();

            // On met dans un dictionnaire la force de chaque curseur des clients distants
            strengths = new Dictionary<int, int>();
        }

        void Update()
        {
            // Si on est le MasterClient
            if (photonView.IsMine || !PhotonNetwork.IsConnected)
            {
                // On compte le nombre de curseurs qui tentent de bouger le cube
                var count = positions.Count + (cursor?1:0);

                // On compte la force totale appliquée au cube
                var globalStrength = strengths.Values.Aggregate(cursor? cursor.strength : 0, (sum, item)=>sum+item);

                // Check si on ne fait pas une div par 0 && si l'ensemble des curseurs sont assez forts pour porter le cube
                if (count != 0 && globalStrength >= weight)
                {
                    // On calcule le barycentre des curseurs pour modifier la position du cube
                    var barycentre = positions.Values.Aggregate(cursor? cursor.transform.position : Vector3.zero, (sum, item)=>sum+item);
                    barycentre /= count;
                    transform.position = barycentre;
                }
            }


            // Si on est un client distant
            else if (cursor)
            {
                // On envoie notre position au MasterClient
                photonView.RPC("UpdatePosition", RpcTarget.MasterClient, cursor.transform.position);
            }
        }

        public void Catch(CursorTool cursor) {
            if (this.cursor == null)
            {
                this.cursor = cursor;
            }

        }

        public void Release(CursorTool cursor) {
            if (this.cursor == cursor)
            {
                this.cursor = null;
                // Si on est pas le master client, on utilise UpdateRelease pour retirer notre curseur du dict positions
                if (!(photonView.IsMine || !PhotonNetwork.IsConnected))
                {
                    photonView.RPC("UpdateRelease", RpcTarget.MasterClient);
                }

            }

        }

        [PunRPC] private void UpdatePosition(Vector3 pos, PhotonMessageInfo info) {
            // Stock la position du remote client dans le dictionnaire positions
            positions[info.Sender.ActorNumber] = pos;
            
        }

        [PunRPC] private void UpdateRelease(PhotonMessageInfo info) {
            positions.Remove(info.Sender.ActorNumber);
        }


        [PunRPC] public void ShowCaught () {
            if (! caught) {
                var rb = GetComponent<Rigidbody> () ;
                rb.isKinematic = true ;
                Renderer renderer = GetComponentInChildren <Renderer> () ;
                oldColor = renderer.material.color ;
                renderer.material.color = Color.yellow ;
                caught = true ;
            }
        }

        [PunRPC] public void ShowReleased () {
            if (caught) {
                var rb = GetComponent<Rigidbody> () ;
                rb.isKinematic = false ;
                Renderer renderer = GetComponentInChildren <Renderer> () ;
                renderer.material.color = oldColor ;
                caught = false ;
            }
        }

        [PunRPC] public void ShowCatchable () {
            if (! caught) {
                if (! catchable) {
                    Renderer renderer = GetComponentInChildren <Renderer> () ;
                    oldAlpha = renderer.material.color.a ;
                    colorBeforeHighlight = renderer.material.color ;
                    //Color c = renderer.material.color ;
                    Color c = Color.cyan ;
                    renderer.material.color = new Color (c.r, c.g, c.b, 0.5f) ;
                    catchable = true ;
                }
            }
        }
        
        [PunRPC] public void HideCatchable () {
            if (! caught) {
                if (catchable) {
                    Renderer renderer = GetComponentInChildren <Renderer> () ;
                    //Color c = renderer.material.color ;
                    Color c = colorBeforeHighlight ;
                    renderer.material.color = new Color (c.r, c.g, c.b, oldAlpha) ;
                    catchable = false ;
                }
            }
        }
        
    }
}

