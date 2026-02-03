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

.PARAMETER Agent
    실행할 Agent (Leader, Developer, Tester, DevOps)

.PARAMETER WorkspacePath
    Workspace 경로 (기본값: c:\Workspace\AntiCorp)

.PARAMETER AntigravityPath
    Antigravity 실행 파일 경로 (기본값: antigravity.exe)

.EXAMPLE
    .\Start-Agent.ps1 -Agent Leader
    
.EXAMPLE
    .\Start-Agent.ps1 -Agent Developer -WorkspacePath "d:\Projects\AntiCorp"
#>

$profileName = "AntiCorp-$Agent"

Write-Host "Starting AntiCorp $Agent Agent..." -ForegroundColor Cyan
Write-Host "  Profile: $profileName" -ForegroundColor White
Write-Host "  Workspace: $WorkspacePath" -ForegroundColor White
Write-Host ""

# Workspace 존재 확인
if (-not (Test-Path $WorkspacePath)) {
    Write-Warning "Workspace path does not exist: $WorkspacePath"
    $create = Read-Host "Create workspace directory? (y/n)"
    if ($create -eq 'y') {
        New-Item -Path $WorkspacePath -ItemType Directory -Force | Out-Null
        Write-Host "Created workspace directory" -ForegroundColor Green
    }
    else {
        Write-Error "Workspace directory required"
        exit 1
    }
}

# Antigravity 실행
try {
    $arguments = @(
        "--profile", $profileName,
        "--workspace", $WorkspacePath
    )
    
    Write-Host "Launching Antigravity..." -ForegroundColor Green
    Write-Host "Command: $AntigravityPath $($arguments -join ' ')" -ForegroundColor Gray
    Write-Host ""
    
    Start-Process -FilePath $AntigravityPath -ArgumentList $arguments
    
    Write-Host "Agent started successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Agent Information:" -ForegroundColor Yellow
    Write-Host "  Role: $Agent" -ForegroundColor White
    
    switch ($Agent) {
        "Leader" {
            Write-Host "  Monitoring: @leader, @all, @new-project" -ForegroundColor White
            Write-Host "  Responsibilities: Project management, task assignment, new project onboarding" -ForegroundColor Gray
        }
        "Developer" {
            Write-Host "  Monitoring: @developer, @all" -ForegroundColor White
            Write-Host "  Responsibilities: Code implementation, file creation/modification" -ForegroundColor Gray
        }
        "Tester" {
            Write-Host "  Monitoring: @tester, @all" -ForegroundColor White
            Write-Host "  Responsibilities: Test creation and execution, quality verification" -ForegroundColor Gray
        }
        "DevOps" {
            Write-Host "  Monitoring: @devops, @all" -ForegroundColor White
            Write-Host "  Responsibilities: Build, deployment, infrastructure management" -ForegroundColor Gray
        }
    }
}
catch {
    Write-Error "Failed to start agent: $_"
    exit 1
}
