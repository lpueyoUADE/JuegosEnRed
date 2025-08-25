using UnityEngine;
using UnityEngine.UI;

public class PlayerDataManager : SingletonMonoBehaviour<PlayerDataManager>
{
    [SerializeField] private Color[] playersSkins;

    private Color firstRandomSkin;
    private Color currentSelectedSkin;

    private int indexSkin = 0;

    public Color FirstRandomSkin { get =>  firstRandomSkin; }
    public Color CurrentSelectedSkin { get => currentSelectedSkin; }


    void Awake()
    {
        CreateSingleton(true);
    }

    void Start()
    {
        SuscribeToPhotonNetworkManagerEvent();
    }


    public void ChangeSkinIndex(Image roomUISkinPreview, int direction)
    {
        indexSkin += direction;

        if (indexSkin >= playersSkins.Length)
        {
            indexSkin = 0; 
        }

        else if (indexSkin < 0)
        {
            indexSkin = playersSkins.Length - 1;
        }

        currentSelectedSkin = playersSkins[indexSkin];
        roomUISkinPreview.color = currentSelectedSkin;
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
