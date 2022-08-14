namespace Reginald.Services.Input
{
    using System;
    using System.Collections.Generic;
    using Reginald.Core.Extensions;
    using static Reginald.Services.Input.NativeMethods;

    public static class InjectedKeyboardInput
    {
        private const string CursorVariable = "{{__cursor__}}";

        /// <summary>
        /// Specifies the possible types of input events.
        /// </summary>
        [Flags]
        private enum InputType : uint
        {
            /// <summary>
            /// The event is a mouse event.
            /// </summary>
            Mouse = 0,

            /// <summary>
            /// The event is a keyboard event.
            /// </summary>
            Keyboard = 1,

            /// <summary>
            /// The event is a hardware event.
            /// </summary>
            Hardware = 2,
        }

        public static List<INPUT> FromTextExpansion(string trigger, string replacement)
        {
            List<INPUT> inputs = new();
            if (string.IsNullOrEmpty(replacement))
            {
                return inputs;
            }

            // Simulates backspace to delete the trigger
            inputs.AddRange(RepeatInput(VirtualKeyShort.BACK, trigger.Length));

            string expression = replacement;
            int cursorIndex = expression.IndexOf(CursorVariable);
            int leftArrowCount = 0;
            if (cursorIndex > 0)
            {
                expression = expression.Replace(CursorVariable, string.Empty, 1);
                leftArrowCount = expression.Length - cursorIndex;
            }

            inputs.AddRange(FromUnicodeString(expression));

            // Simulates left arrow key to set cursor position
            inputs.AddRange(RepeatInput(VirtualKeyShort.LEFT, leftArrowCount));

            return inputs;
        }

        /// <summary>
        /// Converts an array of <see cref="VirtualKeyShort"/> to an array of <see cref="INPUT"/>.
        /// </summary>
        /// <param name="vks">An array of <see cref="VirtualKeyShort"/>.</param>
        /// <returns>A <see cref="INPUT"/>[] containing keyboard inputs.</returns>
        public static INPUT[] FromVirtualKeys(VirtualKeyShort[] vks)
        {
            int length = vks.Length;
            INPUT[] inputs = new INPUT[length * 2];
            for (int i = 0; i < length; i++)
            {
                inputs[i] = new INPUT
                {
                    type = (uint)InputType.Keyboard,
                    U = new InputUnion
                    {
                        ki = new KEYBDINPUT
                        {
                            wVk = vks[i],
                            dwFlags = KEYEVENTF.KEYDOWN,
                            wScan = 0,
                        },
                    },
                };
                inputs[i + length] = new INPUT
                {
                    type = (uint)InputType.Keyboard,
                    U = new InputUnion
                    {
                        ki = new KEYBDINPUT
                        {
                            wVk = vks[i],
                            dwFlags = KEYEVENTF.KEYUP,
                            wScan = 0,
                        },
                    },
                };
            }

            return inputs;
        }

        private static List<INPUT> FromUnicodeString(string expression)
        {
            List<INPUT> inputs = new();
            if (string.IsNullOrEmpty(expression))
            {
                return inputs;
            }

            for (int i = 0; i < expression.Length; i++)
            {
                char ch = expression[i];
                switch (ch)
                {
                    case '\0':
                        break;
                    case '\t':
                        inputs.AddRange(FromVirtualKeys(new VirtualKeyShort[] { VirtualKeyShort.TAB }));
                        break;
                    case '\n':
                        inputs.AddRange(FromVirtualKeys(new VirtualKeyShort[] { VirtualKeyShort.SHIFT, VirtualKeyShort.RETURN }));
                        break;
                    default:
                        INPUT input = new();
                        input.type = (uint)InputType.Keyboard;
                        input.U = new InputUnion
                        {
                            ki = new KEYBDINPUT
                            {
                                wVk = 0,
                                wScan = unchecked((short)ch),
                                dwFlags = KEYEVENTF.UNICODE,
                            },
                        };
                        inputs.Add(input);

                        // Handles repeat characters when using SendInput.
                        input.U.ki.dwFlags |= KEYEVENTF.KEYUP;
                        inputs.Add(input);
                        break;
                }
            }

            return inputs;
        }

        private static List<INPUT> RepeatInput(VirtualKeyShort vks, int count)
        {
            List<INPUT> inputs = new();
            for (int i = 0; i < count; i++)
            {
                inputs.Add(new INPUT
                {
                    type = (uint)InputType.Keyboard,
                    U = new InputUnion
                    {
                        ki = new KEYBDINPUT
                        {
                            wVk = vks,
                            dwFlags = KEYEVENTF.KEYDOWN,
                            wScan = 0,
                        },
                    },
                });

                inputs.Add(new INPUT
                {
                    type = (uint)InputType.Keyboard,
                    U = new InputUnion
                    {
                        ki = new KEYBDINPUT
                        {
                            wVk = vks,
                            dwFlags = KEYEVENTF.KEYUP,
                            wScan = 0,
                        },
                    },
                });
            }

            return inputs;
        }
    }
}
