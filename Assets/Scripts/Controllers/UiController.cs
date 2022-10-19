using System.Collections.Generic;
using DG.Tweening;
using Levels;
using Storage;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Controllers
{
    public class UiController : MonoBehaviour
    {
        #region PUBLIC PROPS

        public static UiController instance { get; private set; }

        #endregion

        #region SERIALIZE FIELDS

        [Header("Panels")]
        [SerializeField] private GameObject gamePlayPanel;

        [SerializeField] private GameObject levelStartPanel;
        [SerializeField] private GameObject levelCompletePanel;
        [SerializeField] private GameObject levelFailPanel;
        [SerializeField] private GameObject tutorialPanel;

        [Header("Coin")]
        [SerializeField] private Image coinIcon;

        [SerializeField] private Text coinText;

        [Header("Level")]
        [SerializeField] private Text levelText;

        [Header("Settings Value")]
        [SerializeField] private int hideTutorialLevelIndex;

        [SerializeField] private float levelCompletePanelShowDelayTime;
        [SerializeField] private float levelFailPanelShowDelayTime;

        #endregion

        #region PRIVATE FIELDS

        private int _levelFinishTotalCount;

        #endregion

        #region PRIVATE METHODS

        private void Initializer()
        {
            // Level Start
            levelStartPanel.GetComponentInChildren<Button>().onClick.AddListener(() =>
            {
                levelStartPanel.SetActive(false);
                LevelManager.instance.LevelStart();
            });

            // Level Complete
            levelCompletePanel.GetComponentInChildren<Button>().onClick.AddListener(() =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            });

            // Level Fail
            levelFailPanel.GetComponentInChildren<Button>().onClick.AddListener(() =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            });

            // Set TotalCoin;
            coinText.text = PlayerPrefsController.GetTotalCurrency().ToString();

            // Set Level Number
            var levelNumber = PlayerPrefsController.GetLevelNumber() + 1;
            levelText.text = $"LEVEL {levelNumber}";
            
        }

        private void ShowTutorial()
        {
            if (PlayerPrefsController.GetLevelIndex() > hideTutorialLevelIndex) return;
            tutorialPanel.SetActive(true);

            Invoke(nameof(HideTutorial), 3);
        }

        private void HideTutorial()
        {
            tutorialPanel.SetActive(false);
        }

        private void ShowLevelCompletePanel()
        {
            if (tutorialPanel.activeSelf)
            {
                tutorialPanel.SetActive(false);
            }

            levelCompletePanel.SetActive(true);
        }

        private void ShowLevelFailPanel()
        {
            if (tutorialPanel.activeSelf)
            {
                tutorialPanel.SetActive(false);
            }

            levelFailPanel.SetActive(true);
        }

        #endregion

        #region PUBLIC METHODS

        public void AddCoin(int coinCount)
        {

            var totalCoin = PlayerPrefsController.GetTotalCurrency();

            totalCoin += coinCount;

            PlayerPrefsController.SetCurrency(totalCoin);

            coinText.text = totalCoin.ToString();

            coinIcon.transform.DOScale(1.2f, 0.2f).SetEase(Ease.InBounce).OnComplete(() =>
            {
                coinIcon.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.InBounce);
            });
        }

        public void AddCoin(int coinCount, Vector3 spawnPos)
        {
            AddCoin(coinCount);
            StartCoroutine(SpawnCoinCo(coinCount, spawnPos));
        }
        #endregion

        #region CUSTOM EVENTS

        private void OnLevelFail(Level levelData)
        {
            Invoke(nameof(ShowLevelFailPanel), levelFailPanelShowDelayTime);
        }

        private void OnLevelStart(Level levelData)
        {
            ShowTutorial();
            gamePlayPanel.SetActive(true);
        }

        private void OnLevelComplete(Level levelData)
        {
            Invoke(nameof(ShowLevelCompletePanel), levelCompletePanelShowDelayTime);
        }

        private void OnLevelStageComplete(Level levelData, int stageIndex)
        {
            // TODO : IF DONT NEED THIS METHODS, YOU DONT REMOVE
        }

        private IEnumerator<WaitForSeconds> SpawnCoinCo(int coinCount, Vector3 spawnPoint)
        {
            for (int i = 0; i < coinCount; i++)
            {
                yield return new WaitForSeconds(0.1f);
                Vector3 pos = Vector3.zero;
                if (Camera.main != null)
                {
                    pos = Camera.main.WorldToScreenPoint(spawnPoint);
                }

                var coi = Instantiate(coinIcon, pos, Quaternion.identity,
                    gamePlayPanel.transform);
                coi.transform.DOMove(coinIcon.transform.position, 0.2f);
                Destroy(coi.gameObject, 0.2f);
            }
        }

        #endregion

        #region UNITY EVENT METHODS

        private void Awake()
        {
            Initializer();

            if (instance == null) instance = this;
        }

        private void Start()
        {
            LevelManager.onLevelStart += OnLevelStart;
            LevelManager.onLevelComplete += OnLevelComplete;
            LevelManager.onLevelFail += OnLevelFail;
            LevelManager.onLevelStageComplete += OnLevelStageComplete;
        }


        private void OnDestroy()
        {
            LevelManager.onLevelStart -= OnLevelStart;
            LevelManager.onLevelComplete -= OnLevelComplete;
            LevelManager.onLevelFail -= OnLevelFail;
            LevelManager.onLevelStageComplete -= OnLevelStageComplete;
        }

        #endregion
    }
}