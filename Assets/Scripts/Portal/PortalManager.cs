using UnityEngine;
using System.Collections.Generic;

public class PortalManager : MonoBehaviour
{
    [Header("Portal Settings")]
    [SerializeField] private GameObject portalPrefab;
    [SerializeField] private GameObject mirrorPrefab;
    [SerializeField] private LayerMask portalPlacementMask;
    [SerializeField] private float minPortalDistance = 1f;

    private List<Portal> activePortals = new List<Portal>();
    private GameObject activeMirror;
    private Camera mainCamera;
    private PlayerController player;

    private void Start()
    {
        mainCamera = Camera.main;
        player = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        HandleGunCreation();
    }

    private void HandleGunCreation()
    {
        // Left click for blue portal
        if (Input.GetMouseButtonDown(0))
        {
            CreatePortal(PortalType.Blue);
        }
        // Right click for orange portal
        else if (Input.GetMouseButtonDown(1))
        {
            CreatePortal(PortalType.Orange);
        }
        // E click for mirror
        else if (Input.GetKeyDown(KeyCode.E))
        {
            CreateMirror();
        }
    }

    private RaycastHit2D GetGunRaycastHit()
    {
        Vector2 startPosition = player.intermediatePosition;
        Vector2 endPosition = player.endingPosition;
        Vector2 direction = (endPosition - startPosition).normalized;
        return Physics2D.Raycast(startPosition, direction, Mathf.Infinity, portalPlacementMask);
    }

    private void CreateMirror()
    {
        RaycastHit2D hit = GetGunRaycastHit();
        if (hit.collider != null)
        {
            // Check if we can place a mirror here
            if (!IsValidMirrorPosition(hit.point))
                return;
            // Remove existing mirror if it exists
            RemoveMirror();
            // Create new mirror
            Vector2 normal = hit.normal;
            float mirrorRotation = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg + 90f;
            GameObject mirrorObj = Instantiate(mirrorPrefab, hit.point, Quaternion.Euler(0, 0, mirrorRotation));
            activeMirror = mirrorObj;
        }
    }
    private bool IsValidMirrorPosition(Vector2 position)
    {
        // Check distance from other mirrors
        if (activeMirror != null)
        {
            if (Vector2.Distance(position, activeMirror.transform.position) < minPortalDistance)
            {
                return false;
            }
        }
        return true;
    }

    private void RemoveMirror()
    {
        if (activeMirror != null)
        {
            Destroy(activeMirror);
            activeMirror = null;
        }
    }

    private void CreatePortal(PortalType type)
    {
        RaycastHit2D hit = GetGunRaycastHit();
        if (hit.collider != null)
        {
            // Check if we can place a portal here
            if (!IsValidPortalPosition(hit.point))
                return;
            // Remove existing portal of same type if it exists
            RemovePortalOfType(type);

            Vector2 normal = hit.normal;
            float portalRotation = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg + 90f;
            // Create new portal
            GameObject portalObj = Instantiate(portalPrefab, hit.point, Quaternion.Euler(0, 0, portalRotation));
            Portal portal = portalObj.GetComponent<Portal>();
            portal.Initialize(type, normal);
            activePortals.Add(portal);

            // Link portals if we have a pair
            if (activePortals.Count == 2)
            {
                LinkPortals();
            }
        }
    }

    private bool IsValidPortalPosition(Vector2 position)
    {
        // Check distance from other portals
        foreach (Portal portal in activePortals)
        {
            if (Vector2.Distance(position, portal.transform.position) < minPortalDistance)
            {
                return false;
            }
        }
        return true;
    }

    private void RemovePortalOfType(PortalType type)
    {
        Portal portalToRemove = activePortals.Find(p => p.Type == type);
        if (portalToRemove != null)
        {
            activePortals.Remove(portalToRemove);
            Destroy(portalToRemove.gameObject);
        }
    }

    private void LinkPortals()
    {
        if (activePortals.Count != 2) return;

        Portal portal1 = activePortals[0];
        Portal portal2 = activePortals[1];

        portal1.LinkedPortal = portal2;
        portal2.LinkedPortal = portal1;
    }
}

public enum PortalType
{
    Blue,
    Orange
}