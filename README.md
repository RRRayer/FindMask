# 프로젝트명
Find Mask

## 개요
온라인 숨바꼭질 파티 게임

## 기술 스택
- Unity C#
- Photon Fusion
- AI Tools: ChatGPT(Codex), Gemini

## 주간 진행 상황

### Week 1 (25.12.22 - 12.28)
**작업 내역**
- 프로젝트 초기 구조/모듈 정리(Classes/Gameplay/Progression/UI 등)
- 기본 플레이어/씬/입력 스캐폴딩

**AI 활용**
- 설계 정리 및 코드 스캐폴딩 보조

**완료 기능**
- 기본 씬/플레이 플로우 진입

**커밋 로그**
- feat(#1): 구조 확정
- FEAT/2 (#1)

**링크**
- https://github.com/RRRayer/Rative/pull/2

**다음 주 계획**
- 클래스/스킬 시스템 기반 구현 및 테스트 씬 보강

### Week 2 (25.12.29 - 26.1.4)
**작업 내역**
- 클래스 시스템(ClassDefinition/PlayerClassState/TestClassFactory)
- 스킬 시스템(SkillSlot/SkillExecutor/SkillBehaviour)
- PlayerManager 통합
- TestLee 씬 및 기본 프리팹 추가
- 1인칭 컨트롤러 및 입력 확장
- 메인메뉴/로비/대기실 UI 및 네트워크 플로우 구축
- GameRoom/InGame/MainMenu 씬 추가 및 연결
- 클래스/스킬 ScriptableObject 테스트 데이터 추가
- 빌드 설정 업데이트(창 모드, 씬 포함)

**AI 활용**
- 스킬 실행 구조/전투 보정 설계 및 코드 적용
- 로비/대기실 UI 구조 설계 및 스크립트 골격 정리

**완료 기능**
- 기본 스킬 실행(콤보/대시/채널/콘)
- 클래스 스탯 적용 및 이동속도 반영
- 메인메뉴/로비/대기실 동작
- 게임룸/인게임 테스트 씬 구성
- 클래스 선택 UI 및 인게임 클래스 표시

**커밋 로그**
- feat(#3): 플레이어 구조 확장
- feat(#3): 클래스, 스킬 테스트 코드 추가
- feat(#3): 예시 클래스 생성
- FEAT/8 (#3)
- feat(#5): 메인메뉴 구현
- feat(#5): 대기실 구현
- feat(#5) 테스트 케이스 추가
- chore(#5): 빌드 설정 변경
- FEAT/10 (#5)

**테스트 결과**
- 플레이어 테스트 씬
<img width="1458" height="436" alt="image" src="https://github.com/user-attachments/assets/d0cf8f2c-27a6-4018-8643-b2c2a9e6aa23" />
- 온라인 로비 테스트 씬
<img width="3439" height="1439" alt="image" src="https://github.com/user-attachments/assets/77cee21e-cb04-4935-99a3-c31599a89e7f" />

**링크**
- https://github.com/RRRayer/Rative/pull/8
- https://github.com/RRRayer/Rative/pull/10

**다음 주 계획**
- 전사 스킬/업그레이드 디테일 구현, 전투 피드백 보강

### Week 3 (1.4 - 1.11)
**작업 내역**
- 전사 클래스 1차 구현 및 업그레이드 트랙/패시브 구조 도입
- 전투 시스템 확장: Airborne, Projectile, WeaponHitbox, SkillBehaviour 보강
- 공유 경험치/레벨링 및 업그레이드 UI/HUD 추가
- TestLee 씬/프리팹 보강 (전사/적/투사체/XP 픽업)
- 플레이어 프리팹 경로 정리

**AI 활용**
- 전사 업그레이드/전투 로직 설계 및 코드 보조
- UI/HUD/진행 시스템 설계 보조

**완료 기능**
- 전사 스킬 업그레이드 트랙 및 패시브 동작
- Airborne/투사체/히트박스 기반 전투 처리
- 공유 XP 및 업그레이드 선택 UI/HUD 동작
- TestLee 전투/프리팹 구성 보강

**커밋 로그**
- feat(#12): 전사 클래스 1차 구현
- feat(#12): 전사, 적 구현
- feat(#12): 플레이어 프리팹 위치 이동
- art(#9): 맵, 플레이어 에셋 추가
- feat(#9): 테스트 씬 생성
- feat: 프로빌더 패키지 추가
- art: 몬스터 추가
- feat(#9): 템페스트 애니메이션 추가

**테스트 결과**
- 플레이어 테스트 씬
<img width="1286" height="873" alt="스크린샷 2026-01-15 144940" src="https://github.com/user-attachments/assets/e63715f1-83a4-466b-b7f4-31035cefc7fa" />
<img width="917" height="514" alt="스크린샷 2026-01-15 145020" src="https://github.com/user-attachments/assets/891ac8e1-161f-46eb-b9fe-936aae14d897" />

**링크**
- https://github.com/RRRayer/Rative/pull/13
- https://github.com/RRRayer/Rative/pull/14

**다음 주 계획**
- GaneManager, 팀 공동 업그레이드 구현
- 실제 아트와 플레이어 연동

### Week 4 (1.12 - 1.18)
**작업 내역**
- GameManager/MonsterManager 생성 및 스테이지 진행 기반 도입
- 팀 공동 업그레이드/멀티 테스트 구현 진행(스크립트 포함/외)
- 몬스터/캐릭터 프리팹 및 풀링 적용
- 맵 오브젝트/지형 배치
- 스킬/슬래시 이펙트 및 템페스트 애니메이션 보강
- 아트 리소스 추가 반영

**AI 활용**
- 팀 업그레이드/멀티 테스트 구조 설계 및 코드 보조
- 스테이지 진행 로직 설계 보조

**완료 기능**
- 스테이지 진행 기본 골격(GameManager/MonsterManager)
- 팀 업그레이드 + 멀티 테스트 기능 베이스
- 풀링 적용
- 몬스터/캐릭터/맵/이펙트/애니메이션 에셋 보강

**커밋 로그**
- feat(#16): GameManager, MonsterManager 생성
- feat(#18): 팀 업그레이드, 멀티 테스트 구현 중
- art(#15) : 맵 오브젝트 배치
- art(#15) : 템페스트 walk, idle, run, normal hit 애니메이션 추가
- feat(#15) : 몬스터, 캐릭터 프리팹 생성
- feat(#18): 멀티 테스트
- feat(#11): 풀링 적용
- art(#15) : 스킬 애니메이션 추가
- art(#15) : 슬래시 이펙트 추가

**테스트 결과**
- 멀티 플레이 테스트
<img width="2290" height="684" alt="스크린샷 2026-01-25 160904" src="https://github.com/user-attachments/assets/9045bd37-68ef-43ce-bc10-56ccd7290819" />

- 아트 적용 씬

<img width="832" height="472" alt="image" src="https://github.com/user-attachments/assets/007b23d2-d87b-4537-8815-8c8fc316b2c6" />

**링크**
- https://github.com/RRRayer/Rative/pull/17
- https://github.com/RRRayer/Rative/pull/19
- https://github.com/RRRayer/Rative/pull/20

**다음 주 계획**
- 팀 업그레이드/멀티 테스트 안정화
- 스테이지 진행 및 몬스터 스폰 고도화

### Week 5 (1.19 - 1.25)
**작업 내역**
- MCP for Unity 패키지 도입 및 설정 파일/패키지 의존성 반영
- 전투/업그레이드 설명 SO 구조 확장(업그레이드 트랙 설명 필드 추가)
- Player/Enemy 전투 동작 및 채널/쿨다운/업그레이드 처리 보강
- XP 픽업/공유 진행 로직 정리 및 UI 선택 동작 보완
- GameScene/TestLee/애니메이터/프리팹/Resources 구조 통합 정리
- 클래스/스킬/업그레이드/프리팹 리소스 경로 재정렬

**AI 활용** 
- 업그레이드 설명 구조 설계 및 코드 리팩터링
- 플레이어/몹 전투 로직 디버깅 및 수정 보조

**완료 기능**
- MCP for Unity 기본 통합 및 설정 반영
- 스킬/패시브 업그레이드 설명을 SO 기반으로 관리 가능
- XP 픽업 및 업그레이드 선택 흐름 보강
- 씬/리소스/애니메이션 통합 정리 완료

**커밋 로그**
- chore(#21): Unity MCP 추가
- feat(#21): 통합 작업

**테스트 결과**
<img width="1372" height="846" alt="스크린샷 2026-01-26 194818" src="https://github.com/user-attachments/assets/85867052-e4d6-4a62-8ab9-29380a35a72a" />


**링크**
- https://github.com/RRRayer/Rative/pull/23

**다음 주 계획**
- 통합 작업 후 안정화/버그 수정
- 멀티 테스트 및 스폰/전투 흐름 정리

### Week 6 (1.26 - 2.1)
**작업 내역**
- 기존 Project S 진행 상황/리스크 재평가 및 기획 회의 진행
- Photon PUN 기반 실시간 멀티 구현의 구조적 한계 분석
- Fusion 전환 비용 산정 및 현실적 일정 시뮬레이션
- 장르(3D 멀티 뱀서라이크) 구현 대비 재미 효율 재검토
- 신규 기획(현 프로젝트) 러프 프로토타입 제작 및 내부 플레이 테스트

**기획 전환 사유**
- 기술적 리스크
  + Photon PUN 기반 실시간 멀티 구현에서 구조적 한계 확인.
  + 문제를 해결하려면 Fusion 중심으로 네트워크 구조를 전면 재작성해야 했음.
  + 이는 단순 마이그레이션이 아니라 신규 프로젝트 수준의 비용으로 판단됨.

- 게임성/비용 효율 문제
  + 3D 멀티 뱀서라이크는 구현 난이도가 높은 반면, 재미 밀도가 낮다고 평가됨.
  + 콘텐츠 투입 대비 체감 재미의 증가폭이 제한적이라는 결론.

- 신규 기획 테스트 결과
  + 현 프로젝트 컨셉을 러프하게 구현 후 내부 테스트 진행.
  + 기존 기획 대비 구현 대비 재미 효율이 높음을 확인.
  + 빠른 반복/개선이 가능한 구조라는 점을 긍정 평가.

**결정 사항**
- Project S 개발 중단
- Fusion 기반 신규 프로젝트로 전환
- 기존 자산/코드 활용은 제한적으로 진행 (레퍼런스 수준)
- 신규 프로젝트 MVP 중심으로 일정 재설계

**리스크 및 대응**
- 기존 개발 투자 손실 최소화를 위해 핵심 재사용 가능한 에셋 선별 예정
- 네트워크 구조/스폰/게임 플로우는 초기부터 Fusion 기준으로 재설계

**다음 주 계획**
- 신규 프로젝트 핵심 루프 설계 확정
- 네트워크 구조(세션/스폰/게임 상태) 초기 구현
- MVP 플레이 흐름 검증

### Week 7 (2.2 - 2.8)
**작업 내역**
- 스폰 위치 조정 및 씬 배치 정리
- 로컬 NPC 전용 스크립트/프리팹 구축
- PlayerStateManager 안정화 및 GameManager 연동 수정
- Fusion 기반 네트워크 구조 리팩토링
- WaitingRoom 씬 구성 및 로비 UI 플로우 정리
- Lobby 팝업 버튼 동작 수정

**AI 활용**
- 네트워크 구조 분리(FusionSessionFlow / SpawnService / RoleAssignmentService) 설계 보조
- 대기실/로비 UI 흐름 정리 및 구조 개선 보조

**완료 기능**
- 로컬 NPC 전용 프리팹/컨트롤러 생성
- WaitingRoom 씬 추가 및 Fusion 플로우 연동
- 스폰/역할/입력 서비스 구조 분리 완료
- Lobby 팝업 UI 수정
- PlayerStateManager 구조 안정화

**커밋 로그**
- 스폰 위치 조정
- 로컬 NPC 전용 프리팹 생성
- PlayerStateManage 구조 안정화
- 구조 리팩토링
- FEAT/11
- fix(#12): 팝업 버튼 수정

**테스트 결과**
- 플레이 화면
<img width="794" height="446" alt="스크린샷 2026-02-20 032413" src="https://github.com/user-attachments/assets/3dbdb34f-ca84-4154-b52f-f0cae338cbe0" />
<img width="3439" height="1439" alt="스크린샷 2026-02-20 032434" src="https://github.com/user-attachments/assets/e6b35533-045d-4935-8999-3a89d0300bff" />
<img width="797" height="448" alt="스크린샷 2026-02-20 032458" src="https://github.com/user-attachments/assets/df888729-9843-41d6-8344-641f5b872834" />
<img width="3439" height="1439" alt="스크린샷 2026-02-20 032523" src="https://github.com/user-attachments/assets/ae0fa9fa-45b7-4316-903c-4eecf6747adb" />

**링크**
- https://github.com/RRRayer/GGJ26/pull/11
- https://github.com/RRRayer/GGJ26/pull/13

**다음 주 계획**
- Fusion 네트워크 플로우 안정화
- 스폰/대기실/게임 시작 흐름 QA
- 멀티 플레이 테스트 확장

## Week 8 (26.2.9 - 2.15)
**작업 내역**
- 술래 스킬 기능 구현 및 입력/이벤트 연동 (feat(#7), FEAT/7)
- 댄스 타임 중 이동/중력/모션 이상 동작 버그 수정
- StunGun 관련 버그 수정 2건 반영 (FIX/4, FIX/21)
- 시체 보간(Death interpolation) 버그 수정 (bug(#27), BUG/27)
- 게임 종료 조건 판정 버그 개선 (bug: 게임 종료 버그 개선)
- Next Dance UI 제거 및 게임씬 정리
- 오디오 설정/믹서 자산 보정 (FIX/19)
- MCP for Unity 관련 정리(chore) 및 디자인 문서 업데이트(메인/너구리 모드)

**AI 활용**
- 술래 스킬 이벤트 채널/설정 SO 구조 정리 보조
- 네트워크/이동/종료 판정 버그 원인 추적 및 수정안 검토
- 문서 구조화 및 모드 기획 문서 정리 보조

**완료 기능**
- 술래 스킬 기본 동작 및 NPC 명령 이벤트 흐름 연결
- 댄스 타임 중 이동 안정화(중력/이동 제한 충돌 완화)
- 시체 위치 보간 안정화
- 게임 종료 판정 신뢰도 개선
- 오디오/씬/UI 일부 정리 완료

**커밋 로그**
- feat(#7): 술래 스킬
- FEAT/7 (#17)
- bug(#20): 댄스 시간 움직임 정상화
- bug(#20): 댄스 시간 중력 적용
- bug(#20): 플레이어 이동 버그 재해결
- FIX/4 (#18)
- FIX/21 (#24)
- bug(#27): 시체 보간
- BUG/27 (#28)
- bug: 게임 종료 버그 개선
- fix(#15): Next Dance UI 제거
- FIX/19 (#25)
- CHORE/22 (#23)
- docs 업데이트
 
**테스트 결과**
- 댄스 타임 중 이동/중력 동작 확인
- 사망 시 시체 위치 보간 확인
- 1v1 종료 조건 판정 확인
- 술래 스킬 이벤트 동작 확인

**링크**
- https://github.com/RRRayer/GGJ26/pull/17
- https://github.com/RRRayer/GGJ26/pull/18
- https://github.com/RRRayer/GGJ26/pull/23
- https://github.com/RRRayer/GGJ26/pull/24
- https://github.com/RRRayer/GGJ26/pull/25
- https://github.com/RRRayer/GGJ26/pull/28

**다음 주 계획**
- 로비/방 목록/방 만들기/대기방 UI 플로우 완성
- 방 비밀번호/모드/최대인원 정책을 세션 속성으로 정리
- 종료/퇴장/씬 전환 공통 핸들러 정리 및 멀티 QA 확대

## Week 9 (26.2.16 - 2.22)
**작업 내역**
- 술래 UI/스킬 UI 개선 및 쿨타임 표시 추가
- 술래 스킨 커스터마이징 기능 구현(스크립트 + 씬/프리팹/리소스 반영)
- Photon Voice2 도입 및 근접 보이스 테스트 환경 구성
- 보이스 설정 보정(AppId/오디오 설정/오디오 믹서 연결)
- 로비 방 생성 플로우 개선(공개방/방 생성 UX 보강)
- 사보타지 연막 이펙트 지속시간/페이드아웃 버그 수정
- 문서 업데이트(커스터마이징, 타이틀 UI Flow, 너구리 모드 상세화)
- 기타 안정화(Seeker Canvas on/off 관련 표시 버그 수정, 환경/패키지 정리)

**AI 활용**
- 술래 스킨 변경 구조(네트워크 상태값-UI-외형 반영) 설계/구현 보조
- Photon Voice2 연동 시나리오 및 믹서 라우팅 점검 보조
- 로비 방 생성 UX 개선안 및 데이터 흐름 정리 보조
- 문서 구조화 및 기획 항목 상세화 보조

**완료 기능**
술래 스킨 변경 기능 동작(선택/적용/표시)
술래 스킬 UI 쿨타임 표시 동작
근접 보이스 기본 동작 + 믹서 연동 완료
로비 방 생성 플로우 개선 반영
연막 이펙트 시간/페이드아웃 안정화
Seeker Canvas 표시 관련 버그 수정

**커밋 로그**
- feat(#10): 개발 환경 변경
- feat(#26): 술래 ui 변경
- feat(#26): 술래 스킬 ui 쿨타임 표시
- docs: 기획 추가(커스터마이징, 타이틀화면 UI flow)
- docs: 너구리 모드 기획 상세화
- feat: photon voice2 에셋 추가
- feat(#10): 술래 스킨 변경 기능 추가
- feat(#10): 술래 스킨 변경 기능 추가 (스크립트 외)
- feat: 근접 보이스 테스트 추가
- feat: parraelSync 패키지 추가
- fix: app in voice 설정 변경
- fix: 클론 프로젝트 gitignore
- fix: 와일드카드 변경
- fix: 소리 설정 변경
- feat: 보이스 오디오 믹서에 연결
- fix: seeker canvas 끔
- fix: 술래 캔버스 안보이던 현상 수정
- feat(#29): 로비 방 생성 개선
- bug: 연막 공통 지속 시간, 패이드 아웃 추가
- Merge branch 'main' into Feat/Customize/#10

**테스트 결과**
- 술래 스킨 선택/반영 및 UI 연동 확인
- <img width="1502" height="848" alt="image" src="https://github.com/user-attachments/assets/337d1633-aee9-400e-abff-4816e95ebd96" />
- 술래 스킬 쿨타임 UI 표시 확인
- <img width="1501" height="848" alt="image" src="https://github.com/user-attachments/assets/67cc8b2e-c52f-4b9a-b739-1afef350f0fc" />
- 근접 보이스 송수신/거리 감쇠/오디오 믹서 라우팅 확인
- 로비 방 생성/진입 플로우 확인
- 연막 이펙트 지속시간 및 페이드아웃 동작 확인

**링크**
- https://github.com/RRRayer/GGJ26/pull/30
- https://github.com/RRRayer/GGJ26/pull/33

**다음 주 계획**
- 보이스/로비/커스터마이징 통합 QA 및 예외 케이스 정리
- 게임 결과/퇴장/재매칭 플로우 안정화
- 스폰/동기화 관련 잔여 버그 정리 및 멀티 테스트 확대

## Week 10 (26.2.23 - 3.1)
**작업 내역**
- 술래 스킨 변경 UI 흐름 보완 및 재적용 작업 진행
- 가면 변경 연출(폭죽 효과) 추가
- 커스터마이징 브랜치 롤백/재정비 후 재업로드
- 너구리 맵 프리팹 추가 및 아트 리소스 반영
- UI 리소스 추가 및 UI 캔버스 구조 정리(퍼즈 캔버스 포함)
- 단체 댄스 피드백 개선
- 비밀방(패스워드 룸) 버그 수정
- 사보타지 기능 구현(스크립트 + 씬/프리팹/세팅 반영)
- 의존성 정리(PUN/Chat 제거) 및 Windows 빌드 프로파일 정비

**AI 활용**
- 사보타지 기능(입력/상태/실행) 구조 설계 및 구현 보조
- 술래 스킨 UI/외형 반영 충돌 원인 점검 및 재적용 보조
- UI 캔버스 구조 정리와 이벤트 흐름 점검 보조
- 비밀방 버그 및 로비 플로우 예외 처리 검토 보조

**완료 기능**
- 사보타지 기능 동작 베이스 완성
- 술래 스킨 변경 UI 개선안 반영
- 가면 변경 폭죽 연출 적용
- 비밀방 버그 해결
- UI 캔버스 구조 안정화 및 리소스 반영
- 사용하지 않는 네트워크 의존성 제거 및 빌드 프로파일 정리

**커밋 로그**
- art: 가면 변경 연출(폭죽 효과) 추가
- feat(#29): 술래 스킨 변경 UI 수정
- FEAT/30 (#10)
- feat: 너구리 맵 프리팹 추가
- Revert "Feat/customize/#10"
- Merge pull request #36 from RRRayer/revert-30-Feat/Customize/#10
- FEAT/8 (#33)
- feat(#10): 재업로드
- docs: 사보타지, 마이크 기획 수정
- FEAT/10 (#38)
- art: UI 리소스 추가
- feat: 퍼즈 캔버스 추가
- fix: ui캔버스 구조 수정
- feat(#9): 단체 댄스 피드백 수정
- FEAT/9 (#43)
- fix(#42): 비밀방 버그 해결
- chore(#41): Remove unused dependencies (PUN, Chat)
- chore: Add new build profile for resizable Windows application
- CHORE/41 (#44)
- feat(#46): 사보타지 구현
- feat(#46): 사보타지 구현 (스크립트 외)

**테스트 결과**
- 사보타지 입력/발동/소모 흐름 확인
- <img width="3439" height="1439" alt="image" src="https://github.com/user-attachments/assets/b394eb6a-3813-491e-9a15-7b820a29c3aa" />
- 가면 변경 폭죽 연출 동작 확인
- <img width="1499" height="848" alt="image" src="https://github.com/user-attachments/assets/bcfbdde5-dfcb-4c48-825a-2993d4280720" />
- 술래 스킨 선택 UI 반영 확인
- 비밀방 생성/입장/검증 흐름 확인
- UI 캔버스 표시/전환 동작 확인

**링크**
- https://github.com/RRRayer/GGJ26/pull/43
- https://github.com/RRRayer/GGJ26/pull/44
- https://github.com/RRRayer/GGJ26/pull/45
- https://github.com/RRRayer/GGJ26/pull/48

**다음 주 계획**
- 사보타지 밸런스/쿨타임/네트워크 동기화 안정화
- 커스터마이징/로비/게임씬 연결부 QA 강화
- UI/오디오/이펙트 통합 테스트 및 잔여 버그 정리

## Week 11 (26.3.2 - 3.8)
**작업 내역**
- 테스트/CI 관련 정리
- Unity 테스트 코드 제거 및 테스트 브랜치 정리
- Unity CD 자동 배포 워크플로 추가 및 버전 문자열 처리 수정
- Unity SmartMerge 설정 스크립트와 .gitattributes 추가
- 역할/스폰/시체 처리 보정
- 호스트 역할 고정 문제 수정 및 플레이어 역할 할당 로직 수정, 최소 플레이어 수 조정
- 시체 2차 보간 보
- 설정/옵션 UI 확장
- 설정 패널/슬라이더/사운드 프리팹 수정
- 설정 패널 입력 처리 개선
- 해상도 및 윈도우 모드 설정 기능 구현
- 보이스 설정 및 런타임 제어 기능 구현
- Photon Voice 데모 폴더 삭제 및 관련 정리
- UI/아트/사보타지 기능 추가
- 사보타지 및 술래 스킬 UI용 이미지 리소스 추가
- Crazy Dance 관련 아트/에셋 추가
- 사보타지 UI 스크립트 구현 및 씬/프리팹 반영
- 신발 던지기용 3D 신발 프리팹 추가
- 로비/방 생성/대기실 UI 개선
- 방 만들기 UI 및 로비 생성 흐름 개선
- 로비 UI 개선
- 대기실 UI 개선 및 WaitingRoom 상태/UI 스크립트 보강
- 술래 미리보기용 PlayerArmature_SeekerPreview 프리팹 추가

**AI 활용**
- 역할 할당 로직과 호스트 고정 버그 원인 분석 및 수정안 검토
- 설정 UI/보이스 설정/입력 처리 구조 정리 보조
- 사보타지 HUD 구조 및 씬 반영 흐름 설계 보조
- 로비/방 생성/대기실 UI 개편 과정에서 스크립트-씬 연결 구조 검토
- CI/CD 및 Unity 협업 설정(SmartMerge) 정리 방향 검토 보조

**완료 기능**
- 호스트 술래 고정 현상 완화 및 역할 할당 로직 보정
- 시체 보간 2차 수정 반영
- 해상도/윈도우 모드/보이스 설정 기능 구현
- 사보타지 UI 및 신발 투사체 리소스 반영
- 로비 방 생성 UX와 대기실 UI 구조 개선
- Unity CD 워크플로 및 SmartMerge 협업 설정 추가

**커밋 로그**
- feat(#46): 테스트 코드 제거
- FEAT/46 (#48)
- fix(#39): 호스트 역할 고정 해결
- FIX/39 (#49)
- fix(#27): 시체 2차 보간
- feat : 설정, 패널 ui 프리팹 추가
- fix : 씬충돌 해결
- Revert "Squashed commit of the following:"
- Feat/37 (#45)
- fix(#39): 플레이어 역할 할당 로직 수정 및 최소 플레이어 수 조정
- Fix/39 (#52)
- art: 사보타지 '신발던지기' 에 던질 신발 3D 추가함
- fix : 포톤 보이스 데모 폴더 삭제
- feat : setting 프리팹 수정
- feat : setting패널 입력 처리
- feat : 해상도, 윈도우 모드 설정 기능 구현
- feat : 보이스 설정 구현
- art: 사보타주 및 술래스킬 이미지 추가(피그마에 UI 올려놓음)
- art: Crazy dance 수정
- feat(#47): 사보타지 UI 구현
- feat(#47): 사보타지 UI 구현 (스크립트 외)
- FEAT/47 (#53)
- feat(#42): 방 만들기 개선
- feat: 로비 UI 개선
- feat: 대기실 UI 개선
- feat(#37) : Add Unity CD workflow for automated releases
- Feat/37 (#54)
- fix(#37) : Fix versioning case sensitivity in unity-cd.yml
- chore(#55) : Add Unity SmartMerge setup script and gitattributes
- CHORE/55 (#56)

**테스트 결과**
- 사보타지 HUD 표시 및 신발 프리팹 반영 확인
- <img width="1150" height="645" alt="image" src="https://github.com/user-attachments/assets/49f790dd-cafa-4a38-84aa-1c867d369bb7" />
- 로비 방 만들기/로비 UI/대기실 UI 개편 내용 확인
- <img width="1433" height="804" alt="image" src="https://github.com/user-attachments/assets/db7ebf07-49b1-4cf4-94f3-41909943198b" />
- 역할 할당 로직 및 호스트 고정 현상 수정 후 멀티플레이 검증 진행
- 시체 보간 재수정 후 사망 연출 위치 확인
- 설정 패널에서 해상도/창 모드/보이스 옵션 동작 확인
- Unity CD 워크플로 및 SmartMerge 설정 파일 반영 확인

**링크**
- https://github.com/RRRayer/GGJ26/pull/48
- https://github.com/RRRayer/GGJ26/pull/49
- https://github.com/RRRayer/GGJ26/pull/52
- https://github.com/RRRayer/GGJ26/pull/53
- https://github.com/RRRayer/GGJ26/pull/54
- https://github.com/RRRayer/GGJ26/pull/56

**다음 주 계획**
- 역할 할당/라운드 전환/종료 판정 관련 잔여 멀티플레이 버그 정리
- 로비-대기실-게임씬 UI 흐름 안정화
- 사보타지 기능 네트워크 동기화 및 밸런스 보정
- 설정/보이스/배포 파이프라인 통합 QA 진행

![Alt](https://repobeats.axiom.co/api/embed/9a6afe0594d2ed6b72ceaacf76f810f29dd2a5ec.svg "Repobeats analytics image")
