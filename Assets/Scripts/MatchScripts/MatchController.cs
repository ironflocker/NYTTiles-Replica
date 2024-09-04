using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class MatchController : CustomSingleton<MatchController>
{
    public System.Action<bool> MatchFoundEvent;

    bool matchFound = false;

    private void Start()
    {
        SelectionManager.instance.LastSelectionChangedEvent += OnLastSelectionChanged;
    }
    
    public void OnLastSelectionChanged(SelectionHandler lastSelected)
    {
        SelectionHandler previousSelection = SelectionManager.instance.GetPreviousSelected();
        if (previousSelection == null)
        {
            Debug.Log("no previousSelection Found!");
            return;
        }

        LayerDistributor previousSelectionLayers = previousSelection.GetComponent<LayerDistributor>();
        LayerDistributor lastSelectionLayers = lastSelected.GetComponent<LayerDistributor>();

        GetMatchedLayer(previousSelectionLayers, lastSelectionLayers);
        
    }
    private void GetMatchedLayer(LayerDistributor previousSelectionLayer, LayerDistributor lastSelectionLayer)
    {
        matchFound = false;
        if (previousSelectionLayer.transform.childCount <= 2) 
        {
            Debug.Log("PreviousSelection was empty !");
            return;
        }
        for (int i = 0; i < previousSelectionLayer.GetCellLayerCount(); i++)
        {
            for (int j = 0; j < lastSelectionLayer.GetCellLayerCount(); j++)
            {
                // Checking Layers with same index.
                if (previousSelectionLayer.GetCellElementByIndex(i) == lastSelectionLayer.GetCellElementByIndex(j))
                {
                    //Debug.Log()
                    LayerDestroyer prev = previousSelectionLayer.GetCellLayerByIndex(i).GetComponent<LayerDestroyer>();
                    LayerDestroyer last = lastSelectionLayer.GetCellLayerByIndex(j).GetComponent<LayerDestroyer>();

                    prev.DestroyMatchedObject();
                    last.DestroyMatchedObject();

                    previousSelectionLayer.RemoveLayerByIndex(i);
                    lastSelectionLayer.RemoveLayerByIndex(j);
                    
                    Debug.Log("Matching Layers..");
                    matchFound = true;
                    MatchFoundEvent?.Invoke(true);

                    // Reset loop
                    i = -1;
                    break;
                }
            }
        }

        SelectionHandler lastSelected = SelectionManager.instance.GetLastSelected();
        if (lastSelected.transform.childCount <=0 )
        {
            Debug.Log("LSL ");
            SelectionManager.instance.ResetSelections();
        }
        if (!matchFound)
        {
            Debug.Log("No Match Found! Combo resets..");
            MatchFoundEvent?.Invoke(false);
        }
    }
}
