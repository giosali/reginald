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

        public static List<INPUT> FromTextExpansion(string trigger, string replacement)
        {
            List<INPUT> inputs = new();
            if (!string.IsNullOrEmpty(replacement))
            {
                // Simulates backspace to delete the trigger
                inputs.AddRange(RepeatInput(VirtualKeyShort.BACK, trigger.Length));

                string expression = replacement;
                int cursorIndex = expression.IndexOf(CursorVariable);
                int leftArrowCount = -1;
                if (cursorIndex > 0)
                {
                    expression = expression.Replace(CursorVariable, string.Empty, 1);
                    leftArrowCount = expression.Length - cursorIndex;
                }

                inputs.AddRange(FromUnicodeString(expression));

                // Simulates left arrow key to set cursor position
                inputs.AddRange(RepeatInput(VirtualKeyShort.LEFT, leftArrowCount));
            }

            return inputs;
        }

        private static List<INPUT> FromUnicodeString(string expression)
        {
            List<INPUT> inputs = new();
            if (!string.IsNullOrEmpty(expression))
            {
                char previousCh = '\0';
                for (int i = 0; i < expression.Length; i++)
                {
                    char ch = expression[i];

                    // Adds nothing if the current character is equal to the previous character
                    if (ch == previousCh)
                    {
                        inputs.Add(new INPUT
                        {
                            type = (uint)InputType.Keyboard,
                            U = new InputUnion
                            {
                                ki = new KEYBDINPUT
                                {
                                    wVk = 0,
                                    dwFlags = KEYEVENTF.UNICODE,
                                    wScan = unchecked(0),
                                },
                            },
                        });
                    }

                    // Simulates the Shift + Enter keys if the character is a newline char
                    if (ch == '\n')
                    {
                        inputs.Add(new INPUT
                        {
                            type = (uint)InputType.Keyboard,
                            U = new InputUnion
                            {
                                ki = new KEYBDINPUT
                                {
                                    wVk = VirtualKeyShort.SHIFT,
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
                                    wVk = VirtualKeyShort.RETURN,
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
                                    wVk = VirtualKeyShort.SHIFT,
                                    dwFlags = KEYEVENTF.KEYUP,
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
                                    wVk = VirtualKeyShort.RETURN,
                                    dwFlags = KEYEVENTF.KEYUP,
                                    wScan = 0,
                                },
                            },
                        });
                    }
                    else if (ch == '\t')
                    {
                        inputs.Add(new INPUT
                        {
                            type = (uint)InputType.Keyboard,
                            U = new InputUnion
                            {
                                ki = new KEYBDINPUT
                                {
                                    wVk = VirtualKeyShort.TAB,
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
                                    wVk = VirtualKeyShort.TAB,
                                    dwFlags = KEYEVENTF.KEYUP,
                                    wScan = 0,
                                },
                            },
                        });
                    }
                    else
                    {
                        inputs.Add(new INPUT
                        {
                            type = (uint)InputType.Keyboard,
                            U = new InputUnion
                            {
                                ki = new KEYBDINPUT
                                {
                                    wVk = 0,
                                    dwFlags = KEYEVENTF.UNICODE,
                                    wScan = unchecked((short)ch),
                                },
                            },
                        });
                    }

                    previousCh = ch;
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
