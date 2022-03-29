// GENERATED AUTOMATICALLY FROM 'Assets/Control.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @Control : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @Control()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Control"",
    ""maps"": [
        {
            ""name"": ""Main"",
            ""id"": ""52427e1c-b0fb-48cc-9337-6eb4122e01bd"",
            ""actions"": [
                {
                    ""name"": ""Left"",
                    ""type"": ""Button"",
                    ""id"": ""6b45e576-2a6e-4e5c-9b31-3cf843847350"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Right"",
                    ""type"": ""Button"",
                    ""id"": ""361cea1a-3415-487f-9a73-bbcd5c562ac0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Spin"",
                    ""type"": ""Button"",
                    ""id"": ""a4887ed6-076e-41ed-8d6d-3ae88a274668"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""InstantFall"",
                    ""type"": ""Value"",
                    ""id"": ""8cea69c4-fa6d-485f-baeb-1cc930bca0cb"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Start"",
                    ""type"": ""Button"",
                    ""id"": ""8aac19f1-1c90-4050-8ae5-299944fd6d71"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""29afe8fb-7f31-4dae-864f-c440166796de"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d941de0d-6171-4e8b-beb4-53f515b37353"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ede9661f-0abb-440b-81dc-86a954c369db"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Spin"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d1adccab-48bd-4db1-9ce6-cfc0060a39fc"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""InstantFall"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e665024c-f586-4dcd-b58d-a452ff321370"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Start"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Main
        m_Main = asset.FindActionMap("Main", throwIfNotFound: true);
        m_Main_Left = m_Main.FindAction("Left", throwIfNotFound: true);
        m_Main_Right = m_Main.FindAction("Right", throwIfNotFound: true);
        m_Main_Spin = m_Main.FindAction("Spin", throwIfNotFound: true);
        m_Main_InstantFall = m_Main.FindAction("InstantFall", throwIfNotFound: true);
        m_Main_Start = m_Main.FindAction("Start", throwIfNotFound: true);
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

    // Main
    private readonly InputActionMap m_Main;
    private IMainActions m_MainActionsCallbackInterface;
    private readonly InputAction m_Main_Left;
    private readonly InputAction m_Main_Right;
    private readonly InputAction m_Main_Spin;
    private readonly InputAction m_Main_InstantFall;
    private readonly InputAction m_Main_Start;
    public struct MainActions
    {
        private @Control m_Wrapper;
        public MainActions(@Control wrapper) { m_Wrapper = wrapper; }
        public InputAction @Left => m_Wrapper.m_Main_Left;
        public InputAction @Right => m_Wrapper.m_Main_Right;
        public InputAction @Spin => m_Wrapper.m_Main_Spin;
        public InputAction @InstantFall => m_Wrapper.m_Main_InstantFall;
        public InputAction @Start => m_Wrapper.m_Main_Start;
        public InputActionMap Get() { return m_Wrapper.m_Main; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MainActions set) { return set.Get(); }
        public void SetCallbacks(IMainActions instance)
        {
            if (m_Wrapper.m_MainActionsCallbackInterface != null)
            {
                @Left.started -= m_Wrapper.m_MainActionsCallbackInterface.OnLeft;
                @Left.performed -= m_Wrapper.m_MainActionsCallbackInterface.OnLeft;
                @Left.canceled -= m_Wrapper.m_MainActionsCallbackInterface.OnLeft;
                @Right.started -= m_Wrapper.m_MainActionsCallbackInterface.OnRight;
                @Right.performed -= m_Wrapper.m_MainActionsCallbackInterface.OnRight;
                @Right.canceled -= m_Wrapper.m_MainActionsCallbackInterface.OnRight;
                @Spin.started -= m_Wrapper.m_MainActionsCallbackInterface.OnSpin;
                @Spin.performed -= m_Wrapper.m_MainActionsCallbackInterface.OnSpin;
                @Spin.canceled -= m_Wrapper.m_MainActionsCallbackInterface.OnSpin;
                @InstantFall.started -= m_Wrapper.m_MainActionsCallbackInterface.OnInstantFall;
                @InstantFall.performed -= m_Wrapper.m_MainActionsCallbackInterface.OnInstantFall;
                @InstantFall.canceled -= m_Wrapper.m_MainActionsCallbackInterface.OnInstantFall;
                @Start.started -= m_Wrapper.m_MainActionsCallbackInterface.OnStart;
                @Start.performed -= m_Wrapper.m_MainActionsCallbackInterface.OnStart;
                @Start.canceled -= m_Wrapper.m_MainActionsCallbackInterface.OnStart;
            }
            m_Wrapper.m_MainActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Left.started += instance.OnLeft;
                @Left.performed += instance.OnLeft;
                @Left.canceled += instance.OnLeft;
                @Right.started += instance.OnRight;
                @Right.performed += instance.OnRight;
                @Right.canceled += instance.OnRight;
                @Spin.started += instance.OnSpin;
                @Spin.performed += instance.OnSpin;
                @Spin.canceled += instance.OnSpin;
                @InstantFall.started += instance.OnInstantFall;
                @InstantFall.performed += instance.OnInstantFall;
                @InstantFall.canceled += instance.OnInstantFall;
                @Start.started += instance.OnStart;
                @Start.performed += instance.OnStart;
                @Start.canceled += instance.OnStart;
            }
        }
    }
    public MainActions @Main => new MainActions(this);
    public interface IMainActions
    {
        void OnLeft(InputAction.CallbackContext context);
        void OnRight(InputAction.CallbackContext context);
        void OnSpin(InputAction.CallbackContext context);
        void OnInstantFall(InputAction.CallbackContext context);
        void OnStart(InputAction.CallbackContext context);
    }
}
