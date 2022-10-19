using UnityEngine;
using Controllers;
using Levels;
using Enemy;

namespace Player
{
    public class PlayerContainerController : MonoBehaviour
    {
        [SerializeField] private float speedForward;
        [SerializeField] private float speedHorizontal;
        [SerializeField] private float maxHorizontalMove;

        private bool _canMove;
        private float _mouseXStartPosition;
        private float _swipeDelta;
        private float _xMove;
        private int _allPlayerNumber;
        private float _activePlayerNumber;
        private float _postLevelValue;
        private int _postLevelXNumber;

        public GiantController giant { get; private set; }

        #region Unity Functions

        private void Awake()
        {
            LevelManager.onLevelStart += OnLevelStart;
            LevelManager.onLevelLoad += OnLoadLevel;
            LevelManager.onLevelComplete += OnLevelComplete;
            LevelManager.onLevelStageComplete += OnLevelStageComplete;
            LevelManager.onLevelFail += OnLevelFail;
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
            LevelManager.onLevelStart -= OnLevelStart;
            LevelManager.onLevelLoad -= OnLoadLevel;
            LevelManager.onLevelComplete -= OnLevelComplete;
            LevelManager.onLevelStageComplete -= OnLevelStageComplete;
            LevelManager.onLevelFail -= OnLevelFail;
        }

        #endregion

        #region Private Functions
        
        private void Movement()
        {
            //Forward Move
            transform.Translate(0, 0, speedForward * Time.deltaTime);
            
            //Values
            _xMove -= TouchSwip() * speedHorizontal * Time.deltaTime;
            _xMove = Mathf.Clamp(_xMove, -maxHorizontalMove, maxHorizontalMove);
            var playerTransform = transform;
            var playerPosition = playerTransform.position;
            
            //Horizontal Move
            playerPosition.x = _xMove;
            playerTransform.position = playerPosition;
        }

        private float TouchSwip()
        {
            // MOUSE DOWN
            if (Input.GetMouseButtonDown(0)) _mouseXStartPosition = Input.mousePosition.x;

            // MOUSE ON PRESS
            if (Input.GetMouseButton(0))
            {
                _swipeDelta = Input.mousePosition.x - _mouseXStartPosition;
                _mouseXStartPosition = Input.mousePosition.x;
            }

            // MOUSE UP
            if (Input.GetMouseButtonUp(0)) _swipeDelta = 0;

            return _swipeDelta;
        }

        #endregion

        #region Public Functions

        public void AddGiant(GiantController gia)
        {
            if (giant != null)
            {
                RemoveGiant();
            }

            gia.Active(Vector3.zero, transform);
            giant = gia;
        }

        public void RemoveGiant()
        {
            giant.transform.SetParent(null);
            giant = null;
        }

        public void CheckNumberPlayers()
        {
            var players = GetComponentsInChildren<PlayerMoveController>();
            if (players.Length <= 1)
            {
                LevelManager.instance.LevelFail();
            }
        }

        public void AddPostLevel()
        {
            _postLevelXNumber++;
            _activePlayerNumber -= _postLevelValue;
            if (_postLevelXNumber >= 10 || _activePlayerNumber <= 0)
            {
                LevelManager.instance.LevelComplete();
            }
        }

        #endregion

        #region EVENTs

        private void OnLevelStart(Level level)
        {
            _canMove = true;
            var players = GetComponentsInChildren<PlayerMoveController>();
            for (int i = 0; i < players.Length; i++)
            {
                var thisTransform = transform;
                players[i].Active(thisTransform, thisTransform, this);
            }
        }

        private void OnLoadLevel(Level level)
        {
            _allPlayerNumber = FindObjectsOfType<PlayerMoveController>().Length;
            _postLevelValue = (float)_allPlayerNumber / 10;
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

        #endregion
    }
}
