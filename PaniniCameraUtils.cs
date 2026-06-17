using UnityEngine;

public static class PaniniCameraUtils
{
    public static Ray ScreenPointToRayPanini(this Camera camera, Vector2 screenPosition, float distance, float cropToFit)
    {
        Vector2 normalizedScreenPos = camera.ScreenToViewportPoint(new Vector3(screenPosition.x, screenPosition.y, 1f));
        
        var viewExtents = PaniniViewExtents(camera);
        var cropExtents = PaniniCropExtents(camera, distance);

        float scaleF = Mathf.Min(cropExtents.x / viewExtents.x, cropExtents.y / viewExtents.y);
        float paniniS = Mathf.Lerp(1f, Mathf.Clamp01(scaleF), cropToFit);

        float ndcX = normalizedScreenPos.x * 2f - 1f;
        float ndcY = normalizedScreenPos.y * 2f - 1f;
        var viewPos = new Vector2(ndcX * viewExtents.x * paniniS, ndcY * viewExtents.y * paniniS);
        var projPos = PaniniGeneric(viewPos, distance);

        var dir = camera.transform.TransformDirection(new Vector3(projPos.x, projPos.y, 1f).normalized);
        return new Ray(camera.transform.position, dir);
    }

    // Port of Panini_Generic from URP's PaniniProjection.shader
    private static Vector2 PaniniGeneric(Vector2 viewPos, float d)
    {
        float viewDist = 1f + d;
        float viewHypSq = viewPos.x * viewPos.x + viewDist * viewDist;

        float isectD = viewPos.x * d;
        float isectDiscrim = viewHypSq - isectD * isectD;

        float cylDistMinusD = (-isectD * viewPos.x + viewDist * Mathf.Sqrt(Mathf.Max(0f, isectDiscrim))) / viewHypSq;
        float cylDist = cylDistMinusD + d;

        Vector2 cylPos = viewPos * (cylDist / viewDist);
        return cylPos / cylDistMinusD;
    }

    // Port of CalcViewExtents from URP's PostProcessPass.cs
    private static Vector2 PaniniViewExtents(Camera camera)
    {
        float extY = Mathf.Tan(camera.fieldOfView * Mathf.Deg2Rad * 0.5f);
        return new Vector2(extY * camera.aspect, extY);
    }

    // Port of CalcCropExtents from URP's PostProcessPass.cs
    private static Vector2 PaniniCropExtents(Camera camera, float d)
    {
        float viewDist = 1f + d;
        var projPos = PaniniViewExtents(camera);
        float projHyp = Mathf.Sqrt(projPos.x * projPos.x + 1f);
        float cylDistMinusD = 1f / projHyp;
        float cylDist = cylDistMinusD + d;
        return projPos * cylDistMinusD * (viewDist / cylDist);
    }
}
