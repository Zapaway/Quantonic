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
                },
                {
                    ""name"": ""Split"",
                    ""type"": ""Button"",
                    ""id"": ""f46e5425-e929-48e3-8a7e-73243af839fe"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Switch"",
                    ""type"": ""Button"",
                    ""id"": ""5e4220be-fe46-4217-b034-ca7472834a66"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SpawnWave"",
                    ""type"": ""Button"",
                    ""id"": ""6f963ddd-63b3-45a8-8153-8b01aa73701a"",
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
                    ""groups"": ""KeyboardMouse"",
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
                    ""groups"": ""KeyboardMouse"",
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
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""de2d03db-c3ed-4f74-8b5a-43204691bdb9"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Split"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4edfe89a-f5eb-41e7-8b40-346f1eeae103"",
                    ""path"": ""<Keyboard>/alt"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""SpawnWave"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1b69c036-0e09-4e0c-ad3a-bf31f7e1f929"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Switch"",
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
                },
                {
                    ""name"": ""Navigate"",
                    ""type"": ""Value"",
                    ""id"": ""431e038d-9322-4ab1-9c0c-b05e57746a06"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Submit"",
                    ""type"": ""Button"",
                    ""id"": ""2ae5f3ba-a415-409f-9d49-aba5b5402ae0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Point"",
                    ""type"": ""PassThrough"",
                    ""id"": ""e8e50eb8-fcce-4790-a17f-8c6eef0b3267"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Click"",
                    ""type"": ""PassThrough"",
                    ""id"": ""d37f7bac-e000-46be-b8a0-de10960a2b6a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MiddleClick"",
                    ""type"": ""PassThrough"",
                    ""id"": ""fbbfe76c-9c10-4270-860c-94a3f3eeeddd"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RightClick"",
                    ""type"": ""PassThrough"",
                    ""id"": ""0ed8d6c1-f986-49a4-b096-25abcd22b141"",
                    ""expectedControlType"": """",
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
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Toggle QQV"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Keyboard"",
                    ""id"": ""1e598392-8407-43d5-9f44-43b0de35aecd"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Navigate"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""da9c30fd-bc82-469f-87d1-362ee8823f8c"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""af3bb335-6729-4b15-b699-0ea34da52ed0"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""a62097e3-21d8-45c8-975a-0c23a7e536d2"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""4bda265a-75d5-4b1c-a7fa-81fc003ecc77"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""536f23cc-55f1-4a2a-8eb8-b54cb738c23d"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Submit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""090b608a-3a3d-4458-ab80-e3ce7e75421e"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Submit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a631d716-78eb-4fe1-9561-a9c5904ce234"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""809a5fa0-0edb-4984-a698-7417653dc1da"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""169d3216-5103-492c-a964-a7319157ddf9"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""MiddleClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""62bf0bc3-a86b-4072-b287-21a45f2a874d"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""RightClick"",
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
            ""name"": ""KeyboardMouse"",
            ""bindingGroup"": ""KeyboardMouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
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
        m_Controllable_Split = m_Controllable.FindAction("Split", throwIfNotFound: true);
        m_Controllable_Switch = m_Controllable.FindAction("Switch", throwIfNotFound: true);
        m_Controllable_SpawnWave = m_Controllable.FindAction("SpawnWave", throwIfNotFound: true);
        // Stage UI
        m_StageUI = asset.FindActionMap("Stage UI", throwIfNotFound: true);
        m_StageUI_ToggleQQV = m_StageUI.FindAction("Toggle QQV", throwIfNotFound: true);
        m_StageUI_Navigate = m_StageUI.FindAction("Navigate", throwIfNotFound: true);
        m_StageUI_Submit = m_StageUI.FindAction("Submit", throwIfNotFound: true);
        m_StageUI_Point = m_StageUI.FindAction("Point", throwIfNotFound: true);
        m_StageUI_Click = m_StageUI.FindAction("Click", throwIfNotFound: true);
        m_StageUI_MiddleClick = m_StageUI.FindAction("MiddleClick", throwIfNotFound: true);
        m_StageUI_RightClick = m_StageUI.FindAction("RightClick", throwIfNotFound: true);
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
    private readonly InputAction m_Controllable_Split;
    private readonly InputAction m_Controllable_Switch;
    private readonly InputAction m_Controllable_SpawnWave;
    public struct ControllableActions
    {
        private @StageInputs m_Wrapper;
        public ControllableActions(@StageInputs wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Controllable_Movement;
        public InputAction @Jump => m_Wrapper.m_Controllable_Jump;
        public InputAction @Split => m_Wrapper.m_Controllable_Split;
        public InputAction @Switch => m_Wrapper.m_Controllable_Switch;
        public InputAction @SpawnWave => m_Wrapper.m_Controllable_SpawnWave;
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
                @Split.started -= m_Wrapper.m_ControllableActionsCallbackInterface.OnSplit;
                @Split.performed -= m_Wrapper.m_ControllableActionsCallbackInterface.OnSplit;
                @Split.canceled -= m_Wrapper.m_ControllableActionsCallbackInterface.OnSplit;
                @Switch.started -= m_Wrapper.m_ControllableActionsCallbackInterface.OnSwitch;
                @Switch.performed -= m_Wrapper.m_ControllableActionsCallbackInterface.OnSwitch;
                @Switch.canceled -= m_Wrapper.m_ControllableActionsCallbackInterface.OnSwitch;
                @SpawnWave.started -= m_Wrapper.m_ControllableActionsCallbackInterface.OnSpawnWave;
                @SpawnWave.performed -= m_Wrapper.m_ControllableActionsCallbackInterface.OnSpawnWave;
                @SpawnWave.canceled -= m_Wrapper.m_ControllableActionsCallbackInterface.OnSpawnWave;
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
                @Split.started += instance.OnSplit;
                @Split.performed += instance.OnSplit;
                @Split.canceled += instance.OnSplit;
                @Switch.started += instance.OnSwitch;
                @Switch.performed += instance.OnSwitch;
                @Switch.canceled += instance.OnSwitch;
                @SpawnWave.started += instance.OnSpawnWave;
                @SpawnWave.performed += instance.OnSpawnWave;
                @SpawnWave.canceled += instance.OnSpawnWave;
            }
        }
    }
    public ControllableActions @Controllable => new ControllableActions(this);

    // Stage UI
    private readonly InputActionMap m_StageUI;
    private IStageUIActions m_StageUIActionsCallbackInterface;
    private readonly InputAction m_StageUI_ToggleQQV;
    private readonly InputAction m_StageUI_Navigate;
    private readonly InputAction m_StageUI_Submit;
    private readonly InputAction m_StageUI_Point;
    private readonly InputAction m_StageUI_Click;
    private readonly InputAction m_StageUI_MiddleClick;
    private readonly InputAction m_StageUI_RightClick;
    public struct StageUIActions
    {
        private @StageInputs m_Wrapper;
        public StageUIActions(@StageInputs wrapper) { m_Wrapper = wrapper; }
        public InputAction @ToggleQQV => m_Wrapper.m_StageUI_ToggleQQV;
        public InputAction @Navigate => m_Wrapper.m_StageUI_Navigate;
        public InputAction @Submit => m_Wrapper.m_StageUI_Submit;
        public InputAction @Point => m_Wrapper.m_StageUI_Point;
        public InputAction @Click => m_Wrapper.m_StageUI_Click;
        public InputAction @MiddleClick => m_Wrapper.m_StageUI_MiddleClick;
        public InputAction @RightClick => m_Wrapper.m_StageUI_RightClick;
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
                @Navigate.started -= m_Wrapper.m_StageUIActionsCallbackInterface.OnNavigate;
                @Navigate.performed -= m_Wrapper.m_StageUIActionsCallbackInterface.OnNavigate;
                @Navigate.canceled -= m_Wrapper.m_StageUIActionsCallbackInterface.OnNavigate;
                @Submit.started -= m_Wrapper.m_StageUIActionsCallbackInterface.OnSubmit;
                @Submit.performed -= m_Wrapper.m_StageUIActionsCallbackInterface.OnSubmit;
                @Submit.canceled -= m_Wrapper.m_StageUIActionsCallbackInterface.OnSubmit;
                @Point.started -= m_Wrapper.m_StageUIActionsCallbackInterface.OnPoint;
                @Point.performed -= m_Wrapper.m_StageUIActionsCallbackInterface.OnPoint;
                @Point.canceled -= m_Wrapper.m_StageUIActionsCallbackInterface.OnPoint;
                @Click.started -= m_Wrapper.m_StageUIActionsCallbackInterface.OnClick;
                @Click.performed -= m_Wrapper.m_StageUIActionsCallbackInterface.OnClick;
                @Click.canceled -= m_Wrapper.m_StageUIActionsCallbackInterface.OnClick;
                @MiddleClick.started -= m_Wrapper.m_StageUIActionsCallbackInterface.OnMiddleClick;
                @MiddleClick.performed -= m_Wrapper.m_StageUIActionsCallbackInterface.OnMiddleClick;
                @MiddleClick.canceled -= m_Wrapper.m_StageUIActionsCallbackInterface.OnMiddleClick;
                @RightClick.started -= m_Wrapper.m_StageUIActionsCallbackInterface.OnRightClick;
                @RightClick.performed -= m_Wrapper.m_StageUIActionsCallbackInterface.OnRightClick;
                @RightClick.canceled -= m_Wrapper.m_StageUIActionsCallbackInterface.OnRightClick;
            }
            m_Wrapper.m_StageUIActionsCallbackInterface = instance;
            if (instance != null)
            {
                @ToggleQQV.started += instance.OnToggleQQV;
                @ToggleQQV.performed += instance.OnToggleQQV;
                @ToggleQQV.canceled += instance.OnToggleQQV;
                @Navigate.started += instance.OnNavigate;
                @Navigate.performed += instance.OnNavigate;
                @Navigate.canceled += instance.OnNavigate;
                @Submit.started += instance.OnSubmit;
                @Submit.performed += instance.OnSubmit;
                @Submit.canceled += instance.OnSubmit;
                @Point.started += instance.OnPoint;
                @Point.performed += instance.OnPoint;
                @Point.canceled += instance.OnPoint;
                @Click.started += instance.OnClick;
                @Click.performed += instance.OnClick;
                @Click.canceled += instance.OnClick;
                @MiddleClick.started += instance.OnMiddleClick;
                @MiddleClick.performed += instance.OnMiddleClick;
                @MiddleClick.canceled += instance.OnMiddleClick;
                @RightClick.started += instance.OnRightClick;
                @RightClick.performed += instance.OnRightClick;
                @RightClick.canceled += instance.OnRightClick;
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
    private int m_KeyboardMouseSchemeIndex = -1;
    public InputControlScheme KeyboardMouseScheme
    {
        get
        {
            if (m_KeyboardMouseSchemeIndex == -1) m_KeyboardMouseSchemeIndex = asset.FindControlSchemeIndex("KeyboardMouse");
            return asset.controlSchemes[m_KeyboardMouseSchemeIndex];
        }
    }
    public interface IControllableActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnSplit(InputAction.CallbackContext context);
        void OnSwitch(InputAction.CallbackContext context);
        void OnSpawnWave(InputAction.CallbackContext context);
    }
    public interface IStageUIActions
    {
        void OnToggleQQV(InputAction.CallbackContext context);
        void OnNavigate(InputAction.CallbackContext context);
        void OnSubmit(InputAction.CallbackContext context);
        void OnPoint(InputAction.CallbackContext context);
        void OnClick(InputAction.CallbackContext context);
        void OnMiddleClick(InputAction.CallbackContext context);
        void OnRightClick(InputAction.CallbackContext context);
    }
    public interface IPauseMenuUIActions
    {
        void OnNewaction(InputAction.CallbackContext context);
    }
}
