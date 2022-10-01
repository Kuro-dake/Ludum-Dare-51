using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;
using IEnumRunner;
using IEnumRunner.Transitions;

public class EnvManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SC.Initialize();
    }

    [SerializeField] private CinemachineVirtualCamera vcam, vcam_follow;
    private bool camFollowPlayer;

    private bool cam_follow_player
    {
        get => camFollowPlayer;
        set
        {
            camFollowPlayer = value;
            vcam.Priority = value ? 0 : 1;
            vcam_follow.Priority = value ? 1 : 0;
        }
    }
    private Player player => Player.inst;

    private Coroutine switch_game_step;
    
    public void StartGame()
    {
        if (switch_game_step != null)
        {
            return;
        }

        switch_game_step = StartCoroutine(StartGameStep());
    }

    IEnumerator StartGameStep()
    {
        yield return Make.The(player).In(1f).MoveTo((Vector2)Camera.main.transform.position + Vector2.up * .4f ).Execute();
        IEnumerator pfloat = Make.The(player).In(.5f).FixedTransition().MoveBy(Vector2.up * .2f ).Execute();
        
        yield return Game.inst.StartGame();
        yield return pfloat;
        //yield return new WaitForSeconds(1f);
        yield return Make.The(player).In(.4f).MoveTo((Vector2)Game.inst.transform.position ).Execute();
        yield return new WaitForSeconds(.2f);
        
        cam_follow_player = true;
        
        Game.inst.SetStartCountdown();
        
        switch_game_step = null;
    }

    private bool game_started = false;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartGame();
        }
    }
}
