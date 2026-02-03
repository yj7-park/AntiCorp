<#
.SYNOPSIS
    모든 AntiCorp 에이전트를 가동하고 윈도우 제어를 통해 모니터링 명령까지 자동 입력하는 통합 스크립트.
#>

$automationExe = "c:\Workspace\AntiCorp\Tools\WindowAutomation\bin\Release\net8.0-windows\WindowAutomation.exe"

Write-Host "AntiCorp 완전 자동화 가동 시스템을 시작합니다..." -ForegroundColor Cyan
Write-Host "---------------------------------------------------------"

if (-not (Test-Path $automationExe)) {
    Write-Host "정적 도구를 찾을 수 없습니다. 빌드를 먼저 수행하세요." -ForegroundColor Red
    exit 1
}

# WindowAutomation의 setup 명령어 호출
# 이 명령어 내부에서 루프를 돌며 에이전트 실행 및 키 입력을 처리합니다.
& $automationExe setup

Write-Host "---------------------------------------------------------"
Write-Host "모든 에이전트의 가동 시퀀스가 종료되었습니다. 🎉" -ForegroundColor Green
Write-Host "각 에이전트 창의 하단 입력창에 '/monitor-issues'가 입력되었는지 확인하세요." -ForegroundColor Gray


