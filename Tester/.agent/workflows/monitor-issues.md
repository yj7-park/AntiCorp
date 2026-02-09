---
description: GitHub Issue를 모니터링하고 담당 작업을 발견하면 대응
---

# Tester Issue 모니터링

## 역할 정의

**Tester**는 품질 보증 담당자입니다.
- 테스트 케이스 작성
- 기능 검증 및 버그 리포트
- 품질 기준 충족 확인

## 모니터링 대상 Label

| Label | 의미 |
|-------|------|
| `@tester` | Tester에게 직접 할당된 작업 |
| `@all` | 전체 공지 |

## 실행 단계

> [!IMPORTANT]
> **강제 실행 규칙**: `@tester` 라벨이 붙은 이슈(특히 기능 테스트 요청)를 발견하면, **즉시** `/verify-feature` 워크플로우를 실행하세요. 절대 무시하거나 건너뛰지 마세요.

// turbo-all

### 1. 이슈 확인

```powershell
c:\Workspace\AntiCorp\Tools\WindowAutomation\bin\Release\net8.0-windows\WindowAutomation.exe check --repo yj7-park/AntiCorp --labels "@tester,@all"
```

### 2. 이슈 분석 및 대응

출력된 이슈 내용을 분석하여 적절한 workflow를 실행합니다:

| 이슈 유형 | 실행할 Workflow | 우선순위 |
|----------|----------------|---------|
| 기능 테스트 요청 | `/verify-feature` | **최상** (즉시 실행) |
| 테스트 케이스 작성 요청 | `/write-tests` | 높음 |
| Clone 요청 | 직접 clone 실행 | 중간 |

### 3. 작업 시작 선언

이슈를 수락하고 작업을 시작하면:

```powershell
gh issue edit <이슈번호> --repo yj7-park/AntiCorp --add-label "@working"
gh issue comment <이슈번호> --repo yj7-park/AntiCorp --body "🧪 Tester 작업 시작"
```