using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionHandler : MonoBehaviour, ISelectable
{
    [SerializeField] GameObject highlightImage;

    private void Start()
    {
        highlightImage = transform.GetComponent<LayerDistributor>().selectionFrame;
        SelectionManager.instance.LastSelectionChangedEvent += OnLastSelectedChanged;
    }
    private void OnLastSelectedChanged(SelectionHandler selection)
    {
        SelectionHandler previousSelection = SelectionManager.instance.GetPreviousSelected();
        if (selection == null) return;
        if (selection == this) HighlightSelected();
        if (previousSelection == null) return;
        if (previousSelection == this) highlightImage.gameObject.SetActive(false);
    }
   

    #region ISelectable Functions
    public void HighlightSelected()
    {
        highlightImage.gameObject.SetActive(true);
    }

    public void Select()
    {
        // TODO - Refactor this method to send grid coords instead for optimization purposes
        SelectionManager.instance.UpdateLastSelected(this);
    }

    public void Deselect()
    {
        highlightImage.gameObject.SetActive(false);
    }
    #endregion
}
