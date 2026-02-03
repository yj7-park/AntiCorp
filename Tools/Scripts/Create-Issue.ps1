param(
    [Parameter(Mandatory=$true)]
    [string]$Title,
    
    [Parameter(Mandatory=$true)]
    [string]$Body,
    
    [Parameter(Mandatory=$true)]
    [string[]]$Labels,
    
    [string]$Repo = "yj7-park/AntiCorp"
)

<#
.SYNOPSIS
    GitHub Issue를 생성하는 스크립트

.DESCRIPTION
    GitHub CLI를 사용하여 Issue를 생성하고 지정된 label을 태그합니다.

.PARAMETER Title
    Issue 제목

.PARAMETER Body
    Issue 본문

.PARAMETER Labels
    태그할 label 목록 (배열)

.PARAMETER Repo
    Repository (기본값: yj7-park/AntiCorp)

.EXAMPLE
    .\Create-Issue.ps1 -Title "새로운 기능 요청" -Body "상세 설명" -Labels "@developer","@all"
#>

# GitHub CLI 설치 확인
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

Write-Host "Creating issue in repository: $Repo" -ForegroundColor Cyan
Write-Host "Title: $Title" -ForegroundColor White
Write-Host "Labels: $($Labels -join ', ')" -ForegroundColor Yellow

# Issue 생성
$labelArgs = $Labels | ForEach-Object { "--label", $_ }

try {
    $issueUrl = gh issue create `
        --repo $Repo `
        --title $Title `
        --body $Body `
        @labelArgs

    Write-Host "`nIssue created successfully!" -ForegroundColor Green
    Write-Host "URL: $issueUrl" -ForegroundColor Cyan
}
catch {
    Write-Error "Failed to create issue: $_"
    exit 1
}
