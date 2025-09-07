using Photon.Pun;

public class ProjectileController : MonoBehaviourPun
{
    private ProjectileModel projectileModel;
    private ProjectileView projectileView;

    public ProjectileModel ProjectileModel { get => projectileModel; }
    public ProjectileView ProjectileView { get => projectileView; }


    void Awake()
    {
        GetComponents();
    }


    private void GetComponents()
    {
        projectileModel = GetComponent<ProjectileModel>();
        projectileView = GetComponent<ProjectileView>();
    }
}
