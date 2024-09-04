using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerDestroyer : MonoBehaviour
{
    bool allCellsValid;
    CustomGrid GridHandler;
    LayerDistributor distributor;
    private void Start()
    {
        distributor = GetComponentInParent<LayerDistributor>();
        GridHandler = LevelManager.instance.GridHandler;

    }
    public void DestroyMatchedObject()
    {
        // Objenin küçülme animasyonu ve ardýndan yok edilmesi
        gameObject.transform.DOScale(Vector3.zero, 1.5f).SetEase(Ease.InOutElastic).OnComplete(() =>
        {
            Destroy(gameObject);
            GridHandler.CheckAllCellsAreEmpty();
        });
    }

  
}
