---
description: Sprint 기능 구현
---

# Sprint 기능 구현

## 역할 정의

**Developer**는 개발자로서 각 Sprint마다:
- 설계 문서 기반 코드 구현
- 테스트 요청 발급
- 버그 수정 (반복)

## 실행 시점

Leader 또는 Planner로부터 구현 요청 이슈(`@developer`)를 받았을 때

## 실행 단계

// turbo-all

### 1. 이슈 수락

```powershell
gh issue edit <이슈번호> --repo yj7-park/AntiCorp --add-label "@working"
gh issue comment <이슈번호> --repo yj7-park/AntiCorp --body "🔧 Developer 구현 시작"
```

### 2. 프로젝트 및 설계 확인

- **프로젝트:** 라벨 `project:<name>` 확인
- **설계 문서:** 이슈 본문 링크 확인

```powershell
$projectName = "project-name"
cd c:\Workspace\AntiCorp\Developer\$projectName
git pull
```

### 3. 코드 구현

설계 문서를 따라 구현:
- 파일 구조 확인
- 기능 코드 작성
- 기본 동작 확인

### 4. 커밋 및 Push

```powershell
git add .
git commit -m "feat(sprint-N): 기능 설명 (#이슈번호)"
git push
```

### 5. Tester에게 테스트 요청 (인계)

**Developer의 작업 종료 → Tester의 작업 시작**
구현이 완료되면 Tester에게 검증을 요청하는 **새로운 이슈**를 생성합니다.

**템플릿:** `.github/ISSUE_TEMPLATE/test_request.md`

```powershell
$projectLabel = "project:$projectName"

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
"@
# @working 라벨 제거 (대기 상태)
gh issue edit <이슈번호> --repo yj7-park/AntiCorp --remove-label "@working"
```

## 버그 수정 사이클

버그 이슈(`@bug`)를 받으면:

1. 버그 이슈 수락 (`@working`)
2. 버그 수정 및 Push
3. 버그 이슈에 코멘트: "🔧 수정 완료. 재테스트 요청"
4. Tester에게 알림 (또는 @working 제거)
