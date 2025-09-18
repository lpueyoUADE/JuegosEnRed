using Photon.Pun;

public class BoomerangController : MonoBehaviourPun
{
    private BoomerangModel boomerangModel;
    private BoomerangView boomerangView;

    public BoomerangModel BoomerangModel { get => boomerangModel; }
    public BoomerangView BoomerangView { get => boomerangView; }


    void Awake()
    {
        GetComponents();
    }


    private void GetComponents()
    {
        boomerangModel = GetComponent<BoomerangModel>();
        boomerangView = GetComponent<BoomerangView>();
    }
}
