using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlateformAnimationEventController : MonoBehaviour
{
    public PlayableDirector platformDirector; // Référence au PlayableDirector de la plateforme
    public deplacementuld deplacementULD; // Référence au script de déplacement de l'ULD
    public GameObject highLoader; // Référence au GameObject du High Loader

    void Start()
    {
        // Ajoutez un gestionnaire d'événements pour l'événement à la fin de l'animation de la plateforme
        platformDirector.stopped += OnPlatformAnimationFinished;
    }

    void OnPlatformAnimationFinished(PlayableDirector director)
    {
        // L'animation de la plateforme est terminée, démarrez le déplacement du Speed Loader
        deplacementULD.StartMovingSpeedLoader();

        // Changer le tag du High Loader une fois la plateforme terminée
        if (highLoader != null)
        {
            highLoader.tag = "HighloaderDown";
        }
    }
}
