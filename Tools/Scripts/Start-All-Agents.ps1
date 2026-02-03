<#
.SYNOPSIS
    紐⑤뱺 AntiCorp ?먯씠?꾪듃瑜???踰덉뿉 ?ㅽ뻾?섎뒗 ?ㅽ겕由쏀듃.
#>

$agents = @("Leader", "Developer", "Tester", "DevOps")
$scriptPath = Join-Path $PSScriptRoot "Start-Agent.ps1"

Write-Host "AntiCorp ?쒖뒪??媛?숈쓣 ?쒖옉?⑸땲??.." -ForegroundColor Cyan
Write-Host "-------------------------------------"

foreach ($agent in $agents) {
    Write-Host "[$agent Agent] 湲곕룞 以?.." -ForegroundColor Yellow
    & $scriptPath -Agent $agent
    Start-Sleep -Seconds 2 # ?ㅽ뻾 媛꾧꺽 議곗젅
}

Write-Host "-------------------------------------"
Write-Host "紐⑤뱺 ?먯씠?꾪듃媛 ?ㅽ뻾?섏뿀?듬땲?? ?럦" -ForegroundColor Green

