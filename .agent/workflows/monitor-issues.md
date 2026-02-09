---
description: GitHub Issue를 확인하고 담당 작업을 발견하면 대응
---

# Issue 확인 워크플로우

중앙 모니터링 시스템(Central monitor)에 의해 트리거되거나, 개별적으로 실행하여 자신에게 할당된 새로운 이슈를 확인합니다.

## 실행 단계

### 1. 이슈 확인 및 상태 전환

자신의 역할에 맞는 라벨의 최신 이슈를 확인하고, 상태를 `@notified`에서 `@working`으로 자동 전환합니다. 이 단계는 통합 도구인 `WindowAutomation.exe`를 사용합니다.

**Leader인 경우:**
```powershell
..\Tools\WindowAutomation\bin\Release\net8.0-windows\WindowAutomation.exe check --repo yj7-park/AntiCorp --labels "@leader,@all,@new-project"
```

**Developer인 경우:**
```powershell
..\Tools\WindowAutomation\bin\Release\net8.0-windows\WindowAutomation.exe check --repo yj7-park/AntiCorp --labels "@developer,@all"
```

**Tester인 경우:**
```powershell
..\Tools\WindowAutomation\bin\Release\net8.0-windows\WindowAutomation.exe check --repo yj7-park/AntiCorp --labels "@tester,@all"
```

**DevOps인 경우:**
```powershell
..\Tools\WindowAutomation\bin\Release\net8.0-windows\WindowAutomation.exe check --repo yj7-park/AntiCorp --labels "@devops,@all"
```

### 2. 이슈 대응

이슈가 발견되면:
1. 이슈 내용을 분석합니다.
2. 할당된 작업이 `@new-project`라면 `create-project.md` 워크플로우를 실행합니다.
3. 그 외의 작업은 `respond-to-issue.md` 워크플로우를 실행합니다.

## 주의사항

> [!NOTE]
> - 이 워크플로우는 한 번 실행하고 종료됩니다. 
> - 중앙 모니터링 시스템이 새로운 이슈를 발견하면 자동으로 이 워크플로우를 트리거합니다.