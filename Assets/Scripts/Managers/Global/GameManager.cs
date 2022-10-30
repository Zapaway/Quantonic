using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Managers {
    /// <summary>
    /// Handles all game information and events. 
    /// </summary>
    [RequireComponent(typeof(AddOns.DontDestroyOnLoad))]
    public class GameManager : Manager<GameManager>
    {
        [SerializeField] private GameObject _pauseMenu;
        [SerializeField] private Button _resumeBtn;
        [SerializeField] private Button _resetBtn;
        [SerializeField] private Button _quitBtn; 

        protected override void Awake()
        {
            base.Awake();
            SetPauseMenuActive(false);
        }

        #region General Methods
        public void ReturnToStartMenu() {
            LoadScene(0);
            SetPauseMenuActive(false);
        }

        public void LoadScene(int buildIndex) {
            _resetAllListeners();
            SceneManager.LoadScene(buildIndex);
        }

        public async UniTask LoadSceneAsync(int buildIndex) {
            _resetAllListeners();
            await SceneManager.LoadSceneAsync(buildIndex, LoadSceneMode.Single);
        }

        public async UniTask LoadNextScene() {
            int currIndex = SceneManager.GetActiveScene().buildIndex;
            await LoadSceneAsync(currIndex + 1);
        }

        public void QuitGame() {
            Application.Quit();
        }
        #endregion General Methods

        #region Pause Menu Methods
        public void SetPauseMenuActive(bool isActive) {
            _pauseMenu.SetActive(isActive);
        }

        public void AddResumeButtonListener(UnityAction action) {
            _resumeBtn.onClick.AddListener(action);
        }
        public void AddResetButtonListener(UnityAction action) {
            _resetBtn.onClick.AddListener(action);
        }
        public void AddQuitButtonListener(UnityAction action) {
            _quitBtn.onClick.AddListener(action);
        }
        private void _resetAllListeners() {
            _resumeBtn.onClick.RemoveAllListeners();
            _resetBtn.onClick.RemoveAllListeners();
            _quitBtn.onClick.RemoveAllListeners();
        }
        #endregion Pause Menu Methods
    }
}
