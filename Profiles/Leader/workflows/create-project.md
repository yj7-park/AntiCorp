---
description: 새 프로젝트를 생성하고 팀원에게 작업 분배
---

# 새 프로젝트 생성 워크플로우

`@new-project` label이 있는 Issue를 받으면 이 워크플로우를 실행하세요.

## 실행 단계

// turbo-all

### 1. 프로젝트 정보 분석

Issue 내용에서 다음 정보를 파악:
- 프로젝트 이름
- 기술 스택 (React, Python, C# 등)
- 주요 기능 요구사항
- 예상 일정

### 2. 프로젝트 폴더 생성

```powershell
# 프로젝트 이름은 kebab-case로 변환 (예: "My Project" -> "my-project")
$projectName = "project-name-here"
$projectPath = "c:\Workspace\AntiCorp\Projects\$projectName"

# 폴더 생성
New-Item -Path $projectPath -ItemType Directory -Force
```

### 3. Git Repository 초기화

```powershell
cd $projectPath
git init
git branch -M main

# 초기 README 생성
@"
# $projectName

프로젝트 설명...
"@ | Out-File -FilePath "README.md" -Encoding utf8

git add README.md
git commit -m "Initial commit"
```

### 4. Workspace에 폴더 추가

Antigravity UI를 사용하여:
1. File -> Add Folder to Workspace
2. `$projectPath` 선택
3. 기존 AntiCorp workspace와 함께 유지

### 5. 작업 분배

프로젝트를 다음과 같이 분배:

**DevOps 작업:**
- 프로젝트 초기 설정
- 의존성 관리 파일 생성
- 빌드 스크립트 작성

**Developer 작업:**
- 핵심 기능 구현
- UI/UX 개발

**Tester 작업:**
- 테스트 케이스 작성
- 기능 검증

### 6. DevOps에게 초기 설정 요청

```powershell
.\Tools\Scripts\Create-Issue.ps1 `
    -Title "[$projectName] 프로젝트 초기 설정" `
    -Body @"
새 프로젝트의 초기 설정을 해주세요.

**프로젝트 경로:** $projectPath
**기술 스택:** (기술 스택 입력)

**작업 내용:**
- package.json / requirements.txt / .csproj 생성
- .gitignore 작성
- 빌드 스크립트 작성
- 필요한 의존성 설치
"@ `
    -Labels "@devops"
```

### 7. Developer에게 기능 구현 요청

주요 기능별로 Issue 생성:

```powershell
.\Tools\Scripts\Create-Issue.ps1 `
    -Title "[$projectName] (기능명) 구현" `
    -Body @"
(기능 상세 설명)

**요구사항:**
- 요구사항 1
- 요구사항 2

**참고사항:**
- 참고사항...
"@ `
    -Labels "@developer"
```

### 8. Tester에게 테스트 요청

```powershell
.\Tools\Scripts\Create-Issue.ps1 `
    -Title "[$projectName] 테스트 케이스 작성" `
    -Body @"
프로젝트의 테스트 케이스를 작성해주세요.

**테스트 대상:**
- 기능 1
- 기능 2

**우선순위:**
- Critical path 테스트 우선
"@ `
    -Labels "@tester"
```

### 9. 프로젝트 추적 Issue 생성

전체 프로젝트 진행 상황을 추적하는 Issue:

```powershell
.\Tools\Scripts\Create-Issue.ps1 `
    -Title "[$projectName] 프로젝트 진행 상황" `
    -Body @"
프로젝트 전체 진행 상황을 추적합니다.

## 작업 현황
- [ ] 프로젝트 초기 설정 (DevOps)
- [ ] 기능 1 구현 (Developer)
- [ ] 기능 2 구현 (Developer)
- [ ] 테스트 작성 (Tester)
- [ ] 통합 테스트 (Tester)

## 일정
- 시작일: (날짜)
- 목표 완료일: (날짜)
"@ `
    -Labels "@all"
```

### 10. 원본 Issue 닫기

새 프로젝트 생성 작업이 완료되면:

```powershell
# Issue 번호 확인 후
gh issue close <issue-number> --repo yj7-park/AntiCorp --comment "프로젝트 생성 완료 및 작업 분배 완료"
```

## 주의사항

> [!IMPORTANT]
> - 프로젝트 폴더는 반드시 `c:\Workspace\AntiCorp\Projects\` 하위에 생성
> - 기존 workspace를 대체하지 말고 폴더만 추가
> - 모든 작업 분배는 명확하고 구체적으로

> [!WARNING]
> - 프로젝트 이름에 특수문자나 공백이 있으면 kebab-case로 변환
> - Git repository는 반드시 초기화

