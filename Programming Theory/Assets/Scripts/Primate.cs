using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class Primate : MonoBehaviour
{
    [SerializeField] GameObject stone;
    private Rigidbody rb;
    private GameManager gameManager;
    private List<Transform> activeUnits = new();
    private Transform closestUnit;
    private Transform throwPos;
    private bool randomMoveStarted = false;
    private bool enemyInRange = false;
    private bool throwOnCooldown = false;
    private bool attackOnCooldown = false;
    private Vector3 randomMove;
    private Vector3 finishPos;
    private float moveRange = 2;
    private float sightRange = 20; 
    private float attackRange = 3;
    private float rotationSpeed = 700;
    private int currentHealth;
    protected float speed = 0.5f;
    protected float throwRange = 15; 
    protected float throwForce = 4;
    protected float throwCooldown = 1f;
    protected float attackCooldown = 1f;
    protected int maxHealth = 100;
    protected int throwDmg = 15;
    protected int attackDmg = 20;


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

        else
        {
            enemyInRange = false;
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

    IEnumerator Throw(GameObject enemy)
    {
        throwOnCooldown = true;
        Vector3 targetDir = enemy.transform.position - rb.position;
        GameObject thrownStone = Instantiate(stone, throwPos.position, transform.rotation);
        thrownStone.GetComponent<ThrowStone>().Throw(targetDir, throwForce, throwDmg);
        yield return new WaitForSeconds(throwCooldown);
        throwOnCooldown = false;
    }

    IEnumerator Attack(Primate enemy)
    {
        attackOnCooldown = true;
        enemy.TakeDamage(attackDmg);
        yield return new WaitForSeconds(attackCooldown);
        attackOnCooldown = false;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth<=0){
            Destroy(gameObject);
            gameManager.OnUnitDestroyed(transform);
        }
    }

    private void Detect(){
        float closestDist = 1000;
        bool closestUnitExists = false;

        foreach (Transform unit in activeUnits)
        {
            float distance = Vector3.Distance(transform.position, unit.position);

            if (!gameObject.CompareTag(unit.tag) && closestDist > distance) //if not same tag, and the new distance is less.
            {
                closestUnit = unit;
                closestUnitExists = true;
            }

            if (closestUnit == unit)
            {
                closestUnitExists = true;
            }
        }

        if (!closestUnitExists)
        {
            closestUnit = null;
        }
    }

    private void AttackDetect()
    {
        if (Vector3.Distance(transform.position, closestUnit.position) < attackRange)
        {
            StartCoroutine(Attack(closestUnit.GetComponent<Primate>()));
        }
    }

    private void ThrowDetect()
    {
        if (Vector3.Distance(transform.position, closestUnit.position) < throwRange && !throwOnCooldown)
        {
            StartCoroutine(Throw(closestUnit.gameObject));
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
