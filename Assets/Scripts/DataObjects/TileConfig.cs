using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class TileConfig
{
    public Vector2 Offsets;
    public Vector2 AnchorMin;
    public Vector2 AnchorMax;
    public Vector2 AnchoredPosition;
    public Vector2 SizeDelta;
    public Vector2 Pivot;
    public List<SpriteConfig> SpriteSettings;
}