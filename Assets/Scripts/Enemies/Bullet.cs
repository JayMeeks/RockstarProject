using System;
using System.Collections;
using System.Numerics;
using UnityEngine;
using UnityEngine.UIElements;
using Vector3 = UnityEngine.Vector3;

namespace Enemies
{
    public class Bullet : MonoBehaviour
    {
        private Vector3 _startPos;
        private Vector3 _targetPosition;

    
        [SerializeField] private float speed = 1f;
        [SerializeField] private int damage;
    
        [SerializeField] private GameObject player;
        
        private PlayerController _playerController;
        private Rigidbody _rb;
        
        void Awake()
        {
            _startPos = transform.position;
            player = GameObject.FindGameObjectWithTag("Player");
            _playerController = player.GetComponent<PlayerController>();
            _rb = GetComponent<Rigidbody>();
            GetPlayerPosition();
            StartCoroutine(SelfDestruct());
        }

        private void GetPlayerPosition()
        {
            _targetPosition = player.transform.position;
            _rb.linearVelocity = (_targetPosition - _startPos).normalized * speed;
        }
    
        // Update is called once per frame
        void Update()
        {
            //transform.position = Vector3.MoveTowards(transform.position, _targetPosition, speed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            _playerController.currentHealth -= damage;
            Destroy(gameObject);
        }

        private IEnumerator SelfDestruct()
        {
            yield return new WaitForSeconds(10f);
            Destroy(gameObject);
        }
    }
}
