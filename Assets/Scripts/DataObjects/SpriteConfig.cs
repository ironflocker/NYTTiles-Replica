using System;
using System.Collections.Generic;

[Serializable]
public class SpriteConfig
{
    public string Ref;
    public string AssetPath;
    public DistributionSettings DistributionSettings;
    public List<AugmentationConfig> AugmentationSettings;
}