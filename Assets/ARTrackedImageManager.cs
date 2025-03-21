using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;

/// <summary>
/// Tracks AR markers in the real world and places virtual rim objects at their positions.
/// Uses AR Foundation's image tracking capabilities to detect and track reference images.
/// </summary>
public class RimTracker : MonoBehaviour
{
    // Reference to AR Foundation's tracked image manager component
    public ARTrackedImageManager trackedImageManager;
    
    // Prefab of the rim object that will be spawned when a marker is detected
    public GameObject rimPrefab;

    // Dictionary to keep track of spawned rim objects by image name
    private Dictionary<string, GameObject> spawnedRims = new Dictionary<string, GameObject>();

    /// <summary>
    /// Subscribe to the tracked images changed event when this component is enabled
    /// </summary>
    void OnEnable() {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }
    
    /// <summary>
    /// Unsubscribe from the tracked images changed event when this component is disabled
    /// </summary>
    void OnDisable() {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }
    
    /// <summary>
    /// Handles changes to tracked images (added, updated, or removed)
    /// </summary>
    /// <param name="eventArgs">Contains lists of added, updated, and removed tracked images</param>
    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs) {
        // Handle new detections
        foreach (var newImage in eventArgs.added) {
            var imageName = newImage.referenceImage.name;
            // Only spawn a new rim if one hasn't been created for this image yet
            if (!spawnedRims.ContainsKey(imageName)) {
                // Instantiate the rim at the detected image's position and rotation
                GameObject rimObject = Instantiate(rimPrefab, newImage.transform.position, newImage.transform.rotation);
                // Parent the rim to the tracked image so it follows automatically
                rimObject.transform.parent = newImage.transform;
                // Store reference to the spawned object
                spawnedRims[imageName] = rimObject;
            }
        }
        
        // Update existing tracked images
        foreach (var updatedImage in eventArgs.updated) {
            // Find the corresponding rim object
            if (spawnedRims.TryGetValue(updatedImage.referenceImage.name, out GameObject rimObject)) {
                // Update the rim's position and rotation to match the tracked image
                rimObject.transform.position = updatedImage.transform.position;
                rimObject.transform.rotation = updatedImage.transform.rotation;
            }
        }
        
        // Remove lost images
        foreach (var removedImage in eventArgs.removed) {
            // Find the corresponding rim object
            if (spawnedRims.TryGetValue(removedImage.referenceImage.name, out GameObject rimObject)) {
                // Destroy the rim object
                Destroy(rimObject);
                // Remove the reference from the dictionary
                spawnedRims.Remove(removedImage.referenceImage.name);
            }
        }
    }
}
