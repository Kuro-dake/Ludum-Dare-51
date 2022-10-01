using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
public abstract class CoroutineContainerAware : ScriptableObject
{
    string object_name { get { return GetType().ToString(); } }

    public Coroutine StartCoroutine(IEnumerator enumerator, string routine_name = "")
    {
        Routines r = SC.routines;
        return SC.routines.StartCoroutine(enumerator, routine_name.Length > 0 ? object_name + "_" + routine_name : "");
    }

    public void StopCoroutine(string routine_name)
    {
        SC.routines.StopCoroutine(object_name + "_" + routine_name);
    }

    public bool IsRunning(string routine_name)
    {
        return SC.routines.IsRunning(object_name + "_" + routine_name);
    }

}

public abstract class Service : CoroutineContainerAware
{
    [SerializeField]
    bool _active = true;
    public bool active
    {

        get => _active;
        set
        {
            _active = value;

        }
    }

    private Transform _service_transform;
    [SerializeField]
    protected Transform service_transform_prefab = null;
    public virtual Transform service_transform
    {
        get
        {
            if (_service_transform == null)
            {
                if (service_transform_prefab == null)
                {
                    _service_transform = new GameObject(GetType().ToString() + "Service").transform;
                }
                else
                {
                    _service_transform = Instantiate(service_transform_prefab);
                    _service_transform.name = GetType().ToString() + "Service";

                }
                _service_transform.position = Vector3.zero * 1000f;
            }
            return _service_transform;
        }
    }
    public bool initialized { get; protected set; } = false;
    /// <summary>
    /// Initialize service for the game wide operations here. Avoid calling other services, since it will cause an exception - use RegisterEventsPostInitialize() for that
    /// </summary>
    public virtual void GameStartInitialize()
    {
        initialized = true;
    }

    public virtual void PostGameStartInitServices()
    {

    }
    public virtual void PostSceneStartInitServices()
    {

    }
    /// <summary>
    /// Initialize service for the current scene here. Avoid calling other services, since it will cause an exception (TODO: implement RegisterEventsPostInitialize() variant for scene if needed)
    /// </summary>
    public virtual void SceneStartInitialize()
    {

    }

    public virtual string sub_services_folder => "";
    public void SetSubServices(List<Service> _sub_services)
    {
        sub_services = _sub_services;
    }
    [System.NonSerialized]
    protected List<Service> sub_services = new List<Service>();
    public T Get<T>() where T : Service => sub_services.Find(s => s is T) as T;

    public virtual void SubControls()
    {

    }

    public virtual void Update() { }

    /*public static void Print(string s) {
        Debug.Log(s);
    }*/


    public void InstantiateIfNotPresent<T>(T prefab, ref T obj) where T : MonoBehaviour
    {
        if (obj == null)
        {
            obj = Instantiate(prefab, service_transform, false);

        }
    }

}
