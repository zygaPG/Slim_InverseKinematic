using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartIKController : MonoBehaviour
{
    [SerializeField] InverseKinematicSlim myIK;

    [SerializeField] Transform root, elbow, tip;

    [SerializeField] Transform target;
    [SerializeField] Transform hint;

    

    void Update()
    {
        myIK.SetIK(root, elbow, tip, target, hint.position);
    }
}
