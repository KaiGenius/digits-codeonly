using Core;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

[DefaultExecutionOrder(1000)]
public class DigitsAsset : UnitySingleton<DigitsAsset>
{
    [SerializeField] private bool ignoreRecalculationMeshes = true;
    [SerializeField] private DigitsAssetSO asset;
    private Dictionary<char, (Mesh, Vector3)> cash_data;

    protected override void SingletonAwake()
    {
        cash_data = new Dictionary<char, (Mesh, Vector3)>(asset.digits.Count);
        foreach(var item in asset.digits)
        {
            var convertedMesh = ConvertMesh(item.mesh);
            cash_data.Add(item.key, (convertedMesh, item.localOffset));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public (Mesh mesh, Vector3 localOffset) GetMesh(char key)
    {
        if (cash_data.TryGetValue(key, out (Mesh, Vector3) mesh))
            return mesh;
        else return default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Mesh ConvertMesh(Mesh source)
    {
        if (ignoreRecalculationMeshes)
            return source;

        var verts = source.vertices;
        float min = float.MinValue;
        for(int i =0;i< verts.Length; i++)
        {
            var vert = verts[i];
            if (vert.x > min)
                min = vert.x;
        }

        for(int i =0;i<verts.Length;i++)
        {
            var vert = verts[i];
            vert.x -= min;
            verts[i] = vert;
        }
        source.vertices = verts;
        source.RecalculateBounds();
        return source;
    }
}
