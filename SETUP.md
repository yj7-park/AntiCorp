# AntiCorp 초기 설정 가이드

4대의 Antigravity 클라이언트를 설정하여 AntiCorp 시스템을 시작하는 가이드입니다.

## 사전 준비

### 필수 소프트웨어
- [x] Windows 10/11
- [x] [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- [x] PowerShell 5.1 이상
- [x] [GitHub CLI](https://cli.github.com/)
- [x] Antigravity

### GitHub 설정
1. GitHub Personal Access Token 생성
   - https://github.com/settings/tokens
   - Scope: `repo` (전체 접근)
   - Token 복사

2. 환경 변수 설정
   ```powershell
   [System.Environment]::SetEnvironmentVariable('GITHUB_TOKEN', 'ghp_your_token_here', 'User')
   ```

## 1단계: 도구 빌드

### IssueMonitor 빌드
```powershell
cd c:\Workspace\AntiCorp\Tools\IssueMonitor
dotnet restore
dotnet build --configuration Release
```

### WindowAutomation 빌드
```powershell
cd c:\Workspace\AntiCorp\Tools\WindowAutomation
dotnet restore
dotnet build --configuration Release
```

### 빌드 확인
```powershell
# IssueMonitor 테스트
c:\Workspace\AntiCorp\Tools\IssueMonitor\bin\Release\net8.0\IssueMonitor.exe

# WindowAutomation 테스트
c:\Workspace\AntiCorp\Tools\WindowAutomation\bin\Release\net8.0-windows\WindowAutomation.exe
```

## 2단계: Google 계정 준비

4개의 Google 계정을 준비하세요 (권장):
- `leader@yourdomain.com` (또는 개인 계정)
- `developer@yourdomain.com`
- `tester@yourdomain.com`
- `devops@yourdomain.com`

> [!NOTE]
> 하나의 Google 계정으로 테스트할 수도 있지만, 별도 계정 사용을 권장합니다.

## 3단계: Antigravity Profile 생성 및 설정

### 3-1. Leader Agent 설정

```powershell
# Leader Agent 실행
antigravity.exe --profile "AntiCorp-Leader" --workspace "c:\Workspace\AntiCorp"

# 또는 스크립트 사용
.\Tools\Scripts\Start-Agent.ps1 -Agent Leader
```

Antigravity가 실행되면:
1. Leader Google 계정으로 로그인
2. Profile이 생성됨: `%APPDATA%\Antigravity\Profiles\AntiCorp-Leader`

Global rules 복사:
```powershell
# Profile 디렉토리 생성
New-Item -Path "$env:APPDATA\Antigravity\Profiles\AntiCorp-Leader\.agent\workflows" -ItemType Directory -Force

# Global rules 복사
Copy-Item "c:\Workspace\AntiCorp\Profiles\Leader\global.rules.md" "$env:APPDATA\Antigravity\Profiles\AntiCorp-Leader\.agent\" -Force

# Workflows 복사
Copy-Item "c:\Workspace\AntiCorp\Profiles\Leader\workflows\*" "$env:APPDATA\Antigravity\Profiles\AntiCorp-Leader\.agent\workflows\" -Force
```

### 3-2. Developer Agent 설정

```powershell
# Developer Agent 실행
antigravity.exe --profile "AntiCorp-Developer" --workspace "c:\Workspace\AntiCorp"
```

Developer Google 계정으로 로그인 후:
```powershell
New-Item -Path "$env:APPDATA\Antigravity\Profiles\AntiCorp-Developer\.agent\workflows" -ItemType Directory -Force
Copy-Item "c:\Workspace\AntiCorp\Profiles\Developer\global.rules.md" "$env:APPDATA\Antigravity\Profiles\AntiCorp-Developer\.agent\" -Force
Copy-Item "c:\Workspace\AntiCorp\Profiles\Developer\workflows\*" "$env:APPDATA\Antigravity\Profiles\AntiCorp-Developer\.agent\workflows\" -Force
```

### 3-3. Tester Agent 설정

```powershell
# Tester Agent 실행
antigravity.exe --profile "AntiCorp-Tester" --workspace "c:\Workspace\AntiCorp"
```

Tester Google 계정으로 로그인 후:
```powershell
New-Item -Path "$env:APPDATA\Antigravity\Profiles\AntiCorp-Tester\.agent\workflows" -ItemType Directory -Force
Copy-Item "c:\Workspace\AntiCorp\Profiles\Tester\global.rules.md" "$env:APPDATA\Antigravity\Profiles\AntiCorp-Tester\.agent\" -Force
Copy-Item "c:\Workspace\AntiCorp\Profiles\Tester\workflows\*" "$env:APPDATA\Antigravity\Profiles\AntiCorp-Tester\.agent\workflows\" -Force
```

### 3-4. DevOps Agent 설정

```powershell
# DevOps Agent 실행
antigravity.exe --profile "AntiCorp-DevOps" --workspace "c:\Workspace\AntiCorp"
```

DevOps Google 계정으로 로그인 후:
```powershell
New-Item -Path "$env:APPDATA\Antigravity\Profiles\AntiCorp-DevOps\.agent\workflows" -ItemType Directory -Force
Copy-Item "c:\Workspace\AntiCorp\Profiles\DevOps\global.rules.md" "$env:APPDATA\Antigravity\Profiles\AntiCorp-DevOps\.agent\" -Force
Copy-Item "c:\Workspace\AntiCorp\Profiles\DevOps\workflows\*" "$env:APPDATA\Antigravity\Profiles\AntiCorp-DevOps\.agent\workflows\" -Force
```

## 4단계: 동작 테스트

### 테스트 Issue 생성

```powershell
cd c:\Workspace\AntiCorp

# Leader에게 테스트 메시지
.\Tools\Scripts\Create-Issue.ps1 `
    -Title "[테스트] Leader에게 인사" `
    -Body "안녕하세요, Leader! 시스템이 정상 작동하는지 확인합니다." `
    -Labels "@leader"
```

### Leader Agent에서 모니터링

Leader Antigravity 창에서:
```
/monitor-issues
```

또는 직접 명령:
```powershell
c:\Workspace\AntiCorp\Tools\IssueMonitor\bin\Release\net8.0\IssueMonitor.exe --repo yj7-park/AntiCorp --labels "@leader,@all,@new-project" --interval 10
```

10초 이내에 Issue가 출력되면 성공!

## 5단계: 첫 프로젝트 시작

새 프로젝트로 시스템 테스트:

```powershell
.\Tools\Scripts\Create-Issue.ps1 `
    -Title "Hello World 프로젝트" `
    -Body @"
간단한 Hello World 웹 애플리케이션을 만들어주세요.

**요구사항:**
- index.html 파일
- "Hello, AntiCorp!" 메시지 표시
- 기본 스타일링

**기술 스택:**
- HTML/CSS/JavaScript
"@ `
    -Labels "@new-project"
```

Leader Agent가:
1. Issue 감지
2. 프로젝트 폴더 생성
3. 작업 분배

## 일상적인 사용

### 아침에 모든 Agent 시작

```powershell
# PowerShell 스크립트로 한번에 시작
.\Tools\Scripts\Start-Agent.ps1 -Agent Leader
.\Tools\Scripts\Start-Agent.ps1 -Agent Developer
.\Tools\Scripts\Start-Agent.ps1 -Agent Tester
.\Tools\Scripts\Start-Agent.ps1 -Agent DevOps
```

### 각 Agent에서 모니터링 시작

각 Antigravity 창에서:
```
/monitor-issues
```

### 작업 요청

```powershell
# 특정 Agent에게
.\Tools\Scripts\Create-Issue.ps1 -Title "..." -Body "..." -Labels "@developer"

# 전체에게
.\Tools\Scripts\Create-Issue.ps1 -Title "..." -Body "..." -Labels "@all"
```

## 문제 해결

### Issue가 감지되지 않음
- GitHub Token 확인: `$env:GITHUB_TOKEN`
- Repository 이름 확인: `yj7-park/AntiCorp`
- Label이 정확한지 확인: `@leader` 등

### 빌드 오류
```powershell
# 의존성 복원
dotnet restore

# Clean 후 재빌드
dotnet clean
dotnet build --configuration Release
```

### Profile이 인식되지 않음
- Global rules 파일이 올바른 위치에 있는지 확인
- Antigravity 재시작

## 다음 단계

설정이 완료되면:
1. README.md 참고하여 시스템 사용
2. 각 Agent의 global.rules.md에서 역할 확인
3. 실제 프로젝트로 테스트

즐거운 협업되세요! 🚀
