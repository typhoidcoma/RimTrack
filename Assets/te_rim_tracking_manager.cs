using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class RimTracker : MonoBehaviour
{
    [Tooltip("Reference to the AR Tracked Image Manager")]
    public ARTrackedImageManager trackedImageManager;
    
    [Tooltip("The prefab to instantiate on the tracked rim")]
    public GameObject rimPrefab;
    
    // Keep track of instantiated rim prefabs by reference image name.
    private Dictionary<string, GameObject> spawnedRims = new Dictionary<string, GameObject>();

    void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        // Handle newly detected images
        foreach (var trackedImage in eventArgs.added)
        {
            UpdateRim(trackedImage);
        }
        // Handle updates to existing tracked images
        foreach (var trackedImage in eventArgs.updated)
        {
            UpdateRim(trackedImage);
        }
        // Clean up when images are no longer tracked
        foreach (var trackedImage in eventArgs.removed)
        {
            if (spawnedRims.ContainsKey(trackedImage.referenceImage.name))
            {
                Destroy(spawnedRims[trackedImage.referenceImage.name]);
                spawnedRims.Remove(trackedImage.referenceImage.name);
            }
        }
    }

    void UpdateRim(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;

        // If we haven't spawned a rim for this image yet, do so.
        if (!spawnedRims.ContainsKey(imageName))
        {
            GameObject newRim = Instantiate(rimPrefab, trackedImage.transform.position, trackedImage.transform.rotation);
            spawnedRims.Add(imageName, newRim);
        }
        else
        {
            // Update the position and rotation if the image is already being tracked.
            GameObject rim = spawnedRims[imageName];
            rim.transform.position = trackedImage.transform.position;
            rim.transform.rotation = trackedImage.transform.rotation;
        }
    }
}
