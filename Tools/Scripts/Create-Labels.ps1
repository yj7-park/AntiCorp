param(
    [string]$Repo = "yj7-park/AntiCorp"
)

# Set terminal encoding to UTF-8
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$OutputEncoding = [System.Text.Encoding]::UTF8

# Check GitHub CLI
if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
    Write-Error "GitHub CLI (gh) Not Found."
    exit 1
}

# Ensure GITHUB_TOKEN is set
$token = [System.Environment]::GetEnvironmentVariable('GITHUB_TOKEN', 'User')
if (-not $token) { $token = [System.Environment]::GetEnvironmentVariable('GITHUB_TOKEN', 'Machine') }
if ($token) { $env:GITHUB_TOKEN = $token }

Write-Host "Setting up AntiCorp Labels for $Repo..." -ForegroundColor Cyan

$labels = @(
    @{ name = "@leader"; color = "d73a4a"; desc = "Messages for the Leader Agent" },
    @{ name = "@developer"; color = "0075ca"; desc = "Messages for the Developer Agent" },
    @{ name = "@tester"; color = "008672"; desc = "Messages for the Tester Agent" },
    @{ name = "@devops"; color = "e99695"; desc = "Messages for the DevOps Agent" },
    @{ name = "@all"; color = "fbca04"; desc = "Broadcast to all Agents" },
    @{ name = "@new-project"; color = "d93f0b"; desc = "Start New Project (Highest Priority for Leader)" }
)

foreach ($l in $labels) {
    Write-Host "Creating $($l.name)..." -NoNewline
    # Suppress errors if already exists, but we deleted them before.
    gh label create $l.name --repo $Repo --color $l.color --description $l.desc --force 2>$null
    if ($LASTEXITCODE -eq 0) {
        Write-Host " OK" -ForegroundColor Green
    } else {
        # Check if already exists
        Write-Host " Done (Checked)" -ForegroundColor Gray
    }
}

Write-Host "`nLabels setup complete!" -ForegroundColor Green


