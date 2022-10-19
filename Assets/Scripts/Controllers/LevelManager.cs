using Levels;
using Storage;
using UnityEngine;

namespace Controllers
{
    public class LevelManager : MonoBehaviour
    {
        #region DELEGATE

        public delegate void LevelLoadHandler(Level levelData);

        public delegate void LevelStartHandler(Level levelData);

        public delegate void LevelStageCompleteHandler(Level levelData, int stageIndex = 0);

        public delegate void LevelCompleteHandler(Level levelData);

        public delegate void LevelFailHandler(Level levelData);

        #endregion

        #region EVENTS

        public static LevelLoadHandler onLevelLoad;

        public static LevelStartHandler onLevelStart;

        public static LevelStageCompleteHandler onLevelStageComplete;

        public static LevelCompleteHandler onLevelComplete;

        public static LevelFailHandler onLevelFail;

        #endregion

        #region PUBLIC FIELDS / PROPS

        public static LevelManager instance { get; private set; }

        #endregion

        #region SERIALIZE PRIVATE FIELDS

        [SerializeField] private LevelSource levelSource;

        [SerializeField] private GameObject levelSpawnPoint;

        [SerializeField] private int loopLevelsStartIndex = 1;

        [SerializeField] private bool loopLevelGetRandom = true;

        #endregion

        #region PRIVATE FIELDS

        private GameObject _activeLevel;

        #endregion

        #region PRIVATE METHODS

        private void CheckRepeatLevelIndex()
        {
            if (loopLevelsStartIndex < levelSource.levelData.Length) return;
            loopLevelsStartIndex = 0;
        }

        private GameObject GetLevel()
        {
            if (PlayerPrefsController.GetLevelIndex() >= levelSource.levelData.Length)
            {
                if (loopLevelGetRandom)
                {
                    var levelIndex = Random.Range(loopLevelsStartIndex, levelSource.levelData.Length - 1);
                    PlayerPrefsController.SetLevelIndex(levelIndex);
                }
            }

            var level = levelSource.levelData[PlayerPrefsController.GetLevelIndex()];

            var levelData = level.GetComponent<Level>();

            levelData.levelIndex = PlayerPrefsController.GetLevelIndex();
            levelData.levelNumber = PlayerPrefsController.GetLevelNumber();

            return level;
        }

        #endregion

        #region PUBLIC METHODS

        
        public void LevelLoad()
        {
            _activeLevel = Instantiate(GetLevel(), levelSpawnPoint.transform, false);
            onLevelLoad?.Invoke(_activeLevel.GetComponent<Level>());
        }

        
        public void LevelStart()
        {
            onLevelStart?.Invoke(_activeLevel.GetComponent<Level>());
        }

        
        public void LevelStageComplete(int stageIndex = 0)
        {
            onLevelStageComplete?.Invoke(_activeLevel.GetComponent<Level>(), stageIndex);
        }

        
        public void LevelComplete()
        {
            PlayerPrefsController.SetLevelIndex(PlayerPrefsController.GetLevelIndex() + 1);

            PlayerPrefsController.SetLevelNumber(PlayerPrefsController.GetLevelNumber() + 1);


            onLevelComplete?.Invoke(_activeLevel.GetComponent<Level>());
        }
        
        public void LevelFail()
        {
            onLevelFail?.Invoke(_activeLevel.GetComponent<Level>());
        }

        #endregion

        #region UNITY EVENT METHODS

        private void Awake()
        {
            CheckRepeatLevelIndex();
            instance = this;
        }

        private void Start() => LevelLoad();

        #endregion
    }
}