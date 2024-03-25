using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Chimp : Primate
{
    [SerializeField] private float throwRange = 15; 
    [SerializeField] private float throwForce = 4;
    [SerializeField] private float throwCooldown = 1f;
    [SerializeField] private int throwDmg = 15;
    [SerializeField] protected GameObject stone;
    private Transform throwPos;
    private bool throwOnCooldown = false;

    protected override void Start()
    {
        base.Start();
        throwPos = transform.GetChild(1);
    }

    protected override void Update()
    {
        Detect();
        if (closestUnit != null)
        {
            AttackDetect();
            ThrowDetect();
        }
    }

    IEnumerator Throw(GameObject enemy)
    {
        throwOnCooldown = true;
        Vector3 targetDir = enemy.transform.position - rb.position;
        GameObject thrownStone = Instantiate(stone, throwPos.position, transform.rotation);
        thrownStone.GetComponent<ThrowStone>().Throw(targetDir, throwForce, throwDmg);
        yield return new WaitForSeconds(throwCooldown);
        throwOnCooldown = false;
    }

        private void ThrowDetect()
    {
        if ( attackRange < Vector3.Distance(transform.position, closestUnit.position) && Vector3.Distance(transform.position, closestUnit.position) < throwRange && !throwOnCooldown)
        {
            StartCoroutine(Throw(closestUnit.gameObject));
        }
    }
}
