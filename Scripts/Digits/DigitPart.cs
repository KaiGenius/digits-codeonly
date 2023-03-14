using UnityEngine;

public class DigitPart : MonoBehaviour
{
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    [SerializeField] private MeshCollider collider;

    public bool IsEnabled
    {
        get => meshRenderer.enabled;
        set
        {
            if(collider != null)
                collider.enabled = value;
            meshRenderer.enabled = value;
        }
    }

    public void SetMesh(Mesh mesh)
    {
        meshFilter.mesh = mesh;
        if(collider != null)
            collider.sharedMesh = mesh;
        IsEnabled = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (meshFilter.mesh == null)
            return;

        Gizmos.color = Color.yellow;
        var bounds = meshFilter.mesh.bounds;
        Gizmos.DrawWireCube(bounds.center + transform.position, bounds.size);
    }
}
