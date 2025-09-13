using UnityEngine;
using Photon.Pun;

public class BoomerangView : MonoBehaviourPun
{
    private SpriteRenderer sprite;
    [SerializeField] private Material myOutlineView;
    [SerializeField] private Material otherOutlineView;
    
    
    void Awake()
    {
        GetComponents();
        InitializeSpriteOutline();
    }


    private void GetComponents()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    private void InitializeSpriteOutline()
    {
        if (photonView.IsMine)
        {
            sprite.material = myOutlineView;
        }

        else
        {
            sprite.material = otherOutlineView;
        }
    }
}
