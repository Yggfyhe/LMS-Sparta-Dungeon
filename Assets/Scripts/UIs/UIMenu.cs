
using UnityEngine;
using DG.Tweening;

public class UIMenu : MonoBehaviour
{
    [SerializeField] private GameObject menu;

    private bool isMove = false;
 public void OnMenu()
    {
        if(isMove== false)
        {
            isMove = true;
           if(!menu.activeSelf)
           {
                menu.SetActive(true);
                menu.transform.DOLocalMoveY(menu.transform.localPosition.y - 550f, 0.5f).SetEase(Ease.OutCubic).OnComplete(() =>
                {
                    isMove = false;
                });
           }
           else
           {         
            menu.transform.DOLocalMoveY(menu.transform.localPosition.y + 550f, 0.5f).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                menu.SetActive(false);
                isMove = false;
            });
           }
        }
    }


}
