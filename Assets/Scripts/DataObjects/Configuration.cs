using System;
using System.Collections.Generic;

[Serializable]
public class Configuration
{
    public int GridSizeX;
    public int GridSizeY;
    public int CellPaddingX;
    public int CellPaddingY;
    public List<TilePack> TilePacks;
}