using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/Enemies/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Settings")]
    public float patrolWaitTime = 2f;
    public float stopAtDistance = 0.5f;
    public float detectionRange = 5f;
    public float viewAngle = 60f;
    public float losePlayerTime = 3f;
    

}
