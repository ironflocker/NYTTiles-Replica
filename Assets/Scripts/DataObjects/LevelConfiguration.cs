using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Serializable]
public class LevelConfiguration
{
    public int GridSizeX;

    public int GridSizeY;

    public int CellPaddingX;

    public int CellPaddingY;

    public List<List<string>> LayerConfigurations;
}