using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlWrapper : MonoBehaviour
{
    [SerializeField] private Joystick joystic;
    [SerializeField] private PlayerControl controller;
    [SerializeField] private Transform forwardVector;

    private void Start()
    {
        GameManager.Self.OnEndGame += Self_OnEndGame;
    }

    private void Self_OnEndGame(GameManager.ActorData[] obj)
    {
        enabled = false;
        controller.SetInput(default);
        Debug.Log("End game call. ControlWrapper disabled");
    }

    private void Update()
    {
        var input = joystic.Direction;

        Vector3 normalizedInput = new Vector3(input.x, 0, input.y);
        normalizedInput = forwardVector.rotation * normalizedInput;

        controller.SetInput(normalizedInput);
    }
}
