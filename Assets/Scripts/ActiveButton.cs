using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActiveButton : MonoBehaviour
{
    public List<TargetInfo> targetInfos = new();

    [Serializable]
    public class TargetInfo
    {
        public List<GameObject> sceneTargets = new List<GameObject>();
        public List<Vector3> sceneTargetPositions = new List<Vector3>();
        public GameObject targetPrefab;

    }
    void Start()
    {
        foreach (var list in targetInfos) 
            foreach (var sceneTarget in list.sceneTargets) list.sceneTargetPositions.Add(sceneTarget.transform.position);
    }

    private IEnumerator TargetReset()
    {
        yield return new WaitForSeconds(2);
        //Person targets
        foreach (var list in targetInfos)
        {
            if(list.sceneTargets.Count > 0) foreach (var target in list.sceneTargets) if(target != null) Destroy(target);
            list.sceneTargets.Clear();
            for (int i = 0; i < list.sceneTargetPositions.Count; i++)
            {
                GameObject target = Instantiate(list.targetPrefab, list.sceneTargetPositions[i], Quaternion.identity);
                list.sceneTargets.Add(target);
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        print($"SOMETHING ENTERED: {other.gameObject.name}");
        if (other.gameObject.CompareTag("Bullet") && !LeanTween.isTweening(gameObject)) ButtonAnimation();
    }

    public void ButtonAnimation()
    {
        LeanTween.moveY(gameObject, transform.position.y - 0.06f, 0.1f).setEase(LeanTweenType.easeInOutSine).setLoopPingPong(1).setOnComplete(() => StartCoroutine(TargetReset()));
    }

}
