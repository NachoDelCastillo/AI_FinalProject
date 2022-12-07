using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        FieldOfView fov = (FieldOfView)target;
        Handles.color = Color.black;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.visionRadius);

        Vector3 viewAngle01 = DirectionFromAngle(fov.transform.eulerAngles.y, -fov.visionAngle / 2);
        Vector3 viewAngle02 = DirectionFromAngle(fov.transform.eulerAngles.y, fov.visionAngle / 2);

        Handles.color = Color.black;
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle01 * fov.visionRadius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle02 * fov.visionRadius);

        if (fov.canSeeTarget)
        {
            Handles.color = Color.green;
            Handles.DrawLine(fov.transform.position, fov.target.transform.position);
        }
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}