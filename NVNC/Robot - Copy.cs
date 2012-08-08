using System;
using System.Runtime.InteropServices;

namespace NVNC
{
    public static class Robot
    {
        public class KeyCode
        {
            public int key;
            public bool isShift;
            public bool isAlt;
            public bool isCtrl;
        }
        private static int mouseModifiers = 0;
        private static bool shift = false;

        private static byte MapKeyCode(int keyCode)
        {
            //TODO there need a keymap for some special chars
            //http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/WinUI/WindowsUserInterface/UserInput/VirtualKeyCodes.asp
            switch (keyCode)
            {
                /*case KeyEventArgs.VK_DELETE:
                    return VK_DELETE;*/
                default:
                    return (byte)keyCode;
            }
        }
        public static void keyEvent(bool pressed, int key)
        {
            KeyCode localKeyCode = keysym.toVKCode(key);
            if (localKeyCode != null)
                try
                {
                    if (pressed)
                    {
                        if (localKeyCode.isShift) ;
                        keyPress(localKeyCode.key);
                        if (localKeyCode.key == 16)
                            shift = true;
                    }
                    else
                    {
                        keyRelease(localKeyCode.key);
                        if (localKeyCode.key == 16)
                            shift = false;
                        if (!localKeyCode.isShift) ;
                    }
                }
                catch (Exception localException1)
                {
                    Console.WriteLine(localException1.Message);
                }
            else if (!pressed)
                try
                {
                    if (shift)
                        keyRelease(16);
                    keyPress(18);
                    String str1 = Convert.ToString(key);
                    for (int i = 0; i < str1.Length; i++)
                    {
                        try
                        {
                            String str2 = null;
                            if (!((i + 1) > str1.Length))
                                str2 = str1.Substring(i, i + 1);
                            else
                                str2 = str1;
                            int j = Int32.Parse(str2);
                            keyPress(96 + j);
                            keyRelease(96 + j);
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            //Console.WriteLine(ex.ToString()); Console.ReadLine();
                        }
                    }
                    keyRelease(18);
                }
                finally
                {
                    keyRelease(18);
                }
        }
        public static void keyPress(int keycode)
        {
            keybd_event(MapKeyCode(keycode), 0, KEYEVENTF_KEYDOWN, IntPtr.Zero);
        }
        public static void keyRelease(int keycode)
        {
            keybd_event(MapKeyCode(keycode), 0, KEYEVENTF_KEYUP, IntPtr.Zero);
        }
        
        public static void PointerEvent(byte Mask, ushort X, ushort Y)
        {
            int i = 0;
            if ((Mask & 0x1) != 0)
                i |= 16;
            if ((Mask & 0x2) != 0)
                i |= 8;
            if ((Mask & 0x4) != 0)
                i |= 4;
            if (i != mouseModifiers)
            {
                if (mouseModifiers == 0)
                {
                    mouseMove(X, Y);
                    mousePress(i);
                }
                else
                {
                    mouseRelease(mouseModifiers);
                }
                mouseModifiers = i;
            }
            else
            {
                mouseMove(X, Y);
            }
        }
        public static void mouseMove(int x, int y)
        {
            System.Windows.Forms.Cursor.Position = new System.Drawing.Point(x, y);
        }
        public static void mousePress(int button)
        {
            int dwFlags = 0;
            switch (button)
            {
                case (int)InputEvent.BUTTON1_MASK:
                    dwFlags |= MOUSEEVENTF_LEFTDOWN;
                    break;
                case (int)InputEvent.BUTTON2_MASK:
                    dwFlags |= MOUSEEVENTF_MIDDLEDOWN;
                    break;
                case (int)InputEvent.BUTTON3_MASK:
                    dwFlags |= MOUSEEVENTF_RIGHTDOWN;
                    break;
            }
            mouse_event(dwFlags, 0, 0, 0, IntPtr.Zero);
        }
        public static void mouseRelease(int button)
        {
            int dwFlags = 0;
            switch (button)
            {
                case (int)InputEvent.BUTTON1_MASK:
                    dwFlags |= MOUSEEVENTF_LEFTUP;
                    break;
                case (int)InputEvent.BUTTON2_MASK:
                    dwFlags |= MOUSEEVENTF_MIDDLEUP;
                    break;
                case (int)InputEvent.BUTTON3_MASK:
                    dwFlags |= MOUSEEVENTF_RIGHTUP;
                    break;
            }
            mouse_event(dwFlags, 0, 0, 0, IntPtr.Zero);
        }
        public static void mouseWheel(int wheel)
        {
            mouse_event(0, 0, 0, wheel, IntPtr.Zero);
        }

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte vk, byte scan, int flags, IntPtr extrainfo);

        private const int KEYEVENTF_KEYDOWN = 0x0000;
        private const int KEYEVENTF_KEYUP = 0x0002;

        [DllImport("user32.dll")]
        private static extern void mouse_event(
            int dwFlags, // motion and click options
            int dx, // horizontal position or change
            int dy, // vertical position or change
            int dwData, // wheel movement
            IntPtr dwExtraInfo // application-defined information
        );

        public enum InputEvent : int
        {
            BUTTON1_MASK = 16,
            BUTTON2_MASK = 8,
            BUTTON3_MASK = 4
        }

        private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const int MOUSEEVENTF_LEFTUP = 0x0004;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const int MOUSEEVENTF_RIGHTUP = 0x0010;
        private const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        private const int MOUSEEVENTF_MIDDLEUP = 0x0040;



        public enum KeyEvent : int
        {
            VK_BACK = 0x08,
            VK_TAB = 0x09,

            /*
             * 0x0A - 0x0B : reserved
             */

            VK_CLEAR = 0x0C,
            VK_RETURN = 0x0D,

            VK_SHIFT = 0x10,
            VK_CONTROL = 0x11,
            VK_MENU = 0x12,
            VK_PAUSE = 0x13,
            VK_CAPITAL = 0x14,

            VK_KANA = 0x15,
            VK_HANGEUL = 0x15,   /* old name - should be here for compatibility */
            VK_HANGUL = 0x15,
            VK_JUNJA = 0x17,
            VK_readonly = 0x18,
            VK_HANJA = 0x19,
            VK_KANJI = 0x19,

            VK_ESCAPE = 0x1B,

            VK_CONVERT = 0x1C,
            VK_NONCONVERT = 0x1D,
            VK_ACCEPT = 0x1E,
            VK_MODECHANGE = 0x1F,

            VK_SPACE = 0x20,
            VK_PRIOR = 0x21,
            VK_NEXT = 0x22,
            VK_END = 0x23,
            VK_HOME = 0x24,
            VK_LEFT = 0x25,
            VK_UP = 0x26,
            VK_RIGHT = 0x27,
            VK_DOWN = 0x28,
            VK_SELECT = 0x29,
            VK_PRINT = 0x2A,
            VK_EXECUTE = 0x2B,
            VK_SNAPSHOT = 0x2C,
            VK_INSERT = 0x2D,
            VK_DELETE = 0x2E,
            VK_HELP = 0x2F,

            /*
             * VK_0 - VK_9 are the same as ASCII '0' - '9' (0x30 - 0x39)
             * 0x40 : unassigned
             * VK_A - VK_Z are the same as ASCII 'A' - 'Z' (0x41 - 0x5A)
             */

            VK_LWIN = 0x5B,
            VK_RWIN = 0x5C,
            VK_APPS = 0x5D,

            /*
             * 0x5E : reserved
             */

            VK_SLEEP = 0x5F,

            VK_NUMPAD0 = 0x60,
            VK_NUMPAD1 = 0x61,
            VK_NUMPAD2 = 0x62,
            VK_NUMPAD3 = 0x63,
            VK_NUMPAD4 = 0x64,
            VK_NUMPAD5 = 0x65,
            VK_NUMPAD6 = 0x66,
            VK_NUMPAD7 = 0x67,
            VK_NUMPAD8 = 0x68,
            VK_NUMPAD9 = 0x69,
            VK_MULTIPLY = 0x6A,
            VK_ADD = 0x6B,
            VK_SEPARATOR = 0x6C,
            VK_SUBTRACT = 0x6D,
            VK_DECIMAL = 0x6E,
            VK_DIVIDE = 0x6F,
            VK_F1 = 0x70,
            VK_F2 = 0x71,
            VK_F3 = 0x72,
            VK_F4 = 0x73,
            VK_F5 = 0x74,
            VK_F6 = 0x75,
            VK_F7 = 0x76,
            VK_F8 = 0x77,
            VK_F9 = 0x78,
            VK_F10 = 0x79,
            VK_F11 = 0x7A,
            VK_F12 = 0x7B,
            VK_F13 = 0x7C,
            VK_F14 = 0x7D,
            VK_F15 = 0x7E,
            VK_F16 = 0x7F,
            VK_F17 = 0x80,
            VK_F18 = 0x81,
            VK_F19 = 0x82,
            VK_F20 = 0x83,
            VK_F21 = 0x84,
            VK_F22 = 0x85,
            VK_F23 = 0x86,
            VK_F24 = 0x87,

            /*
             * 0x88 - 0x8F : unassigned
             */

            VK_NUMLOCK = 0x90,
            VK_SCROLL = 0x91,

            /*
             * NEC PC-9800 kbd definitions
             */
            VK_OEM_NEC_EQUAL = 0x92,    // '=' key on numpad

            /*
             * Fujitsu/OASYS kbd definitions
             */
            VK_OEM_FJ_JISHO = 0x92,    // 'Dictionary' key
            VK_OEM_FJ_MASSHOU = 0x93,    // 'Unregister word' key
            VK_OEM_FJ_TOUROKU = 0x94,    // 'Register word' key
            VK_OEM_FJ_LOYA = 0x95,    // 'Left OYAYUBI' key
            VK_OEM_FJ_ROYA = 0x96,    // 'Right OYAYUBI' key

            /*
             * 0x97 - 0x9F : unassigned
             */

            /*
             * VK_L* & VK_R* - left and right Alt, Ctrl and Shift virtual keys.
             * Used only as parameters to GetAsyncKeyState() and GetKeyState().
             * No other API or message will distinguish left and right keys in this way.
             */
            VK_LSHIFT = 0xA0,
            VK_RSHIFT = 0xA1,
            VK_LCONTROL = 0xA2,
            VK_RCONTROL = 0xA3,
            VK_LMENU = 0xA4,
            VK_RMENU = 0xA5,

            VK_BROWSER_BACK = 0xA6,
            VK_BROWSER_FORWARD = 0xA7,
            VK_BROWSER_REFRESH = 0xA8,
            VK_BROWSER_STOP = 0xA9,
            VK_BROWSER_SEARCH = 0xAA,
            VK_BROWSER_FAVORITES = 0xAB,
            VK_BROWSER_HOME = 0xAC,

            VK_VOLUME_MUTE = 0xAD,
            VK_VOLUME_DOWN = 0xAE,
            VK_VOLUME_UP = 0xAF,
            VK_MEDIA_NEXT_TRACK = 0xB0,
            VK_MEDIA_PREV_TRACK = 0xB1,
            VK_MEDIA_STOP = 0xB2,
            VK_MEDIA_PLAY_PAUSE = 0xB3,
            VK_LAUNCH_MAIL = 0xB4,
            VK_LAUNCH_MEDIA_SELECT = 0xB5,
            VK_LAUNCH_APP1 = 0xB6,
            VK_LAUNCH_APP2 = 0xB7,

            /*
             * VK_0 - VK_9 are the same as ASCII '0' - '9' (0x30 - 0x39)
             * 0x40 : unassigned
             * VK_A - VK_Z are the same as ASCII 'A' - 'Z' (0x41 - 0x5A)
             */
            VK_0 = 0x30,
            VK_1 = 0x31,
            VK_2 = 0x32,
            VK_3 = 0x33,
            VK_4 = 0x34,
            VK_5 = 0x35,
            VK_6 = 0x36,
            VK_7 = 0x37,
            VK_8 = 0x38,
            VK_9 = 0x39,

            VK_A = 65,
            VK_B = 66,
            VK_C = 67,
            VK_D = 68,
            VK_E = 69,
            VK_F = 70,
            VK_G = 71,
            VK_H = 72,
            VK_I = 73,
            VK_J = 74,
            VK_K = 75,
            VK_L = 76,
            VK_M = 77,
            VK_N = 78,
            VK_O = 79,
            VK_P = 80,
            VK_Q = 81,
            VK_R = 82,
            VK_S = 83,
            VK_T = 84,
            VK_U = 85,
            VK_V = 86,
            VK_W = 87,
            VK_X = 88,
            VK_Y = 89,
            VK_Z = 90



        }

        /*
         * 0xB8 - 0xB9 : reserved
         */

        private const int VK_OEM_1 = 0xBA;   // ';:' for US
        private const int VK_OEM_PLUS = 0xBB;   // '+' any country
        private const int VK_OEM_COMMA = 0xBC;   // ',' any country
        private const int VK_OEM_MINUS = 0xBD;   // '-' any country
        private const int VK_OEM_PERIOD = 0xBE;   // '.' any country
        private const int VK_OEM_2 = 0xBF;   // '/?' for US
        private const int VK_OEM_3 = 0xC0;   // '`~' for US

        /*
         * 0xC1 - 0xD7 : reserved
         */

        /*
         * 0xD8 - 0xDA : unassigned
         */

        private const int VK_OEM_4 = 0xDB;  //  '[{' for US
        private const int VK_OEM_5 = 0xDC;  //  '\|' for US
        private const int VK_OEM_6 = 0xDD;  //  ']}' for US
        private const int VK_OEM_7 = 0xDE;  //  ''"' for US
        private const int VK_OEM_8 = 0xDF;

        /*
         * 0xE0 : reserved
         */

        /*
         * Various extended or enhanced keyboards
         */
        private const int VK_OEM_AX = 0xE1;  //  'AX' key on Japanese AX kbd
        private const int VK_OEM_102 = 0xE2;  //  "<>" or "\|" on RT 102-key kbd.
        private const int VK_ICO_HELP = 0xE3;  //  Help key on ICO
        private const int VK_ICO_00 = 0xE4;  //  00 key on ICO


        /*
         * 0xE8 : unassigned
         */

        /*
         * Nokia/Ericsson definitions
         */
        private const int VK_OEM_RESET = 0xE9;
        private const int VK_OEM_JUMP = 0xEA;
        private const int VK_OEM_PA1 = 0xEB;
        private const int VK_OEM_PA2 = 0xEC;
        private const int VK_OEM_PA3 = 0xED;
        private const int VK_OEM_WSCTRL = 0xEE;
        private const int VK_OEM_CUSEL = 0xEF;
        private const int VK_OEM_ATTN = 0xF0;
        private const int VK_OEM_FINISH = 0xF1;
        private const int VK_OEM_COPY = 0xF2;
        private const int VK_OEM_AUTO = 0xF3;
        private const int VK_OEM_ENLW = 0xF4;
        private const int VK_OEM_BACKTAB = 0xF5;

        private const int VK_ATTN = 0xF6;
        private const int VK_CRSEL = 0xF7;
        private const int VK_EXSEL = 0xF8;
        private const int VK_EREOF = 0xF9;
        private const int VK_PLAY = 0xFA;
        private const int VK_ZOOM = 0xFB;
        private const int VK_NONAME = 0xFC;
        private const int VK_PA1 = 0xFD;
        private const int VK_OEM_CLEAR = 0xFE;

        public static class keysym
        {
            private const int DeadGrave = 0xFE50;
            private const int DeadAcute = 0xFE51;
            private const int DeadCircumflex = 0xFE52;
            private const int DeadTilde = 0xFE53;

            private const int BackSpace = 0xFF08;
            private const int Tab = 0xFF09;
            private const int Linefeed = 0xFF0A;
            private const int Clear = 0xFF0B;
            private const int Return = 0xFF0D;
            private const int Pause = 0xFF13;
            private const int ScrollLock = 0xFF14;
            private const int SysReq = 0xFF15;
            private const int Escape = 0xFF1B;

            private const int Delete = 0xFFFF;

            private const int Home = 0xFF50;
            private const int Left = 0xFF51;
            private const int Up = 0xFF52;
            private const int Right = 0xFF53;
            private const int Down = 0xFF54;
            private const int PageUp = 0xFF55;
            private const int PageDown = 0xFF56;
            private const int End = 0xFF57;
            private const int Begin = 0xFF58;

            private const int Select = 0xFF60;
            private const int Print = 0xFF61;
            private const int Execute = 0xFF62;
            private const int Insert = 0xFF63;

            private const int Cancel = 0xFF69;
            private const int Help = 0xFF6A;
            private const int Break = 0xFF6B;
            private const int NumLock = 0xFF6F;

            private const int KpSpace = 0xFF80;
            private const int KpTab = 0xFF89;
            private const int KpEnter = 0xFF8D;

            private const int KpHome = 0xFF95;
            private const int KpLeft = 0xFF96;
            private const int KpUp = 0xFF97;
            private const int KpRight = 0xFF98;
            private const int KpDown = 0xFF99;
            private const int KpPrior = 0xFF9A;
            private const int KpPageUp = 0xFF9A;
            private const int KpNext = 0xFF9B;
            private const int KpPageDown = 0xFF9B;
            private const int KpEnd = 0xFF9C;
            private const int KpBegin = 0xFF9D;
            private const int KpInsert = 0xFF9E;
            private const int KpDelete = 0xFF9F;
            private const int KpEqual = 0xFFBD;
            private const int KpMultiply = 0xFFAA;
            private const int KpAdd = 0xFFAB;
            private const int KpSeparator = 0xFFAC;
            private const int KpSubtract = 0xFFAD;
            private const int KpDecimal = 0xFFAE;
            private const int KpDivide = 0xFFAF;

            private const int KpF1 = 0xFF91;
            private const int KpF2 = 0xFF92;
            private const int KpF3 = 0xFF93;
            private const int KpF4 = 0xFF94;

            private const int Kp0 = 0xFFB0;
            private const int Kp1 = 0xFFB1;
            private const int Kp2 = 0xFFB2;
            private const int Kp3 = 0xFFB3;
            private const int Kp4 = 0xFFB4;
            private const int Kp5 = 0xFFB5;
            private const int Kp6 = 0xFFB6;
            private const int Kp7 = 0xFFB7;
            private const int Kp8 = 0xFFB8;
            private const int Kp9 = 0xFFB9;

            private const int F1 = 0xFFBE;
            private const int F2 = 0xFFBF;
            private const int F3 = 0xFFC0;
            private const int F4 = 0xFFC1;
            private const int F5 = 0xFFC2;
            private const int F6 = 0xFFC3;
            private const int F7 = 0xFFC4;
            private const int F8 = 0xFFC5;
            private const int F9 = 0xFFC6;
            private const int F10 = 0xFFC7;
            private const int F11 = 0xFFC8;
            private const int F12 = 0xFFC9;
            private const int F13 = 0xFFCA;
            private const int F14 = 0xFFCB;
            private const int F15 = 0xFFCC;
            private const int F16 = 0xFFCD;
            private const int F17 = 0xFFCE;
            private const int F18 = 0xFFCF;
            private const int F19 = 0xFFD0;
            private const int F20 = 0xFFD1;
            private const int F21 = 0xFFD2;
            private const int F22 = 0xFFD3;
            private const int F23 = 0xFFD4;
            private const int F24 = 0xFFD5;

            private const int ShiftL = 0xFFE1;
            private const int ShiftR = 0xFFE2;
            private const int ControlL = 0xFFE3;
            private const int ControlR = 0xFFE4;
            private const int CapsLock = 0xFFE5;
            private const int ShiftLock = 0xFFE6;
            private const int MetaL = 0xFFE7;
            private const int MetaR = 0xFFE8;
            private const int AltL = 0xFFE9;
            private const int AltR = 0xFFEA;

            public static int toVK(int keysym)
            {
                switch (keysym)
                {
                    case BackSpace: return (int)KeyEvent.VK_BACK;
                    case Tab: return (int)KeyEvent.VK_TAB;
                    //No Java equivalent: case Linefeed: return (int)KeyEvent.;
                    case Clear: return (int)KeyEvent.VK_CLEAR;
                    case Return: return (int)KeyEvent.VK_RETURN;
                    case Pause: return (int)KeyEvent.VK_PAUSE;
                    case ScrollLock: return (int)KeyEvent.VK_SCROLL;
                    //No Java equivalent: case SysReq: return (int)KeyEvent.;
                    case Escape: return (int)KeyEvent.VK_ESCAPE;

                    case Delete: return (int)KeyEvent.VK_DELETE;

                    case Home: return (int)KeyEvent.VK_HOME;
                    case Left: return (int)KeyEvent.VK_LEFT;
                    case Up: return (int)KeyEvent.VK_UP;
                    case Right: return (int)KeyEvent.VK_RIGHT;
                    case Down: return (int)KeyEvent.VK_DOWN;
                    case PageUp: return (int)KeyEvent.VK_PRIOR;
                    case PageDown: return (int)KeyEvent.VK_NEXT;
                    case End: return (int)KeyEvent.VK_END;
                    //No Java equivalent: case Begin: return (int)KeyEvent.;

                    //No Java equivalent: case Select: return (int)KeyEvent.;
                    case Print: return (int)KeyEvent.VK_SNAPSHOT;
                    //No Java equivalent: case Execute: return (int)KeyEvent.;
                    case Insert: return (int)KeyEvent.VK_INSERT;

                    //case Cancel: return (int)KeyEvent.VK_CANCEL;
                    case Help: return (int)KeyEvent.VK_HELP;
                    //No Java equivalent: case Break: return (int)KeyEvent.;
                    case NumLock: return (int)KeyEvent.VK_NUMLOCK;

                    case KpSpace: return (int)KeyEvent.VK_SPACE;
                    case KpTab: return (int)KeyEvent.VK_TAB;
                    case KpEnter: return (int)KeyEvent.VK_RETURN;

                    case KpHome: return (int)KeyEvent.VK_HOME;
                    case KpLeft: return (int)KeyEvent.VK_LEFT;
                    case KpUp: return (int)KeyEvent.VK_UP;
                    case KpRight: return (int)KeyEvent.VK_RIGHT;
                    case KpDown: return (int)KeyEvent.VK_DOWN;
                    case KpPageUp: return (int)KeyEvent.VK_PRIOR; // = KpPrior
                    case KpPageDown: return (int)KeyEvent.VK_NEXT; // = KpNext
                    case KpEnd: return (int)KeyEvent.VK_END;
                    //No Java equivalent: case KpBegin: return (int)KeyEvent.;
                    case KpInsert: return (int)KeyEvent.VK_INSERT;
                    case KpDelete: return (int)KeyEvent.VK_DELETE;
                    //case KpEqual: return (int)KeyEvent.VK_EQUALS;
                    case KpMultiply: return (int)KeyEvent.VK_MULTIPLY;
                    case KpAdd: return (int)KeyEvent.VK_ADD;
                    case KpSeparator: return (int)KeyEvent.VK_SEPARATOR; // Sun should spellcheck...
                    case KpSubtract: return (int)KeyEvent.VK_SUBTRACT;
                    case KpDecimal: return (int)KeyEvent.VK_DECIMAL;
                    case KpDivide: return (int)KeyEvent.VK_DIVIDE;

                    case KpF1: return (int)KeyEvent.VK_F1;
                    case KpF2: return (int)KeyEvent.VK_F2;
                    case KpF3: return (int)KeyEvent.VK_F3;
                    case KpF4: return (int)KeyEvent.VK_F4;

                    case Kp0: return (int)KeyEvent.VK_NUMPAD0;
                    case Kp1: return (int)KeyEvent.VK_NUMPAD1;
                    case Kp2: return (int)KeyEvent.VK_NUMPAD2;
                    case Kp3: return (int)KeyEvent.VK_NUMPAD3;
                    case Kp4: return (int)KeyEvent.VK_NUMPAD4;
                    case Kp5: return (int)KeyEvent.VK_NUMPAD5;
                    case Kp6: return (int)KeyEvent.VK_NUMPAD6;
                    case Kp7: return (int)KeyEvent.VK_NUMPAD7;
                    case Kp8: return (int)KeyEvent.VK_NUMPAD8;
                    case Kp9: return (int)KeyEvent.VK_NUMPAD9;

                    case F1: return (int)KeyEvent.VK_F1;
                    case F2: return (int)KeyEvent.VK_F2;
                    case F3: return (int)KeyEvent.VK_F3;
                    case F4: return (int)KeyEvent.VK_F4;
                    case F5: return (int)KeyEvent.VK_F5;
                    case F6: return (int)KeyEvent.VK_F6;
                    case F7: return (int)KeyEvent.VK_F7;
                    case F8: return (int)KeyEvent.VK_F8;
                    case F9: return (int)KeyEvent.VK_F9;
                    case F10: return (int)KeyEvent.VK_F10;
                    case F11: return (int)KeyEvent.VK_F11;
                    case F12: return (int)KeyEvent.VK_F12;
                    case F13: return (int)KeyEvent.VK_F12;
                    case F14: return (int)KeyEvent.VK_F12;
                    case F15: return (int)KeyEvent.VK_F12;
                    case F16: return (int)KeyEvent.VK_F12;
                    case F17: return (int)KeyEvent.VK_F12;
                    case F18: return (int)KeyEvent.VK_F12;
                    case F19: return (int)KeyEvent.VK_F12;
                    case F20: return (int)KeyEvent.VK_F12;
                    case F21: return (int)KeyEvent.VK_F12;
                    case F22: return (int)KeyEvent.VK_F12;
                    case F23: return (int)KeyEvent.VK_F12;
                    case F24: return (int)KeyEvent.VK_F12;

                    case CapsLock: return (int)KeyEvent.VK_CAPITAL;
                    //No Java equivalent: case ShiftLock: return (int)KeyEvent.;
                    default: return 0;
                }
            }
            public static KeyCode toVKCode(int paramInt)
            {
                KeyCode localKeyCode = null;
                bool chk = false;
                int i = 0;
                if ((paramInt >= 65) && (paramInt <= 90))
                {
                    i = paramInt;
                    chk = true;
                }
                else if ((paramInt >= 48) && (paramInt <= 57))
                {
                    i = paramInt;
                }
                else if ((paramInt >= 97) && (paramInt <= 122))
                {
                    i = paramInt - 97 + 65;
                }
                else
                {
                    switch (paramInt)
                    {
                        case 60:
                            i = 44;
                            chk = true;
                            break;
                        case 62:
                            i = 46;
                            chk = true;
                            break;
                        case 63:
                            i = 47;
                            chk = true;
                            break;
                        case 58:
                            i = 59;
                            chk = true;
                            break;
                        case 34:
                            i = 222;
                            chk = true;
                            break;
                        case 33:
                            i = 49;
                            chk = true;
                            break;
                        case 64:
                            i = 50;
                            chk = true;
                            break;
                        case 35:
                            i = 51;
                            chk = true;
                            break;
                        case 36:
                            i = 52;
                            chk = true;
                            break;
                        case 37:
                            i = 53;
                            chk = true;
                            break;
                        case 38:
                            i = 55;
                            chk = true;
                            break;
                        case 94:
                            i = 54;
                            chk = true;
                            break;
                        case 40:
                            i = 57;
                            chk = true;
                            break;
                        case 41:
                            i = 48;
                            chk = true;
                            break;
                        case 42:
                            i = 56;
                            chk = true;
                            break;
                        case 43:
                            i = 61;
                            chk = true;
                            break;
                        case 95:
                            i = 45;
                            chk = true;
                            break;
                        case 123:
                            i = 91;
                            chk = true;
                            break;
                        case 125:
                            i = 93;
                            chk = true;
                            break;
                        default:
                            i = toVKall(paramInt);
                            break;
                    }
                }
                if (i != 0)
                {
                    localKeyCode = new KeyCode();
                    localKeyCode.key = i;
                    localKeyCode.isShift = chk;
                }
                return localKeyCode;
            }
            public static int toVKall(int keysym)
            {
                int key = toVK(keysym);
                if (key != 0)
                    return key;

                switch (keysym)
                {
                    case ShiftL: return (int)KeyEvent.VK_LSHIFT;
                    case ShiftR: return (int)KeyEvent.VK_RSHIFT;
                    case ControlL: return (int)KeyEvent.VK_LCONTROL;
                    case ControlR: return (int)KeyEvent.VK_RCONTROL;
                    case AltL: return (int)KeyEvent.VK_LMENU;
                    case AltR: return (int)KeyEvent.VK_RMENU;
                    default: return 0;
                }
            }

            public static int toMask(int keysym)
            {
                switch (keysym)
                {
                    case ShiftL: return (int)KeyEvent.VK_LSHIFT;
                    case ShiftR: return (int)KeyEvent.VK_RSHIFT;
                    case ControlL: return (int)KeyEvent.VK_LCONTROL;
                    case ControlR: return (int)KeyEvent.VK_RCONTROL;
                    case AltL: return (int)KeyEvent.VK_LMENU;
                    case AltR: return (int)KeyEvent.VK_RMENU;
                    default: return 0;
                }
            }
        }
    }
}