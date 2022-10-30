using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

using Managers;

[RequireComponent(typeof(BoxCollider2D))]
public sealed class MeasurementTriggerScript : MonoBehaviour
{
    private StageControlManager ctrlManager => StageControlManager.Instance;
    private StageUIManager uiManager => StageUIManager.Instance;
    private GameManager gameManager => GameManager.Instance;

    private const float _secondsCooldown = 1f;
    private const float _cooldownBeforeLoadingNextScene = 5f;

    private async UniTaskVoid OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Controllable") && other.gameObject.GetInstanceID() == ctrlManager.PlayerGameObjID) {
            // set up controls
            ctrlManager.InQQVPanelMode(true);
            ctrlManager.SetStageControlsActive(false);

            // set up for animation
            Player player = other.gameObject.GetComponent<Player>();
            await uiManager.DisableAnyInteractions(player);
            uiManager.PauseTimer();
            int measurementRes = player.MeasureQubitSubcircuit();

            _measurementAnimation(player, measurementRes).Forget();
        }
    }

    private async UniTaskVoid _measurementAnimation(Player player, int res) {
        await uiManager.EfficientResetQQVToBeginning();
        uiManager.ClearMeasurementText();
        uiManager.SetMeasurementTextActive(true);

        await _waitForSeconds(_secondsCooldown);

        string binary = Convert.ToString(res, 2).PadLeft(player.QubitCount, '0');

        for (int i = binary.Length - 1; i >= 0; --i) {
            await uiManager.ForceMoveQQVRenderTexturesRight(player);
            uiManager.AddBitToMeasurementText(binary[i]);

            if (i == 0) ctrlManager.ActiveQQVPanelMode(false);
            await _waitForSeconds(_secondsCooldown);
        }

        uiManager.SetMeasurementText(res);
        await _waitForSeconds(_secondsCooldown);
        uiManager.SetMeasurementText(res, "\nLoading next level...");
        await _waitForSeconds(_cooldownBeforeLoadingNextScene);

        Destroy(player.gameObject);
        await gameManager.LoadNextScene();
    }

    private async UniTask _waitForSeconds(float seconds) {
        await UniTask.Delay(TimeSpan.FromSeconds(seconds));
    }
}
