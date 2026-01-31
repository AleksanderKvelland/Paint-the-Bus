using System;
using UnityEngine;


namespace Combat
{
    [Serializable] public enum DamageType { Any, Blunt, Slash, Tear, Pierce };

    [Serializable]
    public class DamageModifier
    {
        [SerializeField] public DamageType damageType = DamageType.Any;

        [Range(0.1f, 5f)] public float factor = 1.0f;
    }
}
