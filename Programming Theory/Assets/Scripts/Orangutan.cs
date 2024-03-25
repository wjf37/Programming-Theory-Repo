using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Orangutan : Primate
{
    //orangutan power = clone

    private float cloneRange = 3f;
    private float cloneCooldown = 3f;
    [SerializeField] GameObject clone;
    private bool cloneOnCooldown = false;


    IEnumerator Clone()
    {
        cloneOnCooldown = true;
        Vector3 insPos = new (transform.position.x + Random.Range(-cloneRange,cloneRange),transform.position.y, transform.position.z + Random.Range(-cloneRange, cloneRange));
        Instantiate(clone, insPos, transform.rotation);
        yield return new WaitForSeconds(cloneCooldown);
        cloneOnCooldown = false;
    }

    protected override void AttackDetect()
    {
        base.AttackDetect();
        if (Vector3.Distance(transform.position, closestUnit.position) < attackRange && !cloneOnCooldown)
        {
            StartCoroutine(Clone());
        }
    }
}
