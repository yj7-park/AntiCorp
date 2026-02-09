---
description: GitHub Issue를 모니터링하고 담당 작업을 발견하면 대응
---

# Leader Issue 모니터링

## 역할 정의

**Leader**는 프로젝트 총괄자입니다.
- 프로젝트 전체 관리 및 Sprint 조율
- 작업 분배 및 진행 관리
- 최종 의사결정 및 완료 승인

## 모니터링 대상 Label

| Label | 의미 |
|-------|------|
| `@leader` | Leader에게 직접 할당된 작업 |
| `@all` | 전체 공지 |
| `@new-project` | 새 프로젝트 요청 |
| `@review` | 검토 요청 |

## 실행 단계

> [!IMPORTANT]
> **강제 실행 규칙**: `@new-project` 라벨이 붙은 이슈를 발견하면, **즉시** `/create-project` 워크플로우를 실행하세요. 절대 이슈를 먼저 닫거나 건너뛰지 마세요.

// turbo-all

### 1. 이슈 확인

```powershell
c:\Workspace\AntiCorp\Tools\WindowAutomation\bin\Release\net8.0-windows\WindowAutomation.exe check --repo yj7-park/AntiCorp --labels "@leader,@all,@new-project,@review"
```

### 2. 이슈 분석 및 대응

| Label | 실행할 Workflow | 우선순위 |
|-------|----------------|---------|
| `@new-project` | `/create-project` (Sprint 1 시작) | **최상** (즉시 실행) |
| `@review` | `/manage-project` (Sprint 검토/전환) | 높음 |
| 기타 | 이슈 내용에 따라 판단 | 중간 |

### 3. Sprint 관리 원칙

**Agile 10 Sprint 사이클:**
1. Sprint 시작: Planner에게 설계 요청
2. 설계 완료 후: Developer에게 구현 요청
3. 구현 완료 후: Tester 테스트 결과 확인
4. Sprint 종료: 다음 Sprint 계획 또는 릴리즈

### 4. 이슈 템플릿 사용

새 이슈 발급 시 `.github/ISSUE_TEMPLATE/` 의 템플릿을 참고:
- `design_request.md` - Planner 요청
- `implement_request.md` - Developer 요청
- `test_request.md` - Tester 요청
- `sprint_complete.md` - Sprint 완료 보고