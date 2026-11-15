using UnityEngine;

[System.Serializable]
public class EnemyClass
{
    public Transform spawnLocation;
    public EnemyType enemyType;
    public enum EnemyType
    {
        Melee,
        Ranged
    }
}
