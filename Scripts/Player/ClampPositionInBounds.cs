using UnityEngine;

public class ClampPositionInBounds : MonoBehaviour
{
    private IBoundsProvider _boundsProvider;
    private Bounds thisBounds;
    private Rigidbody rb;

    private void Start()
    {
        _boundsProvider = GetComponent<IBoundsProvider>();
        rb = GetComponent<Rigidbody>();
    }

    private void LateUpdate()
    {
        var bounds = BoundsProvider.CurrentBounds;
        var localBounds = _boundsProvider.Bounds;
        thisBounds = bounds;
        thisBounds.size -= localBounds.size;
        if (!thisBounds.Contains(rb.position))
            transform.position = thisBounds.ClosestPoint(transform.position);
    }

    private void OnDrawGizmosSelected()
    {
        if(Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(thisBounds.center, thisBounds.size);
        }
    }
}
