---
description: Sprint 검토 및 다음 Sprint 관리
---

# Sprint 관리

## 역할 정의

**Leader**는 Sprint 관리자로서:
- Sprint 결과 검토 및 승인
- 다음 Sprint 시작
- 병목 해소 및 조율

## 실행 시점

- `@review` 라벨이 있는 이슈 발견 시
- Sprint 전환 시점

## 실행 단계

// turbo-all

### 1. 현재 Sprint 상태 확인

**프로젝트별 필터링:** `project:<name>` 라벨을 사용하여 해당 프로젝트의 이슈만 확인하세요.

```powershell
$projectName = "project-name" # 대상 프로젝트명
gh issue list --repo yj7-park/AntiCorp --state open --label "@working,project:$projectName"
gh issue list --repo yj7-park/AntiCorp --label "@review,project:$projectName"
```

### 2. 검토 대상 분석

| 검토 요청 출처 | 처리 방법 |
|--------------|----------|
| Planner (설계 완료) | 설계 승인 → Developer 구현 요청 |
| Developer (구현 완료) | 확인 → Tester 결과 대기 |
| Tester (테스트 완료) | Sprint 종료 판단 |

### 3-A. 설계 검토 및 구현 요청

설계가 완료되면 Developer에게 구현 요청:

**템플릿:** `.github/ISSUE_TEMPLATE/implement_request.md`

```powershell
gh issue create --repo yj7-park/AntiCorp `
    --title "[$projectName] Sprint N: 기능명 구현" `
    --body "(템플릿 내용)" `
    --label "@developer,project:$projectName"

# 설계 이슈 완료 처리
gh issue edit <설계이슈> --repo yj7-park/AntiCorp --remove-label "@review" --add-label "@done" --state closed
```

### 3-B. Sprint 완료 처리

모든 테스트가 통과하면:

```powershell
# 관련 이슈들 @done 처리
gh issue edit <이슈번호> --repo yj7-park/AntiCorp --remove-label "@review" --add-label "@done" --state closed

# Sprint 완료 댓글
gh issue comment <Sprint이슈> --repo yj7-park/AntiCorp --body "✅ Sprint N 완료"
```

### 3-C. 다음 Sprint 시작

Sprint N이 완료되면 Sprint N+1 시작:

```powershell
gh issue create --repo yj7-park/AntiCorp `
    --title "[$projectName] Sprint N+1: 설계 요청" `
    --body "(design_request 템플릿)" `
    --label "@planner,project:$projectName"
```

### 4. 병목 해소

```powershell
gh issue list --repo yj7-park/AntiCorp --label "@blocked,project:$projectName"
```
