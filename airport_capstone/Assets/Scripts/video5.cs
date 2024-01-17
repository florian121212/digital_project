using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class video5 : MonoBehaviour
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
    public Transform dol2Transform;
    public Transform dolLoadedTransform;
    
    // 4) Palettes 
    public Transform palletTransform;
    public Transform pallet2Transform;
    
    // GameObject représentant le highloader
    public GameObject hl;

    // Variables de contrôle :
    // 1) Si 'true', la cargaison chargée doit suivre le mouvement d'un autre véhicule
    private bool load1 = false;
    private bool load2 = false;
    
    // 2) Si 'true', la palette est chargée sur 'dol2'
    private bool loadpal1 = false;
    private bool loadpal2 = false;
    
    // 3) False quand le SpeedLoader aura déchargé la cargaison sur 'dol'
    private bool sl = true;
    
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
        slTransform.position = new Vector3(138.54f,0.0f,546.12f);
        slTransform.rotation = Quaternion.Euler(0,180f,0);
        sl2Transform.position = new Vector3(139.5f,0.0f,529.4f);
        sl2Transform.rotation = Quaternion.Euler(0,180f,0);
        slLoadedTransform.position = new Vector3(153f,0.0f,558.5f);
        slLoadedTransform.rotation = Quaternion.Euler(0,-180,0);
        dolTransform.position = new Vector3(122f,0.0f,536f);
        dolTransform.rotation = Quaternion.Euler(0,90,0);
        dolLoadedTransform.position = new Vector3(130.56f,0.0f,551.25f);
        dolLoadedTransform.rotation = Quaternion.Euler(0,0,0);
        dol2Transform.position = new Vector3(105f,0.0f,536f);
        dol2Transform.rotation = Quaternion.Euler(0,90,0);
        
        // Lancement de la coroutine runall()
        StartCoroutine(runall()); 
    }

    IEnumerator runall()
    {
        // Cargaisons : Avion > High-loader > SpeedLoader
        part1.Play();
        yield return new WaitForSeconds(6.5f);
        loadpal1 = true;
        yield return new WaitForSeconds(0.5f);
        hl.tag = "HighloaderDown"; // Changement du tag du High-Loader pour la labelisation
        yield return new WaitForSeconds(2.0f);
        loadpal2 = true;
        while(part1.state == PlayState.Playing)
        {
            yield return null;
        }
        // Les palettes sont chargées sur 'dol2'
        load1 = true;
        load2 = true;
        palletTransform.parent = null;
        pallet2Transform.parent = null;
        
        // SpeedLoader > Dollies
        part2.Play();
        yield return new WaitForSeconds(1.0f);
        hl.tag = "Highloader"; // Changement du tag du High-Loader pour la labelisation
        yield return new WaitForSeconds(2.0f);
        
        // Passage de la cargaison 1 de sl à dol
        load1 = false;
        uld1dol.Play();
        yield return new WaitForSeconds(2.0f);
        
        // Passage de la cargaison 2 de sl2 à dol
        load2 = false;
        uld2dol.Play();
        yield return new WaitForSeconds(2.0f);
        hl.tag = "HighloaderDown"; // Changement du tag du High-Loader pour la labelisation
        while(uld2dol.state == PlayState.Playing)
        {
            yield return null;
        }
        
        // Cargaisons : Dollies (dol)
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
            float angle = dol2Transform.eulerAngles.y * Mathf.Deg2Rad;
            float rel_z = -Mathf.Cos(angle) * offset1;
            float rel_x = -Mathf.Sin(angle) * offset1;
            uldTransform.position = dol2Transform.position + new Vector3(rel_x, -0.25f, rel_z);
            uldTransform.rotation = dol2Transform.rotation * rot_offset;   
        }
        
        // La cargaison 2 est sur le Dolly => elle suit son mouvement
        if(load2 && !sl)
        {
            float offset2 = 10.53f;
            float angle = dol2Transform.eulerAngles.y * Mathf.Deg2Rad;
            float rel_z = -Mathf.Cos(angle) * offset2;
            float rel_x = -Mathf.Sin(angle) * offset2;
            uld2Transform.position = dol2Transform.position + new Vector3(rel_x, -0.25f, rel_z);
            uld2Transform.rotation = dol2Transform.rotation * rot_offset;
        }
        
        // La palette 1 est sur dol2 => elle suit son mouvement
        if(loadpal1)
        {
            float offset1 = 5.15f;
            float angle = dolTransform.eulerAngles.y * Mathf.Deg2Rad;
            float rel_z = -Mathf.Cos(angle) * offset1;
            float rel_x = -Mathf.Sin(angle) * offset1;
            palletTransform.position = dolTransform.position + new Vector3(rel_x, -0.25f, rel_z);
            palletTransform.rotation = dolTransform.rotation * rot_offset;   
        }
        
        // La palette 2 est sur dol2 => elle suit son mouvement
        if(loadpal2)
        {
            float offset2 = 10.53f;
            float angle = dolTransform.eulerAngles.y * Mathf.Deg2Rad;
            float rel_z = -Mathf.Cos(angle) * offset2;
            float rel_x = -Mathf.Sin(angle) * offset2;
            pallet2Transform.position = dolTransform.position + new Vector3(rel_x, -0.25f, rel_z);
            pallet2Transform.rotation = dolTransform.rotation * rot_offset;
        }
    }
}
