using System;
using UnityEngine;
public class TileElement
{
    public Guid ElementId;
    public string Ref;
    public Sprite Sprite;
    public float Rotation;
    public int OccuranceCount = 0;

    public int MaxOccurance,MinOccurance = -1;

    public TileElement(string reference, Sprite sprite, float rotation = 0, int maxOccurance = -1, int minOccurance = -1)
    {
        Ref = reference;
        ElementId = Guid.NewGuid();
        Sprite = sprite;
        Rotation = rotation;
        MaxOccurance = maxOccurance;
        MinOccurance = minOccurance;
    }
}