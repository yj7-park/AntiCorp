---
description: 프로젝트 문서를 작성하고 관리
---

# 문서 작성

## 역할 정의

**Planner**는 기획자/설계자로서 다음을 수행합니다:
- README, API 문서 등 작성
- 프로젝트 문서 관리
- 추가 기능 제안 문서화

## 실행 시점

문서 작성 요청 이슈가 할당되었을 때

## 실행 단계

// turbo-all

### 1. 프로젝트 상태 확인

```powershell
cd c:\Workspace\AntiCorp\Planner\<프로젝트명>
git pull
```

### 2. 문서 작성

요청에 따라 적절한 문서를 작성합니다:

**README.md:**
- 프로젝트 소개
- 설치 방법
- 사용 방법
- 기여 가이드

**docs/ 폴더:**
- API 문서
- 사용자 가이드
- 개발자 가이드

**GitHub Wiki:**
- 상세 설계 문서
- FAQ
- 릴리즈 노트

### 3. 추가 기능 분석 및 제안

프로젝트를 분석하여 추가할 수 있는 기능을 제안합니다:

```powershell
c:\Workspace\AntiCorp\Tools\Scripts\Create-Issue.ps1 `
    -Title "[<프로젝트명>] 기능 제안: <기능명>" `
    -Body @"
## 제안 배경
(왜 이 기능이 필요한지)

## 기능 설명
(기능의 상세 내용)

## 예상 이점
- (이점 1)
- (이점 2)

## 구현 복잡도
(예상 난이도: 낮음/중간/높음)

## 우선순위 제안
(낮음/중간/높음)
"@ `
    -Labels "@leader"
```

### 4. Git Commit 및 Push

```powershell
git add .
git commit -m "docs: 문서 업데이트 (#이슈번호)"
git push
```

### 5. 완료 보고

```powershell
gh issue comment <이슈번호> --repo yj7-park/AntiCorp --body @"
✅ Planner 문서 작성 완료

**작성/수정한 문서:**
- (문서 목록)

**참고 링크:**
- (문서 링크)
"@

gh issue edit <이슈번호> --repo yj7-park/AntiCorp --remove-label "@working" --add-label "@done"
```

## 문서 작성 가이드

> [!TIP]
> **좋은 문서의 특징:**
> - 명확하고 간결한 설명
> - 예제 코드 포함
> - 최신 상태 유지
> - 다이어그램/스크린샷 활용

> [!IMPORTANT]
> **문서 관리 규칙:**
> - 코드 변경 시 관련 문서도 업데이트
> - 버전별 변경 사항 기록
> - 접근성 고려 (검색 가능하도록)
