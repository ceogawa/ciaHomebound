using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whiteboard : MonoBehaviour
{
    public Texture2D texture;
    public Vector2 textureSize = new Vector2(2048, 2048);
    [SerializeField] private bool useRealTimeUpdates = false;
    
    private Renderer rend;
    private Collider boardCollider;
    private bool textureNeedsUpdate = false;
    private float nextUpdateTime = 0f;
    private const float UPDATE_INTERVAL = 0.05f; // 20 updates per second max
    
    void Start()
    {
        rend = GetComponent<Renderer>();
        texture = new Texture2D((int)textureSize.x, (int)textureSize.y, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Bilinear;
        rend.material.mainTexture = texture;
        
        // Get the collider
        boardCollider = GetComponent<Collider>();
        if (boardCollider == null)
        {
            Debug.LogError("Whiteboard needs a collider attached!");
        }
        
        // Clear texture to white
        Color[] clearColors = new Color[texture.width * texture.height];
        for (int i = 0; i < clearColors.Length; i++)
        {
            clearColors[i] = Color.white;
        }
        texture.SetPixels(clearColors);
        texture.Apply();
    }
    
    void Update()
    {
        // Only update the texture periodically if changes were made
        if (textureNeedsUpdate && Time.time >= nextUpdateTime)
        {
            texture.Apply(false); // false = don't update mipmaps for better performance
            textureNeedsUpdate = false;
            nextUpdateTime = Time.time + UPDATE_INTERVAL;
        }
    }

    // Called by the marker when it has set pixels
    public void SetPixels(int x, int y, int penSize, Color[] colors, bool applyImmediately = false)
    {
        if (x < 0 || x + penSize > texture.width || y < 0 || y + penSize > texture.height)
            return;
            
        texture.SetPixels(x, y, penSize, penSize, colors);
        
        if (applyImmediately && useRealTimeUpdates)
        {
            texture.Apply(false);
        }
        else
        {
            textureNeedsUpdate = true;
        }
    }

    public Vector2 WorldToTexturePosition(Vector3 markerPosition, Transform tip, float tipheight)
    {
        // // Cast from slightly above the point to ensure we hit the board
        // Ray ray = new Ray(worldPosition * 0.1f, -transform.up);
        RaycastHit hit;
        Ray ray = new Ray(markerPosition, tip.up);
        float allowanceHeight = tipheight + 0.5f;
        
        if (Physics.Raycast(ray, out hit, allowanceHeight) && hit.collider ==  boardCollider)
        {
            int x = Mathf.RoundToInt(hit.textureCoord.x * textureSize.x);
            int y = Mathf.RoundToInt(hit.textureCoord.y * textureSize.y);
            return new Vector2(x, y);
        }

        // if (Physics.Raycast(_tip.position, transform.up, out _touch, _tipHeight))
        //Debug.DrawRay(ray.origin, ray.direction * 0.2f, Color.red, 1.0f); // Visualize ray
        
        // if (Physics.Raycast(ray, out hit, 0.5f) && hit.collider == boardCollider)
        // {
        //     int x = Mathf.RoundToInt(hit.textureCoord.x * textureSize.x);
        //     int y = Mathf.RoundToInt(hit.textureCoord.y * textureSize.y);
            
        //     // Debug.Log($"Hit detected at UV: {hit.textureCoord}, Pixel: ({x}, {y})");
        //     return new Vector2(x, y);
        // }
        
        Debug.LogWarning("Raycast failed at position: " + markerPosition);
        return new Vector2(-1, -1); // Return invalid coordinates to skip drawing
    }
    // public Vector2 WorldToTexturePosition(Vector3 worldPosition)
    // {
    //     // // Use a cached raycast if possible to improve performance
    //     RaycastHit hit;
    //     Ray ray = new Ray(worldPosition + Vector3.up * 0.1f, Vector3.down);
        
    //     if (boardCollider.Raycast(ray, out hit, 1.0f))
    //     {
    //         // Convert UV coordinates to texture pixels
    //         int x = Mathf.RoundToInt(hit.textureCoord.x * textureSize.x);
    //         int y = Mathf.RoundToInt(hit.textureCoord.y * textureSize.y);
            
    //         return new Vector2(x, y);
    //     }
        
    //     // If the raycast fails, use a simpler approximation
    //     Vector3 localPos = transform.InverseTransformPoint(worldPosition);
    //     float normalizedX = localPos.x / transform.localScale.x + 0.5f;
    //     float normalizedY = localPos.z / transform.localScale.z + 0.5f;
        
    //     int pixelX = Mathf.Clamp(Mathf.RoundToInt(normalizedX * textureSize.x), 0, (int)textureSize.x - 1);
    //     int pixelY = Mathf.Clamp(Mathf.RoundToInt(normalizedY * textureSize.y), 0, (int)textureSize.y - 1);
        
    //     return new Vector2(pixelX, pixelY);
    // }
}