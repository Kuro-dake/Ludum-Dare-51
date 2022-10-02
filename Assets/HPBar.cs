using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using IEnumRunner.Transitions;



public class HPBar : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        BeatTracker.onBeat += OnBeat;
    }

    [SerializeField] private GameObject indicator_prefab;

    void OnBeat(object sender, object args)
    {
        GetComponentsInChildren<MakeTransitions>().ToList().ForEach(mt=>mt.Trigger("blob"));
    }
    
    private int _hp;
    [SerializeField]
    private float margin = 1f;
    public int hp
    {
        get => _hp;
        set
        {
            _hp = value;
            bool carrot_sent = false;
            GetComponentsInChildren<MakeTransitions>().ToList().ForEach(mt=>
            {
                Transform t = mt.transform.parent;
                if (!carrot_sent)
                {
                    carrot_sent = true;
                    float rot = 540f * Common.EitherOr();
                    Make.The(t).In(.3f).MoveTo(Game.inst.transform.position).RotateBy(rot).ScaleTo(2f).then.
                        ScaleTo(0f).RotateBy(rot).then.
                        MakeHappen(() => Destroy(mt)).Happen();
                }
                else
                {
                    Make.The(t).In(.2f).ScaleTo(0f).then.MakeHappen(() => Destroy(mt)).Happen();                    
                }
                
                t.transform.SetParent(null);
            });
            for (int i = 0; i < hp; i++)
            {
                Transform t = Instantiate(indicator_prefab).transform;
                t.SetParent(transform);
                t.localPosition = Vector3.right * i * margin;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
