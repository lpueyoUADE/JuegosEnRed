/*using UnityEngine;
using Photon.Pun;

public class PlayerNetworkSync : MonoBehaviourPun, IPunObservable
{
    private Rigidbody2D rb;
    private Vector2 networkPosition;
    private Vector2 velocity;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            // Interpolamos la posición para suavizar el movimiento
            transform.position = Vector2.Lerp(transform.position, networkPosition, Time.deltaTime * 10f);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Yo soy dueño ? envío mi posición y velocidad
            stream.SendNext(transform.position);
            stream.SendNext(rb.velocity);
        }
        else
        {
            // Soy otro cliente ? recibo la posición y velocidad del dueño
            networkPosition = (Vector2)stream.ReceiveNext();
            velocity = (Vector2)stream.ReceiveNext();
        }
    }
}*/
