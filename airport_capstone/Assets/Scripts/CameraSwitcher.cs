using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables; // Ajoutez cette ligne pour utiliser PlayableDirector

public class CameraSwitcher : MonoBehaviour
{
    public Camera[] cameras; 
    public float switchDelay = 19.0f; 
    public PlayableDirector part1;
    public PlayableDirector part2;
    public PlayableDirector uld1dol;
    public PlayableDirector uld2dol;
    private int currentCameraIndex = 0;
    private float switchTime = 0;

    public Transform uldTransform;
    public Transform uld2Transform;
    public Transform slTransform;
    public Transform sl2Transform; 
    public Transform dolTransform;

    private bool load1 = false;
    private bool load2 = false;
    private bool sl = true;

    private void Start()
    {
        for (int i = 1; i < cameras.Length; i++)
        {
            cameras[i].gameObject.SetActive(false);
        }

        // Initialisez le temps de commutation
        switchTime = Time.time + switchDelay;

        StartCoroutine(runall()); 
    }

    IEnumerator runall()
    {
        load1 = false;
        load2 = false;
        sl = true;
        part1.Play();
        while(part1.state == PlayState.Playing)
        {
            yield return null;
        }
        load1 = true;
        load2 = true;
        part2.Play();
        yield return new WaitForSeconds(5.0f);
        load1 = false;
        uld1dol.Play();
        yield return new WaitForSeconds(1.5f);
        load2 = false;
        uld2dol.Play();
        while(uld2dol.state == PlayState.Playing)
        {
            yield return null;
        }
        sl = false;
        load1 = true;
        load2 = true;
        yield return null;
    }

    private void Update()
    {
        if (Time.time >= switchTime)
        {
            // D�sactivez la cam�ra actuelle
            cameras[currentCameraIndex].gameObject.SetActive(false);

            // Passez � la cam�ra suivante
            currentCameraIndex = (currentCameraIndex + 1) % cameras.Length;

            // Activez la nouvelle cam�ra
            cameras[currentCameraIndex].gameObject.SetActive(true);

        
            // Initialisez le prochain temps de commutation
            switchTime = Time.time + switchDelay;
            part1.Stop();
            part1.time = 0;

            part2.Stop();
            part2.time = 0;

            uld1dol.Stop();
            uld1dol.time = 0;

            uld2dol.Stop();
            uld2dol.time = 0;

            StartCoroutine(runall()); 
        }

        if(load1 && sl)
        {
           uldTransform.position = slTransform.position;
           uldTransform.rotation = slTransform.rotation * Quaternion.Euler(0, -90, 0);
           
        }
        if(load2 && sl)
        {
           uld2Transform.position = sl2Transform.position;
           uld2Transform.rotation = sl2Transform.rotation * Quaternion.Euler(0, -90, 0); 
        }
        if(load1 && !sl)
        {
           uldTransform.position = dolTransform.position + new Vector3(4.538f, 0.68f, 0f);
           uldTransform.rotation = dolTransform.rotation * Quaternion.Euler(0, -90, 0);
           
        }
        if(load2 && !sl)
        {
           uld2Transform.position = dolTransform.position + new Vector3(9.933f, 0.68f, 0f);
           uld2Transform.rotation = dolTransform.rotation * Quaternion.Euler(0, -90, 0); 
        }
    }
}
