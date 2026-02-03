#!/usr/bin/env pwsh
<#
.SYNOPSIS
    AntiCorp 시스템에 필요한 GitHub Labels를 생성하는 스크립트

.DESCRIPTION
    GitHub repository에 Agent 간 커뮤니케이션에 필요한 labels를 생성합니다.

.PARAMETER Repo
    GitHub repository (기본값: yj7-park/AntiCorp)

.EXAMPLE
    .\Create-Labels.ps1
    
.EXAMPLE
    .\Create-Labels.ps1 -Repo "owner/repo"
#>

param(
    [string]$Repo = "yj7-park/AntiCorp"
)

# GitHub CLI 확인
if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
    Write-Error "GitHub CLI (gh)가 설치되어 있지 않습니다. https://cli.github.com/ 에서 설치하세요."
    exit 1
}

# GitHub 인증 확인
$authStatus = gh auth status 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Error "GitHub CLI 인증이 필요합니다. 'gh auth login' 명령을 실행하세요."
    exit 1
}

Write-Host "AntiCorp GitHub Labels 생성" -ForegroundColor Cyan
Write-Host "Repository: $Repo" -ForegroundColor White
Write-Host ""

# Label 정의 (name, color, description)
$labels = @(
    @{
        name = "@leader"
        color = "d73a4a"  # Red
        description = "Leader Agent에게 전달되는 메시지"
    },
    @{
        name = "@developer"
        color = "0075ca"  # Blue
        description = "Developer Agent에게 전달되는 메시지"
    },
    @{
        name = "@tester"
        color = "008672"  # Green
        description = "Tester Agent에게 전달되는 메시지"
    },
    @{
        name = "@devops"
        color = "e99695"  # Light Red
        description = "DevOps Agent에게 전달되는 메시지"
    },
    @{
        name = "@all"
        color = "fbca04"  # Yellow
        description = "모든 Agent에게 전달되는 전체 공지"
    },
    @{
        name = "@new-project"
        color = "d93f0b"  # Orange
        description = "새 프로젝트 수주 (Leader 최우선 처리)"
    }
)

$successCount = 0
$failCount = 0

foreach ($label in $labels) {
    Write-Host "Creating label: $($label.name)..." -NoNewline
    
    try {
        $result = gh label create $label.name `
            --repo $Repo `
            --color $label.color `
            --description $label.description `
            2>&1
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host " ✓" -ForegroundColor Green
            $successCount++
        }
        else {
            # Label이 이미 존재하는 경우
            if ($result -like "*already exists*") {
                Write-Host " (이미 존재함)" -ForegroundColor Gray
                $successCount++
            }
            else {
                Write-Host " ✗" -ForegroundColor Red
                Write-Host "  Error: $result" -ForegroundColor Red
                $failCount++
            }
        }
    }
    catch {
        Write-Host " ✗" -ForegroundColor Red
        Write-Host "  Error: $_" -ForegroundColor Red
        $failCount++
    }
}

Write-Host ""
Write-Host "완료: $successCount개 성공, $failCount개 실패" -ForegroundColor $(if ($failCount -eq 0) { "Green" } else { "Yellow" })

if ($failCount -eq 0) {
    Write-Host ""
    Write-Host "모든 labels가 생성되었습니다! 🎉" -ForegroundColor Green
    Write-Host "GitHub repository에서 확인하세요: https://github.com/$Repo/labels" -ForegroundColor Cyan
}
