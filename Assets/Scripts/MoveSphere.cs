using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MoveSphere : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float addAddFixedYPos = 1.5f;

    public float AddFixedYPos
    {
        get => addAddFixedYPos;
    }
    
    public IEnumerator MoveTowardsPoint(List<Vector3> points)
    {
        Vector3 lastPoint = GameManager.Instance.pointB;
        int currentPoint = points.Count;
        currentPoint--;
        while (currentPoint >= 0)
        {
            Vector3 nextPos = new Vector3(points[currentPoint].x, points[currentPoint].y + AddFixedYPos, points[currentPoint].z);
            while (Vector3.Distance(transform.position,nextPos) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, nextPos, speed * Time.deltaTime);
                yield return null;
            }
            
            currentPoint--;
            yield return null;
        }

        lastPoint = new Vector3(lastPoint.x, lastPoint.y + AddFixedYPos, lastPoint.z);
        while (Vector3.Distance(transform.position,lastPoint) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, lastPoint, speed * Time.deltaTime);
            yield return null;
        }
        
    }
}
