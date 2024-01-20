using UnityEngine;
using Controllers;
using Levels;
using Enemy;

namespace Player
{
    public class PlayerContainerController : MonoBehaviour
    {
        [SerializeField, Tooltip("Forward speed of the player.")]
        private float speedForward;

        [SerializeField, Tooltip("Horizontal speed of the player.")]
        private float speedHorizontal;

        [SerializeField, Tooltip("Maximum horizontal movement limit.")]
        private float maxHorizontalMove;

        private bool _canMove;
        private float _mouseXStartPosition;
        private float _swipeDelta;
        private float _xMove;
        private int _allPlayerNumber;
        private float _activePlayerNumber;
        private float _postLevelValue;
        private int _postLevelXNumber;

        private const int PostLevelMaxCount = 10;

        public GiantController Giant { get; private set; }

        private void Awake()
        {
            SubscribeToLevelEvents();
        }

        private void Update()
        {
            if (_canMove)
            {
                Movement();
            }
        }

        private void OnDestroy()
        {
            UnsubscribeFromLevelEvents();
        }

        private void Movement()
        {
            // Forward Movement
            transform.Translate(0, 0, speedForward * Time.deltaTime);

            // Horizontal Movement
            _xMove = Mathf.Clamp(_xMove - TouchSwipe() * speedHorizontal * Time.deltaTime, -maxHorizontalMove, maxHorizontalMove);
            var playerPosition = transform.position;
            playerPosition.x = _xMove;
            transform.position = playerPosition;
        }

        private float TouchSwipe()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _mouseXStartPosition = Input.mousePosition.x;
            }

            if (Input.GetMouseButton(0))
            {
                _swipeDelta = Input.mousePosition.x - _mouseXStartPosition;
                _mouseXStartPosition = Input.mousePosition.x;
            }

            if (Input.GetMouseButtonUp(0))
            {
                _swipeDelta = 0;
            }

            return _swipeDelta;
        }

        public void AddGiant(GiantController giant)
        {
            if (Giant != null)
            {
                RemoveGiant();
            }

            giant.Activate(Vector3.zero, transform);
            Giant = giant;
        }

        public void RemoveGiant()
        {
            if (Giant != null)
            {
                Giant.transform.SetParent(null);
                Giant = null;
            }
        }

        public void CheckNumberPlayers()
        {
            var players = GetComponentsInChildren<PlayerMoveController>();
            if (players.Length <= 1)
            {
                LevelManager.Instance.LevelFail();
            }
        }

        public void AddPostLevel()
        {
            _postLevelXNumber++;
            _activePlayerNumber -= _postLevelValue;
            if (_postLevelXNumber >= PostLevelMaxCount || _activePlayerNumber <= 0)
            {
                LevelManager.Instance.LevelComplete();
            }
        }

        private void SubscribeToLevelEvents()
        {
            LevelManager.OnLevelStart += OnLevelStart;
            LevelManager.OnLevelLoad += OnLoadLevel;
            LevelManager.OnLevelComplete += OnLevelComplete;
            LevelManager.OnLevelStageComplete += OnLevelStageComplete;
            LevelManager.OnLevelFail += OnLevelFail;
        }

        private void UnsubscribeFromLevelEvents()
        {
            LevelManager.OnLevelStart -= OnLevelStart;
            LevelManager.OnLevelLoad -= OnLoadLevel;
            LevelManager.OnLevelComplete -= OnLevelComplete;
            LevelManager.OnLevelStageComplete -= OnLevelStageComplete;
            LevelManager.OnLevelFail -= OnLevelFail;
        }

        private void OnLevelStart(Level level)
        {
            _canMove = true;
            InitializePlayers();
        }

        private void InitializePlayers()
        {
            var players = GetComponentsInChildren<PlayerMoveController>();
            foreach (var player in players)
            {
                player.Active(transform, transform, this);
            }
        }

        private void OnLoadLevel(Level level)
        {
            _allPlayerNumber = FindObjectsOfType<PlayerMoveController>().Length;
            _postLevelValue = (float)_allPlayerNumber / PostLevelMaxCount;
        }

        private void OnLevelComplete(Level level)
        {
            _canMove = false;
        }

        private void OnLevelStageComplete(Level level, int index)
        {
            _activePlayerNumber = GetComponentsInChildren<PlayerMoveController>().Length;
        }

        private void OnLevelFail(Level level)
        {
            _canMove = false;
        }
    }
}
