using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/*
This class represents a path that enemies will follow in the game. It contains an array of
GameObjects that represent the pathing points, and provides a method to get the position of
a specific pathing point by index. The class also includes a method to draw gizmos in the editor,
which visually represent the path and label each pathing point with its name.
*/
public class Path : MonoBehaviour
{
    public GameObject[] Pathingpoints;

    public Vector3 GetPosition(int index)
    {
        //return the position of the pathing point at the specified index
        return Pathingpoints[index].transform.position;
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (Pathingpoints == null || Pathingpoints.Length == 0)
            return;

        for (int i = 0; i < Pathingpoints.Length; i++)
        {
            if (Pathingpoints[i] == null)
                continue;

#if UNITY_EDITOR
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;

            Handles.Label(
                Pathingpoints[i].transform.position + Vector3.up * 1.5f,
                Pathingpoints[i].name,
                style
            );
#endif

            if (i < Pathingpoints.Length - 1 && Pathingpoints[i + 1] != null)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawLine(
                    Pathingpoints[i].transform.position,
                    Pathingpoints[i + 1].transform.position
                );
            }
        }
    }
    #endif
}
