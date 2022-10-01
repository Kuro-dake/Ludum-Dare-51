using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "WaddleManager", menuName = "Manager/WaddleManager", order = 0)]
public class WaddleManager : Service
{

    public override void SceneStartInitialize()
    {
        base.SceneStartInitialize();
        OnWaddleStart = null;
        OnWaddleStep = null;
        OnWaddleFinished = null;
    }
    public void Waddle(bool move_camera)
    {
        StartCoroutine(TChangeStep(move_camera));
    }


    [SerializeField]
    float _bounce_speed;
    public float bounce_speed => _bounce_speed;
    bool waddle_left;
    int last_change;
    IEnumerator TChangeStep(bool move_camera)
    {
        if (last_change-- <= 0)
        {
            last_change = 3;
        }
        else
        {
            waddle_left = !waddle_left;
        }
        OnWaddleStepArgs args = new OnWaddleStepArgs(move_camera, waddle_left);
        OnWaddleStart?.Invoke(this, args);
        float t = 0f;
        while (t < 1f)
        {

            t = Mathf.Clamp(t + Time.deltaTime * bounce_speed, 0f, 1f);
            args.SetT(t);
            OnWaddleStep?.Invoke(this, args);
            yield return null;
        }
        OnWaddleFinished?.Invoke(this, null);
    }

    // centralize waddle t and feed it into waddlers/camera ?

    public event System.EventHandler<OnWaddleStepArgs> OnWaddleStart;
    public event System.EventHandler<OnWaddleStepArgs> OnWaddleStep;
    public event System.EventHandler OnWaddleFinished;

    public class OnWaddleStepArgs : System.EventArgs
    {
        const float pi_half = Mathf.PI * .5f;
        public void SetT(float t) { 
            sin_t = Mathf.Sin(t * Mathf.PI);
            sin_t_half = Mathf.Sin(t * pi_half);
        }
        public float sin_t { get; protected set; }
        public float sin_t_half { get; protected set; }
        public bool move_camera { get; protected set; }
        public OnWaddleStepArgs(bool move_camera, bool left)
        {
            this.move_camera = move_camera;
            this.left = left;
        }
        public bool left { get; protected set; }
    }

}
