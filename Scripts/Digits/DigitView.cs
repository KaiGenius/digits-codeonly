using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DigitView : MonoBehaviour
{
    public float spaceBetweenCharacters = 0.1f;
    [SerializeField] private Material mainMaterial;
    [SerializeField] private DigitPart part;
    private List<DigitPart> partList = new List<DigitPart>(8);
    private Bounds cash_bounds;
    private bool _isRendering = true;
    private int currentListSize = 0;

    public Bounds Bounds => cash_bounds;

    private void Awake()
    {
        mainMaterial = new Material(mainMaterial);
    }

    public bool IsRendering
    {
        get => _isRendering;
        set
        {
            if(value != _isRendering)
            {
                _isRendering = value;
                for (int i = 0; i < currentListSize; i++)
                {
                    partList[i].IsEnabled = _isRendering;
                }   
            }
        }
    }

    public void SetNumber(string number)
    {
        Vector3 shift = default;
        string textPresent = number.Trim();
        cash_bounds = new Bounds();
        int index, current;
        for(index = 0, current = 0; index < textPresent.Length; index++, current++)
        {
            var meshData = DigitsAsset.Self.GetMesh(textPresent[index]);
            var currentPart = GetNext(current);
            var mesh = meshData.mesh;

            if (mesh != null)
            {
                currentPart.SetMesh(mesh);
                currentPart.transform.localPosition = GetLocalPosition(current, mesh.bounds, meshData.localOffset, ref shift);
                var meshBounds = mesh.bounds;
                meshBounds.center += currentPart.transform.localPosition;
                cash_bounds.Encapsulate(meshBounds);
                currentPart.IsEnabled = _isRendering;
            }
            else
            {
                current--;
            }
        }

        currentListSize = current;
        for(;current<partList.Count;current++)
        {
            partList[current].IsEnabled = false;
        }
    }

    private DigitPart GetNext(int index)
    {
        DigitPart selected = null;
        if (index < partList.Count)
            selected = partList[index];

        if(selected == null)
        {
            selected = Instantiate(part, transform);
            selected.meshRenderer.sharedMaterial = mainMaterial;
            partList.Add(selected);
        }

        return selected;
    }

    public void UpdateMaterial(Color toChangeColor)
    {
        mainMaterial.color = toChangeColor;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Vector3 GetLocalPosition(in int index, in Bounds bounds, in Vector3 localOffset, ref Vector3 globalShift)
    {
        if(index is 0)
        {
            globalShift = -(new Vector3(bounds.size.x + spaceBetweenCharacters, 0, 0) + localOffset);
            return localOffset;
        }
        else
        {
            var pos = globalShift;
            globalShift -= new Vector3(bounds.size.x + spaceBetweenCharacters, 0, 0) + localOffset;
            return pos + localOffset;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        var bounds = Bounds;
        Gizmos.DrawWireCube(bounds.center + transform.position, bounds.size);
    }

    private void OnDisable()
    {
        foreach(var item in partList)
        {
            item.IsEnabled = false;
        }
    }
}
