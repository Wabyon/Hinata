/*
配置後スクリプト テンプレート							
--------------------------------------------------------------------------------------
 このファイルには、ビルド スクリプトに追加される SQL ステートメントが含まれています。		
 SQLCMD 構文を使用して、ファイルを配置後スクリプトにインクルードできます。			
 例:      :r .\myfile.sql								
 SQLCMD 構文を使用して、配置後スクリプト内の変数を参照できます。		
 例:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

MERGE INTO dbo.Users Main
USING (
	VALUES (
		'aed01969-f082-41ae-9ff8-302eb409aa53',
		'user1',
		'user1@test.com',
		1,
		'AMTEji10cZyPnLzLHYg+/Y6a7ZosVrxhyTao6g8rH/tqpSXmj3/ladbo0jpfe6ffxg==', --P@ssw0rd
		'0001/01/01',
		0,
		0
	), (
		'6d0f8d43-43f1-4779-a2e6-ef08b34e216d',
		'user2',
		'user2@test.com',
		1,
		'AMTEji10cZyPnLzLHYg+/Y6a7ZosVrxhyTao6g8rH/tqpSXmj3/ladbo0jpfe6ffxg==', --P@ssw0rd
		'0001/01/01',
		0,
		0
	), (
		'8cb5d058-e80e-42d7-bb8e-60ff321038e9',
		'user3',
		'user3@test.com',
		1,
		'AMTEji10cZyPnLzLHYg+/Y6a7ZosVrxhyTao6g8rH/tqpSXmj3/ladbo0jpfe6ffxg==', --P@ssw0rd
		'0001/01/01',
		0,
		0
	)
) Sub (
	UserId,
	UserName,
	Email,
	EmailConfirmed,
	PasswordHash,
	LockoutEndDateUtc,
	LockoutEnabled,
	AccessFailedCount
)
ON  Main.UserId = Sub.UserId
WHEN MATCHED THEN
UPDATE
SET Main.UserName = Sub.UserName,
	Main.EMail = Sub.EMail,
	Main.EmailConfirmed = Sub.EmailConfirmed,
	Main.PasswordHash = Sub.PasswordHash,
	Main.LockoutEndDateUtc = Sub.LockoutEndDateUtc,
	Main.LockoutEnabled = Sub.LockoutEnabled,
	Main.AccessFailedCount = Sub.AccessFailedCount
WHEN NOT MATCHED THEN
INSERT VALUES (
	Sub.UserId,
	Sub.UserName,
	Sub.Email,
	Sub.EmailConfirmed,
	Sub.PasswordHash,
	Sub.LockoutEndDateUtc,
	Sub.LockoutEnabled,
	Sub.AccessFailedCount
);
