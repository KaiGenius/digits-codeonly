using UnityEngine;

public class Magneting : MonoBehaviour
{
    public Transform target;
    public float speed = 20f;

    private void Update()
    {
        if (target == null || GameManager.IsGameEnded || GameManager.IsGamePaused)
            return;

        var dir = target.position - transform.position;
        dir.Normalize();

        transform.position += speed * Time.deltaTime * dir;
    }
}
