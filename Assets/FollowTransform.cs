using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    [SerializeField] private Transform follow;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, follow.position, Time.deltaTime * 2f);
    }
}
