using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TilePack
{
    public string PackName;
    public List<TileConfig> TileConfigurations;
    public List<string> BackgroundConfigurations;

    [NonSerialized]
    public List<TileStore> TileBank;
    [NonSerialized]
    public List<Sprite> BackgroundSprites;

    public List<TileElement> GetSpritePackFromLayerConfig(List<List<string>> level, int gridIndex)
    {
        List<TileElement> elements = new List<TileElement>();
        for(int i = 0; i < TileBank.Count; i++)
        {
            TileStore tileStore = TileBank[i];
            TileElement element = tileStore.GetTileElementById(Guid.Parse(level[i][gridIndex]));
            elements.Add(element);
        }
        
        return elements;
    }
}