using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    [SerializeField] List<Transform> waypoints;
    [SerializeField] float moveSpeed = 2;
    private int target = 0;

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, waypoints[target].position, moveSpeed * Time.deltaTime);
    }

    private void FixedUpdate() {
        if(transform.position == waypoints[target].position){
            target = ++target % waypoints.Count;
        }
    }
}
