using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CanvasManager : CustomSingleton<CanvasManager>
{
    [Header("Routing Configuration")]
    public List<RouterPageConfig> PageConfigurations;
    public SimpleRouter Router;

    private void InitializeRouter()
    {
        Debug.LogFormat("Initializing Router with {0} pages", PageConfigurations.Count);
        Router = new SimpleRouter(PageConfigurations.ConvertAll(p => p.ToRouterPage()));
        Router.Push("MainMenu", null);
    }

    private void Start()
    {
        InitializeRouter();

        //Event Registries
        GameManager.instance.LevelStartedEvent += OnLevelStarted;
        GameManager.instance.LevelSuccessEvent += OnLevelSuccessfull;
    }

    private void OnLevelStarted(int totalStagesPlayed)
    {
        Debug.Log("Level Started");
        Router.Push("Game", new Dictionary<string, object> {
            {"totalStagesPlayed", totalStagesPlayed}
        });
    }
    private void OnLevelSuccessfull()
    {
        Router.Push("End", null);
    }
}
