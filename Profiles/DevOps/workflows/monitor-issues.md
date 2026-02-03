---
description: GitHub Issue를 모니터링하고 담당 작업을 발견하면 대응
---

# Issue 모니터링 워크플로우

모든 에이전트가 공통으로 사용하여 자신에게 할당된 새로운 이슈를 찾습니다.

## 실행 단계

// turbo-all

### 1. 도구 빌드 (최초 1회)

```powershell
cd c:\Workspace\AntiCorp\Tools\IssueMonitor
dotnet build --configuration Release
```

### 2. 모니터링 실행

자신의 역할에 맞는 라벨을 모니터링합니다.

**Leader인 경우:**
```powershell
..\bin\Release\net8.0\IssueMonitor.exe --repo yj7-park/AntiCorp --labels "@leader,@all,@new-project"
```

**Developer인 경우:**
```powershell
..\bin\Release\net8.0\IssueMonitor.exe --repo yj7-park/AntiCorp --labels "@developer,@all"
```

**Tester인 경우:**
```powershell
..\bin\Release\net8.0\IssueMonitor.exe --repo yj7-park/AntiCorp --labels "@tester,@all"
```

**DevOps인 경우:**
```powershell
..\bin\Release\net8.0\IssueMonitor.exe --repo yj7-park/AntiCorp --labels "@devops,@all"
```

### 3. 이슈 감지 및 대응

새로운 이슈가 콘솔에 출력되면:
1. 이슈 내용을 분석합니다.
2. 할당된 작업이 `@new-project`라면 `create-project.md` 워크플로우를 실행합니다.
3. 그 외의 작업은 `respond-to-issue.md` 워크플로우를 실행합니다.

## 주의사항

> [!NOTE]
> - 이 프로그램은 종료할 때까지 계속해서 이슈를 체크합니다.
> - 새로운 이슈가 발견되면 즉시 알림(stdout)을 줍니다.

