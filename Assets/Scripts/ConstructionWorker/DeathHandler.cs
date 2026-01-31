using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Combat;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;


public class DeathHandler : MonoBehaviour
{
    [SerializeField] private GameObject lootPrefab;
    
    private HealthComponent _healthComponent;

    private void Awake()
    {
        _healthComponent = GetComponent<HealthComponent>();
        if (_healthComponent == null)
            throw new Exception(
                $"The game object \"{gameObject.name}\" has the DeathHandler script attached to it, " +
                "but no HealthComponent. Add a HealthComponent script to it."
            );
        
        _healthComponent.OnDeath += HandleDeath;
    }
    
    private void HandleDeath()
    {
        GameObject[] spawnedLootObjects = SpawnLoot(Random.Range(1, 5));
        
        foreach (GameObject lootObject in spawnedLootObjects)
        {
            AddLootVelocity(lootObject);
        }
    }

    private GameObject[] SpawnLoot(int count)
    {
        List<GameObject> spawnedLootObjects = new List<GameObject>();
        for (int i = 0; i < count; i++)
        {
            GameObject lootObject = Instantiate(
                lootPrefab,
                gameObject.transform.position,
                Random.rotation
            );
            spawnedLootObjects.Add(lootObject);
        }
        return spawnedLootObjects.ToArray();
    }

    private void AddLootVelocity(GameObject lootObject)
    {
        Vector3 velocity = new Vector3();

        velocity.x = Random.Range(-2.0f, 2.0f);
        velocity.y = Random.Range(0.2f, 3.0f);
        velocity.z = Random.Range(-2.0f, 2.0f);
        
        lootObject.GetComponent<Rigidbody>().linearVelocity = velocity;
    }
}