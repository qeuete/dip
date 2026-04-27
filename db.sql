CREATE DATABASE FoodStoreDB;
GO

USE FoodStoreDB;
GO

да
CREATE TABLE Manufacturers (
    IdManufacturer INT PRIMARY KEY IDENTITY(1,1),
    Name VARCHAR(150) NOT NULL,
    Deleted BIT NOT NULL DEFAULT 0
);
дв
CREATE TABLE Categories (
    IdCategory INT PRIMARY KEY IDENTITY(1,1),
    Name VARCHAR(100) NOT NULL,
    Deleted BIT NOT NULL DEFAULT 0
);
да
CREATE TABLE Products (
    IdProduct INT PRIMARY KEY IDENTITY(1,1),
    Name VARCHAR(200) NOT NULL,
    Article VARCHAR(50) NOT NULL UNIQUE,
    CategoryId INT NOT NULL FOREIGN KEY REFERENCES Categories(IdCategory),
    ManufacturerId INT NOT NULL FOREIGN KEY REFERENCES Manufacturers(IdManufacturer),
    Unit VARCHAR(20) NOT NULL,
    VolumeOrWeight DECIMAL(10,2) NOT NULL,
    Description VARCHAR(MAX) NOT NULL,
    Image VARCHAR(255),
    Price DECIMAL(10,2) NOT NULL,
    Quantity INT NOT NULL DEFAULT 0,
    IsAvailable BIT NOT NULL DEFAULT 1,
    Deleted BIT NOT NULL DEFAULT 0,
	CaloriesKcal DECIMAL(6,1),
    ProteinG DECIMAL(6,2),
    FatG DECIMAL(6,2),
    CarbsG DECIMAL(6,2),
	Composition VARCHAR(MAX) NOT NULL DEFAULT ''
);


ДА
CREATE TABLE Roles (
    IdRole INT PRIMARY KEY IDENTITY(1,1),
    Name VARCHAR(50) NOT NULL
);

INSERT INTO Roles (Name)
VALUES 
    ('Customer'),
	('Admin'),
	('Manager');


	ДА
CREATE TABLE Users (	
    IdUser INT PRIMARY KEY IDENTITY(1,1),
    Surname VARCHAR(200) NOT NULL,
    Name VARCHAR(200) NOT NULL,
    MiddleName VARCHAR(200),
    Email VARCHAR(150) NOT NULL UNIQUE,
    Phone VARCHAR(20) NOT NULL,
    PasswordHash VARCHAR(500) NOT NULL,
    RoleId INT NOT NULL FOREIGN KEY REFERENCES Roles(IdRole),
	Deleted BIT NOT NULL DEFAULT 0
);


ДА
CREATE TABLE UserAddresses (
    IdAddress INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL FOREIGN KEY REFERENCES Users(IdUser),
    City VARCHAR(100) NOT NULL,
    Street VARCHAR(100) NOT NULL,
    House VARCHAR(20) NOT NULL,
	Apartament VARCHAR(20),
    CourierComment VARCHAR(500),
    Deleted BIT NOT NULL DEFAULT 0
);

да
CREATE TABLE DeliveryTimeSlots (
    IdDeliverySlot INT PRIMARY KEY IDENTITY(1,1),
    TimeRange VARCHAR(50) NOT NULL,
	Deleted BIT NOT NULL DEFAULT 0
);

INSERT INTO DeliveryTimeSlots (TimeRange)
VALUES 
    ('10:00-18:00'),
	('18:00-22:00');

	да
CREATE TABLE OrderStatuses (
    IdOrderStatus INT IDENTITY(1,1) PRIMARY KEY,
    Name          VARCHAR(50) NOT NULL,
    Deleted       BIT NOT NULL DEFAULT 0
);

INSERT INTO OrderStatuses (Name)
VALUES 
    ('Оформлен'),
	('В работе'),
    ('В пути'),
	('Доставлен'),
	('Завершен'),
	('Отменен');


	да
CREATE TABLE Orders (
    IdOrder INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL FOREIGN KEY REFERENCES Users(IdUser),
    OrderDate DATETIME NOT NULL DEFAULT GETDATE(),
    TotalAmount DECIMAL(10,2) NOT NULL,
    DeliveryDate DATE NOT NULL,
    DeliverySlotId INT NOT NULL FOREIGN KEY REFERENCES DeliveryTimeSlots(IdDeliverySlot),
	OrderStatusId INT NOT NULL FOREIGN KEY REFERENCES OrderStatuses(IdOrderStatus),
	AddressId INT NOT NULL FOREIGN KEY REFERENCES UserAddresses(IdAddress),
	UserCardId INT NOT NULL FOREIGN KEY REFERENCES UserCards(IdUserCard)
);
дв
ALTER TABLE Orders
ADD CourierId INT NULL
    FOREIGN KEY REFERENCES Users(IdUser);
	дв
CREATE TABLE OrderDetails (
    IdOrderDetail INT PRIMARY KEY IDENTITY(1,1),
    OrderId INT NOT NULL FOREIGN KEY REFERENCES Orders(IdOrder),
    ProductId INT NOT NULL FOREIGN KEY REFERENCES Products(IdProduct),
    Quantity INT NOT NULL,
    Price DECIMAL(10,2) NOT NULL
);
да
CREATE TABLE Cart (
    IdCart INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL FOREIGN KEY REFERENCES Users(IdUser),
    ProductId INT NOT NULL FOREIGN KEY REFERENCES Products(IdProduct),
    Quantity INT NOT NULL,
	Price DECIMAL(10, 2) NOT NULL DEFAULT 0
);
дп
CREATE TABLE Favorites (
    IdFavorite INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL FOREIGN KEY REFERENCES Users(IdUser),
    ProductId INT NOT NULL FOREIGN KEY REFERENCES Products(IdProduct)
);

да
CREATE TABLE UserCards (
    IdUserCard INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL FOREIGN KEY REFERENCES Users(IdUser),

    CardNumber CHAR(16) NOT NULL
        CHECK (CardNumber NOT LIKE '%[^0-9]%'),

    ExpiryDate CHAR(5) NOT NULL
        CHECK (ExpiryDate LIKE '[0-1][0-9]/[0-9][0-9]'),

    CVV CHAR(3) NOT NULL
        CHECK (CVV NOT LIKE '%[^0-9]%'),

    Deleted BIT NOT NULL DEFAULT 0
);

да
CREATE TABLE Reviews (
    IdReview  INT IDENTITY(1,1) PRIMARY KEY,
    UserId    INT NOT NULL FOREIGN KEY REFERENCES Users(IdUser),
    ProductId INT NOT NULL FOREIGN KEY REFERENCES Products(IdProduct),
    Rating    INT NOT NULL,
    Comment   VARCHAR(1000),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    Deleted   BIT NOT NULL DEFAULT 0
);

ALTER TABLE Reviews
ADD CONSTRAINT CK_Reviews_Rating CHECK (Rating BETWEEN 1 AND 5);

CREATE TABLE ReviewImages (
    IdReviewImage INT IDENTITY(1,1) PRIMARY KEY,
    ReviewId      INT NOT NULL FOREIGN KEY REFERENCES Reviews(IdReview) ON DELETE CASCADE,
    ImageUrl      VARCHAR(255) NOT NULL
);

-- чат

CREATE TABLE ChatSessions (
    IdChat          INT IDENTITY(1,1) PRIMARY KEY,
    CustomerUserId  INT NULL FOREIGN KEY REFERENCES Users(IdUser),
    AssignedAgentId INT NULL FOREIGN KEY REFERENCES Users(IdUser),
    Status          VARCHAR(20) NOT NULL DEFAULT 'open',
    Priority        INT NOT NULL DEFAULT 0,
    StartedAt       DATETIME NOT NULL DEFAULT GETDATE(),
    ClosedAt        DATETIME NULL,
    LastMessageAt   DATETIME NOT NULL DEFAULT GETDATE(),
    Deleted         BIT NOT NULL DEFAULT 0,
    RowVersion      ROWVERSION
);

CREATE TABLE ChatMessages (
    IdMessage    BIGINT IDENTITY(1,1) PRIMARY KEY,
    ChatId       INT NOT NULL FOREIGN KEY REFERENCES ChatSessions(IdChat) ON DELETE CASCADE,
    SenderUserId INT NULL FOREIGN KEY REFERENCES Users(IdUser),
    SenderRole   VARCHAR(20) NOT NULL,
    MessageType  VARCHAR(20) NOT NULL DEFAULT 'text',
    Body         VARCHAR(MAX) NULL,
    CreatedAt    DATETIME NOT NULL DEFAULT GETDATE(),
    EditedAt     DATETIME NULL,
    IsDeleted    BIT NOT NULL DEFAULT 0
);


CREATE TABLE ChatEvents (
    IdEvent     BIGINT IDENTITY(1,1) PRIMARY KEY,
    ChatId      INT NOT NULL FOREIGN KEY REFERENCES ChatSessions(IdChat) ON DELETE CASCADE,
    ActorUserId INT NULL FOREIGN KEY REFERENCES Users(IdUser),
    EventType   VARCHAR(50) NOT NULL,
    CreatedAt   DATETIME NOT NULL DEFAULT GETDATE()
);

ALTER TABLE Users ADD ResetToken VARCHAR(200) NULL;
ALTER TABLE Users ADD ResetTokenExpires DATETIME NULL;

select * from Users

-- Очередь
GO
CREATE PROCEDURE Chat_GetQueue
AS
BEGIN
    SELECT IdChat, CustomerUserId, StartedAt, LastMessageAt, Priority, RowVersion
    FROM ChatSessions
    WHERE Status = 'open' AND AssignedAgentId IS NULL
    ORDER BY Priority DESC, LastMessageAt DESC;
END
GO

-- Взять чат
CREATE PROCEDURE Chat_Claim
    @ChatId INT,
    @AgentId INT
AS
BEGIN
    UPDATE ChatSessions
    SET AssignedAgentId = @AgentId,
        LastMessageAt = GETDATE()
    WHERE IdChat = @ChatId
      AND AssignedAgentId IS NULL
      AND Status = 'open';

    UPDATE AgentPresence
    SET CurrentActive = CurrentActive + 1,
        UpdatedAt = GETDATE()
    WHERE AgentUserId = @AgentId;

    INSERT INTO ChatEvents (ChatId, ActorUserId, EventType)
    VALUES (@ChatId, @AgentId, 'claimed');
END
GO

-- Закрыть чат
CREATE PROCEDURE Chat_Close
    @ChatId INT,
    @ActorId INT
AS
BEGIN
    UPDATE ChatSessions
    SET Status = 'closed',
        ClosedAt = GETDATE()
    WHERE IdChat = @ChatId AND Status = 'open';

    UPDATE AgentPresence
    SET CurrentActive = CASE WHEN CurrentActive > 0 THEN CurrentActive - 1 ELSE 0 END,
        UpdatedAt = GETDATE()
    WHERE AgentUserId = (SELECT AssignedAgentId FROM ChatSessions WHERE IdChat = @ChatId);

    INSERT INTO ChatEvents (ChatId, ActorUserId, EventType)
    VALUES (@ChatId, @ActorId, 'closed');
END
GO

CREATE TRIGGER TRG_Cleanup_OldChatMessages
ON ChatMessages
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM ChatMessages
    WHERE CreatedAt < DATEADD(YEAR, -1, GETDATE());
END;
GO

BACKUP DATABASE FoodStoreDB
TO DISK = 'C:\Backup\FoodStoreDB_Full.bak'
WITH 
    FORMAT,
    INIT,
    NAME = 'FoodStoreDB Full Backup',
    SKIP,
    STATS = 10;


INSERT INTO Categories (Name)
VALUES
    ('Завтраки'),
    ('Салаты'),
    ('Супы'),
    ('Вторые блюда'),
    ('Десерты'),
    ('Сытная выпечка'),
    ('Сладкая выпечка'),
    ('Горячие напитки'),
    ('Напитки'),
	('Холодные напитки')
GO;

INSERT INTO Manufacturers (Name)
VALUES
    ('Войтех Чешнер'),
    ('Александр Матюнькин'),
    ('Комаров Макар'),
    ('Журавлев Максим'),
    ('Родионова Елизавета'),
    ('Калашникова Виктория'),
    ('Лука Маринелли'),
    ('Сергей Пономарёв'),
    ('Макарова Таисия'),
	('Романова Ксения'),
	('Прочее')
GO

select * from Products

INSERT INTO Products (Name, Article, CategoryId, ManufacturerId, Unit, VolumeOrWeight, Description, Image, Price, Quantity, IsAvailable, CaloriesKcal, ProteinG, FatG, CarbsG, Composition)
VALUES (
    'Сырники со сметаной','BRKF-001', 1, 1, 'г', 250, 'Нежные сырники отлично гармонируют с домошней сметаной. Прекрасное начало дня или сытный полдник.',
    'https://prostokvashino.ru/upload/resize_cache/iblock/b1b/800_800_0/b1b727e06d8f0b23c6ea5e8f31c7df0f.jpg', 270.00, 50, 1, 628.0, 26.5, 29.75, 61.75,
	'сыр творожный (нормализованное молоко, сливки, соль, стабилизаторы (каррагинан, камедь рожкового дерева), закваска молочнокислых микроорганизмов, молокосвертывающий ферментный препарат микробного происхождения), сыр мягкий рикотта (сыворотка молочная, регулятор кислотности - лимонная кислота), творог 9% (молоко нормализованное, молоко восстановленное из сухого обезжиренного молока, закваска, молокосвертывающий ферментный препарат микробного происхождения), сахар, крупа манная, мука пшеничная, масло подсолнечное, ароматизатор - ванилин, соль. Блюдо готовится на производственном участке, где используются яйца и продукты их переработки'),
	
	('Скрэмбл с поджаренным беконом','BRKF-002', 1, 1, 'г', 300, 'Мягкий и сытный скрэмбл со вкусным поджаренным беконом и ароматной петрушкой.',
    'https://media.ovkuse.ru/images/recipes/9728308c-aa1b-42ff-8fa2-1de31e6bfe00/9728308c-aa1b-42ff-8fa2-1de31e6bfe00_840_840.webp', 280.00, 50, 1, 900.0, 27.0, 84.0, 6.0,
	'Скрэмбл (продукты яичные, сливки питьевые, масло подсолнечное, соль), петрушка кудрявая, продукт мясной - грудинка свиная сырокопченая, поджаренный (грудинка свиная, вода, нитритно-посолочная смесь (соль, фиксатор окраски - нитрит натрия), стабилизатор - пирофосфаты, регуляторы кислотности (трифосфаты, цитраты натрия, глюконо-дельта-лактон), антиокислитель - аскорбиновая кислота, консервант - ацетаты натрия, сахариды)'),
	
	('Ролл с ветчиной, чеддером и омлетом', 'BRKF-003', 1, 2, 'г', 210, 'Нежная текстура свежеприготовленного омлета, аппетитная ветчина и насыщенный вкус сыра чеддер. Каждый кусочек — это гармония ингредиентов!',
    'https://www.mealty.ru/upload/56/a7/56a799b8dcde6b78.jpeg', 230.00, 50, 1, 485.0, 23.1, 21.0, 46.2,
	'лепешка (мука пшеничная хлебопекарная высшего сорта, вода питьевая, масло подсолнечное, разрыхлители (гидрокарбонат натрия (сода пищевая), пирофосфаты), соль, эмульгатор - моно- и диглицериды жирных кислот, глютен пшеничный, регулятор кислотности – яблочная кислота, консервант - пиросульфит натрия), ветчина из мяса индейки - продукт вареный из мяса индейки (филе грудки индейки, филе бедра индейки, вода, крахмал картофельный, посолочная смесь нитритная (соль, фиксатор окраски - нитрит натрия), загуститель - каррагинан, регуляторы кислотности (трифосфаты, пирофосфат натрия), мальтодекстрин, усилитель вкуса и аромата - глутамат натрия, виноградный сахар, соль, антиокислители (аскорбиновая кислота, аскорбат натрия, изоаскорбат натрия), ароматизатор, коптильный ароматизатор, глюкоза ферментированная, специи (перец белый, горчица)), омлет (продукты яичные, сливки питьевые, масло подсолнечное, соль), сыр плавленый (сыр, вода питьевая, масло сливочное, сухое обезжиренное молоко, творог, сливки сухие, эмульгаторы (орто- и полифосфаты), загустители (модифицированный крахмал, каррагинан), сыворотка молочная сухая, регулятор кислотности - лимонная кислота, ароматизатор, стабилизатор - ксантановая камедь), сыр чеддер (молоко, соль, закваска, молокосвертывающий ферментный препарат животного происхождения, уплотнитель хлорид кальция, консервант - нитрат калия, краситель - аннато)'),

	('Омлет с ветчиной и сыром чеддер','BRKF-004', 1, 1, 'г', 230, 'Что может быть лучше, чем свежий, румяный, нежный омлет с ветчиной из индейки и сыром чеддер на завтрак.',
    'https://lefood.menu/wp-content/uploads/w_images/2021/11/recept-15705-1-1240x821_w.jpg', 230.00, 50, 1, 391.0, 21.16, 27.6, 12.19,
	'омлет с ветчиной и сыром [меланж яичный, ветчина - продукт из мяса птицы вареный (мясо птицы (филе куриное, филе грудки индейки), вода, крахмал картофельный, свиной белок, сыворотка молочная, стабилизатор - каррагинан, виноградный сахар, регуляторы кислотности (Е451, Е262), антиокислители (Е316, лимонная кислота), соль, гемоглобин свиной, краситель - кармины, фиксатор окраски - нитрит натрия), сливки питьевые, сыр чеддер (молоко пастеризованное, соль, закваска молочнокислых микроорганизмов, молокосвертывающий ферментный препарат животного происхождения, уплотнитель - хлорид кальция, консервант - нитрат калия, краситель - аннато), масло сливочное, петрушка]'),

	('Блинчики с творогом','BRKF-005', 1, 2, 'г', 210, 'Аппетитные домашние блинчики с начинкой из свежего творога. Идеально подходят для лёгкого завтрака. Нежные и умеренно сладкие, они так же могут стать замечательным десертом. Блины станут ещё вкуснее разогретыми: как будто только со сковородки!',
    'https://www.ermolino-produkty.ru/picts/articles/%D0%B1%D0%BB%D0%B8%D0%BD%D1%87%D0%B8%D0%BA%D0%B8_%D0%BC%D0%B0%D0%BC%D0%B8%D0%BD%D1%8B_%D1%81_%D1%82%D0%B2%D0%BE%D1%80%D0%BE%D0%B3%D0%BE%D0%BC.jpg', 210.00, 50, 1, 462.21, 18.9, 15.12, 60.06,
	'блинчики (вода питьевая, мука пшеничная высшего сорта, меланж яичный, масло подсолнечное рафинированное дезодорированное, молоко сухое цельное, сахар, сыворотка сухая молочная, соль пищевая), начинка (творог м.д.ж. 5% (молоко нормализованное пастеризованное м.д.ж. 0,7%, закваска молочнокислых стрептококков, уплотнитель – хлористый кальций, фермент животного происхождения – пепсин), сахар). Продукт производится на предприятии, где используются аллергены: кунжут'),
	
	('Творожная запеканка с соусом из вареной сгущенки', 'BRKF-006', 1, 2, 'г', 250, 'Нежная, воздушная запеканка из творога в сочетании со сгущеным молоком. Прекрасный выбор, когда хочется побаловать себя чем-то вкусным и, одновременно, полезным.',
    'https://menu2go.ru/images/food/149/149_270_20230809144634_2144.jpg', 230.00, 50, 1, 520.0, 19.0, 28.0, 48.25,
	'творожная запеканка [сыр мягкий рикотта (молочная сыворотка, регулятор кислотности - лимонная кислота), сливки питьевые, творог 9% (молоко нормализованное, молоко восстановленное из сухого обезжиренного молока, закваска, молокосвертывающий ферментный препарат микробного происхождения), продукты яичные, сахар, крупа манная, соль, ароматизатор - ванилин], соус из вареной сгущенки [сливки питьевые, молоко сгущенное с сахаром вареное (молоко нормализованное, сахар)]'),

	('Каша рисовая с манго','BRKF-007', 1, 1, 'г', 300, 'Рисовая каша - это великолепное сочетание быстрых и сложных углеводов, которое придаст вам сил и будет обеспечивать энергией на протяжении всего дня. А с добавлением фруктов каша становится еще более вкусной и полезной',
    'https://darina.su/upload/dev2fun.imagecompress/webp/iblock/d39/35pvi0kub86gma26rbnd09ppq7dphuzr.webp', 180.00, 50, 1, 385.0, 6.65, 9.0, 56.1,
	'Молоко питьевое, крупа рисовая, вода питьевая, сливки 22% питьевые, манго'

);


INSERT INTO Products (Name, Article, CategoryId, ManufacturerId, Unit, VolumeOrWeight, Description, Image, Price, Quantity, IsAvailable, CaloriesKcal, ProteinG, FatG, CarbsG, Composition)
VALUES (
    'Зимний салат с курицей и шампиньонами','SAL-001', 2, 3, 'г', 320, 'Уютный зимний салат, который согреет и порадует: мягкое куриное филе, душистые шампиньоны и слегка острый лук объединены в гармоничную композицию с помощью нежного майонеза. Завершающий штрих — рассыпчатый сыр и яркие зёрна граната — превращает блюдо в настоящую гастрономическую находку.',
    'https://www.mealty.ru/upload/c5/12/c51248bb29d6fb30.jpeg', 320.00, 50, 1, 768.0, 44.8, 64.05, 6.4,
	'Грибы шампиньоны, филе куриной грудки (филе куриной грудки, масло подсолнечное, чеснок, соль, приправа на основе лимонного сока), лук репчатый, майонез (масло подсолнечное, вода, сахар, яичный желток, соль, регуляторы кислотности (уксусная кислота, молочная кислота), консервант - сорбиновая кислота, загуститель ксантановая камедь, эфирное масло горчичное, антиокислители (аскорбилпальмитат, смесь натуральных токоферолов), краситель - каротины), сыр, масло подсолнечное, зерно граната (мякоть с семенем внутри), соль'),
	
	('Салат Цезарь с курицей','SAL-002', 2, 1, 'г', 200, 'Салат Цезарь с курицей является одним из самых любимых и популярных салатов во всем мире. Этот салат весьма изысканный – яркие половинки помидоров черри и нежное мясо курицы смотрятся очень живописно среди пышных листьев салата. Тертый пармезан завершает картину.',
    'https://www.u-gago.ru/images/product_images/info_images/60_1.jpg', 230.00, 50, 1, 470.0, 27.2, 39.0, 10.4,
	'салат айсберг, филе куриной грудки (филе куриной грудки, соус соевый* (вода, соевые бобы, пшеница, соль, сахар), чеснок), томаты черри, соус цезарь (майонез* (масло подсолнечное, вода, яичный желток, сахар, уксус столовый, соль, ароматизаторы, сок лимонный, антиокислители (Е385, аскорбилпальмитат, смесь натуральных токоферолов), краситель - каротины, регуляторы кислотности (уксусная кислота, молочная кислота), загуститель - ксантановая камедь, эфирное масло горчичное), маслины без косточки консервированные (маслины, вода, соль, фиксатор окраски - глюконат железа), масло растительное, соус соевый* (содержит соевые бобы, пшеницу), каперсы консервированные (содержат антиокислитель - диоксид серы), чеснок), сыр (молоко, закваска, молокосвертывающий ферментный препарат микробного происхождения, соль, уплотнитель - хлорид кальция) * содержат (в зависимости от состава) консерванты бензоат натрия, сорбат калия, сорбиновая кислота'),

	('Сельдь под шубой','SAL-003', 2, 3, 'г', 250, 'Сельдь под шубой – блюдо, которое никогда не потеряется на праздничном столе!',
    'https://images.gastronom.ru/HfJTgYpTuMs0WLoBplqn3-wrWlDkgxGSL14bk1Wm5Z0/pr:recipe-cover-image/g:ce/rs:auto:0:0:0/L2Ntcy9hbGwtaW1hZ2VzLzA0MzAwYzEyLTg2NjgtNGQxZi04N2UyLWUyMzJjOWE3ZTY0OS5qcGc.webp', 200.00, 50, 1, 450.0, 14.75, 35.75, 17.25,
	'свекла, картофель, филе сельди атлантической слабосоленое* (филе сельди атлантической, масло растительное, соль, регуляторы кислотности (винная кислота, лимонная кислота), усилитель вкуса и аромата – глутамат натрия, консерванты (сорбат калия, бензоат натрия), продукты яичные, майонез (масло подсолнечное, вода, яичный желток, уксус столовый, сахар, соль, ароматизаторы, сок лимонный, антиокислитель Е385, краситель - каротин), морковь'),

	('Греческий салат','SAL-004', 2, 6, 'г', 260, '',
    'https://vkusnoff.com/img/recepty/1977/final.webp', 230.00, 50, 1, 286.0, 5.2, 24.7, 7.8,
	'томаты черри, огурцы, "сиртаки для греческого салата" - комбинированный рассольный продукт смешанного состава (молоко, растительный жир, сухое обезжиренное молоко, молочно-белковый концентрат, соль, регулятор кислотности -глюконо-дельта-лактон, эмульгатор Е481, загуститель – модифицированный крахмал, желатин, стабилизатор - гуаровая камедь, молокосвертывающий ферментный препарат микробного происхождения), салат айсберг, перец сладкий (болгарский), маслины без косточки консервированные (содержат фиксатор окраски - глюконат железа), масло оливковое'),

	('Салат Нисуаз с тунцом','SAL-005', 2, 6, 'г', 250, '',
    'https://www.mealty.ru/upload/c5/b4/c5b42de32a70f3b1.jpeg', 230.00, 50, 1, 350.0, 12.5, 27.5, 11.25,
	'салат айсберг, картофель, томаты черри, ломтики филе тунца в собственном соку консервированного, соус (майонез* (масло подсолнечное, вода, яичный желток, уксус столовый, сахар, соль, ароматизаторы, сок лимонный), тунец в собственном соку консервированный, масло подсолнечное, соус соевый* (вода, соевые бобы, пшеница, соль, сахар), каперсы консервированные, горчица (содержит антиокислитель - пиросульфит калия), соус кимчи* (содержит соус рыбный), ворчестер*(содержит солодовый экстракт (ячменный), горчицу, рыбный порошок, креветки, экстракт сельдерея, лактозу)), яйцо куриное отварное, огурцы, фасоль стручковая, перец сладкий (болгарский), маслины без косточки консервированные (содержат фиксатор окраски - глюконат железа) *содержат (в зависимости от состава) антиокислитель Е385, консервант - сорбат калия, усилители вкуса и аромата (гуанилат натрия, инозинат натрия), регуляторы кислотности (уксусная кислота, лимонная кислота), красители (каротины, кармины, сахарный колер III), ароматизаторы, в т.ч. натуральные'
);


select * from Users
select * from  Cart
select * from  Roles

select * from UserCards

-- 1️⃣ Удаляем избранное пользователя
DELETE FROM Favorites
WHERE UserId = 3;

-- 2️⃣ Удаляем корзину пользователя
DELETE FROM OrderStatuses
WHERE IdOrderStatus = 4;

select * from OrderStatuses

-- 3️⃣ Удаляем адреса пользователя
UPDATE Users
SET RoleId = 2
WHERE IdUser = 4;

-- 4️⃣ Удаляем заказы и детали заказов
DELETE FROM OrderDetails
WHERE OrderId IN (SELECT IdOrder FROM Orders WHERE UserId = 3);


DELETE FROM Orders
WHERE UserId = 3;

-- 5️⃣ Удаляем карты пользователя
DELETE FROM UserCards
WHERE UserId = 3;

UPDATE Users
SET RoleId = 4
WHERE IdUser = 5;

select * from Users
SELECT * FROM OrderDetails WHERE OrderId = 14
select * from Orders
SELECT * FROM OrderDetails WHERE ProductId = 1
SELECT * FROM OrderDetails WHERE OrderId = 16

select * from UserCards
select * from Roles