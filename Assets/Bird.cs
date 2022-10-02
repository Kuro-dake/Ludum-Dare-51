using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IEnumRunner.Transitions;
using UnityEngine;

public class Bird : MonoBehaviour
{
    private Animator anim => GetComponentInChildren<Animator>();

    public bool flying;
    public bool flaping
    {
        get => anim.GetBool("flyin");
        set => anim.SetBool("flyin", value);
    } 
    // Start is called before the first frame update
    public static Bird inst;
    
    void Start()
    {
        inst = this;
    }

    private float sine_mul = 0;
    [SerializeField] private float sine_range;

    [SerializeField] private float sine_speed = 2f;
    // Update is called once per frame
    void Update()
    {
        sine_mul = Mathf.MoveTowards(sine_mul, flying ? 1f : 0f, Time.deltaTime * 3f);

        Transform t = transform.GetChild(0);

        t.localPosition = Vector3.up * Mathf.Sin(Time.time * sine_speed) * sine_mul * sine_range;

    }

    public void ResetBird()
    {
        flying = true;
        flaping = true;
        Debug.Log("reset bird");
        transform.position = new Vector2(13.189f, 8.59f);
    }

    public void Arrive()
    {
        Make.The(gameObject).In(.5f).MoveTo(EnvManager.current_environment.bird_float_point.position).Happen();
    }

    public void Sit()
    {
        Make.The(gameObject).In(.5f).MoveTo(EnvManager.current_environment.bird_sit_point.position).
            then.MakeHappen(() =>
            {
                flying = false;
                flaping = false;                
            }).Happen();
        
    }

    public void Leave()
    {
        Vector2 to = (Vector2)transform.position - new Vector2(13.189f, 0f);
        to.y = 8.59f;
        flying = true;
        flaping = true;
        Make.The(gameObject).In(.5f).MoveTo(to).Happen();
    }
    
}
