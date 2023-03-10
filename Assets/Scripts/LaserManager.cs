using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LaserManager : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform target;
    private static readonly int Origin = Shader.PropertyToID("_Origin");
    private static readonly int Range = Shader.PropertyToID("_LaserRange");
    private float laserMaxRange = 30;
    public LayerMask laserMask;

    // Update is called once per frame
    void Update()
    {
        lineRenderer.SetPosition(0, transform.position);
        if (target != null)//targeted laser logic
        {
            //cast a ray from the laser toward the target going 100 units and set the laser to the point of impact
            RaycastHit hit;
            if (Physics.Raycast(transform.position, target.position - transform.position, out hit, laserMaxRange, laserMask))
            {
                lineRenderer.material.SetVector(Origin, hit.point);
                lineRenderer.SetPosition(1, hit.point);
                lineRenderer.material.SetFloat(Range, hit.distance);
            }
            else
            {
                Vector3 origin = transform.position + ((target.position - transform.position).normalized * laserMaxRange);
                lineRenderer.material.SetVector(Origin, origin);
                lineRenderer.SetPosition(1, origin);
                lineRenderer.material.SetFloat(Range, laserMaxRange);
            }
        }
        else
        {
            //cast a ray forward from the laser going 10 units
            RaycastHit hit;
            //if the ray hits something set the laser range to that distance and position 1 of the line renderer to the hit point
            if (Physics.Raycast(transform.position, transform.forward, out hit, laserMaxRange, laserMask))
            {
                lineRenderer.material.SetVector(Origin, hit.point);
                lineRenderer.SetPosition(1, hit.point);
                lineRenderer.material.SetFloat(Range, Vector3.Distance(transform.position, hit.point));
            }
            else
            {
                //if the ray doesn't hit anything set the laser range to 10 and position 1 of the line renderer to 10 units forward
                Vector3 origin = transform.position + transform.forward * laserMaxRange;
                lineRenderer.material.SetVector(Origin, origin);
                lineRenderer.SetPosition(1, origin);
                lineRenderer.material.SetFloat(Range, laserMaxRange);
            }
        }
    }
}
