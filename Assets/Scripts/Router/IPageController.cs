using System;
using System.Collections.Generic;

public interface IPageController
{
    public void OnPageEnter(Dictionary<string, object> props);
    public void OnPageExit();
    public void OnPageUpdate();
}