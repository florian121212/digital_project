using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class video1 : MonoBehaviour
{
    // PlayableDirectors pour chaque partie de la vidéo
    public PlayableDirector part1; // Cargaisons : Avion> High-loader > SpeedLoader
    public PlayableDirector part2; // SpeedLoader > Dollies
    public PlayableDirector part3; // Cargaisons : Dollies
    public PlayableDirector uld1dol; // Passage de la cargaison 1 du SpeedLoader au Dolly
    public PlayableDirector uld2dol; // Passage de la cargaison 2 du SpeedLoader au Dolly
    
    // Transform des objets :
    // 1) Cargaisons
    public Transform uldTransform;
    public Transform uld2Transform;
    public Transform uld3Transform;
    public Transform uld4Transform;
    
    // 2) SpeedLoader
    public Transform slTransform;
    public Transform sl2Transform;
    public Transform slLoadedTransform; 
    
    // 3) Dollies
    public Transform dolTransform;
    public Transform dolLoadedTransform;
    
    // GameObject représentant le highloader
    public GameObject hl;

    // Variables de contrôle :
    // 1) Si 'true', la cargaison chargée doit suivre le mouvement d'un autre véhicule
    private bool load1 = false;
    private bool load2 = false;
    
    // 2) False quand le SpeedLoader aura déchargé la cargaison sur le Dolly
    private bool sl = true;
    
    // Rotation offset pour les cargaisons
    private Quaternion rot_offset = Quaternion.Euler(0, -90, 0);

    void Start()
    {
        // Initialisation des positions et rotations des objets
        uldTransform.position = new Vector3(172f,4f,559f);
        uldTransform.rotation = Quaternion.Euler(0,0,0);
        uld2Transform.position = new Vector3(172f,4f,564f);
        uld2Transform.rotation = Quaternion.Euler(0,0,0);
        uld3Transform.position = new Vector3(172f,4f,569f);
        uld3Transform.rotation = Quaternion.Euler(0,0,0);
        uld4Transform.position = new Vector3(172f,4f,573f);
        uld4Transform.rotation = Quaternion.Euler(0,0,0);
        slTransform.position = new Vector3(137.82f,0.0f,569.5f);
        slTransform.rotation = Quaternion.Euler(0,219.04f,0);
        sl2Transform.position = new Vector3(137.98f,0.0f,559.57f);
        sl2Transform.rotation = Quaternion.Euler(0,220f,0);
        slLoadedTransform.position = new Vector3(153f,0.0f,558.5f);
        slLoadedTransform.rotation = Quaternion.Euler(0,-180,0);
        dolTransform.position = new Vector3(123.25f,0.0f,535.5f);
        dolTransform.rotation = Quaternion.Euler(0,270,0);
        dolLoadedTransform.position = new Vector3(133.8f,0.0f,579.9f);
        dolLoadedTransform.rotation = Quaternion.Euler(0,-90,0);

        // Lancement de la coroutine runall()
        StartCoroutine(runall()); 
    }

    IEnumerator runall()
    {
        // Cargaisons : Avion > High-loader > SpeedLoader
        part1.Play();
        yield return new WaitForSeconds(7.0f);
        hl.tag = "HighloaderDown"; // Changement du tag du High-Loader pour la labelisation
        while(part1.state == PlayState.Playing)
        {
            yield return null;
        }
        load1 = true;
        load2 = true;
        
        // SpeedLoader > Dollies
        part2.Play();
        yield return new WaitForSeconds(1.0f);
        hl.tag = "Highloader"; // Changement du tag du High-Loader pour la labelisation
        yield return new WaitForSeconds(4.0f);
        
        // Passage de la cargaison 1 du SpeedLoader au Dolly
        load1 = false;
        uld1dol.Play();
        yield return new WaitForSeconds(1.5f);
        
        // Passage de la cargaison 2 du SpeedLoader au Dolly
        load2 = false;
        uld2dol.Play();
        yield return new WaitForSeconds(0.5f);
        hl.tag = "HighloaderDown"; // Changement du tag du High-Loader pour la labelisation
        while(uld2dol.state == PlayState.Playing)
        {
            yield return null;
        }
        
        // Cargaisons : Dollies
        sl = false;
        load1 = true;
        load2 = true;
        part3.Play();
        
    }

    void Update()
    {
        // La cargaison 1 est sur le SpeedLoader => elle suit son mouvement
        if(load1 && sl)
        {
           uldTransform.position = slTransform.position - new Vector3(0f, 0.25f, 0f);
           uldTransform.rotation = slTransform.rotation * rot_offset;
           
        }
        
        // La cargaison 2 est sur le SpeedLoader => elle suit son mouvement
        if(load2 && sl)
        {
           uld2Transform.position = sl2Transform.position - new Vector3(0f, 0.25f, 0f);
           uld2Transform.rotation = sl2Transform.rotation * rot_offset; 
        }
        
        // La cargaison 1 est sur le Dolly => elle suit son mouvement
        if(load1 && !sl)
        {
            float offset1 = 5.15f;
            float angle = dolTransform.eulerAngles.y * Mathf.Deg2Rad;
            float rel_z = -Mathf.Cos(angle) * offset1;
            float rel_x = -Mathf.Sin(angle) * offset1;
            uldTransform.position = dolTransform.position + new Vector3(rel_x, -0.25f, rel_z);
            uldTransform.rotation = dolTransform.rotation * rot_offset;
           
        }
        
        // La cargaison 2 est sur le Dolly => elle suit son mouvement
        if(load2 && !sl)
        {
            float offset2 = 10.53f;
            float angle = dolTransform.eulerAngles.y * Mathf.Deg2Rad;
            float rel_z = -Mathf.Cos(angle) * offset2;
            float rel_x = -Mathf.Sin(angle) * offset2;
            uld2Transform.position = dolTransform.position + new Vector3(rel_x, -0.25f, rel_z);
           uld2Transform.rotation = dolTransform.rotation * rot_offset;
        }
    }
}
