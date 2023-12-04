using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class deplacementuld : MonoBehaviour
{
    public PlayableDirector highLoaderDirector; // Référence au PlayableDirector du High Loader
    public Transform targetLocation; // L'endroit où vous voulez que l'ULD se déplace
    public Transform speedLoader; // Référence au Speed Loader que vous souhaitez faire arriver devant la plateforme
    public PlayableDirector doorDirector;
    public Transform speedLoaderDestination; // Référence à l'objet de destination du Speed Loader

    private bool isMoving = false;
    private Transform uld; // Référence à l'ULD

    void Start()
    {
        uld = transform; // Récupérer la référence de l'ULD
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isMoving)
        {
            StartCoroutine(DoorAnimation());
        }
    }
IEnumerator DoorAnimation()
    {
        
        doorDirector.Play();

        yield return new WaitForSeconds((float)doorDirector.duration);

       
        yield return StartCoroutine(MoveULDToTarget());
    }
    

    IEnumerator MoveULDToTarget()
    {   
        doorDirector.Play();
        
        isMoving = true;

        // Déplacez l'ULD vers la cible (vous pouvez utiliser Vector3.MoveTowards, Lerp, ou d'autres méthodes)
        while (Vector3.Distance(transform.position, targetLocation.position) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetLocation.position, Time.deltaTime * 2f); // Vitesse de déplacement
            yield return null;
        }

        // Une fois l'ULD arrivé à destination, attachez-le à la plateforme pour qu'il la suive
        AttachULDToPlatform();

        // Ensuite, déclenchez l'animation de descente du High Loader via le PlayableDirector
        PlayHighLoaderAnimation();

        // Déplacez le Speed Loader vers la plateforme
        yield return StartCoroutine(MoveSpeedLoaderToPlatform());

        // Une fois le Speed Loader arrivé à destination, détachez l'ULD de la plateforme
        DetachULDFromPlatform();

        // Après avoir détaché l'ULD, commencez son déplacement vers le Speed Loader
        yield return StartCoroutine(MoveULDToSpeedLoader());

        // Une fois que l'ULD a atteint le Speed Loader, faites déplacer le Speed Loader vers sa destination
        MoveSpeedLoaderToDestination();

        isMoving = false;
    }
    public void StartMovingSpeedLoader()
    {
    StartCoroutine(MoveSpeedLoaderToPlatform());
    }

    void AttachULDToPlatform()
    {
        // Attacher l'ULD à la plateforme en faisant de la plateforme le parent de l'ULD
        uld.parent = targetLocation;
    }

    void PlayHighLoaderAnimation()
    {
        // Activer le PlayableDirector pour déclencher l'animation du High Loader
        highLoaderDirector.Play();
    }

    public float speed = 1.0f; // Vitesse de déplacement du Speed Loader, ajustez-la dans l'inspecteur Unity si nécessaire
    private float journeyLength;
    private float startTime;

    IEnumerator MoveSpeedLoaderToPlatform()
    {
    Vector3 targetPosition = GameObject.Find("PositionSpeedLoader").transform.position;
    float journeyLength = Vector3.Distance(speedLoader.position, targetPosition);
    float startTime = Time.time;

    while (speedLoader.position != targetPosition)
    {
        float distanceCovered = (Time.time - startTime) * speed;
        float fractionOfJourney = distanceCovered / journeyLength;

        speedLoader.position = Vector3.Lerp(speedLoader.position, targetPosition, fractionOfJourney);

        yield return null;
    }
    }

    void DetachULDFromPlatform()
    {
        // Détacher l'ULD de la plateforme en affectant le parent à null.
        uld.parent = null;
    }

    IEnumerator MoveULDToSpeedLoader()
    {
    // Calculez la position cible à une distance égale au double de la distance actuelle entre l'ULD et le Speed Loader
    Vector3 directionToSpeedLoader = speedLoader.position - transform.position;
    Vector3 targetPosition = transform.position + 2 * directionToSpeedLoader;

    // Déplacez l'ULD vers la nouvelle position cible
    while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * 2f); // Vitesse de déplacement
        yield return null;
    }
        uld.parent = speedLoader;
    }

    void MoveSpeedLoaderToDestination()
    {
        // Déplacez le Speed Loader vers la destination configurée via l'inspecteur Unity.
        StartCoroutine(MoveSpeedLoaderToPosition(speedLoaderDestination.position));
    }

    IEnumerator MoveSpeedLoaderToPosition(Vector3 targetPosition)
    {
    float journeyLength = Vector3.Distance(speedLoader.position, targetPosition);
    float startTime = Time.time;

    while (Vector3.Distance(speedLoader.position, targetPosition) > 0.01f)
    {
        float distanceCovered = (Time.time - startTime) * speed;
        float fractionOfJourney = distanceCovered / journeyLength;

        speedLoader.position = Vector3.Lerp(speedLoader.position, targetPosition, fractionOfJourney);

        yield return null;
    }

    // Détacher l'ULD du Speed Loader une fois qu'il a atteint sa destination finale
    uld.parent = null;

    // Une fois que le Speed Loader a atteint sa destination finale, déplacez l'ULD vers le dolies
    yield return StartCoroutine(MoveULDToDolies());
    }

    public Transform doliesDestination; // Référence à l'objet de destination du mouvement vers le dolies
    IEnumerator MoveULDToDolies()
    {
    
    // Utilisez Vector3.MoveTowards ou une autre méthode pour déplacer l'ULD vers la position du "dolies"
    Vector3 targetPosition = doliesDestination.position;

    while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * 2f); // Vitesse de déplacement
        yield return null;
    }
    uld.parent = doliesDestination;
    yield return StartCoroutine(MoveTrainDeDolies());
    }
    public Transform trainDeDolies;
    public Transform trainDestination;

    IEnumerator MoveTrainDeDolies()
    {
    Vector3 targetPosition = trainDestination.position;
    float journeyLength = Vector3.Distance(trainDeDolies.position, targetPosition);
    float startTime = Time.time;

    while (Vector3.Distance(trainDeDolies.position, targetPosition) > 0.01f)
    {
        float distanceCovered = (Time.time - startTime) * speed;
        float fractionOfJourney = distanceCovered / journeyLength;

        trainDeDolies.position = Vector3.Lerp(trainDeDolies.position, targetPosition, fractionOfJourney);

        yield return null;
    }

    
    }


}