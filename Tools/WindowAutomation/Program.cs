using System.Runtime.InteropServices;
using System.Text;

namespace WindowAutomation
{
    class Program
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string? lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        private const int SW_RESTORE = 9;

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                PrintUsage();
                return;
            }

            string command = args[0].ToLower();

            switch (command)
            {
                case "activate":
                    if (args.Length < 2)
                    {
                        Console.WriteLine("Error: Window title required for 'activate' command");
                        PrintUsage();
                        return;
                    }
                    ActivateWindow(args[1]);
                    break;

                case "sendkeys":
                    if (args.Length < 3)
                    {
                        Console.WriteLine("Error: Window title and keys required for 'sendkeys' command");
                        PrintUsage();
                        return;
                    }
                    SendKeysToWindow(args[1], args[2]);
                    break;

                case "list":
                    ListWindows();
                    break;

                case "getactive":
                    GetActiveWindow();
                    break;

                case "setup":
                    SetupAllAgents();
                    break;

                default:
                    Console.WriteLine($"Error: Unknown command '{command}'");
                    PrintUsage();
                    break;
            }
        }

        private static void SetupAllAgents()
        {
            string[] agents = { "Leader", "Developer", "Tester", "DevOps" };
            string antigravityPath = "antigravity";
            string workspacePath = "c:\\Workspace\\AntiCorp";

            Console.WriteLine("AntiCorp System Orchestrator - Starting Setup Sequence...");
            Console.WriteLine("---------------------------------------------------------");

            foreach (var agent in agents)
            {
                string profileName = $"AntiCorp-{agent}";
                string windowSearchTitle = profileName; // 에이전트 윈도우 타이틀에 프로필명이 포함된다고 가정

                Console.WriteLine($"\n[Step] Setting up {agent} Agent...");

                IntPtr hWnd = FindWindowByPartialTitle(windowSearchTitle);
                if (hWnd == IntPtr.Zero)
                {
                    Console.WriteLine($"  - Starting {agent} process...");
                    try
                    {
                        var startInfo = new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = antigravityPath,
                            Arguments = $"--profile {profileName} --workspace {workspacePath}",
                            UseShellExecute = true
                        };
                        System.Diagnostics.Process.Start(startInfo);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"  - Error starting {agent}: {ex.Message}");
                        continue;
                    }

                    // 윈도우가 나타날 때까지 대기 (최대 30초)
                    int retries = 30;
                    while (hWnd == IntPtr.Zero && retries > 0)
                    {
                        Thread.Sleep(1000);
                        hWnd = FindWindowByPartialTitle(windowSearchTitle);
                        retries--;
                    }
                }

                if (hWnd != IntPtr.Zero)
                {
                    Console.WriteLine($"  - Found window for {agent}. Activating...");
                    ActivateWindow(windowSearchTitle);
                    Thread.Sleep(1500); // 포커스 대기 시간 확장

                    // 명령어 입력 시퀀스
                    Console.WriteLine($"  - Sending command: /monitor-issues");
                    
                    // Ctrl + L (입력창 포커스/지우기)
                    System.Windows.Forms.SendKeys.SendWait("^l");
                    Thread.Sleep(500);

                    // 명령어 타이핑
                    System.Windows.Forms.SendKeys.SendWait("/monitor-issues");
                    Thread.Sleep(200);

                    // Enter 실행
                    System.Windows.Forms.SendKeys.SendWait("{ENTER}");
                    
                    Console.WriteLine($"  - {agent} Agent initialization sequence completed.");
                }
                else
                {
                    Console.WriteLine($"  - Error: Timed out waiting for {agent} window.");
                }

                Thread.Sleep(1000); // 에이전트 간 실행 간격
            }

            Console.WriteLine("\n---------------------------------------------------------");
            Console.WriteLine("All Agents have been processed. Systems ready.");
        }

        private static void ActivateWindow(string windowTitle)
        {
            IntPtr hWnd = FindWindowByPartialTitle(windowTitle);

            if (hWnd == IntPtr.Zero)
            {
                Console.WriteLine($"Error: Window containing '{windowTitle}' not found");
                return;
            }

            ShowWindow(hWnd, SW_RESTORE);
            SetForegroundWindow(hWnd);
            Console.WriteLine($"Activated window: {windowTitle}");
        }

        private static void SendKeysToWindow(string windowTitle, string keys)
        {
            ActivateWindow(windowTitle);
            Thread.Sleep(500); // 윈도우 활성화 대기

            // SendKeys 사용
            System.Windows.Forms.SendKeys.SendWait(keys);
            Console.WriteLine($"Sent keys '{keys}' to window '{windowTitle}'");
        }

        private static void ListWindows()
        {
            Console.WriteLine("Listing all visible windows:");
            EnumWindows(EnumWindowsCallback, IntPtr.Zero);
        }

        private static void GetActiveWindow()
        {
            IntPtr hWnd = GetForegroundWindow();
            StringBuilder sb = new StringBuilder(256);
            GetWindowText(hWnd, sb, 256);
            Console.WriteLine($"Active window: {sb}");
        }

        private static IntPtr FindWindowByPartialTitle(string partialTitle)
        {
            IntPtr foundWindow = IntPtr.Zero;
            EnumWindows((hWnd, lParam) =>
            {
                StringBuilder sb = new StringBuilder(256);
                GetWindowText(hWnd, sb, 256);
                string title = sb.ToString();

                if (title.Contains(partialTitle, StringComparison.OrdinalIgnoreCase))
                {
                    foundWindow = hWnd;
                    return false; // Stop enumeration
                }
                return true; // Continue enumeration
            }, IntPtr.Zero);

            return foundWindow;
        }

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        private static bool EnumWindowsCallback(IntPtr hWnd, IntPtr lParam)
        {
            if (!IsWindowVisible(hWnd))
                return true;

            StringBuilder sb = new StringBuilder(256);
            GetWindowText(hWnd, sb, 256);

            if (sb.Length > 0)
            {
                Console.WriteLine($"  - {sb}");
            }

            return true;
        }

        private static void PrintUsage()
        {
            Console.WriteLine("AntiCorp Window Automation Tool");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("  WindowAutomation <command> [arguments]");
            Console.WriteLine();
            Console.WriteLine("Commands:");
            Console.WriteLine("  activate <window_title>              Activate window by partial title match");
            Console.WriteLine("  sendkeys <window_title> <keys>       Send keys to window");
            Console.WriteLine("  list                                 List all visible windows");
            Console.WriteLine("  getactive                            Get currently active window");
            Console.WriteLine("  setup                                Launch and initialize all 4 agents");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  WindowAutomation setup");
            Console.WriteLine("  WindowAutomation activate \"Antigravity - Leader\"");
            Console.WriteLine("  WindowAutomation sendkeys \"Antigravity\" \"^k\"");
            Console.WriteLine("  WindowAutomation list");
        }
    }
}
