DROP INDEX IF EXISTS idx_user_username		ON dbo.Users;
DROP INDEX IF EXISTS idx_user_email			ON dbo.Users;
DROP INDEX IF EXISTS idx_user_online		ON dbo.Users;
DROP INDEX IF EXISTS idx_password_userid	ON dbo.UserPasswords;
DROP INDEX IF EXISTS idx_login_userid_type	ON dbo.UserLogins;
DROP INDEX IF EXISTS idx_mail_sender		ON dbo.Mails;
DROP INDEX IF EXISTS idx_mail_receiver		ON dbo.Mails;
DROP INDEX IF EXISTS idx_mail_both_user		ON dbo.Mails;
DROP INDEX IF EXISTS idx_mail_parent		ON dbo.Mails;

DROP TABLE IF EXISTS dbo.Friends;
DROP TABLE IF EXISTS dbo.FriendRequests;
DROP TABLE IF EXISTS dbo.Mails;
DROP TABLE IF EXISTS dbo.UserPasswords;
DROP TABLE IF EXISTS dbo.UserLogins;
DROP TABLE IF EXISTS dbo.Users;

DROP TRIGGER IF EXISTS set_null_addressee_id;
DROP TRIGGER IF EXISTS set_mail_subject_if_reply;

DROP VIEW IF EXISTS UsersWithPassword;

DROP PROCEDURE IF EXISTS add_new_user;
DROP PROCEDURE IF EXISTS add_new_mail;
DROP PROCEDURE IF EXISTS delete_mail;
DROP PROCEDURE IF EXISTS mail_delete_children;
DROP PROCEDURE IF EXISTS new_friend_req;
DROP PROCEDURE IF EXISTS accept_friend_req;
DROP PROCEDURE IF EXISTS update_user;


CREATE TABLE [Users] (
	ID INT IDENTITY NOT NULL,
	Username VARCHAR(32) NOT NULL,
	EmailAddr VARCHAR(64) NOT NULL,
	RegisterDate DATETIME DEFAULT GETDATE(),
	UserStatus INT NOT NULL DEFAULT 0,

	CONSTRAINT pk_user_id PRIMARY KEY(ID),
	CONSTRAINT unique_username UNIQUE(Username),
	CONSTRAINT unique_email UNIQUE(EmailAddr),
	CONSTRAINT check_status CHECK(UserStatus = 0 OR UserStatus = 1) -- 0: offline, 1: online
);

CREATE TABLE [UserPasswords] (
	ID INT IDENTITY NOT NULL,
	UserID INT NOT NULL,
	PasswordHash VARCHAR(64) NOT NULL,
	CreatedAt DATETIME DEFAULT GETDATE(),

	CONSTRAINT pk_pwd_id PRIMARY KEY(ID),
	CONSTRAINT fk_pwd_userid 
		FOREIGN KEY(UserID) REFERENCES Users(ID) 
		ON DELETE CASCADE 
		ON UPDATE CASCADE
);

CREATE TABLE [UserLogins] (
	ID INT IDENTITY NOT NULL,
	UserID INT NOT NULL,
	LoginType INT NOT NULL,
	LoginDate DATETIME DEFAULT GETDATE(),

	CONSTRAINT pk_login_id PRIMARY KEY(ID),
	CONSTRAINT fk_login_userid
		FOREIGN KEY(UserID) REFERENCES Users(ID) 
		ON DELETE CASCADE 
		ON UPDATE CASCADE,
	CONSTRAINT check_type CHECK(LoginType = 0 OR LoginType = 1) /* 0 == Login, 1 == Logout */
);

CREATE TABLE [FriendRequests] (
	ID INT IDENTITY NOT NULL,
	User1 INT NOT NULL, -- õ kérte
	User2 INT NOT NULL,
	Accepted INT DEFAULT 0,
	RequestDate DATETIME DEFAULT GETDATE(),
	
	CONSTRAINT pk_friend_requests_id PRIMARY KEY(ID),
	CONSTRAINT fk_fr_user1
		FOREIGN KEY(User1) REFERENCES Users(ID) 
			ON DELETE CASCADE,
	CONSTRAINT fk_fr_user2
		FOREIGN KEY(User2) REFERENCES Users(ID)
			ON DELETE NO ACTION,
	CONSTRAINT check_accepted_status 
		CHECK(Accepted = 0 OR Accepted = 1)
);

CREATE TABLE [Friends] (
	ID INT IDENTITY NOT NULL,
	User1 INT NOT NULL,
	User2 INT NOT NULL,
	RelatedRequest INT NOT NULL,
	Since DATETIME DEFAULT GETDATE(),

	CONSTRAINT pk_friends_id PRIMARY KEY(ID),
	CONSTRAINT fk_user1
		FOREIGN KEY(User1) REFERENCES Users(ID)
			ON DELETE CASCADE,
	CONSTRAINT fk_user2
		FOREIGN KEY(User2) REFERENCES Users(ID)
			ON DELETE NO ACTION,
	CONSTRAINT fk_friend_req
		FOREIGN KEY(RelatedRequest) REFERENCES FriendRequests(ID)
);

CREATE TABLE [Mails] (
	ID INT IDENTITY NOT NULL,
	SenderID INT DEFAULT NULL,
	AddresseeID INT DEFAULT NULL,
	MailSubject VARCHAR(128) DEFAULT 'No subject',
	MailBody VARCHAR(8000) NOT NULL,
	ParentMail INT DEFAULT NULL,
	MailReadStatus INT DEFAULT 0,
	MailDate DATETIME DEFAULT GETDATE(),

	CONSTRAINT pk_mail_id PRIMARY KEY(ID),
	CONSTRAINT fk_senderid
		FOREIGN KEY(SenderID) REFERENCES Users(ID)
		ON DELETE CASCADE, -- ha a küldõ user törlõdik, akkor törlõdik az üzenet is, TODO: TRIGGER
	CONSTRAINT fk_addresseeid
		FOREIGN KEY(AddresseeID) REFERENCES Users(ID)
		ON DELETE NO ACTION, /* SET NULL nem mûködik, szóval trigger kell rá */
	CONSTRAINT fk_parentmail
		FOREIGN KEY(ParentMail) REFERENCES Mails(ID)
		ON DELETE NO ACTION /* itt is */
);
GO


CREATE PROCEDURE new_friend_req(
	@user1 AS INT,
	@user2 AS INT
)
AS
BEGIN
	DECLARE
		@alreadyFriends   AS INT,
		@alreadyRequested AS INT;
		
	SET @alreadyFriends   = (SELECT COUNT(*) FROM Friends WHERE (User1 = @user1 AND User2 = @user2) OR (User1 = @user2 AND User2 = @user1));
	SET @alreadyRequested = (SELECT COUNT(*) FROM FriendRequests WHERE Accepted = 0 AND ((User1 = @user1 AND User2 = @user2) OR (User1 = @user2 AND User2 = @user1)));

	if @alreadyFriends != 0
		RETURN -1;

	if @alreadyRequested != 0
		RETURN -1;

	INSERT INTO FriendRequests(User1, User2) VALUES(@user1, @user2);
	RETURN 0;
END;
GO

CREATE PROCEDURE accept_friend_req(
	@id AS INT
)
AS
BEGIN
	DECLARE
		@user1 AS INT,
		@user2 AS INT;

	SELECT
		@user1 = User1,
		@user2 = User2
	FROM
		FriendRequests
	WHERE
		ID = @id;

	BEGIN TRANSACTION
		UPDATE FriendRequests SET Accepted = 1 WHERE ID = @id;
		INSERT INTO Friends(User1, User2, RelatedRequest) VALUES(@user1, @user2, @id);
	COMMIT TRANSACTION
	RETURN 0;
END
GO

CREATE PROCEDURE add_new_user(
	@username AS VARCHAR(32),
	@password AS VARCHAR(64),
	@email    AS VARCHAR(64)
)
AS
BEGIN
	DECLARE @UserID AS INT;

	BEGIN TRANSACTION
		INSERT INTO Users(Username, EmailAddr) VALUES(@username, @email);

		SET @UserID = SCOPE_IDENTITY();

		INSERT INTO UserPasswords(UserID, PasswordHash) VALUES(@UserID, @password);
	COMMIT TRANSACTION
	
	RETURN @UserID;
END
GO

CREATE PROCEDURE add_new_mail(
	@senderid		AS INT,
	@addresseeid	AS INT,
	@subject		AS VARCHAR(128),
	@body			AS VARCHAR(8000),
	@parent			AS INT
)
AS
BEGIN
	IF @subject IS NULL
		INSERT INTO Mails(SenderID, AddresseeID, MailBody, ParentMail) VALUES(@senderid, @addresseeid, @body, @parent);
	ELSE
		INSERT INTO Mails(SenderID, AddresseeID, MailSubject, MailBody, ParentMail) VALUES(@senderid, @addresseeid, @subject, @body, @parent);
	
	RETURN SCOPE_IDENTITY();
END
GO
/*
EXEC add_new_user @username = 'tesztelek', @password = 'qPXxZ/RPSWTmyZje6CcRDA==', @email = 'asd@asd.hu';
EXEC add_new_user @username = 'username1', @password = 'qPXxZ/RPSWTmyZje6CcRDA==', @email = 'asd0@asd.hu';
EXEC add_new_user @username = 'username2', @password = 'qPXxZ/RPSWTmyZje6CcRDA==', @email = 'asd@1asd.hu';
EXEC add_new_user @username = 'username3', @password = 'qPXxZ/RPSWTmyZje6CcRDA==', @email = 'asd2@asd.hu';
EXEC add_new_user @username = 'username4', @password = 'qPXxZ/RPSWTmyZje6CcRDA==', @email = 'asd@asd.h3u';
EXEC add_new_user @username = 'username5', @password = 'qPXxZ/RPSWTmyZje6CcRDA==', @email = 'asd@asd.4hu';
EXEC add_new_user @username = 'username6', @password = 'qPXxZ/RPSWTmyZje6CcRDA==', @email = 'asd@asd5.hu';
EXEC add_new_user @username = 'username7', @password = 'qPXxZ/RPSWTmyZje6CcRDA==', @email = 'asd@as6d.hu';
EXEC add_new_user @username = 'username8', @password = 'qPXxZ/RPSWTmyZje6CcRDA==', @email = 'asd@a7sd.hu';
EXEC add_new_user @username = 'username9', @password = 'qPXxZ/RPSWTmyZje6CcRDA==', @email = 'asd@8asd.hu';
EXEC add_new_user @username = 'username0', @password = 'qPXxZ/RPSWTmyZje6CcRDA==', @email = 'asd9@asd.hu';

EXEC add_new_mail @senderid = 7, @addresseeid = 1, @subject = '', @body = '', @parent = NULL;
EXEC add_new_mail @senderid = 1, @addresseeid = 7, @subject = 'Example subject', @body = '', @parent = 1;
EXEC add_new_mail @senderid = 5, @addresseeid = 1, @subject = 'My subject', @body = '', @parent = NULL;
EXEC add_new_mail @senderid = 2, @addresseeid = 1, @subject = 'My sugfb2ject', @body = '', @parent = NULL;
EXEC add_new_mail @senderid = 1, @addresseeid = 3, @subject = 'My s3ubasdfject', @body = '', @parent = NULL;
EXEC add_new_mail @senderid = 3, @addresseeid = 7, @subject = 'My sufdsbject', @body = '', @parent = NULL;
EXEC add_new_mail @senderid = 6, @addresseeid = 1, @subject = 'My sucvbbject', @body = '', @parent = NULL;

select * from FriendRequests;
SELECT * FROM Users;

EXEC new_friend_req @user1 = 4, @user2 = 1;
EXEC new_friend_req @user1 = 5, @user2 = 1;
EXEC new_friend_req @user1 = 7, @user2 = 1;
EXEC new_friend_req @user1 = 8, @user2 = 1;
EXEC new_friend_req @user1 = 9, @user2 = 1;

EXEC accept_friend_req @id = 1;
EXEC accept_friend_req @id = 2;
EXEC accept_friend_req @id = 3;
EXEC accept_friend_req @id = 4;

INSERT INTO UserPasswords(UserID, PasswordHash) VALUES(1, 'asd')

DELETE FROM Users;

SELECT Users.ID AS ID FROM Users INNER JOIN UserPasswords ON Users.ID = UserPasswords.UserID WHERE Username = 'tesztelek' AND PasswordHash = 'qPXxZ/RPSWTmyZje6CcRDA==' AND UserPasswords.CreatedAt = (SELECT TOP(1) CreatedAt FROM UserPasswords WHERE UserID = Users.ID ORDER BY CreatedAt DESC)

UPDATE
	Users, UserPasswords, 

UPDATE Users SET Users.Username = 'tesztelek', Users.EmailAddr = 'asd@asd.hu', UserStatus = 0, up.PasswordHash = 'qPXxZ/RPSWTmyZje6CcRDA==' FROM Users u INNER JOIN UserPasswords up ON u.ID = up.UserID WHERE u.ID = 1;

EXEC delete_mail @currentid = 4, @startid = 4

DELETE FROM Users WHERE ID = 5

DELETE FROM Mails WHERE ID = 3;
SELECT * FROM UserPasswords*/

CREATE PROCEDURE update_user(
	@username AS VARCHAR(32),
	@email	  AS VARCHAR(64),
	@status	  AS INT,
	@pwdHash  AS VARCHAR(64),
	@id		  AS INT
)
AS
BEGIN
	BEGIN TRANSACTION
		UPDATE 
			Users
		SET
			Username = @username,
			EmailAddr = @email,
			UserStatus = @status 
		WHERE
			ID = @id;

		DECLARE @oldPwd AS VARCHAR(64) = (SELECT TOP(1) PasswordHash FROM UserPasswords WHERE UserID = @id ORDER BY CreatedAt DESC);

		IF (@oldPwd != @pwdHash)
			BEGIN
				INSERT INTO UserPasswords(UserID, PasswordHash) VALUES(@id, @pwdHash);
			END
	COMMIT TRANSACTION
END
GO

CREATE PROCEDURE mail_delete_children(
	@id    AS INT,
	@until AS INT
)
AS
BEGIN
	DECLARE @parent AS INT = (SELECT ParentMail FROM Mails WHERE ID = @id);

	DELETE FROM Mails WHERE ID = @id;

	IF (@parent = NULL OR @id = @until)
		RETURN 0;

	EXEC mail_delete_children @id = @parent, @until = @until;
END
GO

CREATE PROCEDURE delete_mail(
	@currentid AS INT,
	@startid   AS INT
)
AS
BEGIN
	DECLARE 
		@next  AS INT = (SELECT ID FROM Mails WHERE ParentMail = @currentid),
		@count AS INT = (SELECT COUNT(*) FROM Mails WHERE ParentMail = @currentid);
	
	IF @count = 0
		EXEC mail_delete_children @id = @currentid, @until = @startid;
	ELSE
		EXEC delete_mail @currentid = @next, @startid = @startid;
END
GO


CREATE TRIGGER set_null_addressee_id
ON Users
AFTER DELETE
AS
BEGIN
	DECLARE
		@DeletedID AS INT = (SELECT ID FROM deleted),
		@MailID AS INT;

	DECLARE MailListCursor CURSOR FOR (SELECT ID FROM Mails WHERE AddresseeID = @DeletedID);
	OPEN MailListCursor;

	FETCH NEXT FROM MailListCursor INTO @MailID;

	WHILE @@FETCH_STATUS = 0
		BEGIN
			UPDATE [Mails] SET AddresseeID = NULL WHERE ID = @MailID;

			FETCH NEXT FROM MailListCursor;
		END
	CLOSE MailListCursor;
	DEALLOCATE MailListCursor;
END
GO

CREATE TRIGGER set_mail_subject_if_reply
ON Mails
INSTEAD OF INSERT
AS
BEGIN
	DECLARE 
		@Parent AS INT = (SELECT ParentMail FROM inserted),
		@Subject AS NVARCHAR(128) = (SELECT MailSubject FROM inserted);

	IF (@Parent IS NOT NULL)
		SET @Subject = CONCAT('Re: ', (SELECT MailSubject FROM Mails WHERE ID = @Parent));

	INSERT INTO Mails (SenderID, AddresseeID, MailSubject, MailBody, ParentMail, MailDate)
		SELECT
			SenderID, AddresseeID, @Subject, MailBody, ParentMail, MailDate
		FROM inserted;
END
GO


CREATE VIEW UsersWithPassword
AS
	SELECT
		Users.ID AS UserID,
		Users.Username,
		UserPasswords.PasswordHash AS UserPassword,
		UserPasswords.CreatedAt,
		Users.EmailAddr,
		Users.RegisterDate,
		Users.UserStatus
	FROM
		Users
	INNER JOIN
		UserPasswords
		ON Users.ID = UserPasswords.UserID;
GO


CREATE INDEX idx_user_username		ON Users(Username);
CREATE INDEX idx_user_email			ON Users(EmailAddr);
CREATE INDEX idx_user_online		ON Users(UserStatus);

CREATE INDEX idx_password_userid	ON UserPasswords(UserID);

CREATE INDEX idx_login_userid_type	ON UserLogins(UserID, LoginType);

CREATE INDEX idx_mail_sender		ON Mails(SenderID);
CREATE INDEX idx_mail_receiver		ON Mails(AddresseeID);
CREATE INDEX idx_mail_both_user		ON Mails(SenderID, AddresseeID);
CREATE INDEX idx_mail_parent		ON Mails(ParentMail);
GO