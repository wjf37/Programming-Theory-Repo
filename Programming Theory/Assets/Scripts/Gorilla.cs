using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gorilla : Primate
{
    //gorilla unique = leap
    private float leapCooldown = 2f;
    private float leapForce = 100f;
    private bool leapOnCooldown = false;


    IEnumerator Leap()
    {
        leapOnCooldown = true;
        Vector3 leapAngle = new Vector3(0,15,0);
        Vector3 leapVector = Vector3.forward +  leapAngle;
        rb.AddForce(leapVector * leapForce, ForceMode.Impulse);
        yield return new WaitForSeconds(leapCooldown);
        leapOnCooldown = false;
    }

    void LeapHandler()
    {
        if (!leapOnCooldown)
        {
            Leap();
        }
    }

    protected override void MoveToEnemy()
    {
        base.MoveToEnemy();
        LeapHandler();
    }
}
