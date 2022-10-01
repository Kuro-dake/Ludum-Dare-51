using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Shaker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        poses = new Vector2[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            poses[i] = transform.GetChild(i).localPosition;
        }
    }
    [SerializeField]
    bool exclusive = true;
    Vector2[] poses;
    [SerializeField]
    List<GameObject> exclude_from_shaking;
    [SerializeField]
    float intensity;
    // Update is called once per frame
    void Update()
    {
        
        for (int i = 0; i < transform.childCount; i++)
        {
            bool contains = exclude_from_shaking.Contains(transform.GetChild(i).gameObject);
            if (exclusive && contains || !exclusive && !contains)
            {
                continue;
            }
            transform.GetChild(i).localPosition = poses[i] + Random.insideUnitCircle * intensity;
        }
    }
}
