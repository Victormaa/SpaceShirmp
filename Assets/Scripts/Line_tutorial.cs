using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line_tutorial : MonoBehaviour
{
    [SerializeField] private float defDistanceRay = 100;
    public Transform laserFirePoint;
    public LineRenderer m_lineRenderer;
    Transform m_transform;


    private void Awake()
    {
        m_transform = GetComponent<Transform>();
    }
    private void Update()
    {
        ShootLaser();
    }
    
    void ShootLaser()
    {
        if (Physics.Raycast(m_transform.position,transform.right,out RaycastHit _hit))
        {
            DrawRay(laserFirePoint.position, _hit.point);
        }
        else
        {
            DrawRay(laserFirePoint.position, laserFirePoint.transform.right * defDistanceRay);
        }    
    }


    void DrawRay(Vector3 startPos, Vector3 endPos )
    {
        m_lineRenderer.SetPosition(0, startPos);
        m_lineRenderer.SetPosition(1, endPos);
    }
}

