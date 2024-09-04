using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;

public class GamePageController : MonoBehaviour, IPageController
{

    [Header("Gameplay References")]
    public CustomGrid GridHandler;

    [Header("UI References")]
    public TextMeshProUGUI CurrentComboText;
    public TextMeshProUGUI LongestComboText;
    public TextMeshProUGUI TutorialSuccessText;
    public Image CheckImage;

    [Header("UI Containers")]
    public RectTransform ScorePanel;
    public RectTransform TutorialPanel;
    public RectTransform TutorialDirectivesPanel;
    public RectTransform GridPanel;
    [SerializeField] private Vector2 _gridPanelOffset;
    [SerializeField] private Vector2 _scorePanelAnchor;

    private void Start()
    {
        GridHandler.AllCellsEmptyEvent += OnAllCellsCleared;
    }

    private void OnAllCellsCleared()
    {
        StartCoroutine(TutorialCompleteRoutine());
    }

    private IEnumerator TutorialCompleteRoutine()
    {
        if (GameManager.instance.GetTotalStagePlayed() == 1) //Tutorial ise
        {
            TutorialSuccessText.gameObject.SetActive(true);
            TutorialSuccessText.transform.DOScale(Vector3.one, 0.5f).From(Vector3.zero);

            CheckImage.gameObject.SetActive(true);
            CheckImage.transform.DOScale(Vector3.one, 0.5f).From(Vector3.zero);

            yield return new WaitForSeconds(1.5f);
            GridHandler.ClearLevel(true, true);
        }
        else
        {
            GridHandler.ClearLevel(false, true);
        }
        yield return null;
    }

    public void OnPageEnter(Dictionary<string, object> props)
    {
        UpdateScorePanelPosition(GridHandler.CurrentGridWidth, GridHandler.CurrentGridHeight);
        props.TryGetValue("totalStagesPlayed", out object totalStagesPlayed);
        if (totalStagesPlayed != null)
        {
            Debug.LogFormat("Total Stages Played: {0}", totalStagesPlayed);
            SetTutorialLayout((int)totalStagesPlayed == 1);
        }
    }

    public void SetTutorialLayout(bool isTutorial)
    {
        if(isTutorial)
        {   
            TutorialDirectivesPanel.gameObject.SetActive(true);
            GridPanel.localScale = new Vector3(0.7f, 0.7f, 0.7f);
            TutorialPanel.anchoredPosition = new Vector2(TutorialPanel.anchoredPosition.x, GridPanel.anchoredPosition.y + 100);
        }
    }

    public void OnPageExit()
    {
        
    }

    public void OnPageUpdate()
    {
        
    }

    /* Event Handlers */
    private void UpdateScorePanelPosition(float gridWidth, float gridHeight)
    {
        ScorePanel.anchoredPosition = new Vector2(0, -(gridHeight/2 + 75));
        ScorePanel.sizeDelta = new Vector2 (gridWidth, ScorePanel.sizeDelta.y);
        ScorePanel.localScale = Vector2.one;
    }

    private void OnScoreChanged()
    {
        CurrentComboText.text = ScoreManager.instance.GetCurrentComboCount().ToString();
        LongestComboText.text = ScoreManager.instance.GetLongestComboCount().ToString();
    }

    /* Initialization Stack  */
    private void SubscribeEvents()
    {
        Debug.LogFormat("Subscribing to ScoreManager Events");
        ScoreManager.instance.ComboScoreChangedEvent += OnScoreChanged;
    }

    private void SetDefaults()
    {
        Debug.LogFormat("Setting Default Values");
        _gridPanelOffset = GridPanel.offsetMax;
        _scorePanelAnchor = ScorePanel.anchoredPosition;
        Debug.LogFormat("Grid Panel Offset: {0}", _gridPanelOffset);
        Debug.LogFormat("Score Panel Anchor: {0}", _scorePanelAnchor);
    }

    void Awake()
    {
        SetDefaults();
        SubscribeEvents();
    }
}