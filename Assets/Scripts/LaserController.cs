using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    public Transform laserOrigin;
    public LineRenderer lineRenderer;
    public bool isOn = true;
    [SerializeField]private float maxShootingDistance = 25f;

    // Laser angle and oscillation
    private float startingAngle;
    public bool oscillate = false;
    public float oscillationSpeed = 0.5f;
    public float oscillationAngle = 45f;

    private PlayerRespawn playerRespawn;
    [SerializeField] private LayerMask mirrorLayer;

    void Awake()
    {
        playerRespawn = FindObjectOfType<PlayerRespawn>();
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

            // Set corner vertices to 2 to make the line renderer look smoother
            lineRenderer.numCornerVertices = 2;
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
                    playerRespawn.Respawn();
                    break;
                }
                else if (hit.collider.CompareTag("Hostility") || 
                (hit.collider.GetComponent<HeadTrigger>() != null))
                {
                    Enemy enemy = hit.collider.GetComponentInParent<Enemy>();
                    if(enemy != null)
                    {
                        enemy.TakeDamage(100);
                    }
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