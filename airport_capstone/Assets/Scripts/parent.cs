using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class parent : MonoBehaviour
{ 
    public PlayableDirector part1;
    public PlayableDirector part2;
    public PlayableDirector part3;
    public PlayableDirector uld1dol;
    public PlayableDirector uld2dol;
    public Transform uldTransform;
    public Transform uld2Transform;
    public Transform slTransform;
    public Transform sl2Transform; 
    public Transform dolTransform;

    void Start()
    {
        StartCoroutine(runall()); 
    }

    IEnumerator runall()
    {
        part1.Play();
        while(part1.state == PlayState.Playing)
        {
            yield return null;
        }
        uldTransform.parent = slTransform;
        uld2Transform.parent = sl2Transform;
        part2.Play();
        yield return new WaitForSeconds(5.0f);
        uldTransform.parent = null;
        uldTransform = slTransform;
        uld1dol.Play();
        yield return new WaitForSeconds(1.5f);
        uld2Transform.parent = null;
        uld2Transform = sl2Transform;
        uld2dol.Play();
        while(uld2dol.state == PlayState.Playing)
        {
            yield return null;
        }
        uldTransform.GetComponent<Animator>().enabled = false;
        uld2Transform.GetComponent<Animator>().enabled = false;

        uldTransform.localPosition = Vector3.zero;
        uldTransform.parent = dolTransform;

        uld2Transform.localPosition = Vector3.zero;
        uld2Transform.parent = dolTransform;

        uldTransform.GetComponent<Animator>().enabled = true;
        uld2Transform.GetComponent<Animator>().enabled = true;
        part3.Play(); 
    }
}