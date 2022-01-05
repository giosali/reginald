using Reginald.Core.DataModels;
using Reginald.Core.Enums;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Reginald.Core.InputInjection
{
    public class Keyboard
    {
        [DllImport("user32.dll")]
        internal static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);

        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT
        {
            internal uint type;
            internal InputUnion U;
            internal static int Size => Marshal.SizeOf(typeof(INPUT));
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct InputUnion
        {
            [FieldOffset(0)]
            internal MOUSEINPUT mi;
            [FieldOffset(0)]
            internal KEYBDINPUT ki;
            [FieldOffset(0)]
            internal HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MOUSEINPUT
        {
            internal int dx;
            internal int dy;
            internal int mouseData;
            internal MOUSEEVENTF dwFlags;
            internal uint time;
            internal UIntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct KEYBDINPUT
        {
            internal VirtualKeyShort wVk;
            internal short wScan;
            internal KEYEVENTF dwFlags;
            internal int time;
            internal UIntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HARDWAREINPUT
        {
            internal int uMsg;
            internal short wParamL;
            internal short wParamH;
        }

        private static List<INPUT> InputsFromString(string expression)
        {
            List<INPUT> inputs = new();
            if (!string.IsNullOrEmpty(expression))
            {
                char? previousCharacter = null;
                for (int i = 0; i < expression.Length; i++)
                {
                    char character = expression[i];
                    // If previous character is equal to the next character, add void
                    if (character == previousCharacter)
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
                                    wScan = unchecked(0)
                                }
                            }
                        });
                    }

                    // If the character is a newline char, simulate the Enter key
                    if (character == '\n')
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
                                    wScan = 0
                                }
                            }
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
                                    wScan = 0
                                }
                            }
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
                                    wScan = 0
                                }
                            }
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
                                    wScan = 0
                                }
                            }
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
                                    wScan = unchecked((short)character)
                                }
                            }
                        });
                    }

                    previousCharacter = character;
                }
            }
            return inputs;
        }

        private static List<INPUT> BackspaceInputsFromString(string expression)
        {
            // Delete previously typed input that initiated the trigger
            List<INPUT> inputs = new();
            for (int i = 0; i < expression.Length + 1; i++)
            {
                inputs.Add(new INPUT
                {
                    type = (uint)InputType.Keyboard,
                    U = new InputUnion
                    {
                        ki = new KEYBDINPUT
                        {
                            wVk = VirtualKeyShort.BACK,
                            dwFlags = KEYEVENTF.KEYDOWN,
                            wScan = 0
                        }
                    }
                });

                inputs.Add(new INPUT
                {
                    type = (uint)InputType.Keyboard,
                    U = new InputUnion
                    {
                        ki = new KEYBDINPUT
                        {
                            wVk = VirtualKeyShort.BACK,
                            dwFlags = KEYEVENTF.KEYUP,
                            wScan = 0
                        }
                    }
                });
            }
            return inputs;
        }

        public static void SendString(string expression)
        {
            if (!string.IsNullOrEmpty(expression))
            {
                List<INPUT> inputs = InputsFromString(expression);
                _ = SendInput((uint)inputs.Count, inputs.ToArray(), INPUT.Size);
            }
        }

        public static void SendString(string trigger, string replacement)
        {
            if (!string.IsNullOrEmpty(replacement))
            {
                List<INPUT> inputs = BackspaceInputsFromString(trigger);
                inputs.AddRange(InputsFromString(replacement));
                _ = SendInput((uint)inputs.Count, inputs.ToArray(), INPUT.Size);
            }
        }

        public static async void DelaySendString(ExpansionDataModel expansion)
        {
            Task task = Task.Run(async () =>
            {
                await Task.Delay(50);
                SendString(expansion.Trigger, expansion.Replacement);
            });
            await task;
        }

        public static void Paste()
        {
            List<INPUT> inputs = new();

            inputs.Add(new INPUT
            {
                type = (uint)InputType.Keyboard,
                U = new InputUnion
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = VirtualKeyShort.CONTROL,
                        dwFlags = KEYEVENTF.KEYDOWN,
                        wScan = 0
                    }
                }
            });

            inputs.Add(new INPUT
            {
                type = (uint)InputType.Keyboard,
                U = new InputUnion
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = VirtualKeyShort.KEY_V,
                        dwFlags = KEYEVENTF.KEYDOWN,
                        wScan = 0
                    }
                }
            });

            inputs.Add(new INPUT
            {
                type = (uint)InputType.Keyboard,
                U = new InputUnion
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = VirtualKeyShort.CONTROL,
                        dwFlags = KEYEVENTF.KEYUP,
                        wScan = 0
                    }
                }
            });

            inputs.Add(new INPUT
            {
                type = (uint)InputType.Keyboard,
                U = new InputUnion
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = VirtualKeyShort.KEY_V,
                        dwFlags = KEYEVENTF.KEYUP,
                        wScan = 0
                    }
                }
            });

            _ = SendInput((uint)inputs.Count, inputs.ToArray(), INPUT.Size);
        }
    }
}
