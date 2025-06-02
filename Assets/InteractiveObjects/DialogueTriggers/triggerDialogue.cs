using UnityEngine;


public class triggerDialogue : MonoBehaviour
{
    public AudioSource audioSource;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    // public XRDirectInteractor interactor;
    bool hasPlayed;
    void Start()
    {
        hasPlayed = false;
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrabbed);
    }

    void OnGrabbed(UnityEngine.XR.Interaction.Toolkit.SelectEnterEventArgs args)
    {
        if (!hasPlayed)
        {
            audioSource.Play();
            hasPlayed = true;
        }

    }
    void OnDestroy()
    {
        // Clean up event subscription
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        }
    }

}
