using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Routines", menuName = "Manager/Routines", order = 0)]
public class Routines : ScriptableObject
{
    CoroutineContainer _c = null;
    CoroutineContainer container
    {
        get
        {
            if (_c == null)
            {
                _c = new GameObject("CoroutineContainer", new System.Type[] { typeof(CoroutineContainer) }).GetComponent<CoroutineContainer>();
                //DontDestroyOnLoad(_c);
            }
            return _c;
        }
    }

    public Coroutine StartCoroutine(IEnumerator enumerator, string routine_name = "")
    {
        if (routine_name.Length > 0)
        {
            return container.CStart(routine_name, enumerator);
        }
        else
        {
            return container.StartCoroutine(enumerator);
        }
    }
    public void StopAllCoroutines() => container._StopAllCoroutines();
    public void StopCoroutine(string routine_name)
    {
        container.Stop(routine_name);
    }

    public bool IsRunning(string routine_name)
    {
        return container.IsRunning(routine_name);
    }

    public IEnumerator WaitForRoutineEnd(string routine_name)
    {
        while (IsRunning(routine_name))
        {
            yield return null;
        }
    }

}
