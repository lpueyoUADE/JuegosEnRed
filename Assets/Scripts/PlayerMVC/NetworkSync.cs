using UnityEngine;
using Photon.Pun;

public class NetworkSync : MonoBehaviourPun, IPunObservable
{
    private Rigidbody2D rb;

    private Vector2 networkPosition;
    private Vector2 networkRotation;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        /*if (!photonView.IsMine)
        {
            rb.position = Vector2.MoveTowards(rb.position, networkPosition, Time.fixedDeltaTime);
        }*/
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        /*if (stream.IsWriting)
        {
            stream.SendNext(rb.position);
            stream.SendNext(rb.rotation);
            stream.SendNext(rb.velocity);
        }

        else
        {
            networkPosition = (Vector2)stream.ReceiveNext();
            networkRotation = (Vector2)stream.ReceiveNext();
            rb.velocity = (Vector2)stream.ReceiveNext();

            float lag = Mathf.Abs((float) (PhotonNetwork.Time - info.SentServerTime));
            networkPosition += rb.velocity * lag;
        }*/
    }
}
