---
description: Sprint 기능 구현
---

# Sprint 기능 구현

## 역할 정의

**Developer**는 개발자로서 각 Sprint마다:
- 설계 문서 기반 코드 구현
- GitHub Issue를 통한 작업 상태 관리
- 구현 완료 시 **반드시** Tester에게 테스트 요청

## 실행 시점

Leader 또는 Planner로부터 구현 요청 이슈(`@developer`)를 받았을 때

## 실행 단계

> [!IMPORTANT]
> 구현 후 **Step 5 (Tester 요청)**를 절대 건너뛰지 마세요. 프로세스가 멈추는 원인이 됩니다.

// turbo-all

### 1. 이슈 수락

```powershell
gh issue edit <이슈번호> --repo yj7-park/AntiCorp --add-label "@working"
gh issue comment <이슈번호> --repo yj7-park/AntiCorp --body "🔧 Developer 구현 시작"
```

### 2. 프로젝트 및 설계 확인 (공용 폴더)

- **프로젝트:** 라벨 `project:<name>` 확인
- **설계 문서:** 이슈 본문 링크 확인

```powershell
$projectName = "project-name"
# 공용 폴더 사용
cd c:\Workspace\AntiCorp\Projects\$projectName
git pull
```

### 3. 코드 구현

설계 문서를 따라 구현:
- 파일 구조 확인
- 기능 코드 작성
- 기본 동작 확인

### 4. Remote 동기화 확인 (필수)

구현된 코드를 반드시 Remote에 반영합니다.

```powershell
git status
# 변경사항이 있으면 커밋 및 푸시
if (git status --porcelain) {
    git add .
    git commit -m "feat(sprint-N): 기능 설명 (#이슈번호)"
    git push
}
# Push 확인
git log origin/main..HEAD
```

> [!WARNING]
> Remote Repo에 코드가 없으면 Tester가 테스트할 수 없습니다.

### 5. Tester에게 테스트 요청 (필수 인계)

**Developer의 작업 종료 → Tester의 작업 시작**
구현이 완료되면 Tester에게 검증을 요청하는 **새로운 이슈**를 생성합니다.

**템플릿:** `.github/ISSUE_TEMPLATE/test_request.md`

```powershell
$projectLabel = "project:$projectName"

# 중요: 이슈 생성 결과를 확인하세요.
gh issue create --repo yj7-park/AntiCorp `
    --title "[$projectName] Sprint N 기능 테스트 요청" `
    --body @"
## 테스트 대상
- **프로젝트:** $projectName
- **Repo:** https://github.com/yj7-park/$projectName
- **구현 이슈:** #<이슈번호>

## 구현 내용 요약
(구현한 기능 설명)

## 테스트 포인트
(설계 문서의 테스트 포인트 참조)

## 완료 조건
- [ ] 테스트 통과 시 @review
- [ ] 버그 발견 시 버그 이슈 발급
"@ `
    --label "@tester,$projectLabel"
```

### 6. 원본 이슈 업데이트

원본 이슈는 Tester의 결과를 기다려야 하므로 **닫지 않고** 코멘트만 남깁니다.

```powershell
gh issue comment <이슈번호> --repo yj7-park/AntiCorp --body @"
✅ Developer 구현 완료

**커밋:** (커밋 링크)
**테스트 요청:** #테스트이슈번호 (Tester에게 전달됨)

Tester의 검증을 대기합니다.
"@
# @working 라벨 제거 (대기 상태)
gh issue edit <이슈번호> --repo yj7-park/AntiCorp --remove-label "@working"
```

## 버그 수정 사이클

버그 이슈(`@bug`)를 받으면:

1. 버그 이슈 수락 (`@working`)
2. 버그 수정 및 Push (위와 동일한 Push 검증 절차 수행)
3. 버그 이슈에 코멘트: "🔧 수정 완료. 재테스트 요청"
4. Tester에게 알림 (또는 @working 제거)
