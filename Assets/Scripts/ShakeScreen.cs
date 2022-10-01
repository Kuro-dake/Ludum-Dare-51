using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeScreen : MonoBehaviour
{
    Coroutine shake_routine;
    [SerializeField]
    List<Pair<string, ShakeScreenData>> variants = new List<Pair<string, ShakeScreenData>>();
    
    public void Shake(string variant, float delay = 0f)
    {
        Debug.Log("shake");
        if(shake_routine != null)
        {
            StopCoroutine(shake_routine);
        }

        shake_routine = StartCoroutine(ShakeStep(variants.Find(v=>v.first == variant).second, delay));
    }

    public void Initialize()
    {
        orig_pos = transform.position;
        orig_ortho_size = cam.orthographicSize;
    }
    Camera cam => GetComponent<Camera>();
    Vector3 orig_pos;
    float orig_ortho_size;
    
    IEnumerator ShakeStep(ShakeScreenData data, float delay)
    {

        float intensity = data.intensity, falloff = data.falloff, stop_intensity = data.stop_intensity, angle_intensity_multiplier = data.angle_intensity_multiplier
            , ortho_size_intensity_multiplier = data.ortho_size_intensity_multiplier, ortho_size_diff_limit= data.ortho_size_diff_limit, restore_speed = data.restore_speed;
        bool inverse = data.inverse;
        

        float orig_intensity = intensity;
        intensity = inverse ? stop_intensity : intensity;
        int modifier = inverse ? -1 : 1;
        yield return new WaitForSeconds(delay);
        
        
        while(inverse ? intensity < orig_intensity : intensity > stop_intensity)
        {
            intensity -= Time.deltaTime * falloff * intensity * modifier;
            
            //transform.position = Vector3.MoveTowards(transform.position, orig_pos + (Vector3)Random.insideUnitCircle, Time.deltaTime * intensity);
            //transform.Rotate(Vector3.forward * Common.EitherOr() * intensity * angle_intensity_multiplier);

            cam.orthographicSize = Mathf.Clamp(orig_ortho_size + Common.EitherOr() * Random.Range(ortho_size_intensity_multiplier, ortho_size_intensity_multiplier * 2f) * intensity * ortho_size_intensity_multiplier, orig_ortho_size, orig_ortho_size + ortho_size_diff_limit);
            
            transform.position = orig_pos + (Vector3)Random.insideUnitCircle * intensity;
            transform.localRotation = Quaternion.Euler(Vector3.forward * Random.Range(ortho_size_intensity_multiplier, ortho_size_intensity_multiplier *2f) * intensity);

            yield return null;
        }
        float t = 0f;
        Vector3 current_pos = transform.position;
        Quaternion current_angle = transform.localRotation;
        float current_ortho_size = cam.orthographicSize;
        while (t < 1f)
        {
            t += Time.deltaTime * restore_speed;
            transform.position = Vector3.Lerp(current_pos, Vector3.back * 15f,t);
            transform.localRotation = Quaternion.Lerp(current_angle, Quaternion.identity, t);
            cam.orthographicSize = Mathf.Lerp(current_ortho_size, orig_ortho_size, t);
            yield return null;
        }
        
    }
    
    [System.Serializable]
    public class ShakeScreenData
    {
        public float intensity = 10f, falloff = 2f;
        public float angle_intensity_multiplier = .5f, ortho_size_intensity_multiplier = .1f, angle_limit = 3f, stop_intensity = .3f, ortho_size_diff_limit = .1f;
        public float restore_speed = 10f;
        public bool inverse = false;
    }
}
