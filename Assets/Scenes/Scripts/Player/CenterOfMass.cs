using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterOfMass : MonoBehaviour
{
    void OnDrawGizmosSelected(){
        Debug.DrawRay(transform.position, Vector3.up * 2f, Color.blue,  Time.deltaTime);
    }
}
