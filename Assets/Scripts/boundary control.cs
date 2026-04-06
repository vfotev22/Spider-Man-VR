using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boundarycontrol :MonoBehaviour
{
    public float threshold;

    void Fixedupdate()
    {
        if(transform.position.y < threshold)
        {
            transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        }
    }
}
