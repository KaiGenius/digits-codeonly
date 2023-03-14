using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public enum Operation
{
    Summary,
    Substract,
    Multiplicate,
    Divided,
    Flat,
}
public class Number3D : MonoBehaviour
{
    public Operation operation = Operation.Summary;
    [NonSerialized] public uint spawnIteration;
    public float size = 1;
    public int number;
    public Color goodColor, badColor;
    public bool isMagneting = false;
    [SerializeField] private DigitView view;
    [SerializeField] private BoxCollider boxCollider;

    private bool spawnInvalid = true;
    private bool isUsed = false;
    public Bounds Bounds => view.Bounds;

    private void OnEnable()
    {
        spawnInvalid = true;
        Observable.NextFrame().TakeUntilDestroy(gameObject).Subscribe(_ =>
        {
            view.IsRendering = false;
            view.SetNumber(this.ToString());
            UpdateMats(Player.ActiveInstance, false);
            transform.localScale = Vector3.one * size;

            var meshBounds = view.Bounds;
            boxCollider.center = meshBounds.center;
            boxCollider.size = meshBounds.size * 1.025f;

            Observable.NextFrame(FrameCountType.FixedUpdate).TakeUntilDestroy(gameObject).Subscribe(_ => 
            {
                view.IsRendering = true;
                spawnInvalid = false;
            });
        });
    }

    public override string ToString()
    {
        char op = operation switch
        {
            Operation.Summary => '+',
            Operation.Substract => '-',
            Operation.Multiplicate => '*',
            Operation.Divided => '/',
            _ => ' '
        };

        return op + number.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9)
            return;

        if(other.attachedRigidbody!=null)
        {
            if (other.attachedRigidbody.TryGetComponent<IActor>(out var player))
            {
                if (IsValid(player))
                {
                    if (!spawnInvalid)
                    {
                        player.UpdateScore(number, operation);
                        Player.ActiveInstance.OnUpdateScore -= UpdateMats;
                    }
                    Destroy(gameObject);
                    isUsed = true;
                }
            }
            else
            {
                if (other != null && this != null && other.attachedRigidbody.TryGetComponent<Number3D>(out var number))
                {
                    if (this.isMagneting || number.isMagneting)
                        return;

                    if (this.spawnIteration > number.spawnIteration)
                    {
                        Destroy(gameObject);
                        isUsed = true;
                    }
                }
            }
        }
    }

    private void Start()
    {
        Player.ActiveInstance.OnUpdateScore += UpdateMats;
    }

    private void OnDestroy()
    {
        Player.ActiveInstance.OnUpdateScore -= UpdateMats;
    }

    private void UpdateMats(IActor player, bool _)
    {
        if (IsValid(player) && IsPositive())
            view.UpdateMaterial(goodColor);
        else
            view.UpdateMaterial(badColor);
    }

    public bool IsValid(IActor forPlayer)
    {
        return !isUsed;
    }

    public bool IsPositive()
    {
        return operation switch
        {
            Operation.Summary => true,
            Operation.Substract => false,
            Operation.Multiplicate => true,
            Operation.Divided => false,
            _ => true,
        };
    }
}
