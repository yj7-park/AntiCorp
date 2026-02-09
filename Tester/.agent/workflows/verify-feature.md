---
description: Sprint 기능 검증 및 버그 리포트
---

# Sprint 기능 검증

## 역할 정의

**Tester**는 품질 보증 담당자로서 각 Sprint마다:
- Developer 구현 검증
- 버그 발견 시 이슈 발급
- 검증 완료 시 Leader 보고

## 실행 시점

Developer로부터 테스트 요청 이슈(`@tester`)를 받았을 때

## 실행 단계

> [!IMPORTANT]
> 검증이 끝나면 **반드시** 결과를 이슈로 생성해야 합니다. 성공 시 Leader에게 보고(`@review`), 실패 시 Developer에게 반환(`@developer`). 절대 침묵하지 마세요.

// turbo-all

### 1. 이슈 수락

```powershell
gh issue edit <이슈번호> --repo yj7-park/AntiCorp --add-label "@working"
gh issue comment <이슈번호> --repo yj7-park/AntiCorp --body "🧪 Tester 테스트 시작"
```

### 2. 프로젝트 환경 설정 (공용 폴더)

- **프로젝트:** 라벨 `project:<name>` 확인

```powershell
$projectName = "project-name"
# 공용 폴더 사용
cd c:\Workspace\AntiCorp\Projects\$projectName
git pull
```

### 3. 테스트 실행

설계 문서의 테스트 포인트를 기반으로 검증.

```powershell
# Test 실행
# dotnet test / npm test 등 (프로젝트 유형에 맞춰 실행)
```

### 4. Remote 동기화 확인 (필수)

테스트 코드나 로그를 추가했다면 반드시 Push해야 합니다.

```powershell
git status
# 변경사항이 있으면 커밋 및 푸시
if (git status --porcelain) {
    git add .
    git commit -m "test: $projectName 검증 수행 (#이슈번호)"
    git push
}
# Push 확인
git log origin/main..HEAD
```

> [!WARNING]
> Remote Repo에 반영되지 않은 작업은 무효입니다. Push를 확인하세요.

### 5-A. 버그 발견 시 (Developer에게 반환)

**템플릿:** `.github/ISSUE_TEMPLATE/bug_report.md`

```powershell
$projectLabel = "project:$projectName"

gh issue create --repo yj7-park/AntiCorp `
    --title "[$projectName] 버그: 간단한 설명" `
    --body "(버그 리포트 템플릿 내용)" `
    --label "@developer,$projectLabel"
```

테스트 이슈 업데이트:
```powershell
gh issue comment <테스트이슈> --repo yj7-park/AntiCorp --body "🐛 버그 발견: #버그이슈번호 (Developer에게 전달됨)"
```

### 5-B. 테스트 통과 시 (Leader에게 보고)

```powershell
gh issue comment <이슈번호> --repo yj7-park/AntiCorp --body @"
✅ Tester Sprint N 테스트 완료

**테스트 결과:** PASS

**검증 항목:**
- [x] 정상 동작
- [x] 엣지 케이스

@leader 검토 부탁드립니다.
"@

# 원본 이슈에서 @working 제거, @review 추가
gh issue edit <이슈번호> --repo yj7-park/AntiCorp --remove-label "@working" --add-label "@review,project:$projectName"
```

## Sprint 완료 보고 (최종)

Sprint의 모든 기능 테스트가 완료되면:

**템플릿:** `.github/ISSUE_TEMPLATE/sprint_complete.md`

```powershell
$projectLabel = "project:$projectName"

gh issue create --repo yj7-park/AntiCorp `
    --title "[$projectName] Sprint N 완료 보고" `
    --body "(sprint_complete 템플릿)" `
    --label "@leader,@review,$projectLabel"
```
