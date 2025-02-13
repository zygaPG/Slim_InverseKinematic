using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseKinematicSlim : MonoBehaviour
{
    // Preallocated helper variables.
    // These are used to avoid allocating garbage but you can use them if you want.
    Vector3 AC, directionToElbow, forwardVector, directionToIndex;
    Vector3 planeNormal, directionAC, midpoint, perpendicular, b1, b2, chosenB;

    float upperArmLength, forearmLength;
    

    public void SetIK(Transform _root, Transform _elbow, Transform _tip, Transform _handIK, Vector3 _hintPosition)
    {
        //calculate bone lengths
        upperArmLength = (_root.position - _elbow.position).magnitude;
        forearmLength = (_elbow.position - _tip.position).magnitude;

        //calculate new elbow position
        Vector3 _elbowPosition = FindMiddlePointOnPlane(_handIK.position, _root.position, forearmLength, upperArmLength, _hintPosition);

        //calculate _root direction
        directionToElbow = (_elbowPosition - _root.position).normalized;
        forwardVector = Vector3.Cross(directionToElbow, _root.right).normalized;

        //In my case (hands imported from Blender), the object has swapped axes and is rotated by (0, -90, -90). so UP is now FORWARD, RIGHT is now UP, and FORWARD is now RIGHT
        _root.rotation = Quaternion.LookRotation(-forwardVector, directionToElbow);

        //calculate _elbow direction
        directionToIndex = (_handIK.position - _elbowPosition).normalized;
        forwardVector = Vector3.Cross(directionToIndex, _elbow.right).normalized;

        //In my case (hands imported from Blender), the object has swapped axes and is rotated by (0, -90, -90). so UP is now FORWARD, RIGHT is now UP, and FORWARD is now RIGHT
        _elbow.rotation = Quaternion.LookRotation(-forwardVector, directionToIndex);

        //set hand rotation
        _tip.rotation = _handIK.rotation;
    }
    
    
    private Vector3 FindMiddlePointOnPlane(Vector3 _tip, Vector3 _root, float _forearmLength, float _upperArmLength, Vector3 _hint)
    {
        AC = _root - _tip;
        float distanceAC = AC.magnitude;

        if (distanceAC > _forearmLength + _upperArmLength || distanceAC < Mathf.Abs(_forearmLength - _upperArmLength))
        {
            return _root + ((_tip - _root).normalized * _upperArmLength);
        }


        planeNormal = Vector3.Cross(AC, _hint - _tip).normalized;

        directionAC = AC.normalized;


        float p = (_forearmLength * _forearmLength - _upperArmLength * _upperArmLength + distanceAC * distanceAC) / (2 * distanceAC);
        midpoint = _tip + directionAC * p;


        float radiusSquared = _forearmLength * _forearmLength - p * p;
        if (radiusSquared < 0)
        {
            return _root + ((_tip - _root).normalized * _upperArmLength);
        }
        float radius = Mathf.Sqrt(radiusSquared);


        perpendicular = Vector3.Cross(directionAC, planeNormal).normalized;


        b1 = midpoint + perpendicular * radius;
        b2 = midpoint - perpendicular * radius;


        chosenB = Vector3.Distance(b1, _hint) < Vector3.Distance(b2, _hint) ? b1 : b2;

        return chosenB;
    }

}
