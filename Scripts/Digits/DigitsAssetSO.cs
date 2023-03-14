using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DigitsAssetSO : ScriptableObject
{
    public List<DigitDefinition> digits = new List<DigitDefinition>();

    [System.Serializable]
    public class DigitDefinition
    {
        public char key;
        public Mesh mesh;
        public Vector3 localOffset;
    }
}
