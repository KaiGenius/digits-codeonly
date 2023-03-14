using UnityEngine;

public class BoundsProvider : MonoBehaviour, IBoundsProvider
{
    public static Bounds CurrentBounds { get; private set; }
    [SerializeField] private Bounds playingBound;
    public Bounds Bounds => playingBound;

    private void Awake()
    {
        CurrentBounds = playingBound;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(playingBound.center, playingBound.size);
    }
}

public interface IBoundsProvider
{
    Bounds Bounds { get; }
}
