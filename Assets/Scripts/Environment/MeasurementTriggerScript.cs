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

    private const float _secondsCooldown = 1f;

    private async UniTaskVoid OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Controllable") && other.gameObject.GetInstanceID() == ctrlManager.PlayerGameObjID) {
            // set up controls
            ctrlManager.InQQVPanelMode(true);
            ctrlManager.SetStageControlsActive(false);

            // set up for animation
            Player player = other.gameObject.GetComponent<Player>();
            await uiManager.DisableAnyInteractions(player);
            int measurementRes = player.MeasureQubitSubcircuit();

            MeasurementAnimation(player, measurementRes).Forget();
        }
    }

    private async UniTaskVoid MeasurementAnimation(Player player, int res) {
        await uiManager.EfficientResetQQVToBeginning();
        uiManager.ClearMeasurementText();
        uiManager.SetMeasurementTextActive(true);

        await UniTask.Delay(TimeSpan.FromSeconds(_secondsCooldown));

        string binary = Convert.ToString(res, 2).PadLeft(player.QubitCount, '0');

        for (int i = binary.Length - 1; i >= 0; --i) {
            await uiManager.ForceMoveQQVRenderTexturesRight(player);
            uiManager.AddBitToMeasurementText(binary[i]);

            if (i == 0) ctrlManager.ActiveQQVPanelMode(false);
            await UniTask.Delay(TimeSpan.FromSeconds(_secondsCooldown));
        }

        uiManager.SetMeasurementText(res);
    }
}
