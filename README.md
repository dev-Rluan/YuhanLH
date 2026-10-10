# YuhanLH

2021년 유한대학교 캡스톤 프로젝트 ‘유한등대’의 C# 소켓 서버 구현 기록입니다. 학생·교수의 연결, 수업별 Room, 세션과 수업 데이터를 처리합니다.

## 담당 범위와 구성

| 경로 | 역할 |
|---|---|
| [Server](Server/) | 수업 Room, 사용자 세션, 패킷과 DB 처리 |
| [ServerCore](ServerCore/) | 공통 소켓 통신·송수신 버퍼 |
| [PacketGenerator](PacketGenerator/) | 패킷 코드 생성 |
| [PClient](PClient/) · [SClient](SClient/) | 교수·학생 통신 클라이언트 |
| [DummyClient](DummyClient/) | 통신 실험용 클라이언트 |
| [Protocol.xlsx](Protocol.xlsx) | 프로토콜 자료 |

별도 클라이언트 프로젝트는 [yh_clientTeam](https://github.com/KOHYEONJEONG/yh_clientTeam), 기존 주요 기능 자료는 [shc 브랜치](https://github.com/KOHYEONJEONG/yh_clientTeam/tree/shc)에 있습니다.

## 코드 확인과 실행 준비

[Server.sln](Server.sln)을 솔루션으로 열어 프로젝트 구성을 확인합니다. 서버 프로젝트는 `net5.0`을 대상으로 하며 Oracle 연동 패키지를 사용합니다. DB·네트워크 설정과 기존 개발 환경을 준비한 뒤 실행해야 합니다.

수업 Room 생성 방식 변경 등 기존 개선 메모는 개발 당시 계획으로 보관합니다. 이 문서는 현재 운영 중인 수업 서비스를 뜻하지 않습니다.

## 프로토콜과 개발 당시 계획

![프로토콜 기록](https://user-images.githubusercontent.com/73861946/144662179-6dd8c2d7-97b5-4a3f-b908-06edbbcd9fb6.png)

기존 README에는 로그인·방 입장 요청 시 수업 시간에 맞춰 Room을 만드는 방식을 교수 기준 생성으로 바꾸려는 계획이 기록되어 있습니다. 이 항목은 개발 당시 계획으로 구분합니다.

## 코드 구조

![image](https://user-images.githubusercontent.com/73861946/197353990-34fe2ae0-d0f3-41f0-87c9-923cadffba4c.png)

## 주요 개발 코드

### 콘솔 프로그램 + Socket 통신

 * ![image](https://user-images.githubusercontent.com/73861946/141686016-addfc4f3-3d79-4684-9e02-8766fd4b5786.png)
 * 서버 가동
- 비동기 방식 사용하여 데이터 통신관리
  * ![image](https://user-images.githubusercontent.com/73861946/141670455-93808b18-55bc-49f3-a4a5-a40b0806044e.png)
- ServerCore라는 클래스 라이브러리에 공통부분 작성
- 세션을 통해서 사용자의 정보를 관리한다.
 * session 이라는 클래스에 통신을 위한 메소드 정의 
 * ![image](https://user-images.githubusercontent.com/73861946/197354294-697af6ac-d886-4c69-a1f7-2158d65f0082.png)
  + 연결된 소켓 저장, 네트워크 통신을 위한 메서드 정의
  + SendBuffer, RecvBuffer를 사용하여 송, 수신 데이터 전송
 * 서버는 ClientSession, 클라이언트는 ServerSession 이라는 클래스에 session 클래스를 상속받아서 각자 접속한 유저와 서버와의 통신을 관리한다.
  + 세션 안에는 객체( 유저, 서버 )의 정보를 기록한다.
  + ![image](https://user-images.githubusercontent.com/73861946/197354331-3ec3ca28-c636-4433-b6ea-dcebe2ad39d2.png)
  + ↑ 각 유저의 정보(ClientSession)


### SessionManager : 전체 세션을 의미한다. 

 * 모든 정보처리를 담당한다. 
  + 서버에 처음 접속하는 객체는 세션 번호를 key로 가진 Dictionary에 저장한다.
  + 데이터 손실이 일어날만한 곳은 lock을 사용하여 처리한다.
  + https://github.com/dev-Rluan/YuhanLH/blob/main/Server/Session/SessionManager.cs
- classroom : class(수업) 하나를 나타낸다.
 *  수업, 교수, 학생들의 세션 정보를 가지며 이벤트를 위한 메서드들을 가지고 있다.
 *  https://github.com/dev-Rluan/YuhanLH/blob/main/Server/ClassRoom.cs

 
### PacketManager : 패킷을 관리하는 클래스

- XML 파일에 패킷 구조를 정의하고 xml파일을 읽어들여 미리 정의 한 string Format형식으로 패킷을 정의하고 읽고 쓸수있는 클래스 생성
 * ![image](https://user-images.githubusercontent.com/73861946/197355206-0a6d1752-d110-4bf9-8a36-86b2e21fac70.png)
 * 완성된 패킷 클래스
 * ![image](https://user-images.githubusercontent.com/73861946/141686504-c75de10a-c30b-4a75-9ff9-03209b644e83.png)
 * (전체 구조는 너무 많기 때문에 생략하였습니다 - PacketGenerator: Program.cs, Server : GenPacket.cs)
 * 전체 패킷 핸들러 함수 저장과 그것을 실행하는 Action을 Dictionary에 저장
 * ![image](https://user-images.githubusercontent.com/73861946/141686327-b3286c0b-79e2-4747-b6af-4a76191b62e8.png)
 * 핸들러 추가
 * ![image](https://user-images.githubusercontent.com/73861946/197355142-1a6447b2-cce4-4f40-a872-4091d3c1a4b1.png)
 * 서버로 데이터가 Recv되었을때 호출하여 패킷을 검사하는 메서드
 * ![image](https://user-images.githubusercontent.com/73861946/197355163-ef48d879-8aa9-45b4-9e3d-f99d37c05fde.png)
 * 핸들러 함수가 실행하는 함수들
 * ![image](https://user-images.githubusercontent.com/73861946/197355176-207c5420-d132-46d3-8fb1-a047a0b0cf49.png)



핸들러 함수까지오면 클라이언트측에서 데이터 관리한다.

## 데이터베이스 연동 기록

현재 [Server/DB/Database.cs](Server/DB/Database.cs)는 Oracle 클라이언트를 사용합니다. 아래 이미지와 설명은 기존 개발 기록으로 보관합니다.
- Database 객체 초기화해서 사용
- ![image](https://user-images.githubusercontent.com/73861946/197355255-94c8f6de-b93a-4562-9ed7-da0bd5fb2385.png)
- ![image](https://user-images.githubusercontent.com/73861946/197355262-abe5746e-a72f-4fb4-aa66-34d013626022.png)
- https://github.com/dev-Rluan/YuhanLH/blob/main/Server/DB/Database.cs


## 발표 자료

<details>
<summary>캡스톤 발표 슬라이드와 화면 기록</summary>

![yuhanCap1](https://user-images.githubusercontent.com/73861946/144713109-1ada90ec-c453-4b53-9f07-40a03d979d04.png)
![yuhanCap2](https://user-images.githubusercontent.com/73861946/144713111-deae79ed-c8d4-498a-9800-18f8e513ee47.png)
![yuhanCap3](https://user-images.githubusercontent.com/73861946/144713112-4e59818e-4727-40a7-99fa-c802879aaa63.png)
![yuhanCap4](https://user-images.githubusercontent.com/73861946/144713115-d11196db-23db-4253-b5c2-b2bf87391ff6.png)
![yuhanCap5](https://user-images.githubusercontent.com/73861946/144713117-c4fac784-1fcd-4af4-aac5-8e5b92d274a7.png)
![yuhanCap6](https://user-images.githubusercontent.com/73861946/144713180-98244aea-12f7-4dfa-b697-3c4f146549ac.png)
![yuhanCap7](https://user-images.githubusercontent.com/73861946/144713183-4e6fb48f-45c9-4bd2-86dd-cc0fbdf3cf20.png)
![yuhanCap8](https://user-images.githubusercontent.com/73861946/144713124-64062bf5-f9b2-489b-8db8-225b8a353f10.png)
![yuhanCap9](https://user-images.githubusercontent.com/73861946/144713132-d52b3f30-1741-400c-8f61-f2067204237f.png)
![yuhanCap10](https://user-images.githubusercontent.com/73861946/144713133-b8ac0a11-67e8-4b41-94a0-2560a14afed5.png)
![yuhanCap11](https://user-images.githubusercontent.com/73861946/144713137-6e266d23-7720-4865-a6b6-f1adcbd01b79.png)
![yuhanCap12](https://user-images.githubusercontent.com/73861946/144713138-76bb9386-ac7e-44d0-b081-7cc1a74db72d.png)
![yuhanCap13](https://user-images.githubusercontent.com/73861946/144713139-944ca13e-65a1-4a44-b5e5-6b9d9dde93a1.png)
![yuhanCap14](https://user-images.githubusercontent.com/73861946/144713140-d8a1ee14-e580-4bba-ad73-9aa4b5641103.png)
![yuhanCap15](https://user-images.githubusercontent.com/73861946/144713142-5592309f-e437-4297-8e5d-f49fd1589da7.png)
![yuhanCap16](https://user-images.githubusercontent.com/73861946/144713143-520915c5-b073-4f9f-af1f-424dc1367d40.png)
![yuhanCapServer](https://user-images.githubusercontent.com/73861946/144753968-186442a2-2c2c-48a5-99c7-0dfdaca7ee52.png)
![yuhanCapPacket](https://user-images.githubusercontent.com/73861946/144753971-78752a5b-f08a-48ad-ab5a-381e989debce.png)
![PowerPoint 슬라이드 쇼  -  YuhanCapston(Server) pptx 2021-12-05 오전 4_24_04](https://user-images.githubusercontent.com/73861946/144723122-f4ed43e9-e2f3-4d96-98df-c1d3bcf7e741.png)
![yuhanCap20](https://user-images.githubusercontent.com/73861946/144753884-b8a1b433-7a76-49ef-8bf8-b933dfc2183c.png)

</details>
