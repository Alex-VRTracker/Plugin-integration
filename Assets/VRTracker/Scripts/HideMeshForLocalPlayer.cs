using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/* VR Tracker
 * Le but de ce script est descativer le mesh du controller si on est le local player
 * afin d'utiliser le même mesh dans les controlleurs de VRTK
 * On veut garder le mesh activé pour les autres users lors du networking multiplayer.
 */

public class HideMeshForLocalPlayer : MonoBehaviour
{

    public GameObject controllerMesh;

    // Use this for initialization
    void Start()
    {
        if (controllerMesh)
        {
            NetworkIdentity netId = transform.GetComponentInParent<NetworkIdentity>();
            if ((netId != null && netId.isLocalPlayer) || netId == null)
            {
                controllerMesh.SetActive(false);
            }
            else
            {
                controllerMesh.SetActive(true);
            }
        }
    }
}
