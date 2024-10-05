using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddMath
{    
    static public Vector3 SetZto(Vector3 a, float z = 0)
    {
        return new Vector3(a.x, a.y, z);
    }
    static public float MoveTowards(float a, float b, float maxDistance)
    {
        if(a < b) {
            if ((a + maxDistance) > b)
            {
                return b;
            }
            else return a + maxDistance;
        }
        else
        {
            if ((a - maxDistance) < b)
            {
                return b;
            }
            else return a - maxDistance;
        }
    }
}
