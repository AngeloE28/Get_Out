using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalDisable : MonoBehaviour
{
    [SerializeField] private BoxCollider portalCollider;
    [SerializeField] private GameObject portalRenderPlane;
    [SerializeField] private Material disabledMat;

    private Renderer portalRenderer;
    private bool _overlapped = false;

    private void Awake()
    {
        portalRenderer = portalRenderPlane.GetComponent<Renderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _overlapped = false;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(_overlapped)
        {
            portalCollider.enabled = false;
            portalRenderer.material = disabledMat;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.TryGetComponent<PlayerMovement>(out var player);
        if (player != null)
        {
            _overlapped = true;
        }
    }

}
