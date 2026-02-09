<#
.SYNOPSIS
    AntiCorp 모든 에이전트 및 중앙 모니터링 가동을 위한 홈 바로가기 스크립트.
#>

$scriptPath = Join-Path $PSScriptRoot "Tools\Scripts\Start-All-Agents.ps1"

if (Test-Path $scriptPath) {
    Write-Host "AntiCorp 가동 시퀀스를 시작합니다..." -ForegroundColor Cyan
    & $scriptPath
}
else {
    Write-Host "[ERROR] 시작 스크립트를 찾을 수 없습니다: $scriptPath" -ForegroundColor Red
}
