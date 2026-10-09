using UnityEngine;

public class TestMovingEnemy : MonoBehaviour
{
    public Transform playerObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, playerObject.position, 2 * Time.deltaTime);
    }
}
