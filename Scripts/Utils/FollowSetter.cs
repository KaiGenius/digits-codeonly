using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowSetter : MonoBehaviour
{
    [SerializeField] private Player initialTarget;
    [SerializeField] private TransformFollow follower;

    private void Start()
    {
        follower.SetTarget(initialTarget);
    }
}
