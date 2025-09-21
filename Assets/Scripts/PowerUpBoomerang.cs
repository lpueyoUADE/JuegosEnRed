using Photon.Pun;
using UnityEngine;

public class PowerUpBoomerang : MonoBehaviourPun
{
    [SerializeField] private GameObject boomerangToInstantiate;

    [Header("Floating Effect")]
    [SerializeField] private float amplitude;
    [SerializeField] private float speed; 

    private Vector3 startPosition;


    void Awake()
    {
        photonView.RPC("PlaySound", RpcTarget.All, SoundEffect.Spawn);
        InitializeStartPosition();
    }

    void Update()
    {
        MoveUpAndDown();
        RotateLeftAndRight();
    }

    void OnTriggerEnter2D(Collider2D collder)
    {
        OnTriggerEnterWithPlayer(collder);
    }


    private void InitializeStartPosition()
    {
        startPosition = transform.position;
    }

    private void MoveUpAndDown()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * speed) * amplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

    private void RotateLeftAndRight()
    {
        float scaleX = Mathf.Sin(Time.time * speed) * 0.5f;
        transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
    }

    private void OnTriggerEnterWithPlayer(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            PlayerController player = collider.GetComponent<PlayerController>();
            if (!player.photonView.IsMine) return;

            GameObject boomerangGO = PhotonNetwork.Instantiate("Prefabs/Boomerangs/" + boomerangToInstantiate.name, player.PlayerModel.BoomerangHandPosition.position, Quaternion.identity);
            BoomerangController newBoomerang = boomerangGO.GetComponent<BoomerangController>();
            newBoomerang.BoomerangModel.photonView.RPC("Initialize", RpcTarget.All, player.photonView.OwnerActorNr);
            player.PlayerModel.ChangeCurrentBoomerangToNewOne(newBoomerang);
            AudioManager.Instance.PlaySound(SoundEffect.PickUp);
            photonView.RPC("DisablePowerUp", RpcTarget.All);
        }
    }


    [PunRPC]
    private void PlaySound(SoundEffect soundType)
    {
        AudioManager.Instance.PlaySound(soundType);
    }

    [PunRPC]
    private void DisablePowerUp()
    {
        gameObject.SetActive(false);
    }
}
