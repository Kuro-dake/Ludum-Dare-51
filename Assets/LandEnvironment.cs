using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LandEnvironment : MonoBehaviour
{
    public string env_name;
    [SerializeField] public List<string> dialogue_filename;
    private int current_dialogue;
    public string tempo = "8tempo";
    public int level_moves = 10;
    public Transform bunni_point;
    public int glyphs = 2;

    public Transform bird_float_point;
    public Transform bird_sit_point;

    private void Start()
    {
        foreach (Transform t in new Transform[] { bird_float_point, bird_sit_point, bunni_point })
        {
            if (t != null && t.GetComponent<SpriteRenderer>() != null)
            {
                t.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
        
    }

    public string next_dialogue
    {
        get
        {
            string ret = dialogue_filename[current_dialogue];
            current_dialogue++;
            if (current_dialogue >= dialogue_filename.Count)
            {
                current_dialogue = 1;
            }

            return ret;
        }
    }
    
    
}
