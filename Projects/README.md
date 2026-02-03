# Projects 폴더 설명

이 폴더는 AntiCorp 시스템이 새 프로젝트를 수주할 때 자동으로 프로젝트를 생성하는 디렉토리입니다.

## 사용 방법

Leader Agent가 `@new-project` label이 있는 Issue를 감지하면:

1. 이 폴더 하위에 새 프로젝트 폴더 생성
2. Git repository 초기화
3. Workspace에 폴더 추가

## 예시 구조

```
Projects/
├── portfolio-website/       # 첫 번째 프로젝트
│   ├── src/
│   ├── README.md
│   └── ...
├── todo-app/                 # 두 번째 프로젝트
│   ├── src/
│   ├── README.md
│   └── ...
└── ...
```

## 주의사항

- 각 프로젝트는 독립된 Git repository입니다
- 프로젝트별로 고유한 .gitignore를 가질 수 있습니다
- 모든 프로젝트는 AntiCorp workspace에 함께 추가됩니다

