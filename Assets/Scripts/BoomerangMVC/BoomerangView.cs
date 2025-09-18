using UnityEngine;
using Photon.Pun;

public class BoomerangView : MonoBehaviourPun
{
    private SpriteRenderer sprite;
    TrailRenderer trailRenderer;
    [SerializeField] private Material myOutlineView;


    void Awake()
    {
        SuscribeToBoomerangModelEvent();
        GetComponents();
        InitializeSpriteOutline();
        InitializeTrailRenderer();
    }

    void OnDestroy()
    {
        UnsuscribeToBoomerangModelEvent();
    }


    private void SuscribeToBoomerangModelEvent()
    {
        BoomerangModel.OnDisableSprite += OnDisableSprite;
        BoomerangModel.OnShowTrail += OnShowTrail;
    }

    private void UnsuscribeToBoomerangModelEvent()
    {
        BoomerangModel.OnDisableSprite -= OnDisableSprite;
        BoomerangModel.OnShowTrail -= OnShowTrail;
    }

    private void GetComponents()
    {
        sprite = GetComponent<SpriteRenderer>();
        trailRenderer = GetComponentInChildren<TrailRenderer>();
    }

    // El boomerang correcto reacciona a la desactivacion del sprite gracias al viewID y que esta invocando el evento por un RPC
    // Entonces si el viewID no es el mismo no ejecuta nada de forma local, pero si ejecuta en todos porque avisa desde el RPC
    private void OnDisableSprite(int viewID)
    {
        if (photonView.ViewID != viewID) return;

        sprite.enabled = false;
        sprite.material = null;
    }

    private void OnShowTrail(int viewID, bool status)
    {
        if (photonView.ViewID != viewID) return;

        if (status)
        {
            trailRenderer.Clear();
        }

        trailRenderer.emitting = status;
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

    private void InitializeTrailRenderer()
    {
        if (photonView.Owner.CustomProperties.ContainsKey("SkinIndex"))
        {
            int skinIndex = (int)photonView.Owner.CustomProperties["SkinIndex"];
            Color skinColor = PlayerSkinManager.Instance.PlayerSkins[skinIndex];

            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(skinColor, 0f), new GradientColorKey(skinColor, 1f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) }
            );

            trailRenderer.colorGradient = gradient;
        }
    }
}
