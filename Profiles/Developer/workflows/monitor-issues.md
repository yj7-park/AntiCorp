---
description: GitHub Issue를 모니터링하고 새 Issue에 응답
---

# Issue 모니터링 워크플로우

이 워크플로우는 자동으로 GitHub Issue를 모니터링하고, 자신에게 할당된 label이 있는 Issue를 감지하여 처리합니다.

## 실행 단계

// turbo-all

### 1. IssueMonitor 프로그램 빌드 (최초 1회)

처음 실행하는 경우, IssueMonitor를 빌드합니다:

```powershell
cd c:\Workspace\AntiCorp\Tools\IssueMonitor
dotnet build --configuration Release
```

### 2. 환경 변수 설정

GitHub Personal Access Token을 환경 변수로 설정:

```powershell
$env:GITHUB_TOKEN = "ghp_your_token_here"
```

또는 시스템 환경 변수로 영구 설정하세요.

### 3. IssueMonitor 실행

자신의 역할에 맞는 label로 모니터링 시작:

**Leader Agent:**
```powershell
c:\Workspace\AntiCorp\Tools\IssueMonitor\bin\Release\net8.0\IssueMonitor.exe --repo yj7-park/AntiCorp --labels "@leader,@all,@new-project" --interval 10
```

**Developer Agent:**
```powershell
c:\Workspace\AntiCorp\Tools\IssueMonitor\bin\Release\net8.0\IssueMonitor.exe --repo yj7-park/AntiCorp --labels "@developer,@all" --interval 10
```

**Tester Agent:**
```powershell
c:\Workspace\AntiCorp\Tools\IssueMonitor\bin\Release\net8.0\IssueMonitor.exe --repo yj7-park/AntiCorp --labels "@tester,@all" --interval 10
```

**DevOps Agent:**
```powershell
c:\Workspace\AntiCorp\Tools\IssueMonitor\bin\Release\net8.0\IssueMonitor.exe --repo yj7-park/AntiCorp --labels "@devops,@all" --interval 10
```

### 4. stdout 출력 대기

프로그램이 새 Issue를 감지하면 다음과 같은 형식으로 출력합니다:

```
---
[NEW ISSUE] #123
Title: 새로운 기능 요청
Labels: @developer
Created: 2026-02-03T10:30:00Z
URL: https://github.com/yj7-park/AntiCorp/issues/123
Body:
React 기반 대시보드를 구현해주세요.
---
```

### 5. Issue 내용 분석

출력된 Issue를 읽고:
- Title과 Body에서 요구사항 파악
- Labels를 확인하여 우선순위 판단
- 필요한 정보가 부족하면 추가 질문

### 6. 작업 처리

Issue 내용에 따라 적절한 작업 수행:
- **@new-project** (Leader만): 새 프로젝트 생성 프로세스 실행
- **@leader**: 작업 분배 또는 프로젝트 관리
- **@developer**: 코드 구현
- **@tester**: 테스트 작성 및 실행
- **@devops**: 빌드/배포 설정
- **@all**: 전체 공지 확인

### 7. 작업 완료 후

작업을 완료하면:
1. 결과를 GitHub Issue로 보고
2. 필요시 다른 Agent에게 작업 위임
3. 다시 모니터링 재개 (프로그램은 자동으로 계속 실행됨)

## 주의사항

> [!IMPORTANT]
> - IssueMonitor는 백그라운드에서 계속 실행되어야 합니다
> - 처리된 Issue는 자동으로 기록되어 중복 처리되지 않습니다
> - GitHub API rate limit을 고려하여 적절한 interval 설정 (기본 10초)

> [!TIP]
> - Antigravity의 장시간 대기 설정을 활용하여 프로그램이 출력할 때까지 기다리세요
> - 여러 Issue가 한번에 출력될 수 있으니 모두 확인하세요
