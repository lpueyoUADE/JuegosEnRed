using Photon.Pun;

public class PlayerController : MonoBehaviourPun
{
    private PlayerModel playerModel;
    private PlayerView playerView;


    void Awake()
    {
        GetComponents();
    }

    void Update()
    {
        CheckInputs();
    }


    private void GetComponents()
    {
        playerModel = GetComponent<PlayerModel>();
        playerView = GetComponent<PlayerView>();
    }

    private void CheckInputs()
    {
        if (PlayerInputsManager.Instance.Interact())
        {
            playerModel.Interact();
        }
    }
}
