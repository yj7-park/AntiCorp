---
description: Sprint 기능 설계 및 문서 작성
---

# Sprint 기능 설계

## 역할 정의

**Planner**는 기획자/설계자로서 각 Sprint마다:
- 해당 Sprint 기능 상세 설계
- GitHub Wiki에 문서 작성
- 다음 Sprint 사전 분석

## 실행 시점

Leader로부터 Sprint N 설계 요청 이슈를 받았을 때

## 실행 단계

// turbo-all

### 1. Sprint 이슈 수락

```powershell
gh issue edit <이슈번호> --repo yj7-park/AntiCorp --add-label "@working"
gh issue comment <이슈번호> --repo yj7-park/AntiCorp --body "📋 Planner Sprint 설계 시작"
```

### 2. 요구사항 분석

Sprint 이슈에서 분석:
- 이번 Sprint 목표
- 구현할 기능 목록
- 제약사항

### 3. 설계 문서 작성

GitHub Wiki에 Sprint별 설계 문서 작성:

```powershell
cd c:\Workspace\AntiCorp\Planner\<프로젝트>.wiki
```

**설계 문서 구조:**
```markdown
# Sprint N 설계

## 목표
(Sprint 목표)

## 기능 명세
### 기능 1
- **목적:** 
- **입력:** 
- **출력:** 
- **흐름:** 

## 아키텍처
(다이어그램 또는 설명)

## API 정의 (해당 시)
| 엔드포인트 | 메서드 | 설명 |
|-----------|--------|------|
| | | |

## 데이터 모델 (해당 시)
| 필드 | 타입 | 설명 |
|------|------|------|
| | | |

## 구현 가이드
- 권장 파일 구조
- 주의사항

## 테스트 포인트
- 확인해야 할 동작
- 엣지 케이스
```

### 4. 설계 문서 Push

```powershell
cd <프로젝트>.wiki
git add .
git commit -m "docs: Sprint N 설계 문서"
git push
```

### 5. 완료 보고

```powershell
gh issue comment <이슈번호> --repo yj7-park/AntiCorp --body @"
✅ Planner Sprint N 설계 완료

**설계 문서:** https://github.com/yj7-park/<프로젝트>/wiki/Sprint-N

**주요 내용:**
- (설계 핵심 요약)

**Developer를 위한 구현 포인트:**
- (구현 가이드 요약)

**Tester를 위한 테스트 포인트:**
- (테스트 가이드 요약)

@leader 검토 부탁드립니다.
"@

gh issue edit <이슈번호> --repo yj7-park/AntiCorp --remove-label "@working" --add-label "@review"
```

### 6. 다음 Sprint 사전 분석 (Optional)

여유가 있으면 다음 Sprint 기능 사전 검토 및 메모
