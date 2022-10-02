using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IEnumRunner;
using IEnumRunner.Transitions;
public class MovementIndicator : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 orig_scale;
    void Start()
    {
        orig_scale = transform.localScale;
        PlaceAt(direction.none);
    }

    private Sequence placement_sequence;
    public void PlaceAt(direction dir)
    {
        placement_sequence?.Stop();
        Make._Batch b = Make.The(gameObject).In(.1f).ScaleTo(0f);

        if (dir != direction.none)
        {
            Transform t = Game.GetDirectionTransform(dir);
            b = b.then.instantly.MoveTo((Vector2)t.transform.position).then.In(.1f).ScaleTo(orig_scale);
        }

        placement_sequence = b.Happen();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
