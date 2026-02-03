# AntiCorp 초기 설정 가이드

4대의 Antigravity 클라이언트를 설정하여 AntiCorp 시스템을 시작하는 가이드입니다.

## 1단계: 필수 요구사항 설치
- **.NET 8.0 SDK**: `winget install Microsoft.DotNet.SDK.8`
- **GitHub CLI**: `winget install GitHub.cli`

## 2단계: 도구 빌드
```powershell
# IssueMonitor 빌드
cd c:\Workspace\AntiCorp\Tools\IssueMonitor
dotnet build --configuration Release

# WindowAutomation 빌드
cd ..\WindowAutomation
dotnet build --configuration Release
```

## 3단계: GitHub 인증 및 토큰 설정
1. `gh auth login`으로 로그인
2. [GitHub Personal Access Token](https://github.com/settings/tokens) 생성 (repo 권한)
3. 시스템 환경 변수 `GITHUB_TOKEN`에 토큰 값 등록

## 4단계: 레이블 및 Profile 설정
```powershell
# GitHub 레이블 생성
.\Tools\Scripts\Create-Labels.ps1

# Antigravity Profile 자동 설정
.\Tools\Scripts\Setup-Profiles.ps1
```

## 5단계: Agent 실행 및 테스트
1. 각 Agent 실행: `.\Tools\Scripts\Start-Agent.ps1 -Agent Leader` 등
2. Antigravity에서 `/monitor-issues` 입력하여 모니터링 시작


