using Octokit;
using Newtonsoft.Json;

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

        static async Task Main(string[] args)
        {
            if (!ParseArguments(args))
            {
                PrintUsage();
                return;
            }

            Console.WriteLine($"[IssueMonitor] Monitoring repository: {_repoOwner}/{_repoName}");
            Console.WriteLine($"[IssueMonitor] Watching labels: {string.Join(", ", _labels)}");
            Console.WriteLine($"[IssueMonitor] Poll interval: {_pollIntervalSeconds}s");
            Console.WriteLine();

            var client = new GitHubClient(new ProductHeaderValue("AntiCorp-IssueMonitor"));
            
            if (!string.IsNullOrEmpty(_githubToken))
            {
                client.Credentials = new Credentials(_githubToken);
            }

            while (true)
            {
                try
                {
                    await CheckForNewIssues(client);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"[ERROR] {ex.Message}");
                }

                await Task.Delay(_pollIntervalSeconds * 1000);
            }
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
                // 이미 처리한 issue는 스킵
                if (ProcessedIssues.Contains(issue.Number))
                    continue;

                // label이 일치하는지 확인
                var issueLabels = issue.Labels.Select(l => l.Name).ToList();
                bool hasMatchingLabel = _labels.Any(label => issueLabels.Contains(label));

                if (hasMatchingLabel)
                {
                    // 새로운 issue 발견 - stdout으로 출력
                    OutputIssue(issue);
                    ProcessedIssues.Add(issue.Number);
                }
            }
        }

        private static void OutputIssue(Issue issue)
        {
            Console.WriteLine("---");
            Console.WriteLine($"[NEW ISSUE] #{issue.Number}");
            Console.WriteLine($"Title: {issue.Title}");
            Console.WriteLine($"Labels: {string.Join(", ", issue.Labels.Select(l => l.Name))}");
            Console.WriteLine($"Created: {issue.CreatedAt}");
            Console.WriteLine($"URL: {issue.HtmlUrl}");
            Console.WriteLine("Body:");
            Console.WriteLine(issue.Body);
            Console.WriteLine("---");
            Console.WriteLine();
        }

        private static bool ParseArguments(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLower())
                {
                    case "--token":
                    case "-t":
                        if (i + 1 < args.Length)
                            _githubToken = args[++i];
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
                        if (i + 1 < args.Length)
                        {
                            if (int.TryParse(args[++i], out int interval))
                                _pollIntervalSeconds = interval;
                        }
                        break;
                }
            }

            // GitHub Token은 환경변수에서도 읽기 가능
            if (string.IsNullOrEmpty(_githubToken))
            {
                _githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
            }

            return !string.IsNullOrEmpty(_repoOwner) && 
                   !string.IsNullOrEmpty(_repoName) && 
                   _labels.Any();
        }

        private static void PrintUsage()
        {
            Console.WriteLine("AntiCorp Issue Monitor");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("  IssueMonitor --repo <owner/repo> --labels <label1,label2,...> [options]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  --repo, -r       Repository in format 'owner/repo' (required)");
            Console.WriteLine("  --labels, -l     Comma-separated list of labels to monitor (required)");
            Console.WriteLine("  --token, -t      GitHub personal access token (optional, can use GITHUB_TOKEN env var)");
            Console.WriteLine("  --interval, -i   Poll interval in seconds (default: 10)");
            Console.WriteLine();
            Console.WriteLine("Example:");
            Console.WriteLine("  IssueMonitor --repo yj7-park/AntiCorp --labels \"@leader,@all\" --interval 10");
        }
    }
}
