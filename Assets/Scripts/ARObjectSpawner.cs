using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ARObjectSpawner : MonoBehaviour
{
    [SerializeField] GameObject m_Prefab;
    [SerializeField] ARPlacementIndicator m_ARPlacementIndicator;
    [SerializeField] GraphicRaycaster m_GraphicRaycaster;

    void Update()
    {
        if (m_ARPlacementIndicator.IsSurfaceDetected && Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began && !IsClickOverUI())
        {
            Instantiate(m_Prefab, m_ARPlacementIndicator.transform.position, m_ARPlacementIndicator.transform.rotation);
        }
    }

    bool IsClickOverUI()
    {
        PointerEventData data = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        m_GraphicRaycaster.Raycast(data, results);

        return results.Count > 0;
    }
}
