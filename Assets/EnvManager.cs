using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;
using IEnumRunner.Transitions;

public class EnvManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SC.Initialize();
    }

    [SerializeField] private CinemachineVirtualCamera vcam;
    private Player player => Player.inst;
    public void StartGame()
    {
        vcam.Follow = player.transform;
        Make.The(player).In(.5f).MoveTo((Vector2)Camera.main.transform.position).then.MakeHappen(() => Game.game_started = true)
            .Happen();
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
