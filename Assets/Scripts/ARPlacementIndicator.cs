using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SurfaceDetectedEventArgs : EventArgs
{
    public readonly ARRaycastHit? RayCastHit;

    public SurfaceDetectedEventArgs(ARRaycastHit? hit)
    {
        RayCastHit = hit;
    }
}

public class ARPlacementIndicator : MonoBehaviour
{
    [SerializeField] ARPlaneManager m_ARPlaneManager;
    [SerializeField] ARRaycastManager m_RaycastManager;
    [SerializeField] GameObject m_PlacementMarker;
    [SerializeField] Transform m_RotationTarget;

    public event EventHandler<SurfaceDetectedEventArgs> OnSurfaceDetected;
    public bool IsPlaneTrackingActive => m_IsPlaneTrackingActive;
    public bool IsSurfaceDetected => m_IsSurfaceDetected;

    bool m_IsSurfaceDetected;
    bool m_IsPlaneTrackingActive = true;
    readonly List<ARRaycastHit> m_Hits = new List<ARRaycastHit>();

    public void SetPlaneTrackingActive(bool isOn)
    {
        m_PlacementMarker?.SetActive(isOn);
        m_IsPlaneTrackingActive = isOn;
    }

    void Start()
    {
        m_PlacementMarker?.SetActive(false);
        SetSurfaceDetectedTo(null);
    }

    void Update()
    {
        Vector2 centerOfScreen = new Vector2(Screen.width / 2, Screen.height / 2);
        m_RaycastManager.Raycast(centerOfScreen, m_Hits, TrackableType.Planes);

        if (m_Hits.Count > 0)
        {
            ARRaycastHit hit = m_Hits[0];
            HandleRaycast(hit);
            SetSurfaceDetectedTo(hit);
        }
        else
        {
            SetSurfaceDetectedTo(null);
        }
    }

    void HandleRaycast(ARRaycastHit raycastHit)
    {
        Pose pose = raycastHit.pose;

        if (raycastHit.hitType == TrackableType.PlaneEstimated) // Strangely undocumented. PlaneEstimated may NOT have an id. Defaults to (0000000000000000-0000000000000000)
        {
            transform.SetPositionAndRotation(pose.position, pose.rotation);
            return;
        }

        ARPlane plane = m_ARPlaneManager.GetPlane(raycastHit.trackableId);

        if (plane.alignment == PlaneAlignment.HorizontalUp)
        {
            Quaternion rotation = RotationFacingTargetForFloorPlane();
            transform.SetPositionAndRotation(pose.position, rotation);
        }
        else
        {
            transform.SetPositionAndRotation(pose.position, pose.rotation);
        }
    }

    Quaternion RotationFacingTargetForFloorPlane()
    {
        Vector3 position = new Vector3(m_RotationTarget.transform.forward.x, 0f, m_RotationTarget.transform.forward.z).normalized;
        return Quaternion.LookRotation(position);
    }

    void SetSurfaceDetectedTo(ARRaycastHit? raycastHit)
    {
        if (m_IsSurfaceDetected == raycastHit.HasValue) { return; }

        m_IsSurfaceDetected = raycastHit.HasValue;
        m_PlacementMarker?.SetActive(raycastHit.HasValue);

        if (m_IsSurfaceDetected)
        {
            OnSurfaceDetected?.Invoke(this, new SurfaceDetectedEventArgs(raycastHit));
        }
        else
        {
            OnSurfaceDetected?.Invoke(this, new SurfaceDetectedEventArgs(null));
        }
    }
}
