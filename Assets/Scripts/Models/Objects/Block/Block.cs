using System.Collections;
using System.Collections.Generic;
using BOW.Models;
using UnityEngine;

public abstract class Block : MonoBehaviour ,IMovable
{
    [SerializeField]
    private BlockBehaviour blockBehaviour;
    public Sprite sprite;
    public int blockFallSpeed;
    public bool isFalling = false;
    public bool isDropping = true;

    public abstract void Fall();

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
