using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
public class BeatTracker : MonoBehaviour
{
    // Start is called before the first frame update
    private int beat_count = 0;
    private float last_beat_time = 0;
    public const float beat_time = .46875f;
    private AudioSource source => GetComponent<AudioSource>();
    private int total_count;
    public static int total_beats => inst.total_count;
   
    private static BeatTracker inst;
    void Start()
    {
        inst = this;
    }

    public static event System.EventHandler<OnBeatArgs> onBeat; 

    // Update is called once per frame
    void Update()
    {

        if (last_beat_time > source.time)
        {
            beat_count = 0;
            
        }
        
        if (source.time > beat_count * beat_time)
        {
           
            onBeat?.Invoke(this, new OnBeatArgs(total_count, 8));
            beat_count++;
            total_count++;
//            Debug.Log(total_count);
        }

        last_beat_time = source.time;
    }
    
    
}

public class OnBeatArgs : System.EventArgs
{
    protected int beat_number;
    protected int tempo;

    public BeatValidator validator { get; protected set; }
    
    public int GetBeatNumber(int every = -1)
    {
        return (every == -1 ? beat_number : beat_number % every);
    }

    
    public OnBeatArgs(int beatNumber, int tempo)
    {
        beat_number = beatNumber;
        validator = BeatValidator.GetValidator(tempo, beatNumber);
    }
}
