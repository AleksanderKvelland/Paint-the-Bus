using UnityEngine;
using Combat;
using UnityEngine.AI;
using Unity.VisualScripting;
using System;

public class ConstructionWorker : MonoBehaviour
{
    public GameObject attackPoint;
    public GameObject paintDecal;
    public float attackCooldown = 2f;
    public float paintSprayRadius = 1.5f;
    public float tapeDamageReduction = 1f;
    public float paintBaseDamage = 10f;

    private GameObject bus;
    [SerializeField] private float speed = 5f;
    private float tryAttackRange = 20f;
    private float attackRange = 5f;
    private int bus_layer;
    private int tape_layer;
    private NavMeshAgent agent;
    private float attackCooldownRemaining = 0f;
    private float lastBusHitDistance = 0f;
    private HealthComponent busHealth;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (!agent.isOnNavMesh)
        {
            Destroy(gameObject);
            return;
        }
        HealthComponent healthComponent = gameObject.GetComponent<HealthComponent>();
        healthComponent.OnDeath += HandleDeath;
        bus_layer = 1 << LayerMask.NameToLayer("Bus");
        tape_layer = 1 << LayerMask.NameToLayer("Tape");
    }

    public void SetBusTarget(GameObject targetBus)
    {
        bus = targetBus;
        busHealth = bus.GetComponent<HealthComponent>();
    }

    private void HandleDeath()
    {
        Destroy(gameObject);
    }

    void Update()
    {
        if (bus == null)
            return;

        if (attackCooldownRemaining > 0f)
        {
            attackCooldownRemaining -= Time.deltaTime;
        }

        Vector3 busToTagger = transform.position - bus.transform.position;
        agent.SetDestination(bus.transform.position + busToTagger.normalized * lastBusHitDistance);

        if (attackCooldownRemaining <= 0f)
        {
            if (Vector3.Distance(transform.position, bus.transform.position) < tryAttackRange)
            {
                RaycastHit hit;
                if (Physics.SphereCast(attackPoint.transform.position, .5f, attackPoint.transform.forward, out hit, attackRange, bus_layer))
                {
                    attackCooldownRemaining = attackCooldown;

                    lastBusHitDistance = Vector3.Distance(hit.point, transform.position);
                    Instantiate(paintDecal, bus.transform);
                    paintDecal.transform.position = bus.transform.InverseTransformPoint(hit.point);

                    float totalTapeLengthInArea = 0f;
                    foreach (Collider col in Physics.OverlapSphere(hit.point, paintSprayRadius, tape_layer))
                    {
                        totalTapeLengthInArea += col.transform.localScale.z;
                    }
                    if (totalTapeLengthInArea < tapeDamageReduction)
                    {
                        float dmgReductionPercentage = totalTapeLengthInArea / tapeDamageReduction;
                        int dmg = (int)Math.Ceiling(paintBaseDamage - paintBaseDamage * dmgReductionPercentage);
                        //Debug.Log(dmg);
                        busHealth.Damage(dmg);
                    }
                }
            }
        }
    }
}
