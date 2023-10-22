using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class buttonAnimation : MonoBehaviour
{

    public float popTime = 0.15f;
    public float PopScale = 1.2f;
    [Required]
    public RectTransform myRectTransform;
    public void popButton()
    {
        UniTask.Void(() => PopButton(myRectTransform));
    }

    public async UniTaskVoid PopButton(RectTransform transform)
    {
        var originScale = transform.localScale;
        var targetScale = originScale * PopScale;
        float counter = 0f;
        var _popTime = popTime / 2f;
        while (counter < _popTime)
        {
            counter += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originScale, targetScale, counter / _popTime);
            await UniTask.Yield();
        }

        counter = 0f;
        while (counter < _popTime)
        {
            counter += Time.deltaTime;
            transform.localScale = Vector3.Lerp(targetScale, originScale, counter / _popTime);
            await UniTask.Yield();
        }
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}