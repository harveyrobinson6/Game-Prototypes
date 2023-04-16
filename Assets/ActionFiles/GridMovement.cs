//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.4
//     from Assets/ActionFiles/GridMovement.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @GridMovement : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @GridMovement()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""GridMovement"",
    ""maps"": [
        {
            ""name"": ""CursorMovement"",
            ""id"": ""e4b07b64-def1-446d-8cdf-16af7556e3fd"",
            ""actions"": [
                {
                    ""name"": ""Left"",
                    ""type"": ""Button"",
                    ""id"": ""85d0eb18-7003-4396-9eb2-edd1511bb2b1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Right"",
                    ""type"": ""Button"",
                    ""id"": ""022827b3-7780-4bc8-97aa-2108efaa0941"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Up"",
                    ""type"": ""Button"",
                    ""id"": ""d2983123-f80d-47a3-8290-37e869f20d7d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Down"",
                    ""type"": ""Button"",
                    ""id"": ""a9fcf47c-d351-4e55-9345-e90e36b8e841"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Select"",
                    ""type"": ""Button"",
                    ""id"": ""363f4003-9caf-4c6d-8470-cc5c2c09d27c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Back"",
                    ""type"": ""Button"",
                    ""id"": ""64f3cd37-416a-436e-a502-d75a2db870e7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Menu"",
                    ""type"": ""Button"",
                    ""id"": ""e348f4cc-3393-4698-8e24-ffbb484f52ac"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""BumperLeft"",
                    ""type"": ""Button"",
                    ""id"": ""4249d55d-2590-4e6a-8958-64f40883a5fe"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""BumperRight"",
                    ""type"": ""Button"",
                    ""id"": ""b294cea4-f606-42bb-ab9f-4fd5973cec35"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""CameraZoomUp"",
                    ""type"": ""Button"",
                    ""id"": ""8bb82775-1c1c-42bc-bc73-da4f34d36bd2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""CameraZoomDown"",
                    ""type"": ""Button"",
                    ""id"": ""416e5502-4c67-43ce-b6e5-b0e91683edff"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""6f521d56-713c-4635-bc93-32117d600224"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7a2a6eaf-c130-445e-bbf8-183389beac42"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f3873a0c-dc5b-4d16-9dd8-77f28bc35d2f"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6a06c6fd-c0ed-487d-82cf-d9fac4bc9ac6"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ab94bc5d-d85f-4c0c-a57c-a271e9230d0a"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Up"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3792388b-6c34-4fda-8ecf-c94d6492d462"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Up"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""327195c0-1170-4631-af1b-a1fa788762c2"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Down"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f397c429-7aaa-4006-82e0-907b8eee6539"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Down"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""57d3ca81-a2c0-4dd6-83c2-d5eee4830c75"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Select"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8c03fec3-786b-47f2-832f-206e30a68e8c"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Select"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f1dce4ee-f375-4a74-b0be-3e8c966e34c1"",
                    ""path"": ""<Keyboard>/backspace"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f42f82e6-648b-4aa1-b51f-05cf3360e5b7"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""951d03e2-7b6a-4e64-90d5-8249fd81a1a0"",
                    ""path"": ""<Keyboard>/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Menu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""51d6a65f-3aba-45d0-bac8-a478e59d52be"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Menu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""08800aca-dfc0-4dcd-b781-aeade35e1847"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""BumperLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f2d58672-e465-4edb-9824-98b5b5b3bb9f"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""BumperRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""307aee0c-153f-450e-8cef-65c595dbcc91"",
                    ""path"": ""<Gamepad>/rightStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraZoomUp"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d25d32c6-80b7-4879-87e0-57c6e44a0714"",
                    ""path"": ""<Gamepad>/rightStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraZoomDown"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // CursorMovement
        m_CursorMovement = asset.FindActionMap("CursorMovement", throwIfNotFound: true);
        m_CursorMovement_Left = m_CursorMovement.FindAction("Left", throwIfNotFound: true);
        m_CursorMovement_Right = m_CursorMovement.FindAction("Right", throwIfNotFound: true);
        m_CursorMovement_Up = m_CursorMovement.FindAction("Up", throwIfNotFound: true);
        m_CursorMovement_Down = m_CursorMovement.FindAction("Down", throwIfNotFound: true);
        m_CursorMovement_Select = m_CursorMovement.FindAction("Select", throwIfNotFound: true);
        m_CursorMovement_Back = m_CursorMovement.FindAction("Back", throwIfNotFound: true);
        m_CursorMovement_Menu = m_CursorMovement.FindAction("Menu", throwIfNotFound: true);
        m_CursorMovement_BumperLeft = m_CursorMovement.FindAction("BumperLeft", throwIfNotFound: true);
        m_CursorMovement_BumperRight = m_CursorMovement.FindAction("BumperRight", throwIfNotFound: true);
        m_CursorMovement_CameraZoomUp = m_CursorMovement.FindAction("CameraZoomUp", throwIfNotFound: true);
        m_CursorMovement_CameraZoomDown = m_CursorMovement.FindAction("CameraZoomDown", throwIfNotFound: true);
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
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // CursorMovement
    private readonly InputActionMap m_CursorMovement;
    private ICursorMovementActions m_CursorMovementActionsCallbackInterface;
    private readonly InputAction m_CursorMovement_Left;
    private readonly InputAction m_CursorMovement_Right;
    private readonly InputAction m_CursorMovement_Up;
    private readonly InputAction m_CursorMovement_Down;
    private readonly InputAction m_CursorMovement_Select;
    private readonly InputAction m_CursorMovement_Back;
    private readonly InputAction m_CursorMovement_Menu;
    private readonly InputAction m_CursorMovement_BumperLeft;
    private readonly InputAction m_CursorMovement_BumperRight;
    private readonly InputAction m_CursorMovement_CameraZoomUp;
    private readonly InputAction m_CursorMovement_CameraZoomDown;
    public struct CursorMovementActions
    {
        private @GridMovement m_Wrapper;
        public CursorMovementActions(@GridMovement wrapper) { m_Wrapper = wrapper; }
        public InputAction @Left => m_Wrapper.m_CursorMovement_Left;
        public InputAction @Right => m_Wrapper.m_CursorMovement_Right;
        public InputAction @Up => m_Wrapper.m_CursorMovement_Up;
        public InputAction @Down => m_Wrapper.m_CursorMovement_Down;
        public InputAction @Select => m_Wrapper.m_CursorMovement_Select;
        public InputAction @Back => m_Wrapper.m_CursorMovement_Back;
        public InputAction @Menu => m_Wrapper.m_CursorMovement_Menu;
        public InputAction @BumperLeft => m_Wrapper.m_CursorMovement_BumperLeft;
        public InputAction @BumperRight => m_Wrapper.m_CursorMovement_BumperRight;
        public InputAction @CameraZoomUp => m_Wrapper.m_CursorMovement_CameraZoomUp;
        public InputAction @CameraZoomDown => m_Wrapper.m_CursorMovement_CameraZoomDown;
        public InputActionMap Get() { return m_Wrapper.m_CursorMovement; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(CursorMovementActions set) { return set.Get(); }
        public void SetCallbacks(ICursorMovementActions instance)
        {
            if (m_Wrapper.m_CursorMovementActionsCallbackInterface != null)
            {
                @Left.started -= m_Wrapper.m_CursorMovementActionsCallbackInterface.OnLeft;
                @Left.performed -= m_Wrapper.m_CursorMovementActionsCallbackInterface.OnLeft;
                @Left.canceled -= m_Wrapper.m_CursorMovementActionsCallbackInterface.OnLeft;
                @Right.started -= m_Wrapper.m_CursorMovementActionsCallbackInterface.OnRight;
                @Right.performed -= m_Wrapper.m_CursorMovementActionsCallbackInterface.OnRight;
                @Right.canceled -= m_Wrapper.m_CursorMovementActionsCallbackInterface.OnRight;
                @Up.started -= m_Wrapper.m_CursorMovementActionsCallbackInterface.OnUp;
                @Up.performed -= m_Wrapper.m_CursorMovementActionsCallbackInterface.OnUp;
                @Up.canceled -= m_Wrapper.m_CursorMovementActionsCallbackInterface.OnUp;
                @Down.started -= m_Wrapper.m_CursorMovementActionsCallbackInterface.OnDown;
                @Down.performed -= m_Wrapper.m_CursorMovementActionsCallbackInterface.OnDown;
                @Down.canceled -= m_Wrapper.m_CursorMovementActionsCallbackInterface.OnDown;
                @Select.started -= m_Wrapper.m_CursorMovementActionsCallbackInterface.OnSelect;
                @Select.performed -= m_Wrapper.m_CursorMovementActionsCallbackInterface.OnSelect;
                @Select.canceled -= m_Wrapper.m_CursorMovementActionsCallbackInterface.OnSelect;
                @Back.started -= m_Wrapper.m_CursorMovementActionsCallbackInterface.OnBack;
                @Back.performed -= m_Wrapper.m_CursorMovementActionsCallbackInterface.OnBack;
                @Back.canceled -= m_Wrapper.m_CursorMovementActionsCallbackInterface.OnBack;
                @Menu.started -= m_Wrapper.m_CursorMovementActionsCallbackInterface.OnMenu;
                @Menu.performed -= m_Wrapper.m_CursorMovementActionsCallbackInterface.OnMenu;
                @Menu.canceled -= m_Wrapper.m_CursorMovementActionsCallbackInterface.OnMenu;
                @BumperLeft.started -= m_Wrapper.m_CursorMovementActionsCallbackInterface.OnBumperLeft;
                @BumperLeft.performed -= m_Wrapper.m_CursorMovementActionsCallbackInterface.OnBumperLeft;
                @BumperLeft.canceled -= m_Wrapper.m_CursorMovementActionsCallbackInterface.OnBumperLeft;
                @BumperRight.started -= m_Wrapper.m_CursorMovementActionsCallbackInterface.OnBumperRight;
                @BumperRight.performed -= m_Wrapper.m_CursorMovementActionsCallbackInterface.OnBumperRight;
                @BumperRight.canceled -= m_Wrapper.m_CursorMovementActionsCallbackInterface.OnBumperRight;
                @CameraZoomUp.started -= m_Wrapper.m_CursorMovementActionsCallbackInterface.OnCameraZoomUp;
                @CameraZoomUp.performed -= m_Wrapper.m_CursorMovementActionsCallbackInterface.OnCameraZoomUp;
                @CameraZoomUp.canceled -= m_Wrapper.m_CursorMovementActionsCallbackInterface.OnCameraZoomUp;
                @CameraZoomDown.started -= m_Wrapper.m_CursorMovementActionsCallbackInterface.OnCameraZoomDown;
                @CameraZoomDown.performed -= m_Wrapper.m_CursorMovementActionsCallbackInterface.OnCameraZoomDown;
                @CameraZoomDown.canceled -= m_Wrapper.m_CursorMovementActionsCallbackInterface.OnCameraZoomDown;
            }
            m_Wrapper.m_CursorMovementActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Left.started += instance.OnLeft;
                @Left.performed += instance.OnLeft;
                @Left.canceled += instance.OnLeft;
                @Right.started += instance.OnRight;
                @Right.performed += instance.OnRight;
                @Right.canceled += instance.OnRight;
                @Up.started += instance.OnUp;
                @Up.performed += instance.OnUp;
                @Up.canceled += instance.OnUp;
                @Down.started += instance.OnDown;
                @Down.performed += instance.OnDown;
                @Down.canceled += instance.OnDown;
                @Select.started += instance.OnSelect;
                @Select.performed += instance.OnSelect;
                @Select.canceled += instance.OnSelect;
                @Back.started += instance.OnBack;
                @Back.performed += instance.OnBack;
                @Back.canceled += instance.OnBack;
                @Menu.started += instance.OnMenu;
                @Menu.performed += instance.OnMenu;
                @Menu.canceled += instance.OnMenu;
                @BumperLeft.started += instance.OnBumperLeft;
                @BumperLeft.performed += instance.OnBumperLeft;
                @BumperLeft.canceled += instance.OnBumperLeft;
                @BumperRight.started += instance.OnBumperRight;
                @BumperRight.performed += instance.OnBumperRight;
                @BumperRight.canceled += instance.OnBumperRight;
                @CameraZoomUp.started += instance.OnCameraZoomUp;
                @CameraZoomUp.performed += instance.OnCameraZoomUp;
                @CameraZoomUp.canceled += instance.OnCameraZoomUp;
                @CameraZoomDown.started += instance.OnCameraZoomDown;
                @CameraZoomDown.performed += instance.OnCameraZoomDown;
                @CameraZoomDown.canceled += instance.OnCameraZoomDown;
            }
        }
    }
    public CursorMovementActions @CursorMovement => new CursorMovementActions(this);
    public interface ICursorMovementActions
    {
        void OnLeft(InputAction.CallbackContext context);
        void OnRight(InputAction.CallbackContext context);
        void OnUp(InputAction.CallbackContext context);
        void OnDown(InputAction.CallbackContext context);
        void OnSelect(InputAction.CallbackContext context);
        void OnBack(InputAction.CallbackContext context);
        void OnMenu(InputAction.CallbackContext context);
        void OnBumperLeft(InputAction.CallbackContext context);
        void OnBumperRight(InputAction.CallbackContext context);
        void OnCameraZoomUp(InputAction.CallbackContext context);
        void OnCameraZoomDown(InputAction.CallbackContext context);
    }
}
