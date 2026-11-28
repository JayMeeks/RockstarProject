using UnityEngine;

[System.Serializable]
public class EnemyClass
{
    public LocationMarker spawnLocation;
    public EnemyType enemyType;
    public enum EnemyType
    {
        Melee,
        Ranged
    }
}
