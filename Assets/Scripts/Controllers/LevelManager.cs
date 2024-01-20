using Levels;
using Storage;
using UnityEngine;

namespace Controllers
{
    public class LevelManager : MonoBehaviour
    {
        #region Delegates and Events

        public delegate void LevelLoadHandler(Level levelData);
        public delegate void LevelStartHandler(Level levelData);
        public delegate void LevelStageCompleteHandler(Level levelData, int stageIndex);
        public delegate void LevelCompleteHandler(Level levelData);
        public delegate void LevelFailHandler(Level levelData);

        public static event LevelLoadHandler OnLevelLoad;
        public static event LevelStartHandler OnLevelStart;
        public static event LevelStageCompleteHandler OnLevelStageComplete;
        public static event LevelCompleteHandler OnLevelComplete;
        public static event LevelFailHandler OnLevelFail;

        #endregion

        #region Singleton

        public static LevelManager Instance { get; private set; }

        #endregion

        #region Serialized Fields

        [SerializeField, Tooltip("The source of level data.")]
        private LevelSource levelSource;

        [SerializeField, Tooltip("The spawn point for levels.")]
        private GameObject levelSpawnPoint;

        [SerializeField, Tooltip("Index from where levels start looping.")]
        private int loopLevelsStartIndex = 1;

        [SerializeField, Tooltip("Whether to get random levels when looping.")]
        private bool loopLevelGetRandom = true;

        #endregion

        #region Private Fields

        private GameObject _activeLevel;

        #endregion

        #region Private Methods

        private void CheckRepeatLevelIndex()
        {
            if (levelSource == null || levelSource.levelData.Length <= loopLevelsStartIndex)
            {
                loopLevelsStartIndex = 0;
            }
        }

        private GameObject GetLevel()
        {
            if (levelSource == null)
            {
                Debug.LogError("Level source is not assigned.");
                return null;
            }

            int levelIndex = PlayerPrefsController.GetLevelIndex();
            if (levelIndex >= levelSource.levelData.Length)
            {
                if (loopLevelGetRandom)
                {
                    levelIndex = Random.Range(loopLevelsStartIndex, levelSource.levelData.Length);
                    PlayerPrefsController.SetLevelIndex(levelIndex);
                }
            }

            var level = Instantiate(levelSource.levelData[levelIndex], levelSpawnPoint.transform, false);
            var levelData = level.GetComponent<Level>();

            if (levelData != null)
            {
                levelData.levelIndex = levelIndex;
                levelData.levelNumber = PlayerPrefsController.GetLevelNumber();
            }

            return level;
        }

        #endregion

        #region Public Methods

        public void LevelLoad()
        {
            _activeLevel = GetLevel();
            OnLevelLoad?.Invoke(_activeLevel.GetComponent<Level>());
        }

        public void LevelStart()
        {
            OnLevelStart?.Invoke(_activeLevel.GetComponent<Level>());
        }

        public void LevelStageComplete(int stageIndex = 0)
        {
            OnLevelStageComplete?.Invoke(_activeLevel.GetComponent<Level>(), stageIndex);
        }

        public void LevelComplete()
        {
            int currentLevelIndex = PlayerPrefsController.GetLevelIndex();
            PlayerPrefsController.SetLevelIndex(currentLevelIndex + 1);

            int currentLevelNumber = PlayerPrefsController.GetLevelNumber();
            PlayerPrefsController.SetLevelNumber(currentLevelNumber + 1);

            OnLevelComplete?.Invoke(_activeLevel.GetComponent<Level>());
        }

        public void LevelFail()
        {
            OnLevelFail?.Invoke(_activeLevel.GetComponent<Level>());
        }

        #endregion

        #region Unity Event Methods

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                CheckRepeatLevelIndex();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            LevelLoad();
        }

        #endregion
    }
}
