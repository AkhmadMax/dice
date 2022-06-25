using System.Collections;
using UnityEngine;

public class Dice : MonoBehaviour
{
    public float size = 0.015f;
    public bool ready = true;
    public int currentValue = -1;
    public Collider[] sides;
    public Collider ground;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.maxAngularVelocity = 100;
    }

    void Update()
    {
        if (ready)
        {
            float t = Time.time * 0.1f;
            Vector3 euler = new Vector3(Mathf.Sin(t) * 360, Mathf.Cos(t) * 360);
            transform.eulerAngles = euler;
        }
    }

    public IEnumerator Throw()
    {
        ready = false;
        rb.isKinematic = false;
        float power = 15f;
        
        rb.WakeUp();
        rb.AddForce(Random.Range(-1f, 1f) * power, 0, Random.Range(-1f, 1f) * power, ForceMode.Force);
        rb.AddTorque(Random.insideUnitSphere * power, ForceMode.Impulse);

        yield return StartCoroutine(InProgress());

        currentValue = CheckSide();
    }

    private int CheckSide()
    {
        foreach (Collider side in sides)
        {
            bool isOnGround = ground.bounds.Contains(side.transform.position);
            if (isOnGround)
            {
                return int.Parse(side.name); // TODO: refactor, source of errors
            }
        }
        return -1;
    }

    public void ResetDice()
    {
        ready = true;
        rb.isKinematic = true;
    }

    IEnumerator InProgress()
    {
        while (!rb.IsSleeping())
        {
            yield return new WaitForFixedUpdate();
        }
    }
}
