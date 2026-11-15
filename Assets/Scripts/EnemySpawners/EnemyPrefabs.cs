using UnityEngine;

[CreateAssetMenu(fileName = "EnemyPrefabs", menuName = "Scriptable Objects/EnemieSpawner/EnemyPrefabData")]
public class EnemyPrefabs : ScriptableObject
{
    public GameObject meleePrefab;
    public GameObject rangedPrefab;
}
