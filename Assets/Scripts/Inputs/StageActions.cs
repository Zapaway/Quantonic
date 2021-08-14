// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Inputs/StageActions.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @StageInputs : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @StageInputs()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""StageActions"",
    ""maps"": [
        {
            ""name"": ""Controllable"",
            ""id"": ""0daf09cc-0227-491a-99f2-6579eb8d07e3"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""99e7bd1f-35b3-4de0-9f89-e2820b27f17a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""da061aff-0bf1-4285-8eeb-dd41d44c9cb9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""Sideways"",
                    ""id"": ""79c93737-527f-4daf-a423-a1853dc8aefe"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""6b6b7c70-ce8b-477a-8470-563a2cf0765f"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""bcb11a6d-44de-4a2f-a256-0250ab33baa5"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""4525e02d-7b21-44a2-94d3-365fe8145a26"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Stage UI"",
            ""id"": ""666555d0-d593-427c-b673-7f87087fe934"",
            ""actions"": [
                {
                    ""name"": ""Toggle QQV"",
                    ""type"": ""Button"",
                    ""id"": ""9284bc9c-d39d-47fe-ae07-9f526cf9497e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""ef79dfe2-68e2-4f8e-b39d-43c76c6487b4"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Toggle QQV"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Pause Menu UI"",
            ""id"": ""1cfddcdf-7238-4473-9eb6-02696c8a9084"",
            ""actions"": [
                {
                    ""name"": ""New action"",
                    ""type"": ""Button"",
                    ""id"": ""7117a6b5-d746-4596-8727-68ca6b015189"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""23d3ba5c-b0ab-462d-8196-8af023aaca22"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""New action"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard"",
            ""bindingGroup"": ""Keyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Controllable
        m_Controllable = asset.FindActionMap("Controllable", throwIfNotFound: true);
        m_Controllable_Movement = m_Controllable.FindAction("Movement", throwIfNotFound: true);
        m_Controllable_Jump = m_Controllable.FindAction("Jump", throwIfNotFound: true);
        // Stage UI
        m_StageUI = asset.FindActionMap("Stage UI", throwIfNotFound: true);
        m_StageUI_ToggleQQV = m_StageUI.FindAction("Toggle QQV", throwIfNotFound: true);
        // Pause Menu UI
        m_PauseMenuUI = asset.FindActionMap("Pause Menu UI", throwIfNotFound: true);
        m_PauseMenuUI_Newaction = m_PauseMenuUI.FindAction("New action", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Controllable
    private readonly InputActionMap m_Controllable;
    private IControllableActions m_ControllableActionsCallbackInterface;
    private readonly InputAction m_Controllable_Movement;
    private readonly InputAction m_Controllable_Jump;
    public struct ControllableActions
    {
        private @StageInputs m_Wrapper;
        public ControllableActions(@StageInputs wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Controllable_Movement;
        public InputAction @Jump => m_Wrapper.m_Controllable_Jump;
        public InputActionMap Get() { return m_Wrapper.m_Controllable; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(ControllableActions set) { return set.Get(); }
        public void SetCallbacks(IControllableActions instance)
        {
            if (m_Wrapper.m_ControllableActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_ControllableActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_ControllableActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_ControllableActionsCallbackInterface.OnMovement;
                @Jump.started -= m_Wrapper.m_ControllableActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_ControllableActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_ControllableActionsCallbackInterface.OnJump;
            }
            m_Wrapper.m_ControllableActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
            }
        }
    }
    public ControllableActions @Controllable => new ControllableActions(this);

    // Stage UI
    private readonly InputActionMap m_StageUI;
    private IStageUIActions m_StageUIActionsCallbackInterface;
    private readonly InputAction m_StageUI_ToggleQQV;
    public struct StageUIActions
    {
        private @StageInputs m_Wrapper;
        public StageUIActions(@StageInputs wrapper) { m_Wrapper = wrapper; }
        public InputAction @ToggleQQV => m_Wrapper.m_StageUI_ToggleQQV;
        public InputActionMap Get() { return m_Wrapper.m_StageUI; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(StageUIActions set) { return set.Get(); }
        public void SetCallbacks(IStageUIActions instance)
        {
            if (m_Wrapper.m_StageUIActionsCallbackInterface != null)
            {
                @ToggleQQV.started -= m_Wrapper.m_StageUIActionsCallbackInterface.OnToggleQQV;
                @ToggleQQV.performed -= m_Wrapper.m_StageUIActionsCallbackInterface.OnToggleQQV;
                @ToggleQQV.canceled -= m_Wrapper.m_StageUIActionsCallbackInterface.OnToggleQQV;
            }
            m_Wrapper.m_StageUIActionsCallbackInterface = instance;
            if (instance != null)
            {
                @ToggleQQV.started += instance.OnToggleQQV;
                @ToggleQQV.performed += instance.OnToggleQQV;
                @ToggleQQV.canceled += instance.OnToggleQQV;
            }
        }
    }
    public StageUIActions @StageUI => new StageUIActions(this);

    // Pause Menu UI
    private readonly InputActionMap m_PauseMenuUI;
    private IPauseMenuUIActions m_PauseMenuUIActionsCallbackInterface;
    private readonly InputAction m_PauseMenuUI_Newaction;
    public struct PauseMenuUIActions
    {
        private @StageInputs m_Wrapper;
        public PauseMenuUIActions(@StageInputs wrapper) { m_Wrapper = wrapper; }
        public InputAction @Newaction => m_Wrapper.m_PauseMenuUI_Newaction;
        public InputActionMap Get() { return m_Wrapper.m_PauseMenuUI; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PauseMenuUIActions set) { return set.Get(); }
        public void SetCallbacks(IPauseMenuUIActions instance)
        {
            if (m_Wrapper.m_PauseMenuUIActionsCallbackInterface != null)
            {
                @Newaction.started -= m_Wrapper.m_PauseMenuUIActionsCallbackInterface.OnNewaction;
                @Newaction.performed -= m_Wrapper.m_PauseMenuUIActionsCallbackInterface.OnNewaction;
                @Newaction.canceled -= m_Wrapper.m_PauseMenuUIActionsCallbackInterface.OnNewaction;
            }
            m_Wrapper.m_PauseMenuUIActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Newaction.started += instance.OnNewaction;
                @Newaction.performed += instance.OnNewaction;
                @Newaction.canceled += instance.OnNewaction;
            }
        }
    }
    public PauseMenuUIActions @PauseMenuUI => new PauseMenuUIActions(this);
    private int m_KeyboardSchemeIndex = -1;
    public InputControlScheme KeyboardScheme
    {
        get
        {
            if (m_KeyboardSchemeIndex == -1) m_KeyboardSchemeIndex = asset.FindControlSchemeIndex("Keyboard");
            return asset.controlSchemes[m_KeyboardSchemeIndex];
        }
    }
    public interface IControllableActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
    }
    public interface IStageUIActions
    {
        void OnToggleQQV(InputAction.CallbackContext context);
    }
    public interface IPauseMenuUIActions
    {
        void OnNewaction(InputAction.CallbackContext context);
    }
}
