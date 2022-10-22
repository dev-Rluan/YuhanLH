# YuhanLH

유한대학교 2021캡스톤 디자인 프로젝트 '유한등대' 

Server
- C#을 사용한 소켓 통신 프로그램을 이용하여 유저(학생, 교수)간의 통신과 데이터 처리를 담당
- 기초적인 서버 프로그램 로직은 먼저 올려놓은 게임서버로직과 비슷함
- 수업마다 다른 Room 생성  
- 현재 프로토콜 목록
- ![image](https://user-images.githubusercontent.com/73861946/144662179-6dd8c2d7-97b5-4a3f-b908-06edbbcd9fb6.png)
- 세션관리를 통한 유저의 정보 관리
  * 현재 학생이든 교수이던 로그인 또는 방 입장요청이 룸생성 전에 먼저 들어오면 시간에 맞춰서 방을 생성해주는 로직인데 이부분을 교수자에 따라서 방을 생성해주는 로직으로 변경 할 예정이다.

주요기능 
[https://github.com/KOHYEONJEONG/yh_clientTeam/tree/shc#%EC%A3%BC%EC%9A%94-%EA%B8%B0%EB%8A%A5](https://github.com/KOHYEONJEONG/yh_clientTeam/tree/shc)

DB
- oracle을 사용하여 구성
- 학생, 교수, 수업정보, 수업, 출석정보, 로그인 테이블 등으로 구성
- 서버에서 동작하여 데이터 처리를 돕는다.


Client
- PClient - 교수 프로그램
- SCient - 학생 프로그램
- Client 프로그램 - https://github.com/KOHYEONJEONG/yh_clientTeam

PPT 정리
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
















