using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class MenuController : MonoBehaviour
{

    public float popTime = 0.1f;
    public float PopScale = 1.2f;
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

    public RectTransform MenuInPosition;
    public RectTransform MenuOutPosition;
    public RectTransform MenuTransfrom;
    public float MenuMoveTime = 0.35f;

    [Button]
    public void MoveMainMenu(bool inOrOut)
    {
        if (inOrOut)
        {
            UniTask.Void(MoveMenuIn);
        }
        else
        {
            UniTask.Void(MoveMenuOut);
        }
    }

    public void MoveGameOverMenu(bool inOrOut)
    {
        if (inOrOut)
        {
            UniTask.Void(moveGameOverMenuIn);
        }
        else
        {
            UniTask.Void(moveGameOverMenuOut);
        }
    }

    public AnimationCurve MenuCurve;

    [Required]
    public RectTransform GameOverOutPosition;
    [Required]
    public RectTransform GameOverMenu;

    public async UniTaskVoid moveGameOverMenuIn()
    {
        float counter = 0f;
        while (counter < MenuMoveTime)
        {
            counter += Time.deltaTime;
            GameOverMenu.anchoredPosition = Vector2.Lerp(GameOverOutPosition.anchoredPosition, MenuInPosition.anchoredPosition, MenuCurve.Evaluate(counter / MenuMoveTime));
            await UniTask.Yield();
        }
    }

    public async UniTaskVoid moveGameOverMenuOut()
    {
        float counter = 0f;
        while (counter < MenuMoveTime)
        {
            counter += Time.deltaTime;
            GameOverMenu.anchoredPosition = Vector2.Lerp(MenuInPosition.anchoredPosition, GameOverOutPosition.anchoredPosition, counter / MenuMoveTime);
            await UniTask.Yield();
        }
    }


    public async UniTaskVoid MoveMenuIn()
    {
        float counter = 0f;
        while (counter < MenuMoveTime)
        {
            counter += Time.deltaTime;
            MenuTransfrom.anchoredPosition = Vector2.Lerp(MenuOutPosition.anchoredPosition, MenuInPosition.anchoredPosition, MenuCurve.Evaluate(counter / MenuMoveTime));
            await UniTask.Yield();
        }
    }

    public async UniTaskVoid MoveMenuOut()
    {
        float counter = 0f;
        while (counter < MenuMoveTime)
        {
            counter += Time.deltaTime;
            MenuTransfrom.anchoredPosition = Vector2.Lerp(MenuInPosition.anchoredPosition, MenuOutPosition.anchoredPosition, counter / MenuMoveTime);
            await UniTask.Yield();
        }
    }

    public RectTransform StartButton;
    [Button]
    void PopStart()
    {
        UniTask.Void(() => PopButton(StartButton));
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