using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Crayon : MonoBehaviour
{
    [SerializeField] private Transform tip;
    [SerializeField] private int penSize = 10;
    [SerializeField] private bool lockRotationOnContact = true;
    [SerializeField] private float rotationLockSpeed = 10f;
    [SerializeField] private float minDrawDistance = 0.001f; // Minimum distance to move before drawing a new point
    [SerializeField] private int maxPointsPerFrame = 5; // Limit interpolated points per frame
    public AudioSource audioSource;
    // private bool hasPlayed = false;
    private float tipheight;
    private Renderer tipRenderer;
    private Color[] colors;
    private Vector2 lastTexturePos;
    private bool isDrawing = false;
    private Whiteboard currentWhiteboard = null;
    private Vector3 lastTipPosition;
    private Quaternion lockedRotation;
    private bool rotationLocked = false;
    
    // Performance optimization variables
    private float lastDrawTime;
    private const float MIN_DRAW_INTERVAL = 0.01f; // 100 draw operations per second max
    
    // Pending line tracking
    private List<Vector2> pendingPoints = new List<Vector2>();
    private bool processingLine = false;

    void Start()
    {
        // Get the tip renderer
        tipRenderer = tip.GetComponent<Renderer>();
        if (tipRenderer == null)
        {
            Debug.LogError("Tip needs a renderer component!");
            return;
        }

        // Create color array for drawing
        colors = Enumerable.Repeat(tipRenderer.material.color, penSize * penSize).ToArray();

        tipheight = tip.localScale.y;
        lastTipPosition = tip.position;
        lastDrawTime = Time.time;

        audioSource = GetComponentInParent<AudioSource>();
    }

    void Update()
    {
        // Handle rotation locking when drawing
        if (isDrawing && rotationLocked && lockRotationOnContact)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lockedRotation, Time.deltaTime * rotationLockSpeed);
        }
        
        // Process any pending drawing points
        if (pendingPoints.Count > 0 && !processingLine && currentWhiteboard != null)
        {
            StartCoroutine(ProcessPendingPoints());
        }
    }
    
    IEnumerator ProcessPendingPoints()
    {
        processingLine = true;
        
        // Process points in batches to spread the work over frames
        while (pendingPoints.Count > 1)
        {
            Vector2 start = pendingPoints[0];
            Vector2 end = pendingPoints[1];
            pendingPoints.RemoveAt(0);
            
            int startX = (int)(start.x - (penSize / 2));
            int startY = (int)(start.y - (penSize / 2));
            int endX = (int)(end.x - (penSize / 2));
            int endY = (int)(end.y - (penSize / 2));
            
            // Draw directly at end point
            currentWhiteboard.SetPixels(endX, endY, penSize, colors);
            
            // for (float f = 0.01f; f < 1.00f; f += 0.01f)
            // {
            //     var lerpX = (int)Mathf.Lerp(startX, endX, f);
            //     var lerpY = (int)Mathf.Lerp(startY, endY, f);
            //     currentWhiteboard.SetPixels(lerpX, lerpY, penSize, colors);
            // }

            // Limit points for performance
            int steps = Mathf.Min(maxPointsPerFrame, Mathf.CeilToInt(Vector2.Distance(start, end) / 5f));
            
            for (int i = 1; i <= steps; i++)
            {
                float t = (float)i / (steps + 1);
                int lerpX = (int)Mathf.Lerp(startX, endX, t);
                int lerpY = (int)Mathf.Lerp(startY, endY, t);
                
                currentWhiteboard.SetPixels(lerpX, lerpY, penSize, colors);
            }
            
            // Yield to prevent frame drops
            yield return null;
        }
        
        if (pendingPoints.Count > 0)
        {
            // Draw final point
            Vector2 final = pendingPoints[0];
            int finalX = (int)(final.x - (penSize / 2));
            int finalY = (int)(final.y - (penSize / 2));
            currentWhiteboard.SetPixels(finalX, finalY, penSize, colors, true);
            pendingPoints.Clear();
        }
        
        processingLine = false;
    }

    private void OnTriggerEnter(Collider other)
    {
    
        // Check if we hit a whiteboard
        if (!other.CompareTag("Whiteboard")) return;

        // if (!hasPlayed)
        // {
        //     audioSource.Play();
        //     hasPlayed = true;
        // }
        
        currentWhiteboard = other.GetComponent<Whiteboard>();
        if (currentWhiteboard == null) return;
        
        isDrawing = true;
        
        // Lock rotation when first touching
        if (lockRotationOnContact && !rotationLocked)
        {
            lockedRotation = transform.rotation;
            rotationLocked = true;
        }
        
        // Clear pending points and last position
        pendingPoints.Clear();
        lastTexturePos = Vector2.zero;
        
        // Initial drawing point
        if (Time.time - lastDrawTime >= MIN_DRAW_INTERVAL)
        {
            Vector2 pos = currentWhiteboard.WorldToTexturePosition(tip.position, tip, tipheight);
            AddDrawingPoint(pos);
            lastDrawTime = Time.time;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Rate limit drawing operations based on time
        if (Time.time - lastDrawTime < MIN_DRAW_INTERVAL) return;
        
        // Continue drawing only if still touching whiteboard
        if (isDrawing && other.CompareTag("Whiteboard") && 
            currentWhiteboard != null && other.GetComponent<Whiteboard>() == currentWhiteboard)
        {
            Vector3 currentTipPos = tip.position;
            
            // Only draw if moved enough to avoid unnecessary updates
            if (Vector3.Distance(lastTipPosition, currentTipPos) >= minDrawDistance)
            {
                //ector3 markerPosition, Transform tip, float tipheight
                Vector2 pos = currentWhiteboard.WorldToTexturePosition(currentTipPos, tip, tipheight);
                // Vector2 pos = currentWhiteboard.WorldToTexturePosition(currentTipPos);
                AddDrawingPoint(pos);
                
                lastTipPosition = currentTipPos;
                lastDrawTime = Time.time;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Stop drawing when leaving the whiteboard
        if (other.CompareTag("Whiteboard") && 
            currentWhiteboard != null && other.GetComponent<Whiteboard>() == currentWhiteboard)
        {
            // Process any pending points
            if (pendingPoints.Count > 0 && !processingLine)
            {
                StartCoroutine(ProcessPendingPoints());
            }
            
            isDrawing = false;
            rotationLocked = false;
            lastTexturePos = Vector2.zero;
            
            // Keep whiteboard reference until pending points are processed
            if (pendingPoints.Count == 0)
            {
                currentWhiteboard = null;
            }
        }
    }

    private void AddDrawingPoint(Vector2 texturePos)
    {
        // Skip if out of bounds
        if (texturePos.x < 0 || texturePos.x >= currentWhiteboard.textureSize.x ||
            texturePos.y < 0 || texturePos.y >= currentWhiteboard.textureSize.y)
        {
            return;
        }
            
        // Add the point to the pending list
        pendingPoints.Add(texturePos);
        
        // If this is the first point, draw it immediately
        if (pendingPoints.Count == 1 && lastTexturePos == Vector2.zero)
        {
            int x = (int)(texturePos.x - (penSize / 2));
            int y = (int)(texturePos.y - (penSize / 2));
            currentWhiteboard.SetPixels(x, y, penSize, colors);
        }
        
        lastTexturePos = texturePos;
    }
}