using Octokit;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace IssueMonitor
{
    class Program
    {
        private static readonly HashSet<int> ProcessedIssues = new();
        private static string? _githubToken;
        private static string? _repoOwner;
        private static string? _repoName;
        private static List<string> _labels = new();
        private static int _pollIntervalSeconds = 10;
        private static bool _monitorAll = false;
        private static bool _runOnce = false;

        // Windows API P/Invoke
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string? lpClassName, string? lpWindowName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        private const byte VK_CONTROL = 0x11;
        private const byte VK_L = 0x4C;
        private const byte VK_RETURN = 0x0D;
        private const uint KEYEVENTF_KEYUP = 0x0002;

        private static readonly Dictionary<string, string> RoleToLabel = new()
        {
            { "Leader", "@leader" },
            { "Developer", "@developer" },
            { "Tester", "@tester" },
            { "Planner", "@planner" }
        };

        static async Task Main(string[] args)
        {
            if (!ParseArguments(args))
            {
                PrintUsage();
                return;
            }

            Console.WriteLine($"[IssueMonitor] Repository: {_repoOwner}/{_repoName}");
            if (_monitorAll)
            {
                Console.WriteLine("[IssueMonitor] Mode: Centralized Monitor (All roles)");
            }
            else
            {
                Console.WriteLine($"[IssueMonitor] Watching labels: {string.Join(", ", _labels)}");
            }
            Console.WriteLine($"[IssueMonitor] Poll interval: {_pollIntervalSeconds}s");
            Console.WriteLine();

            var client = new GitHubClient(new ProductHeaderValue("AntiCorp-IssueMonitor"));
            
            if (!string.IsNullOrEmpty(_githubToken))
            {
                client.Credentials = new Credentials(_githubToken);
            }

            do
            {
                try
                {
                    await CheckForNewIssues(client);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"[ERROR] {ex.Message}");
                }

                if (!_runOnce)
                {
                    await Task.Delay(_pollIntervalSeconds * 1000);
                }
            } while (!_runOnce);
        }

        private static async Task CheckForNewIssues(GitHubClient client)
        {
            var issueRequest = new RepositoryIssueRequest
            {
                State = ItemStateFilter.Open,
                SortProperty = IssueSort.Created,
                SortDirection = SortDirection.Descending
            };

            var issues = await client.Issue.GetAllForRepository(_repoOwner, _repoName, issueRequest);

            foreach (var issue in issues)
            {
                if (ProcessedIssues.Contains(issue.Number))
                    continue;

                var issueLabels = issue.Labels.Select(l => l.Name).ToList();
                
                if (_monitorAll)
                {
                    // 중앙 모니터링: 각 역할별로 확인
                    foreach (var role in RoleToLabel.Keys)
                    {
                        if (issueLabels.Contains(RoleToLabel[role]) || issueLabels.Contains("@all"))
                        {
                            OutputIssue(issue);
                            TriggerAgent(role);
                        }
                    }
                    ProcessedIssues.Add(issue.Number);
                }
                else
                {
                    // 개별 에이전트 모드 (기존 호환성 유지)
                    bool hasMatchingLabel = _labels.Any(label => issueLabels.Contains(label));
                    if (hasMatchingLabel)
                    {
                        OutputIssue(issue);
                        ProcessedIssues.Add(issue.Number);
                    }
                }
            }
        }

        private static void TriggerAgent(string role)
        {
            string windowTitle = $"Antigravity - AntiCorp-{role}";
            Console.WriteLine($"[TRIGGER] Sending /monitor-issues to agent window: {windowTitle}");

            IntPtr hWnd = FindWindow(null, windowTitle);
            if (hWnd == IntPtr.Zero)
            {
                // 브라우저 탭이나 다른 타이틀 형식이 있을 수 있으므로 부분 일치 검색 탐색 (선택 사항)
                Console.WriteLine($"[WARNING] Could not find window: {windowTitle}");
                return;
            }

            SetForegroundWindow(hWnd);
            Thread.Sleep(500); // 윈도우 전환 대기

            // Ctrl+L
            keybd_event(VK_CONTROL, 0, 0, 0);
            keybd_event(VK_L, 0, 0, 0);
            keybd_event(VK_L, 0, KEYEVENTF_KEYUP, 0);
            keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, 0);

            Thread.Sleep(200);

            // Type "/monitor-issues"
            string command = "/monitor-issues";
            foreach (char c in command)
            {
                SendChar(c);
            }

            // Enter twice
            keybd_event(VK_RETURN, 0, 0, 0);
            keybd_event(VK_RETURN, 0, KEYEVENTF_KEYUP, 0);
            Thread.Sleep(100);
            keybd_event(VK_RETURN, 0, 0, 0);
            keybd_event(VK_RETURN, 0, KEYEVENTF_KEYUP, 0);
        }

        private static void SendChar(char c)
        {
            // 간단한 구현: 대문자/특수문자 처리는 생략 (여기서는 고정된 명령어만 사용하므로)
            // 실제 운영 환경에서는 SendKeys 등이 더 나을 수 있음
            short vk = (short)c;
            if (char.IsLetter(c)) vk = (short)char.ToUpper(c);
            if (c == '/') vk = 0xBF; // VK_DIVIDE or VK_OEM_2
            if (c == '-') vk = 0xBD; // VK_OEM_MINUS

            keybd_event((byte)vk, 0, 0, 0);
            keybd_event((byte)vk, 0, KEYEVENTF_KEYUP, 0);
        }

        private static void OutputIssue(Issue issue)
        {
            Console.WriteLine("---");
            Console.WriteLine($"[NEW ISSUE] #{issue.Number} - {issue.Title}");
            Console.WriteLine($"URL: {issue.HtmlUrl}");
            Console.WriteLine("---");
        }

        private static bool ParseArguments(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLower())
                {
                    case "--token":
                    case "-t":
                        if (i + 1 < args.Length) _githubToken = args[++i];
                        break;
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
                        if (i + 1 < args.Length)
                        {
                            var labels = args[++i];
                            _labels = labels.Split(',').Select(l => l.Trim()).ToList();
                        }
                        break;
                    case "--interval":
                    case "-i":
                        if (i + 1 < args.Length && int.TryParse(args[++i], out int interval))
                            _pollIntervalSeconds = interval;
                        break;
                    case "--all":
                    case "-a":
                        _monitorAll = true;
                        break;
                    case "--once":
                        _runOnce = true;
                        break;
                }
            }

            if (string.IsNullOrEmpty(_githubToken))
                _githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");

            return !string.IsNullOrEmpty(_repoOwner) && !string.IsNullOrEmpty(_repoName) && (_labels.Any() || _monitorAll);
        }

        private static void PrintUsage()
        {
            Console.WriteLine("AntiCorp Issue Monitor");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("  IssueMonitor --repo <owner/repo> --all [options]          (Central Monitor)");
            Console.WriteLine("  IssueMonitor --repo <owner/repo> --labels <l1,l2> [options] (Single Agent)");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  --all, -a        Monitor all AntiCorp roles and trigger windows");
            Console.WriteLine("  --once           Run once and exit");
            Console.WriteLine("  --repo, -r       Repository 'owner/repo'");
            Console.WriteLine("  --labels, -l     Labels for single agent mode");
            Console.WriteLine("  --token, -t      GitHub token");
            Console.WriteLine("  --interval, -i   Poll interval (default: 10)");
        }
    }
}
