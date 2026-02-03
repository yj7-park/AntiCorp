---
description: 할당받은 Issue를 처리하고 결과 보고
---

# Issue 응답 워크플로우

자신에게 할당된 label이 있는 Issue를 처리하는 워크플로우입니다.

## 실행 단계

// turbo-all

### 1. Issue 내용 분석

받은 Issue에서 다음을 파악:
- **요구사항**: 무엇을 해야 하는가?
- **제약사항**: 고려해야 할 제한사항은?
- **우선순위**: 긴급한가?
- **의존성**: 다른 작업과의 관계는?

불명확한 부분이 있으면 즉시 질문:

```powershell
.\Tools\Scripts\Create-Issue.ps1 `
    -Title "[질문] (질문 내용)" `
    -Body "다음 사항이 명확하지 않아 질문 드립니다: ..." `
    -Labels "@leader"
```

### 2. 작업 계획 수립

간단한 작업 계획 작성:
- [ ] 작업 1
- [ ] 작업 2
- [ ] 작업 3

복잡한 작업은 작은 단위로 분해하세요.

### 3. 작업 수행

자신의 역할에 맞게 작업 수행:

**Developer:**
- 코드 구현
- Git commit & push

**Tester:**
- 테스트 작성
- 테스트 실행 및 결과 기록

**DevOps:**
- 설정 파일 작성
- 빌드 스크립트 작성

### 4. 작업 검증

완료 전 자체 검증:
- [ ] 요구사항을 모두 충족했는가?
- [ ] 에러나 버그는 없는가?
- [ ] 코드/설정이 깔끔한가?
- [ ] 문서화가 필요한가?

### 5. 결과 보고

작업 완료 후 Leader에게 보고:

**성공적으로 완료한 경우:**
```powershell
.\Tools\Scripts\Create-Issue.ps1 `
    -Title "[완료] (작업명)" `
    -Body @"
요청하신 작업을 완료했습니다.

**완료 내용:**
- 항목 1
- 항목 2

**변경된 파일:**
- file1.js
- file2.css

**테스트 여부:** (필요시)
- [x] 로컬 테스트 완료
- [ ] 추가 테스트 필요

**참고사항:**
추가로 알려드릴 내용...
"@ `
    -Labels "@leader"
```

**문제가 발생한 경우:**
```powershell
.\Tools\Scripts\Create-Issue.ps1 `
    -Title "[문제 발생] (작업명)" `
    -Body @"
작업 중 다음 문제가 발생했습니다.

**문제 내용:**
(문제 상세 설명)

**시도한 해결 방법:**
- 방법 1: 결과...
- 방법 2: 결과...

**도움 요청:**
다음 부분에 대한 지원이 필요합니다: ...
"@ `
    -Labels "@leader"
```

### 6. 원본 Issue 댓글 또는 닫기

```powershell
# Issue에 댓글 추가
gh issue comment <issue-number> --repo yj7-park/AntiCorp --body "작업 완료했습니다. 결과는 #<new-issue-number>에 보고했습니다."

# Issue 닫기 (완료된 경우)
gh issue close <issue-number> --repo yj7-park/AntiCorp
```

### 7. 필요시 후속 작업 요청

다른 Agent에게 후속 작업이 필요한 경우:

```powershell
# 예: Developer가 구현 완료 후 Tester에게 테스트 요청
.\Tools\Scripts\Create-Issue.ps1 `
    -Title "(기능명) 테스트 요청" `
    -Body "구현이 완료되었습니다. 테스트를 부탁드립니다..." `
    -Labels "@tester"
```

## 보고 템플릿

### 개발 완료 보고 (Developer)
```markdown
## 완료 내용
- 구현한 기능 목록
- 사용한 기술/라이브러리

## 변경 파일
- src/components/Login.jsx
- src/utils/auth.js

## 테스트
- [x] 로컬에서 기본 동작 확인
- [ ] 통합 테스트 필요 (Tester에게 요청)

## 참고사항
- 특이사항이나 추가 설명
```

### 테스트 완료 보고 (Tester)
```markdown
## 테스트 결과
- 총 테스트: 15개
- 통과: 14개
- 실패: 1개

## 테스트 커버리지
- 단위 테스트: 85%
- 통합 테스트: 완료

## 발견된 버그
- [x] 버그 #1: 로그인 실패 시 에러 메시지 미표시 (별도 Issue 생성)
```

### 설정 완료 보고 (DevOps)
```markdown
## 완료 내용
- package.json 생성
- .gitignore 작성
- 빌드 스크립트 작성

## 설치된 의존성
- react: 18.3.0
- vite: 5.0.0

## 빌드 명령
\`\`\`bash
npm run build
\`\`\`
```

## 주의사항

> [!IMPORTANT]
> - 작업 완료 후 반드시 보고하세요
> - 문제가 발생하면 즉시 알리세요
> - 보고는 구체적이고 명확하게 작성하세요

> [!TIP]
> - 작업 중간에도 진행 상황을 공유하면 좋습니다
> - 스크린샷이나 로그를 첨부하면 이해가 쉽습니다
