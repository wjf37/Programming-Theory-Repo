using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Primate : MonoBehaviour
{
    private Rigidbody rb;
    private float speed = 1;
    private float moveRange = 5;
    private float sightRange;
    private float throwRange;
    private float attackRange;
    private bool enemyDetect;
    private bool friendDetect;
    private int maxHealth;
    private int currentHealth;
    private List<Transform> activeUnits = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Transform unit in activeUnits){
            float distance = Vector3.Distance(transform.position, unit.position);

            if (distance <= sightRange){

            }
        }
    }

    public virtual void Move(){
        Vector3 randomMove = new Vector3(Random.Range(-moveRange,moveRange),0,Random.Range(-moveRange,moveRange));
        rb.MovePosition(rb.position + randomMove * Time.deltaTime * speed);
    }

    public virtual void Throw(Primate enemy){
        enemy.TakeDamage(20);
    }

    public virtual void Attack(Primate enemy){
        enemy.TakeDamage(20);
    }

    public void TakeDamage(int damage){
        currentHealth -= damage;

        if (currentHealth<=0){
            Destroy(gameObject);
        }
    }

    private void Detect(Collider collider){
        
    }



}
