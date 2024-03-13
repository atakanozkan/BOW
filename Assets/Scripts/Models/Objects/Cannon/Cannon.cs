using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace BOW.Models.Objects {
    public class Cannon :  MonoBehaviour , IMovable
    {
        public int colIndex;
        public Vector3 currentPosition;
        public Vector3 targetDestination;
        public float movementSpeed = 0.4f;

        private void Start()
        {
            currentPosition = transform.position;
        }


        public void Move(Vector3 target)
        {
            SetDestination(target);

            if (!Mathf.Approximately(currentPosition.x - targetDestination.x, 0))
            {
                transform.DOMoveX(targetDestination.x, movementSpeed).
                    SetEase(Ease.OutCubic).
                    OnComplete(
                    () => currentPosition = targetDestination
                    );
            }
        }

        public void SetDestination(Vector3 destination)
        {
            targetDestination = destination;
        }

        public void Stop()
        {
            DOTween.Kill(transform);
            currentPosition = transform.position;
        }
    }
}


