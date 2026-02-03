param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("Leader", "Developer", "Tester", "DevOps")]
    [string]$Agent,
    
    [string]$WorkspacePath = "c:\Workspace\AntiCorp",
    
    [string]$AntigravityPath = "antigravity.exe"
)

<#
.SYNOPSIS
    특정 Agent의 Antigravity 클라이언트를 실행하는 스크립트
.DESCRIPTION
    각 Agent의 profile을 사용하여 Antigravity 클라이언트를 실행합니다.
#>

$profileName = "AntiCorp-$Agent"

Write-Host "Starting AntiCorp $Agent Agent..." -ForegroundColor Cyan
Write-Host "  Profile: $profileName" -ForegroundColor White
Write-Host "  Workspace: $WorkspacePath" -ForegroundColor White
Write-Host ""

# Workspace 존재 확인
if (-not (Test-Path $WorkspacePath)) {
    Write-Host "Creating workspace directory: $WorkspacePath" -ForegroundColor Yellow
    New-Item -Path $WorkspacePath -ItemType Directory -Force | Out-Null
}

# Antigravity 실행
try {
    $arguments = @(
        "--profile", $profileName,
        "--workspace", $WorkspacePath
    )
    
    Write-Host "Launching Antigravity..." -ForegroundColor Green
    Write-Host "Command: $AntigravityPath $($arguments -join ' ')" -ForegroundColor Gray
    
    Start-Process -FilePath $AntigravityPath -ArgumentList $arguments
    
    Write-Host "Agent started successfully!" -ForegroundColor Green
}
catch {
    Write-Error "Failed to start agent: $_"
    exit 1
}

