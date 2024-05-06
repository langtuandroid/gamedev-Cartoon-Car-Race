using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeAreapr : MonoBehaviour
{
    [SerializeField]
    private bool dontSafeBottompr;
    [SerializeField]
    private  float _downOffset = 0.1f;
       
    private RectTransform fittedRectTransformpr;
    private Rect safeRectComponentpr;
    private Vector2 minAnchorVectorpr;
    private Vector2 maxAnchorVectorpr;

    private void Awake()
    {
        fittedRectTransformpr = GetComponent<RectTransform>();
        safeRectComponentpr = Screen.safeArea;
        minAnchorVectorpr = safeRectComponentpr.position;
        maxAnchorVectorpr = minAnchorVectorpr + safeRectComponentpr.size;
        
        minAnchorVectorpr.x /= Screen.width;
        minAnchorVectorpr.y = dontSafeBottompr ? minAnchorVectorpr.y = 0 : _downOffset;
        maxAnchorVectorpr.x /= Screen.width;
        maxAnchorVectorpr.y /= Screen.height;
        
        fittedRectTransformpr.anchorMin = minAnchorVectorpr;
        fittedRectTransformpr.anchorMax = maxAnchorVectorpr;
    }
}
