using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    public Transform laserOrigin;
    public LineRenderer lineRenderer;
    public bool isOn = true;
    private float maxShootingDistance = 50f;

    // Laser angle and oscillation
    private float startingAngle;
    public bool oscillate = false;
    public float oscillationSpeed = 0.5f;
    public float oscillationAngle = 45f;

    [SerializeField] private LayerMask mirrorLayer;

    void Awake()
    {
        
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
            //lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.red;
            lineRenderer.sortingOrder = 100;
        }
        if(laserOrigin == null)
        {
            laserOrigin = transform;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        startingAngle = laserOrigin.eulerAngles.z;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isOn)
        {
            lineRenderer.enabled = false;
            return;
        }

        if(oscillate)
        {
            float angleOffset = Mathf.Sin(Time.time * oscillationSpeed) * oscillationAngle;
            laserOrigin.rotation = Quaternion.Euler(0, 0, startingAngle + angleOffset);
        }
        
        DrawLaser();
    }

    void DrawLaser()
    {
        lineRenderer.enabled = true;
        Vector2 start = laserOrigin.position;
        Vector2 direction = laserOrigin.up;
        List<Vector3> linePositions = new List<Vector3> { start };
        float remainingDistance = maxShootingDistance;

        while(remainingDistance > 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(start, direction, remainingDistance);
            
            if(hit)
            {
                linePositions.Add(hit.point);
                if(hit.collider.CompareTag("Player"))
                {
                    // Insert outcome
                    break;
                }
                else if (hit.collider.CompareTag("Hostility"))
                {
                    // Insert outcome
                    break;
                }
                else if (((1 << hit.collider.gameObject.layer) & mirrorLayer) != 0)
                {
                    direction = Vector2.Reflect(direction, hit.normal);
                    start = hit.point + direction * 0.05f; // Offset to prevent self-hits
                    remainingDistance -= hit.distance;
                }
                else if (hit.collider.CompareTag("Portal"))
                {
                    // Insert outcome
                    
                }
                else break;
                
            }
            else
            {
                linePositions.Add(start + direction * maxShootingDistance);
                break;
            }
        }
        lineRenderer.positionCount = linePositions.Count;
        lineRenderer.SetPositions(linePositions.ToArray());
    }

    // For button presses
    public void SetActive(bool state)
    {
        isOn = state;
    }
}
