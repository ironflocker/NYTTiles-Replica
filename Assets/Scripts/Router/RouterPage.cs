using System;
using UnityEngine;

[Serializable]
public class RouterPage
{
    public string Name { get; set; }
    public IPageController Controller { get; set; }
    public RectTransform View { get; set; }
}

[Serializable]
public class RouterPageConfig
{
    public string Name;
    public MonoBehaviour Controller;
    public RectTransform View;

    public RouterPage ToRouterPage()
    {
        return new RouterPage
        {
            Name = Name,
            Controller = (IPageController) Controller,
            View = View
        };
    }
}