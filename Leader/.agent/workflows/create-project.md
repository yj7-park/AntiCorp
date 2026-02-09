---
description: 새 프로젝트를 생성하고 Sprint 1을 시작
---

# 새 프로젝트 생성 (Sprint 1 시작)

## 역할 정의

**Leader**는 새 프로젝트 요청을 받으면:
1. GitHub repo 생성 및 초기화
2. **프로젝트 전용 라벨 생성** (`project:<name>`)
3. Sprint 1 계획 수립
4. **Planner에게 초기 설계 요청** (새 이슈 생성)

## 실행 시점

`@new-project` 라벨이 있는 Issue 발견 시 실행

## 실행 단계

> [!IMPORTANT]
> 아래 모든 단계를 **순서대로 빠짐없이** 실행하세요. 특히 `gh` 명령어들은 실행 결과를 확인해야 합니다.

// turbo-all

### 1. 프로젝트 정보 분석

Issue 내용에서 다음 정보를 파악:
- 프로젝트 이름 (kebab-case 변환, 예: `nasdaq-trading`)
- 기술 스택
- MVP 핵심 기능
- 추가 기능

### 2. GitHub Repository 생성

```powershell
$projectName = "project-name-here" # 실제 프로젝트명으로 변경

# Repo 생성
gh repo create yj7-park/$projectName --public --description "프로젝트 설명"

# 검증 (Repo가 생성되었는지 확인)
gh repo view yj7-park/$projectName
```

### 3. 로컬에 Clone 및 초기화

```powershell
cd c:\Workspace\AntiCorp\Projects
git clone https://github.com/yj7-park/$projectName.git
cd $projectName

# (선택) 초기 파일 생성
```

### 4. 프로젝트 라벨 생성

이슈 필터링을 위한 전용 라벨을 생성합니다.

```powershell
# 라벨 색상은 임의 지정 (예: 1d76db)
gh label create "project:$projectName" --repo yj7-park/AntiCorp --color 1d76db --description "$projectName 관련 이슈"
```

### 5. Sprint 계획 이슈 생성

```powershell
gh issue create --repo yj7-park/AntiCorp `
    --title "[$projectName] Sprint 계획 (전체)" `
    --body @"
# 프로젝트 Sprint 계획

## Sprint 1-2: MVP
- [ ] 핵심 기능 1
- [ ] 핵심 기능 2

## Sprint 3-6: 추가 기능
- [ ] 추가 기능 1
- [ ] 추가 기능 2

## Sprint 7-8: 안정화
- [ ] 버그 수정

## Sprint 9-10: 릴리즈
- [ ] 배포 준비
"@ `
    --label "@leader,project:$projectName"
```

### 6. Planner에게 Sprint 1 설계 요청 (인계)

**Leader의 작업 종료 → Planner의 작업 시작**
새로운 이슈를 생성하여 Planner에게 명확한 지시사항을 전달합니다.

**템플릿:** `.github/ISSUE_TEMPLATE/design_request.md`

```powershell
gh issue create --repo yj7-park/AntiCorp `
    --title "[$projectName] Sprint 1: MVP 설계 요청" `
    --body @"
## Sprint 정보
- **프로젝트:** $projectName
- **Sprint 번호:** 1
- **목표:** MVP 핵심 기능 설계

## 설계 대상 기능
(MVP 기능 목록)

## 요구사항
(원본 이슈에서 추출)

## 산출물 기대
- [ ] GitHub Wiki에 설계 문서
- [ ] 아키텍처 다이어그램

## 완료 조건
- [ ] 설계 문서 작성 완료
- [ ] Leader 검토 요청 (@review)
"@ `
    --label "@planner,project:$projectName"
```

### 7. 전체 에이전트에게 Clone 요청

```powershell
gh issue create --repo yj7-park/AntiCorp `
    --title "[$projectName] 프로젝트 Clone 요청" `
    --body @"
새 프로젝트($projectName)가 생성되었습니다. 각자의 workspace에 clone해주세요.

**Repo:** https://github.com/yj7-park/$projectName

\`\`\`powershell
cd (자신의 workspace)
git clone https://github.com/yj7-park/$projectName.git
\`\`\`
"@ `
    --label "@all,project:$projectName"
```

### 8. 원본 이슈 완료 처리

모든 하위 이슈가 생성되었으므로, 원본 요청을 완료 처리합니다.

```powershell
gh issue comment <원본이슈번호> --repo yj7-park/AntiCorp --body @"
✅ 프로젝트 생성 및 초기화 완료

**생성 내역:**
- **Repo:** https://github.com/yj7-park/$projectName
- **Sprint 계획:** #계획이슈번호
- **Sprint 1 설계 요청:** #설계이슈번호 (Planner에게 전달됨)

Sprint 1이 시작되었습니다.
"@

# 원본 이슈에서 @new-project 제거 및 완료 처리
gh issue edit <원본이슈번호> --repo yj7-park/AntiCorp --remove-label "@new-project" --add-label "@done" --state closed
```
