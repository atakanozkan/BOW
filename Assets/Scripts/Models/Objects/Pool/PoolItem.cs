using System;
using UnityEngine;

namespace BOW.Models.Pool
{
    [Serializable]
    public class PoolItem : MonoBehaviour
    {
        [SerializeField] private PoolItemType poolItemType;

        public PoolItemType GetPoolItemType()
        {
            return poolItemType;
        }

        public void SetPoolItemType(PoolItemType poolItemType)
        {
            this.poolItemType = poolItemType;
        }
    }

}