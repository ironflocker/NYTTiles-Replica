using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : CustomSingleton<SelectionManager>
{
    public System.Action SelectionResetsEvent;
    public System.Action<SelectionHandler> LastSelectionChangedEvent;

    [Header("Debug")]
    [SerializeField] SelectionHandler previousSelected;
    [SerializeField] SelectionHandler lastSelectedCell;
    

    private void SetPreviousSelected()
    {
        previousSelected = lastSelectedCell;
    }
    public SelectionHandler GetPreviousSelected()
    {
        return previousSelected;
    }
    public SelectionHandler GetLastSelected()
    {
        return lastSelectedCell;
    }
    public void ResetSelections()
    {
        lastSelectedCell.Deselect();
        previousSelected = null;
        lastSelectedCell = null;
        
    }
    public void UpdateLastSelected(SelectionHandler lastSelection)
    {
        if (lastSelection == lastSelectedCell)
        {
            Debug.Log("Selection cannot be the same with lastSelectedCell !");
            return;
        }
            
        SetPreviousSelected();
        lastSelectedCell = lastSelection;
        LastSelectionChangedEvent?.Invoke(lastSelection);
    }
}
