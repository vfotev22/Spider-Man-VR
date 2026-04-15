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
            transform.position = new Vector3(-284.0f, 181.2f, 275.0f);
        }
    }
}
