---
description: GitHub Issue를 모니터링하고 담당 작업을 발견하면 대응
---

# Planner Issue 모니터링

## 역할 정의

**Planner**는 기획자/설계자입니다.
- 기능 요구사항 분석 및 설계
- 문서 작성 및 관리 (GitHub Wiki, README 등)
- 추가 기능 제안 및 아키텍처 고민
- 기술적 의사결정 지원

## 모니터링 대상 Label

| Label | 의미 |
|-------|------|
| `@planner` | Planner에게 직접 할당된 작업 |
| `@all` | 전체 공지 |

## 실행 단계

// turbo-all

### 1. 이슈 확인

```powershell
c:\Workspace\AntiCorp\Tools\WindowAutomation\bin\Release\net8.0\WindowAutomation.exe check --repo yj7-park/AntiCorp --labels "@planner,@all"
```

### 2. 이슈 분석 및 대응

출력된 이슈 내용을 분석하여 적절한 workflow를 실행합니다:

| 이슈 유형 | 실행할 Workflow |
|----------|----------------|
| 설계 요청 | `/plan-feature` |
| 문서 작성 요청 | `/document` |
| Clone 요청 | 직접 clone 실행 |

### 3. 작업 시작 선언

이슈를 수락하고 작업을 시작하면:

```powershell
gh issue edit <이슈번호> --repo yj7-park/AntiCorp --add-label "@working"
gh issue comment <이슈번호> --repo yj7-park/AntiCorp --body "📋 Planner 작업 시작"
```
