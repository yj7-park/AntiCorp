$profiles = @("Leader", "Developer", "Tester", "DevOps")
$appData = [System.Environment]::GetFolderPath('ApplicationData')
$baseTargetDir = Join-Path $appData "Antigravity\Profiles"
$baseSourceDir = "c:\Workspace\AntiCorp\Profiles"

foreach ($agent in $profiles) {
    $profileName = "AntiCorp-$agent"
    $targetDir = Join-Path $baseTargetDir $profileName
    $agentDir = Join-Path $targetDir ".agent"
    $workflowDir = Join-Path $agentDir "workflows"
    
    Write-Host "Setting up Profile for $agent..." -ForegroundColor Cyan
    
    # ?붾젆?좊━ ?앹꽦
    if (-not (Test-Path $workflowDir)) {
        New-Item -Path $workflowDir -ItemType Directory -Force | Out-Null
    }
    
    # Global Rules 蹂듭궗
    $sourceRules = Join-Path $baseSourceDir "$agent\global.rules.md"
    if (Test-Path $sourceRules) {
        Copy-Item -Path $sourceRules -Destination $agentDir -Force
        Write-Host "  Copying global.rules.md to $agentDir" -ForegroundColor Green
    }
    
    # Workflows 蹂듭궗
    $sourceWorkflows = Join-Path $baseSourceDir "$agent\workflows\*"
    if (Test-Path (Join-Path $baseSourceDir "$agent\workflows")) {
        Copy-Item -Path $sourceWorkflows -Destination $workflowDir -Force
        Write-Host "  Copying workflows to $workflowDir" -ForegroundColor Green
    }
}

Write-Host "`nAll Profiles have been set up successfully!" -ForegroundColor Green



