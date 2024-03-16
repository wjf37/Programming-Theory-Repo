using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Primate : MonoBehaviour
{
    private Rigidbody rb;
    private float speed = 1;
    private float moveRange = 5;
    private float sightRange = 25; 
    private float throwRange = 15; 
    private float attackRange = 5;
    //private bool enemyDetect;
    //private bool friendDetect;
    private int maxHealth = 100;
    private int currentHealth;
    private GameManager gameManager;
    private List<Transform> activeUnits = new();
    private List<(Transform,float)> seeUnits = new();
    private List<(Transform,float)> throwUnits = new();
    private List<(Transform,float)> attackUnits = new();

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.OnListUpdated += HandleListUpdate;
    }

    // Update is called once per frame
    void Update()
    {
        Detect();
    }

    public virtual void Move(){
        //if ()
        Vector3 randomMove = new Vector3(Random.Range(-moveRange,moveRange),0,Random.Range(-moveRange,moveRange));
        rb.MovePosition(rb.position + randomMove * Time.deltaTime * speed);
    }

    public virtual void Throw(Primate enemy){
        enemy.TakeDamage(10);
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

    private void Detect(){
        //apes check for their closest targets

        //Vector3 targetMove = unit.position - rb.position;
        //rb.MovePosition(rb.position + targetMove * Time.deltaTime * speed);
        
        //check if ape can see unit. if it can then move to check if in throw range. 
        foreach (Transform unit in activeUnits){
            float distance = Vector3.Distance(transform.position, unit.position);

            if (distance <= sightRange){
                if (!ContainsUnit(unit, seeUnits)){
                    seeUnits.Add((unit, distance));
                }
                
                else
                {
                    UpdateDist(unit, seeUnits, distance);
                }
                if (distance <= throwRange){
                    if (!ContainsUnit(unit, throwUnits)){
                        throwUnits.Add((unit, distance));
                    }

                    else
                    {
                        UpdateDist(unit, throwUnits, distance);
                    }

                    if (distance <= attackRange){
                        if (!ContainsUnit(unit, attackUnits)){
                            attackUnits.Add((unit, distance));
                        }

                        else
                        {
                            UpdateDist(unit, attackUnits, distance);
                        }
                    }

                    else if (ContainsUnit(unit, attackUnits)){
                        attackUnits.RemoveAll(t => t.Item1 == unit);
                    }
                }

                else if (ContainsUnit(unit, throwUnits)){
                    throwUnits.RemoveAll(t => t.Item1 == unit);
                }
            }

            else if (ContainsUnit(unit, seeUnits)){
                seeUnits.RemoveAll(t => t.Item1 == unit);
            }
        }
    }

    private void CalcClosest()
    {
        // foreach (Transform unit in seeUnits)
        // {

        // }
    }

    private void HandleListUpdate(List<Transform> updatedList){
        activeUnits = updatedList;
    }

    private bool ContainsUnit(Transform iUnit, List<(Transform, float)> unitList)
    {
        foreach (var unitTuple in unitList)
        {
            if(unitTuple.Item1 == iUnit)
            {
                return true;
            }
        }

        return false;
    }

    private void UpdateDist(Transform iUnit, List<(Transform, float)> unitList, float distance)
    {
        for (int i = 0; i < unitList.Count; i++)
        {
            if(unitList[i].Item1 == iUnit)
            {
                unitList[i] = (iUnit, distance);
            }
        }
    }
}
