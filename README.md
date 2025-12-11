<img width="1411" height="793" alt="시작" src="https://github.com/user-attachments/assets/733f961a-78ed-4ba7-83f1-4ac849ff2622" />
<br>
<h1>유니티 게임 포트폴리오</h1>


> 유니티 6.0 사용<br>
> 주변의 물체를 밀어내 점수를 얻는 게임<br>
> 기존(2023년)에 만들었던 미니게임을 개선 및 기능 적용<br>
> 선택한 경로의 Excel 파일을 로드하여 Json으로 변환 기능<br>
> Addressable 사용하여 UI 등 프리팹 로드<br>
> EventBus 사용하여 옵저버 패턴 구현<br>
> UniRx, UniTask 사용<br>

<br><br>

<h3>로비</h3>
<img width="1414" height="795" alt="로비" src="https://github.com/user-attachments/assets/bdf263f3-0257-4d15-b1c3-e9c349706354" />
플레이할 캐릭터를 표기해 줍니다.

<br>

<h3>캐릭터 변경</h3>
<img width="1415" height="795" alt="플레이어 정보창" src="https://github.com/user-attachments/assets/e6d3235d-a2d4-4a73-8a77-1798ca6a5c3a" />
<img width="1416" height="802" alt="캐릭터 변경" src="https://github.com/user-attachments/assets/bc24d5de-1122-4bfb-8004-382675c3f37d" />
플레이어 정보 화면에서 선택 시 캐릭터 변경 가능합니다.

<br>
<h3>게임 플레이</h3>
- 이동 : WSAD로 상하좌우 이동 가능
- 점프 : 스페이스로 점프 가능
- 마우스 : 이동에 따라 캐릭터 및 카메라 회전
- 스킬 : Q 키를 누른상태에서 게이지 생성 -> 키를 뗄 때의 수치를 반영하여 주변에 충격파 발생
- 게임 목적 : 제한시간 동안 랜덤으로 생성되는 물건을 밖으로 많이 떨어뜨려 점수를 획득

게이지 표기
<img width="1415" height="797" alt="게이지" src="https://github.com/user-attachments/assets/f9c7201a-ff69-4e6f-a0da-294e0872ed4c" />
<br>
물체 충격파 효과
<img width="1415" height="798" alt="충격파" src="https://github.com/user-attachments/assets/93aee2ee-37ec-4ee7-bec3-b8f69c6b1f18" />
결과
<img width="1416" height="795" alt="결과화면" src="https://github.com/user-attachments/assets/8775cac1-6bc3-4be3-9bd6-7a6e1e862854" />
