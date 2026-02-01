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
    private RectTransform healthBar;
    private GameObject finishScreen;

    private GameObject bus;
    [SerializeField] private float speed = 5f;
    private float tryAttackRange = 20f;
    private float attackRange = 5f;
    private int bus_layer;
    private int tape_layer;
    private CharacterController controller;
    private float attackCooldownRemaining = 0f;
    private HealthComponent busHealth;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        HealthComponent healthComponent = gameObject.GetComponent<HealthComponent>();
        healthComponent.OnDeath += HandleDeath;
        bus_layer = 1 << LayerMask.NameToLayer("Bus");
        tape_layer = 1 << LayerMask.NameToLayer("Tape");
    }

    public void SetBusTarget(GameObject targetBus, RectTransform healthBar, GameObject finishScreen)
    {
        bus = targetBus;
        busHealth = bus.GetComponent<HealthComponent>();
        this.healthBar = healthBar;
        this.finishScreen = finishScreen;
    }

    private void HandleDeath()
    {
        Destroy(gameObject);
    }

    void Update()
    {
        if (bus == null || controller == null)
            return;

        if (attackCooldownRemaining > 0f)
        {
            attackCooldownRemaining -= Time.deltaTime;
        }

         // Calculate direction to bus
        Vector3 toBus = bus.transform.position - transform.position;

        // Move towards bus
        if (true)//toBus.magnitude < attackRange * .5f)
        {
            Vector3 movement = toBus.normalized * speed * Time.deltaTime;
            controller.Move(movement);
        }

        if (attackCooldownRemaining <= 0f)
        {
            if (Vector3.Distance(transform.position, bus.transform.position) < tryAttackRange)
            {
                RaycastHit hit;
                if (Physics.SphereCast(attackPoint.transform.position, .5f, attackPoint.transform.forward, out hit, attackRange, bus_layer))
                {
                    attackCooldownRemaining = attackCooldown;

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
                        float percentageHealth = ((float)busHealth.GetHealth) / ((float)busHealth.GetMaxHealth);
                        healthBar.localScale = new Vector3(percentageHealth, 1f, 1f);
                        if (busHealth.GetHealth <= 0)
                        {
                            finishScreen.SetActive(true);
                        }
                    }
                }
            }
        }
    }
}
