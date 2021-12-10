using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTeleporter : MonoBehaviour
{
    [SerializeField] private Transform portalParent;
    [SerializeField] private Transform portalExitParent;
    [SerializeField] private Transform portalExitCollider;
    [SerializeField] private GameObject playerGO;
    [SerializeField] private bool turnPlayerAround = false;
    private PlayerMovement player;
    private bool overlapped = false;

    private void Awake()
    {
        if (player == null)
            player = playerGO.GetComponent<PlayerMovement>();
    }

    void Update()
    {
            Vector3 portalToPlayer = playerGO.transform.position - transform.position;
            float dotProduct = Vector3.Dot(transform.forward, portalToPlayer);
        if(overlapped)
        {

            if(dotProduct < 0.0f)
            {
                float rotationDiff = -Quaternion.Angle(transform.rotation, portalExitParent.rotation);

                if (turnPlayerAround)
                    rotationDiff += 180.0f;

                if(player != null)
                {
                    playerGO.transform.Rotate(transform.up, rotationDiff);

                    Vector3 posOffset = Quaternion.Euler(0.0f, rotationDiff, 0.0f) * portalToPlayer;
                    playerGO.transform.position = portalExitCollider.position + posOffset;
                    overlapped = false;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {        
        other.gameObject.TryGetComponent<PlayerMovement>(out var player);
        if (player != null)
        {
            overlapped = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        other.gameObject.TryGetComponent<PlayerMovement>(out var player);
        if (player != null)
        {
            overlapped = false;
        }
    }
}
