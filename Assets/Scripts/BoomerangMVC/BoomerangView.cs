using UnityEngine;
using Photon.Pun;

public class BoomerangView : MonoBehaviourPun
{
    private SpriteRenderer sprite;
    [SerializeField] private Material myOutlineView;
    
    
    void Awake()
    {
        SuscribeToBoomerangModelEvent();
        GetComponents();
        InitializeSpriteOutline();
    }

    void OnDestroy()
    {
        UnsuscribeToBoomerangModelEvent();
    }

    private void SuscribeToBoomerangModelEvent()
    {
        BoomerangModel.OnDisableSprite += OnDisableSprite;
    }

    private void UnsuscribeToBoomerangModelEvent()
    {
        BoomerangModel.OnDisableSprite -= OnDisableSprite;
    }

    private void GetComponents()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    // El boomerang correcto reacciona a la desactivacion del sprite gracias al viewID y que esta invocando el evento por un RPC
    // Entonces si el viewID no es el mismo no ejecuta nada de forma local, pero si ejecuta en todos porque avisa desde el RPC
    private void OnDisableSprite(int viewID)
    {
        if (photonView.ViewID != viewID) return;

        sprite.enabled = false;
        sprite.material = null;
    }

    private void InitializeSpriteOutline()
    {
        sprite.material = new Material(myOutlineView);

        if (photonView.Owner.CustomProperties.ContainsKey("SkinIndex"))
        {
            int skinIndex = (int)photonView.Owner.CustomProperties["SkinIndex"];
            sprite.material.SetColor("_SolidOutline", PlayerSkinManager.Instance.PlayerSkins[skinIndex]);
        }
    }
}
