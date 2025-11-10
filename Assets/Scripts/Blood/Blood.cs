using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blood : MonoBehaviourPun
{
    private ParticleSystem ps;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (ps && !ps.IsAlive())
        {
            // Solo el dueño (instanciador) puede destruirlo en red
            if (photonView.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
