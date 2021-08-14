using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;
using Nito.Collections;

namespace Managers
{
    /// <summary>
    /// All UI events and elements can be found here.
    /// <summary>
    public sealed class StageUIManager : Manager<StageUIManager>
    {   
        #region Quick Qubit Viewer (QQV) Data
        [System.Serializable] 
        private class QQV {
            public GameObject panel;
            public RawImage[] clickableRawImages;
        }
        [SerializeField] private QQV _QQV;

        // serves to decide whether or not to store references to qubit render textures depending 
        // on how many raw images are available in the QQV panel
        private int _rawImagesIndex = 0;  
        private bool _isQQVDisplayed = false;
        private Deque<RenderTexture> _renderTextureRefs = new Deque<RenderTexture>();
        #endregion Quick Qubit Viewer (QQV) Data

        // render textures of the current controllable
        private List<RenderTexture> _renderTextures;       

        #region Event Methods
        protected override void Awake()
        {
            base.Awake();    
        }

        private void Start() {
            Controllable controllable = ControlManager.Instance.CurrentControllable;
            SetQQVRenderTextures(controllable);
            _QQV.panel.SetActive(_isQQVDisplayed);
        }
        #endregion Event Methods

        #region QQV Methods
        public void ToggleQuickQubitPanel() {
            _isQQVDisplayed = !_isQQVDisplayed;
            _QQV.panel.SetActive(_isQQVDisplayed);
        }

        // all render texture methods
        public void SetQQVRenderTextures(Controllable controllable) {
            controllable.SubscribeToQubitCollection(_qqvHandleChange);
        }
        public void RemoveQQVRenderTextures(Controllable controllable) {
            controllable.UnsubscribeToQubitCollection(_qqvHandleChange);
            _renderTextureRefs.Clear();
        }
        public void ChangeQQVRenderTextures(Controllable oldControllable, Controllable newControllable) {
            RemoveQQVRenderTextures(oldControllable);
            SetQQVRenderTextures(newControllable);
        }
        /// <summary>
        /// Observe any changes in the controllable's qubit collection and reflect it onto the render
        /// textures.
        /// </summary>
        private void _qqvHandleChange(object sender, NotifyCollectionChangedEventArgs e) {
            RawImage[] rawImages = _QQV.clickableRawImages;
            int avalRawImages = rawImages.Length;

            switch (e.Action) {
                case NotifyCollectionChangedAction.Add: 
                    RenderTexture renderTexture = (e.NewItems[0] as Qubit).RenderTexture;

                    if (_rawImagesIndex < avalRawImages) {  // if there is an available raw image 
                        rawImages[_rawImagesIndex].texture = renderTexture;
                    }
                    else {  
                        _renderTextureRefs.AddToBack(renderTexture);
                    }
                    
                    _rawImagesIndex += 1;  
                    break;
            }
        }
        #endregion QQV Methods
    }
}
