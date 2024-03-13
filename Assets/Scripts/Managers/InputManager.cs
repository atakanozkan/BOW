using System.Collections;
using System.Collections.Generic;
using BOW.Controllers;
using BOW.Models.Objects;
using Unity.Burst.CompilerServices;
using UnityEngine;

namespace BOW.Managers
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField]
        private PlayerController playerController;
        private Camera mainCamera;
        private RaycastHit2D[] hit;
        private bool resume = true;
        private bool isPressing = false;
        private int hit_count;

        private void Awake()
        {
            mainCamera = Camera.main;
            hit = new RaycastHit2D[1];
        }

        private void Update()
        {
            GenerateInput();
        }

        void GenerateInput()
        {
            if (!resume)
            {
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                isPressing = true;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Debug.Log("button up!");
                isPressing = false;
            }
            Vector3 pos;
            if (isPressing)
            {
                pos = Input.mousePosition;
                pos.z = mainCamera.transform.position.z;
                GameObject hittedObject = GetHittedObject(pos);

                if (!hittedObject)
                {
                    return;
                }

                if (hittedObject.CompareTag("Cell"))
                {
                    Cell hitCell = hittedObject.GetComponent<Cell>();
                    playerController.TriggerCannonMovement(hitCell.GetColumnIndex());
                }
            }

        }


        private GameObject GetHittedObject(Vector3 position)
        {
            hit_count = Physics2D.RaycastNonAlloc(mainCamera.ScreenToWorldPoint(position), Vector2.zero, hit);

            if (hit_count > 0 && hit[0].collider != null)
            {
                return hit[0].collider.gameObject;
            }
            return null;
        }
    }

}

