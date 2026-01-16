using System;
using TMPro;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    [SerializeField] private GameObject _endPosition;

    [SerializeField] private float _speed;

    private bool _isMoving = false;
    
    
    [DialogueEvent, Button]
    public void StartMoving()
    {
        _isMoving = true;
    }

    public void Update()
    {
        if (_isMoving)
        {
            Vector3 position = new Vector3();

            position.x = Mathf.Lerp(transform.position.x, _endPosition.transform.position.x, Time.deltaTime * _speed);
            position.y = Mathf.Lerp(transform.position.y, _endPosition.transform.position.y, Time.deltaTime * _speed);
            position.z = Mathf.Lerp(transform.position.z, _endPosition.transform.position.z, Time.deltaTime * _speed);

            transform.position = position;

            if (Vector3.Distance(transform.position, _endPosition.transform.position) <= 0.1)
            {
                _isMoving = false;
            }
        }
    }
}
