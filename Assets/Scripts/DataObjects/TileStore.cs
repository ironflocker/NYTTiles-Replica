using System;
using System.Collections.Generic;
using UnityEngine;

public class TileStore
{
    public List<TileElement> TileElements;

    public TileElement GetTileByReference(string reference)
    {
        foreach (TileElement tileElement in TileElements)
        {
            if (tileElement.Ref == reference)
            {
                return tileElement;
            }
        }

        return null;
    }

    public TileElement GetTileElementById(Guid elementId)
    {
        foreach (TileElement tileElement in TileElements)
        {
            if (tileElement.ElementId == elementId)
            {
                return tileElement;
            }
        }

        return null;
    }

    public List<TileElement> GetEligibleTiles()
    {
        List<TileElement> eligibleTiles = new List<TileElement>();
        foreach (TileElement tileElement in TileElements)
        {
            if (tileElement.MaxOccurance == -1 || tileElement.OccuranceCount < tileElement.MaxOccurance)
            {
                eligibleTiles.Add(tileElement);
            }
        }
        //Sort eligible tiles by occurance count ascending
        eligibleTiles.Sort((x, y) => x.OccuranceCount.CompareTo(y.OccuranceCount));
        return eligibleTiles;
    }

    public List<string> GetEnforcedTiles()
    {
        List<string> enforcedTiles = new List<string>();
        foreach (TileElement tileElement in TileElements)
        {
            if (tileElement.MinOccurance > 0)
            {
                for (int i = 0; i < tileElement.MinOccurance; i++)
                {
                    enforcedTiles.Add(tileElement.ElementId.ToString());
                    tileElement.OccuranceCount++;
                }
            }
        }
        return enforcedTiles;
    }

    public void AddTileElement(TileElement tileElement)
    {
        TileElements.Add(tileElement);
    }

    public TileStore()
    {
        TileElements = new List<TileElement>();
    }
}