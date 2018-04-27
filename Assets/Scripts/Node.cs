using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {
    
    public int nodeIndex = 0; //What node is this in the patrol route (0 index based)
    public int nextNodeIndex = 1; //index for next node in patrol route
    public float nodeRange = 1.0f; //Range player must be within for player to detect node

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, nodeRange); //Draw Node Range Sphere
    }
}
