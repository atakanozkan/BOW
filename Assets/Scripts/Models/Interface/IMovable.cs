using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovable
{
    void Move(Vector3 target);

    void Stop();

    void SetDestination(Vector3 destination);
}

