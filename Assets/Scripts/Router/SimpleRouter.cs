using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SimpleRouter
{  
    public List<RouterPage> Pages;
    public Stack<RouterPage> RoutingStack = new Stack<RouterPage>();

    //Events
    public event Action<RouterPage> PagePushed;
    public event Action<RouterPage> PagePopped;

    public List<RouterPage> GetHistory()
    {
        return RoutingStack.ToList();
    }

    public void Push(string pageName, Dictionary<string, object> props)
    {
        RouterPage page = Pages.Find(p => p.Name == pageName);
        if (page == null)
        {
            Debug.LogError("Page not found");
            return;
        }

        if (RoutingStack.Count > 0)
        {
            RouterPage currentPage = RoutingStack.Peek();
            currentPage.Controller.OnPageExit();
            // TODO - Add switching animations etc
            currentPage.View.gameObject.SetActive(false);
        }

        Debug.LogFormat("Pushing page {0}", pageName);
        page.Controller.OnPageEnter(props);
        page.View.gameObject.SetActive(true);
        RoutingStack.Push(page);
        PagePushed?.Invoke(page);
    }

    public void Pop()
    {
        if (RoutingStack.Count == 0)
        {
            Debug.LogError("No pages to pop");
            return;
        }

        RouterPage currentPage = RoutingStack.Pop();
        currentPage.Controller.OnPageExit();
        currentPage.View.gameObject.SetActive(false);

        if (RoutingStack.Count > 0)
        {
            RouterPage previousPage = RoutingStack.Peek();
            previousPage.Controller.OnPageEnter(null);
            previousPage.View.gameObject.SetActive(true);
        }

        PagePopped?.Invoke(currentPage);
    }

    public SimpleRouter()
    {
        Pages = new List<RouterPage>();
    }

    public SimpleRouter(List<RouterPage> pages)
    {
        Pages = pages;
    }
}