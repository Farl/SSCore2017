using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{

    public class ControllerSystem : Singleton<ControllerSystem>
    {
        public enum Button
        {
            A,
            B,
            X,
            Y,
            Up,
            Down,
            Left,
            Right,
            Dpad_L_Up,
            Dpad_L_Down,
            Dpad_L_Left,
            Dpad_L_Right,
            Dpad_R_Up,
            Dpad_R_Down,
            Dpad_R_Left,
            Dpad_R_Right,
            Confirm,
            Cancel,
            Stick_L_Up,
            Stick_L_Down,
            Stick_L_Left,
            Stick_L_Right,
            Stick_R_Up,
            Stick_R_Down,
            Stick_R_Left,
            Stick_R_Right,
        }

        public enum Axis
        {
            Stick_L_X,
            Stick_L_Y,
            Stick_R_X,
            Stick_R_Y,
            Horizontal,
            Vertical,
            Trigger_L,
            Trigger_R,

        }
        public enum ButtonState
        {
            Pressed,
            Up,
            Down
        }
        public static Controller[] controllers = new Controller[maxControllerNum];
        public const int maxControllerNum = 4;

        // Use Awake() to initialization
        protected override void Awake()
        {
            base.Awake();
            if (_instance == this)
            {
                DontDestroyOnLoad(gameObject);
                for (int i = 0; i < maxControllerNum; i++)
                {
                    controllers[i] = new Controller(i);
                }
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void BeforeSceneLoad()
        {
            if (!IsAlive)
            {
                ControllerSystem cs = Instance;
            }
        }

        public static float GetAxis(Axis axis, int controllerID = 0)
        {
            switch (axis)
            {
                case Axis.Stick_R_X:
                    return Input.GetAxis("CameraHorizontal");
                case Axis.Stick_R_Y:
                    return Input.GetAxis("CameraVertical");

                case Axis.Stick_L_X:
                case Axis.Horizontal:
                    return Input.GetAxis("Horizontal");
                case Axis.Stick_L_Y:
                case Axis.Vertical:
                    return Input.GetAxis("Vertical");
            }
            return 0;
        }

        private static bool GetKey(ButtonState state, KeyCode key)
        {
            switch (state)
            {
                case ButtonState.Pressed:
                    return Input.GetKey(key);
                case ButtonState.Down:
                    return Input.GetKeyDown(key);
                case ButtonState.Up:
                    return Input.GetKeyUp(key);
            }
            return false;
        }

        private static bool GetButton(ButtonState state, Button button)
        {
            switch (button)
            {
                case Button.Up:
                case Button.Dpad_L_Up:
                    return GetKey(state, KeyCode.W);

                case Button.Down:
                case Button.Dpad_L_Down:
                    return GetKey(state, KeyCode.S);

                case Button.Left:
                case Button.Dpad_L_Left:
                    return GetKey(state, KeyCode.A);

                case Button.Right:
                case Button.Dpad_L_Right:
                    return GetKey(state, KeyCode.D);

                case Button.Dpad_R_Up:
                case Button.Y:
                    return GetKey(state, KeyCode.I);

                case Button.Dpad_R_Down:
                case Button.A:
                    return GetKey(state, KeyCode.K);

                case Button.Dpad_R_Left:
                case Button.X:
                    return GetKey(state, KeyCode.J);

                case Button.Dpad_R_Right:
                case Button.B:
                    return GetKey(state, KeyCode.L);

            }
            return false;
        }

        public static bool GetButton(Button button, int controllerID = 0)
        {
            return GetButton(ButtonState.Pressed, button);
        }

        public static bool GetButtonUp(Button button, int controllerID = 0)
        {
            return GetButton(ButtonState.Up, button);
        }

        public static bool GetButtonDown(Button button, int controllerID = 0)
        {
            return GetButton(ButtonState.Down, button);
        }

    }

    public class Controller
    {
        public int id;

        public Controller(int id)
        {
            this.id = id;
        }
        
    }

}