using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Primate : MonoBehaviour
{
    [SerializeField] GameObject stone;
    private Rigidbody rb;
    private float speed = 1;
    private float moveRange = 2;
    private float sightRange = 25; 
    private float throwRange = 15; 
    private float attackRange = 5;
    private float throwForce = 10;
    private float rotationSpeed = 700;
    //private bool enemyDetect;
    //private bool friendDetect;
    private int maxHealth = 100;
    private int currentHealth;
    private int throwDmg = 15;
    private int attackDmg = 20;
    private GameManager gameManager;
    private List<Transform> activeUnits = new();
    private Transform closestUnit;
    private Transform throwPos;
    private bool randomMoveStarted = false;
    private bool enemyInRange = false;
    //private bool posFound = false;
    private Vector3 randomMove;
    private Vector3 finishPos;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.OnListUpdated += HandleListUpdate;
        throwPos = transform.GetChild(1);
    }

    // Update is called once per frame
    void Update()
    {
        Detect();
        if (closestUnit != null)
        {
            AttackDetect();
            ThrowDetect();
        }
    }

    void FixedUpdate()
    {
        Move();

    }
    private void Move(){
        if (closestUnit != null)
        {
            if (Vector3.Distance(transform.position, closestUnit.position) < sightRange)
            {
                randomMoveStarted = false;
                enemyInRange = true;
                Vector3 targetMove = closestUnit.position - rb.position;
                Rotate(targetMove.x, targetMove.z);
                rb.MovePosition(rb.position + targetMove * Time.deltaTime * speed);
            }

            else
            {
                enemyInRange = false;
            }
        }

        if (!enemyInRange)
        {
            GenPos();
            MoveFin();
        }
    }

    private void GenPos()
    {
        if (!randomMoveStarted)
        {
            randomMove = new Vector3(Random.Range(-moveRange,moveRange), 0, Random.Range(-moveRange,moveRange));
            finishPos = rb.position + randomMove;
            randomMoveStarted = true;
        }
    }
    private void MoveFin()
    {
        if (randomMoveStarted)
        {
            Rotate(randomMove.x, randomMove.z);
            rb.MovePosition(rb.position + randomMove * Time.deltaTime * speed);
        }
        if (Vector3.Distance(rb.position, finishPos) < 0.3f)
        {
            randomMoveStarted = false;
            rb.velocity = Vector3.zero;
        }
    }

    private void Throw(GameObject enemy)
    {
        Vector3 targetDir = enemy.transform.position - rb.position;
        Instantiate(stone, throwPos);
        stone.GetComponent<ThrowStone>().Throw(targetDir, throwForce, throwDmg);
    }

    private void Attack(Primate enemy)
    {
        enemy.TakeDamage(attackDmg);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth<=0){
            Destroy(gameObject);
        }
    }

    private void Detect(){
        float closestDist = 1000;
        
        foreach (Transform unit in activeUnits)
        {
            float distance = Vector3.Distance(transform.position, unit.position);

            if (!gameObject.CompareTag(unit.tag) && closestDist > distance) //if not same tag, and the new distance is less.
            {
                closestUnit = unit;
            }
        }
    }

    private void AttackDetect()
    {
        if (Vector3.Distance(transform.position, closestUnit.position) < attackRange)
        {
            Attack(closestUnit.GetComponent<Primate>());
        }
    }

    private void ThrowDetect()
    {
        if (Vector3.Distance(transform.position, closestUnit.position) < throwRange)
        {
            Throw(closestUnit.gameObject);
        }
    }

    private void Rotate(float x, float z)
    {
        float targetAngle = Mathf.Atan2(-x, -z) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
        rb.rotation = Quaternion.RotateTowards(rb.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    private void HandleListUpdate(List<Transform> updatedList){
        activeUnits = updatedList;
    }
}
