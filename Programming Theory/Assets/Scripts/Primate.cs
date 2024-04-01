using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Primate : MonoBehaviour
{
    protected Rigidbody rb;
    protected GameManager gameManager;
    protected List<Transform> activeUnits = new();
    private Transform arms;
    protected Transform closestUnit;
    protected bool randomMoveStarted = false;
    protected bool enemyInRange = false;
    protected bool attackOnCooldown = false;
    protected Vector3 randomMove;
    protected Vector3 finishPos;
    protected float moveRange = 2;
    protected float sightRange = 20; 
    protected float attackRange = 3;
    protected float rotationSpeed = 700;
    private float armRotationSpeed = 200;
    protected int currentHealth;
    [SerializeField] protected float speed = 0.5f;
    [SerializeField] protected float attackCooldown = 1f;
    [SerializeField] protected int maxHealth = 100;
    [SerializeField] protected int attackDmg = 20;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.OnListUpdated += HandleListUpdate;
        arms = transform.GetChild(0).GetChild(1);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        Detect();
        if (closestUnit != null)
        {
            AttackDetect();
        }
    }

    void FixedUpdate()
    {
        Move();

    }
    
    public void InitUnitList(List<Transform> initUnits)
    {
        activeUnits.AddRange(initUnits);
    }

    private void Move(){
        if (closestUnit != null)
        {
            if (Vector3.Distance(transform.position, closestUnit.position) < sightRange)
            {
                MoveToEnemy();
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

    protected virtual void MoveToEnemy()
    {
        randomMoveStarted = false;
        enemyInRange = true;
        Vector3 targetMove = closestUnit.position - rb.position;
        Rotate(targetMove.x, targetMove.z);
        rb.MovePosition(rb.position + targetMove * Time.deltaTime * speed);
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

    protected IEnumerator Attack(Primate enemy)
    {
        attackOnCooldown = true;
        //Quaternion ogRotation = Quaternion.Euler(0f,0f,0f);
        //Quaternion armsUp =  Quaternion.Euler(80f,0f,0f);
        Vector3 armsUp =  new (80f,0f,0f);
        //arms.rotation = Quaternion.RotateTowards(ogRotation, armsUp, Time.deltaTime * armRotationSpeed);
        arms.Rotate(Vector3.right, 80f, Space.Self);
        enemy.TakeDamage(attackDmg);
        //arms.rotation = Quaternion.RotateTowards(armsUp, ogRotation, Time.deltaTime * armRotationSpeed);
        yield return new WaitForSeconds(0.3f);
        arms.Rotate(Vector3.right, -80f, Space.Self);
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

    protected void Detect(){
        float closestDist = 1000;
        bool closestUnitExists = false;

        foreach (Transform unit in activeUnits)
        {
            float distance = Vector3.Distance(transform.position, unit.position);

            if (!gameObject.CompareTag(unit.tag) && closestDist > distance) //if not same tag, and the new distance is less.
            {
                closestUnit = unit;
                closestUnitExists = true;
                closestDist = distance;
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

    protected virtual void AttackDetect()
    {
        if (Vector3.Distance(transform.position, closestUnit.position) < attackRange && !attackOnCooldown)
        {
            StartCoroutine(Attack(closestUnit.GetComponent<Primate>()));
        }
    }

    private void Rotate(float x, float z)
    {
        float targetAngle = Mathf.Atan2(-x, -z) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
        rb.rotation = Quaternion.RotateTowards(rb.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    private void HandleListUpdate(List<Transform> updatedList){
        activeUnits.Clear();
        activeUnits.AddRange(updatedList);
        /*for (int i = 0; i < activeUnits.Count; i++)
        {
            Debug.Log(activeUnits[i].gameObject);
        }*/
    }

    protected virtual void OnDestroy()
    {
        gameManager.OnListUpdated -= HandleListUpdate;
    }
}
