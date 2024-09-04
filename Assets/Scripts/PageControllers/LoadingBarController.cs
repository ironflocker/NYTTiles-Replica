using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingBarController : MonoBehaviour
{
    public float loadingTime; // Doldurma süresi
    public Image Filler; // Doldurulacak UI image

    private void Start()
    {
        // Coroutine'i baþlatýyoruz
        StartCoroutine(FillLoadingBar());
    }

    private IEnumerator FillLoadingBar()
    {
        float elapsedTime = 0f; 

        while (elapsedTime < loadingTime)
        {
            elapsedTime += Time.deltaTime; 
            Filler.fillAmount = Mathf.Clamp01(elapsedTime / loadingTime); 
            yield return null;
        }

        
        Filler.fillAmount = 1f;
        GameManager.instance.StartGame();
    }
}
