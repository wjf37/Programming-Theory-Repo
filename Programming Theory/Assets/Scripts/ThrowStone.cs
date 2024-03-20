using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowStone : MonoBehaviour
{
    private Rigidbody rb;
    private int damage;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Throw(Vector3 dir, float mag, int t_damage)
    {
        rb.AddForce(dir * mag, ForceMode.Impulse);
        damage = t_damage;
    }

    void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.CompareTag("Ground"))
        {
            other.gameObject.GetComponent<Primate>().TakeDamage(damage);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
