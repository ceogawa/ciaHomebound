// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System.Linq;
// using UnityEngine.InputSystem;
// using UnityEngine.XR.Interaction.Toolkit;

// public class MarkerTipTrigger : MonoBehaviour
// {
//     public WhiteboardMarker marker;

//     private void OnTriggerEnter(Collider other)
//     {
//         if (other.CompareTag("Whiteboard"))
//         {
//             marker.SetTipTouching(true, other);
//         }
//     }

//     private void OnTriggerExit(Collider other)
//     {
//         if (other.CompareTag("Whiteboard"))
//         {
//             marker.SetTipTouching(false, null);
//         }
//     }
// }
