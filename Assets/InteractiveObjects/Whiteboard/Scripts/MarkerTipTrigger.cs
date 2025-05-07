public class MarkerTipTrigger : MonoBehaviour
{
    public WhiteboardMarker marker;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Whiteboard"))
        {
            marker.SetTipTouching(true, other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Whiteboard"))
        {
            marker.SetTipTouching(false, null);
        }
    }
}
