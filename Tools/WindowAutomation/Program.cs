using System.Runtime.InteropServices;
using System.Text;
using Octokit;

namespace WindowAutomation
{
    class Program
    {
        private static readonly HashSet<int> ProcessedIssues = new();
        private static string? _githubToken;
        private static string? _repoOwner;
        private static string? _repoName;

        private static readonly Dictionary<string, List<string>> RoleToLabels = new()
        {
            { "Leader", new List<string> { "@leader", "@new-project" } },
            { "Developer", new List<string> { "@developer" } },
            { "Tester", new List<string> { "@tester" } },
            { "Planner", new List<string> { "@planner" } }
        };

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

                case "monitor":
                    ParseMonitorArgs(args);
                    MonitorIssues().GetAwaiter().GetResult();
                    break;

                case "check":
                    ParseCheckArgs(args);
                    CheckIssue().GetAwaiter().GetResult();
                    break;

                default:
                    Console.WriteLine($"Error: Unknown command '{command}'");
                    PrintUsage();
                    break;
            }
        }

        private static string[]? _checkLabels;
        private static bool _markWorking = true;

        private static void ParseCheckArgs(string[] args)
        {
            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i].ToLower())
                {
                    case "--repo":
                    case "-r":
                        if (i + 1 < args.Length)
                        {
                            var repo = args[++i];
                            var parts = repo.Split('/');
                            if (parts.Length == 2)
                            {
                                _repoOwner = parts[0];
                                _repoName = parts[1];
                            }
                        }
                        break;
                    case "--labels":
                    case "-l":
                        if (i + 1 < args.Length) _checkLabels = args[++i].Split(',');
                        break;
                    case "--token":
                    case "-t":
                        if (i + 1 < args.Length) _githubToken = args[++i];
                        break;
                    case "--no-mark":
                        _markWorking = false;
                        break;
                }
            }

            if (string.IsNullOrEmpty(_githubToken))
                _githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
        }

        private static async Task CheckIssue()
        {
            if (string.IsNullOrEmpty(_repoOwner) || string.IsNullOrEmpty(_repoName))
            {
                Console.WriteLine("Error: Repository owner and name are required.");
                return;
            }

            var client = new GitHubClient(new ProductHeaderValue("AntiCorp-WindowAutomation"));
            if (!string.IsNullOrEmpty(_githubToken))
            {
                client.Credentials = new Credentials(_githubToken);
            }

            try
            {
                var issues = await client.Issue.GetAllForRepository(_repoOwner, _repoName, new RepositoryIssueRequest
                {
                    State = ItemStateFilter.Open,
                    SortProperty = IssueSort.Created,
                    SortDirection = SortDirection.Descending
                });

                foreach (var issue in issues)
                {
                    var labels = issue.Labels.Select(l => l.Name).ToList();
                    
                    bool matches = _checkLabels == null || _checkLabels.Any(l => labels.Contains(l));
                    if (matches)
                    {
                        Console.WriteLine($"Found matched issue: #{issue.Number} - {issue.Title}");
                        Console.WriteLine($"Body: {issue.Body}");
                        
                        // 라벨 업데이트 (작무 중으로 변경)
                        if (_markWorking)
                        {
                            if (labels.Contains("@notified"))
                            {
                                await client.Issue.Labels.RemoveFromIssue(_repoOwner, _repoName, issue.Number, "@notified");
                            }
                            await client.Issue.Labels.AddToIssue(_repoOwner, _repoName, issue.Number, new[] { "@working" });
                            Console.WriteLine($"[Check] Marked issue #{issue.Number} as '@working' and removed '@notified'.");
                        }
                        
                        return; // 최근 이슈 하나만 처리
                    }
                }
                Console.WriteLine("No matching issues found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Check Error] {ex.Message}");
            }
        }

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        private const byte VK_CONTROL = 0x11;
        private const byte VK_SHIFT = 0x10;
        private const byte VK_L = 0x4C;
        private const byte VK_1 = 0x31;
        private const byte VK_RETURN = 0x0D;
        private const byte VK_OEM_2 = 0xBF; // '/'
        private const byte VK_OEM_MINUS = 0xBD; // '-'
        private const byte VK_2 = 0x32; // '@' is Shift + 2
        private const uint KEYEVENTF_KEYUP = 0x0002;

        private static async Task MonitorIssues()
        {
            if (string.IsNullOrEmpty(_repoOwner) || string.IsNullOrEmpty(_repoName))
            {
                Console.WriteLine("Error: Repository owner and name are required for monitoring.");
                return;
            }

            Console.WriteLine($"[Monitor] Starting GitHub verification for {_repoOwner}/{_repoName}...");
            if (string.IsNullOrEmpty(_githubToken))
            {
                Console.WriteLine("[Monitor Warning] GITHUB_TOKEN is not set. Label updates will fail.");
            }
            
            var client = new GitHubClient(new ProductHeaderValue("AntiCorp-WindowAutomation"));
            if (!string.IsNullOrEmpty(_githubToken))
            {
                client.Credentials = new Credentials(_githubToken);
            }

            int pollCount = 1;
            while (true)
            {
                try
                {
                    Console.WriteLine($"[Monitor] Poll Cycle #{pollCount++} at {DateTime.Now:HH:mm:ss}...");
                    var issues = await client.Issue.GetAllForRepository(_repoOwner, _repoName, new RepositoryIssueRequest
                    {
                        State = ItemStateFilter.Open,
                        SortProperty = IssueSort.Created,
                        SortDirection = SortDirection.Descending
                    });

                    foreach (var issue in issues)
                    {
                        var labels = issue.Labels.Select(l => l.Name).ToList();
                        
                        // 이미 알림이 갔거나 처리 중인 경우 스킵
                        if (labels.Contains("@notified") || labels.Contains("@working") || labels.Contains("@done"))
                            continue;

                        foreach (var role in RoleToLabels.Keys)
                        {
                            bool hasRoleLabel = labels.Any(l => RoleToLabels[role].Contains(l)) || labels.Contains("@all");
                            
                            if (hasRoleLabel)
                            {
                                Console.WriteLine($"[Monitor] Triggering {role} Agent for Issue #{issue.Number}...");
                                if (TriggerAgent(role))
                                {
                                    // 알림 성공 시 '@notified' 라벨 추가
                                    await client.Issue.Labels.AddToIssue(_repoOwner, _repoName, issue.Number, new[] { "@notified" });
                                    Console.WriteLine($"[Monitor] Label '@notified' added to Issue #{issue.Number}.");
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Monitor Error] {ex.GetType().Name}: {ex.Message}");
                }

                await Task.Delay(10000);
            }
        }

        private static bool TriggerAgent(string role)
        {
            string windowSearchTitle = role; 
            IntPtr hWnd = FindWindowByPartialTitle(windowSearchTitle);

            if (hWnd == IntPtr.Zero)
            {
                Console.WriteLine($"[Trigger Error] Could not find window for {role} Agent.");
                return false;
            }

            ActivateWindow(windowSearchTitle);
            Thread.Sleep(1000);

            // Ctrl + 1
            keybd_event(VK_CONTROL, 0, 0, 0);
            keybd_event(VK_1, 0, 0, 0);
            keybd_event(VK_1, 0, KEYEVENTF_KEYUP, 0);
            keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, 0);
            Thread.Sleep(300);

            // Ctrl + L
            keybd_event(VK_CONTROL, 0, 0, 0);
            keybd_event(VK_L, 0, 0, 0);
            keybd_event(VK_L, 0, KEYEVENTF_KEYUP, 0);
            keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, 0);
            Thread.Sleep(300);

            // Type "/monitor-issues"
            TypeString("/monitor-issues");
            Thread.Sleep(200);

            // Enter twice
            keybd_event(VK_RETURN, 0, 0, 0);
            keybd_event(VK_RETURN, 0, KEYEVENTF_KEYUP, 0);
            Thread.Sleep(200);
            keybd_event(VK_RETURN, 0, 0, 0);
            keybd_event(VK_RETURN, 0, KEYEVENTF_KEYUP, 0);
            
            Console.WriteLine($"[Trigger] Automation commands sent to {role} Agent window.");
            return true;
        }

        private static void TypeString(string text)
        {
            foreach (char c in text)
            {
                SendChar(c);
                Thread.Sleep(20);
            }
        }

        private static void SendChar(char c)
        {
            bool shift = false;
            byte vk = 0;

            if (char.IsLetter(c))
            {
                vk = (byte)char.ToUpper(c);
                if (char.IsUpper(c)) shift = true;
            }
            else if (char.IsDigit(c))
            {
                vk = (byte)c;
            }
            else
            {
                switch (c)
                {
                    case '/': vk = VK_OEM_2; break;
                    case '-': vk = VK_OEM_MINUS; break;
                    case '@': vk = VK_2; shift = true; break;
                    case ' ': vk = 0x20; break;
                }
            }

            if (vk == 0) return;

            if (shift) keybd_event(VK_SHIFT, 0, 0, 0);
            keybd_event(vk, 0, 0, 0);
            keybd_event(vk, 0, KEYEVENTF_KEYUP, 0);
            if (shift) keybd_event(VK_SHIFT, 0, KEYEVENTF_KEYUP, 0);
        }

        private static void SetupAllAgents()
        {
            string[] agents = { "Leader", "Developer", "Tester", "Planner" };
            string antigravityPath = "antigravity";
            string workspacePath = "c:\\Workspace\\AntiCorp";

            Console.WriteLine("AntiCorp System Orchestrator - Starting Setup Sequence...");
            Console.WriteLine("---------------------------------------------------------");

            foreach (var agent in agents)
            {
                string profileName = $"AntiCorp-{agent}";
                string windowSearchTitle = agent; 

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
                            Arguments = $"--user-data-dir {workspacePath}\\Profiles\\{profileName} {workspacePath}\\{agent}",
                            UseShellExecute = true
                        };
                        System.Diagnostics.Process.Start(startInfo);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"  - Error starting {agent}: {ex.Message}");
                        continue;
                    }

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
                    TriggerAgent(agent);
                }
                else
                {
                    Console.WriteLine($"  - Error: Timed out waiting for {agent} window.");
                }

                Thread.Sleep(1000);
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
            Thread.Sleep(500); 

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

        private static void ParseMonitorArgs(string[] args)
        {
            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i].ToLower())
                {
                    case "--repo":
                    case "-r":
                        if (i + 1 < args.Length)
                        {
                            var repo = args[++i];
                            var parts = repo.Split('/');
                            if (parts.Length == 2)
                            {
                                _repoOwner = parts[0];
                                _repoName = parts[1];
                            }
                        }
                        break;
                    case "--token":
                    case "-t":
                        if (i + 1 < args.Length) _githubToken = args[++i];
                        break;
                }
            }

            if (string.IsNullOrEmpty(_githubToken))
                _githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
        }

        private static void PrintUsage()
        {
            Console.WriteLine("AntiCorp Unified Automation Tool");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("  WindowAutomation <command> [arguments]");
            Console.WriteLine();
            Console.WriteLine("Commands:");
            Console.WriteLine("  monitor --repo <owner/repo>          Central polling loop for issues");
            Console.WriteLine("  check --repo <repo> --labels <l1,..> Single-pass check with label transition (@notified -> @working)");
            Console.WriteLine("  setup                                Launch and initialize all 4 agents");
            Console.WriteLine("  activate <window_title>              Activate window by partial title match");
            Console.WriteLine("  list                                 List all visible windows");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  WindowAutomation monitor --repo yj7-park/AntiCorp");
            Console.WriteLine("  WindowAutomation check --repo yj7-park/AntiCorp --labels \"@leader,@all\"");
        }
    }
}
