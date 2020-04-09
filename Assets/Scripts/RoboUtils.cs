using  UnityEngine;
public static class RoboUtils
{
    public static float Scale(float oldMin, float oldMax, float newMin, float newMax, float oldValue)
    {
        float oldRange = (oldMax - oldMin);
        float newRange = (newMax - newMin);
        float newValue = (((oldValue - oldMin) * newRange) / oldRange) + newMin;

        return (newValue);
    }

    public static void ClearConsole()
    {
        var logEntries = System.Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
        var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        clearMethod.Invoke(null, null);
    }
    
    public static void DrawRay(Vector3 from, Vector3 direction, Color c )
    {
        Gizmos.color = c;
        Gizmos.DrawRay(from, direction);
        Gizmos.DrawSphere(from + direction, 0.02f);
    }
    
    public static void DrawLocalRay(Transform transform, Vector3 from, float length, Vector3 dir, Color c)
    {
        Gizmos.color = c;
        Gizmos.DrawRay(transform.position + from, length * dir);
        Gizmos.DrawSphere(transform.position + from + length * dir, 0.03f);
    }
}