using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rotator : MonoBehaviour
{

    [SerializeField]
    public List<RotatorData> rotations;

    // Update is called once per frame
    void Update()
    {
        rotations.ForEach(r => r.what.Rotate(Vector3.forward * r.speed * Time.deltaTime));
    }
}

[System.Serializable]
public class RotatorData
{
    public Transform what;
    public float speed;
}