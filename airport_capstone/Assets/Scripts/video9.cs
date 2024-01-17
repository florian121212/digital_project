using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class video9 : MonoBehaviour
{ 
    // PlayableDirectors pour chaque partie de la vidéo
    public PlayableDirector part1; // Cargaisons : Avion> High-loader > SpeedLoader
    public PlayableDirector part2; // SpeedLoader > Dollies
    public PlayableDirector part3; // Cargaisons : Dollies
    public PlayableDirector uld1dol; // Passage de la cargaison 1 du SpeedLoader au Dolly
    public PlayableDirector uld2dol; // Passage de la cargaison 2 du SpeedLoader au Dolly
    public PlayableDirector uld3dol; // Passage de la cargaison 3 du SpeedLoader au Dolly
    public PlayableDirector uld4dol; // Passage de la cargaison 4 du SpeedLoader au Dolly
    
    // Transform des objets :
    // 1) Cargaisons
    public Transform uldTransform;
    public Transform uld2Transform;
    public Transform uld3Transform;
    public Transform uld4Transform;
    
    // 2) SpeedLoader
    public Transform slTransform;
    public Transform sl2Transform;
    
    // 3) Dollies
    public Transform dolTransform;
    public Transform dol2Transform;
    public Transform dolLoadedTransform;
    
    // GameObject représentant le highloader
    public GameObject hl;

    // Variables de contrôle :
    // 1) Cargaison chargée doit suivre le mouvement d'un autre véhicule = true
    private bool load1 = false;
    private bool load2 = false;
    private bool load3 = false;
    private bool load4 = false;
    
    // 2) False quand le SpeedLoader aura déchargé la cargaison sur le Dolly
    private bool sl1 = true;
    private bool sl2  = true;
    
    // Rotation offset pour les cargaisons
    private Quaternion rot_offset = Quaternion.Euler(0, -90, 0);

    void Start()
    {
        // Initialisation des positions et rotations des objets
        uldTransform.position = new Vector3(72f,4f,559f);
        uldTransform.rotation = Quaternion.Euler(0,0,0);
        uld2Transform.position = new Vector3(72f,4f,564f);
        uld2Transform.rotation = Quaternion.Euler(0,0,0);
        uld3Transform.position = new Vector3(72f,4f,569f);
        uld3Transform.rotation = Quaternion.Euler(0,0,0);
        uld4Transform.position = new Vector3(72f,4f,573f);
        uld4Transform.rotation = Quaternion.Euler(0,0,0);
        slTransform.position = new Vector3(140.2f,0.0f,566f);
        slTransform.rotation = Quaternion.Euler(0,270f,0);
        sl2Transform.position = new Vector3(145.8f,0.0f,566f);
        sl2Transform.rotation = Quaternion.Euler(0,270f,0);
        dolTransform.position = new Vector3(110f,0.0f,560f);
        dolTransform.rotation = Quaternion.Euler(0,90,0);
        dolLoadedTransform.position = new Vector3(134.84f,0.0f,574.02f);
        dolLoadedTransform.rotation = Quaternion.Euler(0,-90,0);
        dol2Transform.position = new Vector3(90f,0.0f,590f);
        dol2Transform.rotation = Quaternion.Euler(0,-90,0);
        
        // Lancement de la coroutine runall()
        StartCoroutine(runall()); 
    }

    IEnumerator runall()
    {
        // Cargaisons : Avion > High-loader > SpeedLoader
        part1.Play();
        yield return new WaitForSeconds(7.0f);
        hl.tag = "HighloaderDown"; // Changement du tag du High-Loader pour la labelisation
        yield return new WaitForSeconds(4.3f);
        hl.tag = "Highloader"; // Changement du tag du High-Loader pour la labelisation
        yield return new WaitForSeconds(6.0f);
        hl.tag = "HighloaderDown"; // Changement du tag du High-Loader pour la labelisation
        while(part1.state == PlayState.Playing)
        {
            yield return null;
        }
        load1 = true;
        load2 = true;
        load3 = true;
        load4 = true;
        
        // SpeedLoader > Dollies
        part2.Play();
        yield return new WaitForSeconds(3.5f);
        
        // Passage de la cargaison 2 du SpeedLoader au Dolly
        load2 = false;
        load4 = false;
        sl2 = false;
        uld2dol.Play();
        yield return new WaitForSeconds(1.5f);
        
        // Passage de la cargaison 1 du SpeedLoader au Dolly
        load1 = false;
        load3 = false;
        sl1 = false;
        uld1dol.Play();
        while(uld2dol.state == PlayState.Playing)
        {
            yield return null;
        }
        
        // Passage de la cargaison 4 du SpeedLoader au Dolly
        load2 = true;
        uld4dol.Play();
        while(uld1dol.state == PlayState.Playing)
        {
            yield return null;
        }
        
        // Passage de la cargaison 3 du SpeedLoader au Dolly
        load1 = true;
        uld3dol.Play();
        while(uld3dol.state == PlayState.Playing)
        {
            yield return null;
        }
        
        // Cargaisons : Dollies
        load3 = true;
        load4 = true;
        part3.Play();
    }

    void Update()
    {
        // La cargaison 1 est sur le SpeedLoader 1 => elle suit son mouvement
        if(load1 && sl1)
        {
           float offset1 = 3.5f;
           float angle1 = slTransform.eulerAngles.y * Mathf.Deg2Rad;
           float rel_z = -Mathf.Sin(angle1) * offset1;
           float rel_x = Mathf.Cos(angle1) * offset1;
           uldTransform.position = slTransform.position + new Vector3(rel_x, -0.25f, rel_z);
           uldTransform.rotation = slTransform.rotation * rot_offset; 
        }
        
        // La cargaison 2 est sur le SpeedLoader 2 => elle suit son mouvement
        if(load2 && sl2)
        {
           float offset2 = 3.5f;
           float angle2 = sl2Transform.eulerAngles.y * Mathf.Deg2Rad;
           float rel_z2 = -Mathf.Sin(angle2) * offset2;
           float rel_x2 = Mathf.Cos(angle2) * offset2;
           uld2Transform.position = sl2Transform.position + new Vector3(rel_x2, -0.25f, rel_z2);
           uld2Transform.rotation = sl2Transform.rotation * rot_offset;
        }
        
        // La cargaison 3 est sur le SpeedLoader 1 => elle suit son mouvement
        if(load3 && sl1)
        {
           uld3Transform.position = slTransform.position - new Vector3(0f, 0.25f, 0f);
           uld3Transform.rotation = slTransform.rotation * rot_offset;
        }
        
        // La cargaison 4 est sur le SpeedLoader 2 => elle suit son mouvement
        if(load4 && sl2)
        {

           uld4Transform.position = sl2Transform.position - new Vector3(0f, 0.25f, 0f);
           uld4Transform.rotation = sl2Transform.rotation * rot_offset; 
        }
        
        // La cargaison 1 est sur le Dolly 2 => elle suit son mouvement
        if(load1 && !sl1)
        {
            float offset1 = 5.15f;
            float angle1 = dol2Transform.eulerAngles.y * Mathf.Deg2Rad;
            float rel_z = -Mathf.Cos(angle1) * offset1;
            float rel_x = -Mathf.Sin(angle1) * offset1;
            uldTransform.position = dol2Transform.position + new Vector3(rel_x, -0.25f, rel_z);
            uldTransform.rotation = dol2Transform.rotation * rot_offset;   
        }
        
        // La cargaison 2 est sur le Dolly 2 => elle suit son mouvement
        if(load2 && !sl2)
        {
            float offset2 = 5.15f;
            float angle2 = dolTransform.eulerAngles.y * Mathf.Deg2Rad;
            float rel_z2 = -Mathf.Cos(angle2) * offset2;
            float rel_x2 = -Mathf.Sin(angle2) * offset2;
            uld2Transform.position = dolTransform.position + new Vector3(rel_x2, -0.25f, rel_z2);
            uld2Transform.rotation = dolTransform.rotation * Quaternion.Euler(0,90,0);;
        }
        
        // La cargaison 3 est sur le Dolly 1 => elle suit son mouvement
        if(load3 && !sl1)
        {
            float offset3 = 10.53f;
            float angle3 = dol2Transform.eulerAngles.y * Mathf.Deg2Rad;
            float rel_z3 = -Mathf.Cos(angle3) * offset3;
            float rel_x3 = -Mathf.Sin(angle3) * offset3;
            uld3Transform.position = dol2Transform.position + new Vector3(rel_x3, -0.25f, rel_z3);
            uld3Transform.rotation = dol2Transform.rotation * rot_offset;   
        }
        
        // La cargaison 4 est sur le Dolly 2 => elle suit son mouvement
        if(load4 && !sl2)
        {
            float offset4 = 10.53f;
            float angle4 = dolTransform.eulerAngles.y * Mathf.Deg2Rad;
            float rel_z4 = -Mathf.Cos(angle4) * offset4;
            float rel_x4 = -Mathf.Sin(angle4) * offset4;
            uld4Transform.position = dolTransform.position + new Vector3(rel_x4, -0.25f, rel_z4);
            uld4Transform.rotation = dolTransform.rotation * Quaternion.Euler(0,90,0);;
        }
    }
}
