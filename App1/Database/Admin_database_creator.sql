CREATE TABLE Permissions (
    permissionId INT PRIMARY KEY Identity(1,1),
	permissionName varchar (255),

);




CREATE TABLE Users (
    userId INT PRIMARY KEY Identity(1,1),
    email VARCHAR(255) UNIQUE NOT NULL,
    name VARCHAR(255) NOT NULL,
    numberOfDeletedReviews INT DEFAULT 0,
    hasAppealed bit default 0,
    
);

Create Table UsersPermissions(

    userID INT not null,
	permissionID INT not null
   
   FOREIGN KEY (permissionID) REFERENCES Permissions(permissionId),
   FOREIGN KEY (userID) REfereNCES Users(userId),


);

CREATE TABLE Reviews (
    reviewID INT PRIMARY KEY Identity(1,1),
    numberOfFlags INT DEFAULT 0,
    content TEXT NOT NULL,
    isHidden bit DEFAULT 0,
    userID INT NOT NULL,
    FOREIGN KEY (userID) REFERENCES Users(userId) ON DELETE CASCADE
);

CREATE TABLE RoleRequests (
    requestID INT PRIMARY KEY Identity(1,1),
    userID INT NOT NULL,
    requestedPermissionID INT NOT NULL,
    FOREIGN KEY (userID) REFERENCES Users(userId) ON DELETE CASCADE,
    FOREIGN KEY (requestedPermissionID) REFERENCES Permissions(permissionId) ON DELETE CASCADE
);
go



Alter Table RoleRequests
DROP CONSTRAINT FK__RoleReque__reque__571DF1D5
Alter Table RoleRequests
drop column requestedPermissionID


go