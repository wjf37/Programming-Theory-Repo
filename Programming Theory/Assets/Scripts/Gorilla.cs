using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gorilla : Primate
{
    //gorilla unique = leap
    private float leapCooldown = 2f;
    private float leapForce = 1500f;
    private bool leapOnCooldown = false;


    IEnumerator Leap()
    {
        leapOnCooldown = true;
        yield return new WaitForSeconds(0.3f);
        Vector3 leapAngle = new Vector3(0,0.4f,0);
        Vector3 leapVector = -transform.forward + leapAngle;
        rb.AddForce(leapVector * leapForce, ForceMode.Impulse);
        yield return new WaitForSeconds(leapCooldown);
        leapOnCooldown = false;
    }

    protected override void MoveToEnemy()
    {
        base.MoveToEnemy();
        if (!leapOnCooldown)
        {
            StartCoroutine(Leap());
        }
    }
}
