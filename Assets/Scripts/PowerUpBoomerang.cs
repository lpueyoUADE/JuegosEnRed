using Photon.Pun;
using UnityEngine;

public class PowerUpBoomerang : MonoBehaviourPun
{
    [SerializeField] private GameObject boomerangToInstantiate;

    private Vector3 startPosition;
    
    [Header("Floating Effect")]
    [SerializeField] private float amplitude;
    [SerializeField] private float speed;

    private float timeAfterSpawn = 0f;

    private string powerUpId;


    void Awake()
    {
        photonView.RPC("PlaySound", RpcTarget.All, SoundEffect.Spawn);
        InitializeStartPosition();
    }

    void Update()
    {
        MoveUpAndDown();
        countTimeAfterSpawn();
    }

    void OnTriggerEnter2D(Collider2D collder)
    {
        OnTriggerEnterWithPlayer(collder);
    }


    public void Initialize(string powerUpId)
    {
        this.powerUpId = powerUpId;
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

    private void countTimeAfterSpawn()
    {
        if (gameObject.activeSelf)
        {
            timeAfterSpawn += Time.deltaTime;
        }
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

            string playerId = PlayerAnalyticsId.GetOrCreateId();
            AnalyticsEventsManager.Instance.PowerUpColledtedEvent(playerId, powerUpId, timeAfterSpawn);
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
