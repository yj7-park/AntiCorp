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
    GitHub Issue瑜??앹꽦?섎뒗 ?ㅽ겕由쏀듃

.DESCRIPTION
    GitHub CLI瑜??ъ슜?섏뿬 Issue瑜??앹꽦?섍퀬 吏?뺣맂 label???쒓렇?⑸땲??

.PARAMETER Title
    Issue ?쒕ぉ

.PARAMETER Body
    Issue 蹂몃Ц

.PARAMETER Labels
    ?쒓렇??label 紐⑸줉 (諛곗뿴)

.PARAMETER Repo
    Repository (湲곕낯媛? yj7-park/AntiCorp)

.EXAMPLE
    .\Create-Issue.ps1 -Title "?덈줈??湲곕뒫 ?붿껌" -Body "?곸꽭 ?ㅻ챸" -Labels "@developer","@all"
#>

# GitHub CLI ?ㅼ튂 ?뺤씤
if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
    Write-Error "GitHub CLI (gh)媛 ?ㅼ튂?섏뼱 ?덉? ?딆뒿?덈떎. https://cli.github.com/ ?먯꽌 ?ㅼ튂?섏꽭??"
    exit 1
}

# GitHub ?몄쬆 ?뺤씤
$authStatus = gh auth status 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Error "GitHub CLI ?몄쬆???꾩슂?⑸땲?? 'gh auth login' 紐낅졊???ㅽ뻾?섏꽭??"
    exit 1
}

Write-Host "Creating issue in repository: $Repo" -ForegroundColor Cyan
Write-Host "Title: $Title" -ForegroundColor White
Write-Host "Labels: $($Labels -join ', ')" -ForegroundColor Yellow

# Issue ?앹꽦
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


