using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Managers;

[RequireComponent(typeof(AddOns.DontDestroyOnLoad))]
/// <summary>
/// Contains information and methods for the cursor.
/// </summary>
public sealed class CursorController : MonoBehaviour
{
    [SerializeField] private Texture2D _cursor;
    public Texture2D CursorTexture => _cursor;
    [SerializeField] private Texture2D _cursorClicked;
    public Texture2D CursorClickedTexture => _cursorClicked;

    private void Awake() {
        ChangeCursor(_cursor);
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void ChangeCursor(Texture2D cursorType) {
        Cursor.SetCursor(cursorType, Vector2.zero, CursorMode.Auto);
    }
}
