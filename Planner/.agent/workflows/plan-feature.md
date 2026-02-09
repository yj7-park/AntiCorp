---
description: Sprint 기능 설계 및 문서 작성
---

# Sprint 기능 설계

## 역할 정의

**Planner**는 기획자/설계자로서 각 Sprint마다:
- 해당 Sprint 기능 상세 설계
- GitHub Wiki에 문서 작성 (프로젝트별)
- 다음 Sprint 사전 분석

## 실행 시점

Leader로부터 Sprint N 설계 요청 이슈(`@planner`)를 받았을 때

## 실행 단계

// turbo-all

### 1. 이슈 수락

```powershell
gh issue edit <이슈번호> --repo yj7-park/AntiCorp --add-label "@working"
gh issue comment <이슈번호> --repo yj7-park/AntiCorp --body "📋 Planner Sprint 설계 시작"
```

### 2. 요구사항 분석

Sprint 이슈에서 분석:
- **프로젝트:** 라벨 `project:<name>` 확인
- 이번 Sprint 목표
- 구현할 기능 목록

### 3. 설계 문서 작성

GitHub Wiki에 Sprint별 설계 문서 작성:

```powershell
$projectName = "project-name" # 이슈 라벨에서 확인
cd c:\Workspace\AntiCorp\Planner\$projectName.wiki
```

**설계 문서 구조:**
(기존 템플릿 참조)

### 4. 설계 문서 Push

```powershell
cd $projectName.wiki
git add .
git commit -m "docs: Sprint N 설계 문서"
git push
```

### 5. Developer에게 구현 요청 (인계)

**Planner의 작업 종료 → Developer의 작업 시작**
설계가 완료되면 Developer에게 구현을 요청하는 **새로운 이슈**를 생성합니다.

**템플릿:** `.github/ISSUE_TEMPLATE/implement_request.md`

```powershell
# 프로젝트 라벨 변수 자동 적용
$projectLabel = "project:$projectName"

gh issue create --repo yj7-park/AntiCorp `
    --title "[$projectName] Sprint N: 기능명 구현" `
    --body "(구현 요청 템플릿 내용)" `
    --label "@developer,$projectLabel"
```

### 6. 완료 보고 및 원본 이슈 정리

```powershell
gh issue comment <원본이슈번호> --repo yj7-park/AntiCorp --body @"
✅ Planner Sprint N 설계 완료

**설계 문서:** https://github.com/yj7-park/$projectName/wiki/Sprint-N

**Developer 요청:**
- #구현이슈번호 (생성됨)

@leader 검토 부탁드립니다.
"@

# 원본 이슈에서 @working 제거 및 완료 처리
gh issue edit <원본이슈번호> --repo yj7-park/AntiCorp --remove-label "@working" --add-label "@done" --state closed
```
