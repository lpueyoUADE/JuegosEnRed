using UnityEngine;
using UnityEngine.UI;

public class PlayerSkinManager : MonoBehaviour
{
    private static PlayerSkinManager instance;

    [SerializeField] private Color[] playersSkins;

    private Color firstRandomSkin;
    private Color currentSelectedSkin;

    private int indexSkin = 0;

    public static PlayerSkinManager Instance { get => instance; }

    public Color FirstRandomSkin { get =>  firstRandomSkin; }
    public Color CurrentSelectedSkin { get => currentSelectedSkin; }


    void Awake()
    {
        CreateSingleton();
    }

    void Start()
    {
        SuscribeToPhotonNetworkManagerEvent();
    }


    public void ChangeSkinIndex(Image roomUISkinPreview)
    {        
        indexSkin ++;

        if (indexSkin >= playersSkins.Length)
        {
            indexSkin = 0;
        }

        currentSelectedSkin = playersSkins[indexSkin];
        roomUISkinPreview.color = currentSelectedSkin;
    }


    private void CreateSingleton()
    {
        if (instance == null)
        {
            instance = this;
        }

        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void SuscribeToPhotonNetworkManagerEvent()
    {
        PhotonNetworkManager.Instance.OnJoinedRoomEvent += OnInitializeRandomCurrentSelectedColor;
    }

    private void OnInitializeRandomCurrentSelectedColor()
    {
        int randomColor = Random.Range(0, playersSkins.Length);

        firstRandomSkin = playersSkins[randomColor];
        currentSelectedSkin = firstRandomSkin;

        indexSkin = randomColor;
    }
}
