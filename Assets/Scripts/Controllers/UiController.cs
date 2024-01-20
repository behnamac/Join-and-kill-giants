using System.Collections;
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
        #region Public Properties

        public static UiController Instance { get; private set; }

        #endregion

        #region Serialized Fields

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

        #region Private Fields

        private int _levelFinishTotalCount;

        #endregion

        #region Initialization and Button Listeners

        private void InitializeUI()
        {
            SetButtonListeners();
            UpdateCoinDisplay();
            UpdateLevelDisplay();
        }

        private void SetButtonListeners()
        {
            levelStartPanel.GetComponentInChildren<Button>().onClick.AddListener(StartLevel);
            levelCompletePanel.GetComponentInChildren<Button>().onClick.AddListener(ReloadScene);
            levelFailPanel.GetComponentInChildren<Button>().onClick.AddListener(ReloadScene);
        }

        private void StartLevel()
        {
            levelStartPanel.SetActive(false);
            LevelManager.Instance.LevelStart();
        }

        private void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        #endregion

        #region Public Methods

        public void AddCoin(int coinCount)
        {
            var totalCoin = PlayerPrefsController.GetTotalCurrency();
            totalCoin += coinCount;
            PlayerPrefsController.SetCurrency(totalCoin);
            UpdateCoinDisplay();

            AnimateCoinIcon();
        }

        public void AddCoin(int coinCount, Vector3 spawnPos)
        {
            AddCoin(coinCount);
            StartCoroutine(SpawnCoinAnimation(coinCount, spawnPos));
        }

        #endregion

        #region Private Methods

        private void UpdateCoinDisplay()
        {
            coinText.text = PlayerPrefsController.GetTotalCurrency().ToString();
        }

        private void UpdateLevelDisplay()
        {
            int levelNumber = PlayerPrefsController.GetLevelNumber() + 1;
            levelText.text = $"LEVEL {levelNumber}";
        }

        private void AnimateCoinIcon()
        {
            coinIcon.transform.DOScale(1.2f, 0.2f).SetEase(Ease.InBounce)
                .OnComplete(() => coinIcon.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.InBounce));
        }

        private IEnumerator SpawnCoinAnimation(int coinCount, Vector3 spawnPoint)
        {
            for (int i = 0; i < coinCount; i++)
            {
                yield return new WaitForSeconds(0.1f);
                var pos = Camera.main != null ? Camera.main.WorldToScreenPoint(spawnPoint) : Vector3.zero;
                var coin = Instantiate(coinIcon, pos, Quaternion.identity, gamePlayPanel.transform);
                coin.transform.DOMove(coinIcon.transform.position, 0.2f);
                Destroy(coin.gameObject, 0.2f);
            }
        }

        #endregion

        #region Level Event Methods

        private void OnLevelStart(Level levelData)
        {
            if (PlayerPrefsController.GetLevelIndex() <= hideTutorialLevelIndex)
            {
                tutorialPanel.SetActive(true);
                Invoke(nameof(HideTutorial), 3);
            }
            gamePlayPanel.SetActive(true);
        }

        private void HideTutorial()
        {
            tutorialPanel.SetActive(false);
        }

        private void OnLevelComplete(Level levelData)
        {
            Invoke(nameof(ShowLevelCompletePanel), levelCompletePanelShowDelayTime);
        }

        private void ShowLevelCompletePanel()
        {
            tutorialPanel.SetActive(false);
            levelCompletePanel.SetActive(true);
        }

        private void OnLevelFail(Level levelData)
        {
            Invoke(nameof(ShowLevelFailPanel), levelFailPanelShowDelayTime);
        }

        private void ShowLevelFailPanel()
        {
            tutorialPanel.SetActive(false);
            levelFailPanel.SetActive(true);
        }

        #endregion

        #region Unity Event Methods

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                InitializeUI();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            LevelManager.OnLevelStart += OnLevelStart;
            LevelManager.OnLevelComplete += OnLevelComplete;
            LevelManager.OnLevelFail += OnLevelFail;
            // Subscribe to other LevelManager events if necessary
        }

        private void OnDestroy()
        {
            LevelManager.OnLevelStart -= OnLevelStart;
            LevelManager.OnLevelComplete -= OnLevelComplete;
            LevelManager.OnLevelFail -= OnLevelFail;
            // Unsubscribe from other LevelManager events if necessary
        }

        #endregion
    }
}
