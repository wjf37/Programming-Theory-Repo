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
    GameObject insClone; //instantiated clone


    IEnumerator Clone()
    {
        cloneOnCooldown = true;
        Vector3 insPos = new (transform.position.x + Random.Range(-cloneRange,cloneRange),transform.position.y, transform.position.z + Random.Range(-cloneRange, cloneRange));
        insClone = Instantiate(clone, insPos, transform.rotation);
        gameManager.OnUnitSpawned(insClone.transform);
        yield return new WaitForSeconds(cloneCooldown);
        cloneOnCooldown = false;
        if (insClone != null)
        {
            Destroy(insClone);
            gameManager.OnUnitDestroyed(insClone.transform);
        }
    }

    protected override void AttackDetect()
    {
        base.AttackDetect();
        if (Vector3.Distance(transform.position, closestUnit.position) < attackRange && !cloneOnCooldown)
        {
            StartCoroutine(Clone());
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (insClone != null)
        {
            Destroy(insClone);
            gameManager.OnUnitDestroyed(insClone.transform);
        }
    }
}
