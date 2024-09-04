using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using DG.Tweening;
using UnityEngine.Networking;

public class LevelManager : CustomSingleton<LevelManager>
{
    public enum LevelLoadMethod
    {
        Generate,
        LoadFromConfig
    }

    [Header("Tile Packs")]
    public List<TilePack> tilePacks;
    public TilePack activePack;
    public List<List<string>> level;

    //TODO - REFACTOR THIS OUT OF HERE WHEN PRODUCT IS READY
    [Header("Level Generation Settings")]
    public LevelLoadMethod LoadMethod;
    public string LevelName;

    [Header("Grid Dependencies")]
    public CustomGrid GridHandler;
    public bool allCellsValid;

    [Header("Grid Adjustments")]
    [SerializeField] private int rows = 5;
    [SerializeField] private int columns = 5;
    [SerializeField] private Vector2 cellPadding = new Vector2(10, 10);
    
    void Start()
    {
        InitializeConfiguration();
        MatchController.instance.MatchFoundEvent += OnMatchFound;
    }

    private void InitializeConfiguration()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Test.json");
        GameConfiguration gameConfig = LoadJsonConfiguration<GameConfiguration>(path);
         LoadConfiguration(gameConfig.Configuration);
    }

    //TODO - Refactor this out of the level manager, potentially to a auxilliary class
    private T LoadJsonConfiguration<T>(string path)
    {
        string jsonString;

        if (Application.platform == RuntimePlatform.Android)
        {
            // On Android, use UnityWebRequest
            string androidPath = "jar:file://" + Application.dataPath + path;
            UnityWebRequest www = UnityWebRequest.Get(androidPath);
            www.SendWebRequest();

            // Wait for request to complete
            while (!www.isDone) { }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to load JSON: " + www.error);
                return default(T);
            }

            jsonString = www.downloadHandler.text;
        }
        else
        {
            // For PC, iOS, and other platforms, use File.ReadAllText
            if (!File.Exists(path))
            {
                Debug.LogError("File does not exist!");
                return default(T);
            }
            jsonString = File.ReadAllText(path);
        }

        try
        {
            Debug.Log("Loaded JSON: " + jsonString);
            T loadedFile = JsonConvert.DeserializeObject<T>(jsonString);
            return loadedFile;
        }
        catch (JsonException e)
        {
            Debug.LogError("Failed to deserialize JSON: " + e.Message);
            return default(T);
        }
    }

    //TODO - This method should not have static loaders, it should be dynamic
    private void LoadFromConfig(string levelName)
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Levels", levelName + ".json");
        LevelConfiguration levelConfig = LoadJsonConfiguration<LevelConfiguration>(path);
        Debug.Log("Loaded Level Configuration: " + levelConfig.GridSizeX + "x" + levelConfig.GridSizeY);
        rows = levelConfig.GridSizeX;
        columns = levelConfig.GridSizeY;

        int flattenedMapSize = rows * columns;
        List<List<string>> levelMapping = new List<List<string>>();
        //TODO - Add error checks if layers are greater than tile pack supports or rows*cols is not equals to count i,j
        for (int i = 0; i < levelConfig.LayerConfigurations.Count; i++)
        {
            TileStore store = activePack.TileBank[i];
            List<string> layer = new List<string>();
            for (int j = 0; j < levelConfig.LayerConfigurations[i].Count; j++)
            {
                TileElement element = store.GetTileByReference(levelConfig.LayerConfigurations[i][j]);
                layer.Add(element.ElementId.ToString());
            }
            levelMapping.Add(layer);
        }

        level = levelMapping;
    }

    private void InitializeLevel()
    {
        switch (LoadMethod)
        {
            case LevelLoadMethod.Generate:
                GenerateLevel();
                break;
            case LevelLoadMethod.LoadFromConfig:
                //TODO - Refactor LevelName usage
                LoadFromConfig(LevelName);
                break;
            default:
                break;
        }
    }

    private void LoadConfiguration(Configuration config)
    {
        LoadTilePacks(config.TilePacks);
        InitializeGridFromConfig(config);
        InitializeLevel();
        GridHandler.InitializeGrid(rows, columns, cellPadding, activePack, level);
    }
    private void LoadTilePacks(List<TilePack> configPacks)
    {
        foreach (TilePack pack in configPacks)
        {
            List<TileStore> tileBank = new List<TileStore>();
            foreach (TileConfig tileConfiguration in pack.TileConfigurations)
            {
                TileStore store = new TileStore();
                for (int i = 0; i < tileConfiguration.SpriteSettings.Count; i++)
                {
                    SpriteConfig spriteConfig = tileConfiguration.SpriteSettings[i];
                    Sprite sprite = Resources.Load<Sprite>(spriteConfig.AssetPath);
                    if (sprite == null)
                    {
                        Debug.LogError("Sprite not found at path " + spriteConfig.AssetPath);
                        return;
                    }
                    // Create base tile element
                    store.AddTileElement(new TileElement(spriteConfig.Ref, sprite, 0, spriteConfig.DistributionSettings.EnforceMaxCap, spriteConfig.DistributionSettings.EnforceMinCap));
                    Debug.Log("Sprite added to store: " + sprite.name);
                    // Check for augmentations
                    if (spriteConfig.AugmentationSettings != null)
                    {
                        for (int j = 0; j < spriteConfig.AugmentationSettings.Count; j++)
                        {
                            string augmentedRef = spriteConfig.Ref + "_rot_" + spriteConfig.AugmentationSettings[j].RotationAngle;
                            store.AddTileElement(new TileElement(augmentedRef, sprite, spriteConfig.AugmentationSettings[j].RotationAngle, spriteConfig.DistributionSettings.EnforceMaxCap, spriteConfig.DistributionSettings.EnforceMinCap));
                            Debug.Log("Augmented Sprite added to store: " + sprite.name + " with rotation " + spriteConfig.AugmentationSettings[j].RotationAngle);
                        }
                    }
                }
                tileBank.Add(store);
            }

            List<Sprite> bgSprites = new List<Sprite>();
            foreach(string bgPath in pack.BackgroundConfigurations)
            {
                Sprite bgSprite = Resources.Load<Sprite>(bgPath);
                if (bgSprite == null)
                {
                    Debug.LogError("Background Sprite not found at path " + bgPath);
                }
                bgSprites.Add(bgSprite);
            }
            pack.BackgroundSprites = bgSprites;
            pack.TileBank = tileBank;
        }

        tilePacks = configPacks;

        // TODO - REMOVE WHEN TILEPACK SELECTION LOGIC IS MADE
        activePack = tilePacks[0];
    }
    private void InitializeGridFromConfig(Configuration config)
    {
        rows = config.GridSizeX;
        columns = config.GridSizeY;
        cellPadding = new Vector2(config.CellPaddingX, config.CellPaddingY);
    }
    private void GenerateLevel()
    {
        int flattenedMapSize = rows * columns;
        List<List<string>> levelMapping = new List<List<string>>();
        for (int j = 0; j < activePack.TileBank.Count; j++)
        {
            TileStore store = activePack.TileBank[j];
            List<string> initialPool = store.GetEnforcedTiles();
            if (initialPool.Count > flattenedMapSize || initialPool.Count % 2 != 0)
            {
                Debug.LogError("Enforced tiles exceed grid size or are not even!");
                return;
            }
            while (initialPool.Count < flattenedMapSize)
            {
                List<TileElement> eligibleTiles = store.GetEligibleTiles();
                if (eligibleTiles.Count == 0)
                {
                    Debug.LogError("No eligible tiles left to fill the grid!");
                    return;
                }
                TileElement selectedTile = eligibleTiles[0];
                initialPool.Add(selectedTile.ElementId.ToString());
                initialPool.Add(selectedTile.ElementId.ToString());
                selectedTile.OccuranceCount += 2;
            }
            levelMapping.Add(initialPool);
            ShuffleTileCombinations(initialPool);
            Debug.LogFormat("Layer {0} generated with {1} tiles with Flattened Map Size of {2}", j, initialPool.Count, flattenedMapSize);
        }

        level = levelMapping;
    }
    private void ShuffleTileCombinations(List<string> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            string value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    private void OnMatchFound(bool hasFound)
    {
        
        if (hasFound)
        {
           
        }
    }
    
}
