using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers {
    /// <summary>
    /// Handles all game information and events. 
    /// </summary>
    [RequireComponent(typeof(AddOns.DontDestroyOnLoad), typeof(CursorController))]
    public class GlobalControlManager : Manager<GlobalControlManager>
    {
        #region Fields/Properties
        private GlobalInputs _globalInputs;

        // cursor info
        private CursorController _cursorController;
        private bool _isCursorHeldDown = false;
        public bool IsCursorHeldDown => _isCursorHeldDown;
        #endregion Fields/Properties

        protected override void Awake()
        {
            base.Awake();

            // create global inputs
            _globalInputs = new GlobalInputs();

            _cursorController = GetComponent<CursorController>();
        }

        private void OnEnable() {
            _globalInputs.Enable();
        }

        private void OnDisable() {
            _globalInputs?.Disable();
        }

        private void Start() {
            _globalInputs.Mouse.Click.started += _ => _clickStarted();
            _globalInputs.Mouse.Click.performed += _ => _clickEnded();
        }

        // cursor methods
        public Vector3 GetMouseWorldPosition() {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            worldPos.z = 0;
            return worldPos;
        }
        public float GetAngleRelativeToMouse(Vector3 originCenterPoint) {
            return GetAngleRelativeToTarget(GetMouseWorldPosition(), originCenterPoint);
            // Vector3 mouseWorldPos = GetMouseWorldPosition();
            // Vector3 aimDir = (mouseWorldPos - centerPoint).normalized;
            // return Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        }
        private void _clickStarted() {
            _isCursorHeldDown = true;
            _cursorController.ChangeCursor(_cursorController.CursorClickedTexture);
        }
        private void _clickEnded() {
            _isCursorHeldDown = false;
            _cursorController.ChangeCursor(_cursorController.CursorTexture);
        }

        // helper methods
        public float GetAngleRelativeToTarget(Vector2 targetPoint, Vector2 originCenterPoint) {
            Vector2 aimDir = (targetPoint - originCenterPoint).normalized;
            return  Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        }
    }
}

