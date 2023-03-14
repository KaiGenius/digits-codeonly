using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class TransformFollow : MonoBehaviour
{
    [Range(20, 60)]public float topDownFOV = 35;
    public float topDownForwardOffset = -2f, topDownPositionOffset = -2f, topDownSizeDelta_C = 1;
    public float viewChangeSpeed = 1;
    public float scaleMod = 1.5f;
    public ScaleFormulaSO scaleFormula;
    //private SizeLevelList sizeLevelList => target.sizeLevelList;
    private Player target;
    private Vector3 offset, addOffset, targetAddOffset, topdownOffset, topdownSmoothPos;
    [SerializeField] private bool isTopDown = false;
    private Camera _cam;
    private float sizeDelta;
    private Vector3 velTopDown;

    public void SetTarget(Player newTarget)
    {
        _cam = GetComponent<Camera>();
        target = newTarget;
        offset = transform.position - target.transform.position;
        addOffset = default;
        target.OnUpdateScore += Target_OnUpdateScore;
        target.GetComponent<EXPProvider>().OnUpdateLevel += TransformFollow_OnUpdateLevel;
        topdownOffset = offset;
        isTopDown = false;

        TransformFollow_OnUpdateLevel(null, 1);
    }

    private void TransformFollow_OnUpdateLevel(EXPProvider arg1, int level)
    {
        var dir = transform.position - target.transform.position;
        dir.Normalize();
        sizeDelta = scaleFormula.CalculateScale(level) - scaleFormula.baseScale;
        targetAddOffset = dir * sizeDelta;
    }

    private void Target_OnUpdateScore(IActor obj, bool _)
    {
       
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        addOffset = Vector3.Lerp(addOffset, targetAddOffset, Time.smoothDeltaTime);
        if (isTopDown)
        {
            var targetOffset = offset;
            targetOffset.x = targetOffset.z = 0;
            targetOffset.x = topDownForwardOffset;
            topdownOffset = Vector3.Lerp(topdownOffset, targetOffset, Time.smoothDeltaTime * viewChangeSpeed);
            var targetPos = target.transform.position;
            targetPos.x += topDownPositionOffset - sizeDelta * topDownSizeDelta_C;
            topdownSmoothPos = Vector3.SmoothDamp(topdownSmoothPos, targetPos + topdownOffset + addOffset * scaleMod, ref velTopDown, 0.1f, 100f, Time.smoothDeltaTime);
            transform.position = topdownSmoothPos;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetPos - transform.position), Time.smoothDeltaTime * 16);
        }
        else
        {
            topdownOffset = Vector3.Lerp(topdownOffset, offset, Time.smoothDeltaTime * viewChangeSpeed);
            transform.position = target.transform.position + topdownOffset + addOffset * scaleMod;
            transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position);
        }

       
    }

    public void SetTopDownCameraView(bool val)
    {
        isTopDown = val;
        if(val)
        {
            velTopDown = default;
            topdownSmoothPos = transform.position;
            _cam.DOFieldOfView(topDownFOV + sizeDelta, 1f / viewChangeSpeed);
        }
        else
        {
            _cam.DOFieldOfView(60, 1f / viewChangeSpeed);
        }
    }

}
