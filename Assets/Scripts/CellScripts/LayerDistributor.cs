using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayerDistributor : MonoBehaviour
{
    [SerializeField] List<Image> CellLayers;
    public List<TileElement> CellElements;
    public GameObject selectionFrame;
    public Image higlightImage;
    private void SetRectPropertiesFromConfig(TileConfig config, Image image)
    {
        image.rectTransform.anchorMin = config.AnchorMin;    
        image.rectTransform.anchorMax = config.AnchorMax;
        image.rectTransform.anchoredPosition = config.AnchoredPosition + config.Offsets;
        image.rectTransform.sizeDelta = config.SizeDelta;
        image.rectTransform.pivot = config.Pivot;
    }

    public void DistributeLayers(TilePack pack, List<TileElement> tileElements, Sprite boardSprite)
    {
        CellLayers = new List<Image>();

        GameObject gridCell = new GameObject("GridCell_" + 0);
        gridCell.transform.SetParent(transform, false);
        RectTransform gridRectTransform = gridCell.AddComponent<RectTransform>();

        gridRectTransform.anchorMin = new Vector2(0, 0);
        gridRectTransform.anchorMax = new Vector2(1, 1);

        gridRectTransform.offsetMin = new Vector2(0, 0);  // left & bottom side 
        gridRectTransform.offsetMax = new Vector2(0, 0); // right and top side

        //TODO : Create image and set its sprite as gridIndex%2
        Image gridImage = gridCell.AddComponent<Image>();
        gridImage.canvas.sortingOrder = -1;
        gridImage.sprite = boardSprite;
        


        for (int i = 0; i < tileElements.Count; i++)
        {
            TileConfig config = pack.TileConfigurations[i];
            TileElement element = tileElements[i];
            GameObject layer = new GameObject("Layer" + i);
            layer.transform.SetParent(transform, false); 
            RectTransform layerRectTransform = layer.AddComponent<RectTransform>(); 
            Image image = layer.AddComponent<Image>();
            image.sprite = tileElements[i].Sprite;
            image.rectTransform.rotation = Quaternion.Euler(0, 0, element.Rotation);
            image.canvas.sortingOrder = i;
            layer.AddComponent<LayerDestroyer>();
            SetRectPropertiesFromConfig(config, image);
            CellLayers.Add(image);
        }
        selectionFrame = Instantiate(higlightImage.gameObject, transform);
        CellElements = tileElements;
    }
    
    public int GetCellLayerCount()
    {
        return CellLayers.Count;
    }
    public Sprite GetCellLayerSprite(int layerIndex)
    {
        return CellLayers[layerIndex].sprite;
    }
    public Guid GetCellElementByIndex(int layerIndex)
    {
        //Debug.Log(CellElements[layerIndex].ElementId);
        return CellElements[layerIndex].ElementId;
    }
    public GameObject GetCellLayerByIndex(int index) 
    {
        return CellLayers[index].gameObject;
    }
    public void RemoveLayerByIndex(int index)
    {
        CellLayers.RemoveAt(index);
        CellElements.RemoveAt(index);
    }
}
