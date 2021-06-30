using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ToggleAR : MonoBehaviour
{
    [SerializeField] ARPlaneManager m_ARPlaneManager;
    [SerializeField] ARPointCloudManager m_PointCloudManager;

    public void OnValueChanged(bool isOn)
    {
        VisualizePlanes(isOn);
        VisualizePoints(isOn);
    }

    void VisualizePlanes(bool active)
    {
        foreach (ARPlane plane in m_ARPlaneManager.trackables)
        {
            plane.gameObject.SetActive(active);
        }
    }

    void VisualizePoints(bool active)
    {
        m_PointCloudManager.enabled = active;

        foreach (ARPointCloud point in m_PointCloudManager.trackables)
        {
            point.gameObject.SetActive(active);
        }
    }
}
