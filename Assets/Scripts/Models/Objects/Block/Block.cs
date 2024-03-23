using System.Collections;
using System.Collections.Generic;
using BOW.Models;
using TMPro;
using UnityEngine;

public abstract class Block : MonoBehaviour, IMovable
{
    [SerializeField]
    private BlockBehaviour blockBehaviour;

    [SerializeField]
    private TextMeshPro textMeshPro;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    public Sprite sprite;
    public int blockFallSpeed;
    public bool isFalling = false;
    public bool isDropping = true;

    public abstract void Fall();

    public void SetSprite(Sprite sprite)
    {
        this.sprite = sprite;
        spriteRenderer.sprite = this.sprite;
    }

    public void SetBlockText(string text)
    {
        textMeshPro.text = text;
    }

    public BlockBehaviour GetBlockBehaviour()
    {
        return blockBehaviour;
    }

    public void Move(Vector3 target)
    {
        throw new System.NotImplementedException();
    }

    public void Stop()
    {
        throw new System.NotImplementedException();
    }

    public void SetDestination(Vector3 destination)
    {
        throw new System.NotImplementedException();
    }
}
