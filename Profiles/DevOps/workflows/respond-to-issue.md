---
description: 할당된 이슈에 대해 계획을 세우고 작업을 수행한 뒤 보고
---

# 이슈 대응 워크플로우

할당된 개별 이슈를 처리할 때 이 워크플로우를 따르세요.

## 실행 단계

// turbo-all

### 1. 이슈 분석 및 계획

이슈의 요구사항을 분석하고 작업 계획을 세웁니다.
- 수정/생성해야 할 파일 확인
- 요구사항 충족 여부 확인 방법 결정

### 2. 작업 수행

필요한 코드를 작성하거나 문서를 수정합니다.

```powershell
# 예시: 작업 중인 프로젝트 폴더로 이동
cd c:\Workspace\AntiCorp\Projects\your-project
```

### 3. 결과 확인 (자가 테스트)

작업이 의도대로 되었는지 확인합니다.
- 빌드 테스트
- 기능 동작 확인

### 4. Git 커밋 및 Push

```powershell
git add .
git commit -m "feat: 이슈 설명 (Close #이슈번호)"
git push
```

### 5. 작업 완료 보고

GitHub Issue에 진행 상황을 기록하고 닫습니다.

```powershell
gh issue close <issue-number> --repo yj7-park/AntiCorp --comment "작업 완료 보고: (상세 내용)"
```

## 보고 형식 (권장)

이슈를 닫을 때 다음 내용을 포함하세요:
- **수행 내용**: 무엇을 했는지 요약
- **결과**: 작업 결과물 (파일 경로 등)
- **테스터 알림**: 테스트가 필요한 경우 `@tester` 소환

## 주의사항

> [!IMPORTANT]
> - 모든 작업은 Git에 커밋되어야 합니다.
> - 이슈 번호를 커밋 메시지에 포함하면 자동으로 연결됩니다.

