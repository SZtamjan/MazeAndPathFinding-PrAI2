using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSphere : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    
    public IEnumerator MoveTowardsPoint(List<Vector3> points)
    {
        int currentPoint = points.Count;
        currentPoint--;
        while (currentPoint >= 0)
        {
            while (Vector3.Distance(transform.position,points[currentPoint]) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, points[currentPoint], speed * Time.deltaTime);
                yield return null;
            }
            
            currentPoint--;
            yield return null;
        }
    }
}
