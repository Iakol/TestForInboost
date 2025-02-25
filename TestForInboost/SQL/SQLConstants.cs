namespace TestForInboost.SQL
{
    public static class SQLConstants
    {
        // User Table inituate and cheak
        public const string CreateUserTable = "CREATE TABLE Users (\r\n\tId bigInt NOT NULL,\r\n\tIsBot bit NOT NULL,\r\n\tFirstName nvarchar(max) NOT NULL,\r\n\tLastName nvarchar(max) NULL,\r\n\tUsername nvarchar(max) NULL,\r\n\tPRIMARY KEY (ID)\r\n\r\n);";

        public const string CheakUserTable = "SELECT TABLE_NAME \r\nFROM InBootTest.INFORMATION_SCHEMA.TABLES \r\nWhere TABLE_NAME LIke 'Users';";

        // WeatherHistory Table inituate and cheak

        public const string CreateWeatherHistoryTable = "CREATE TABLE WeatherHistory \r\n(\r\nId int IDENTITY(1,1),\r\nvisibility int NOT NULL,\r\nUserId bigint FOREIGN KEY REFERENCES Users(Id)\r\nPRIMARY KEY (Id)\r\n);";
        public const string CheakWeatherHistoryCheak = "SELECT TABLE_NAME \r\nFROM InBootTest.INFORMATION_SCHEMA.TABLES \r\nWhere TABLE_NAME LIke 'WeatherHistory';";

        // clouds Table inituate and cheak

        public const string CreateCloudsTable = "CREATE TABLE clouds (\r\nId int IDENTITY(1,1),\r\nWeatherHistoryId int FOREIGN KEY REFERENCES WeatherHistory(Id),\r\n[all] int NOT NULL,\r\nPRIMARY KEY (Id)\r\n);";
        public const string CheakCloudsCheak = "SELECT TABLE_NAME \r\nFROM InBootTest.INFORMATION_SCHEMA.TABLES \r\nWhere TABLE_NAME LIke 'clouds';";


        // main Table inituate and cheak

        public const string CreateMainTable = "CREATE TABLE main \r\n(\r\nId int IDENTITY(1,1),\r\nWeatherHistoryId int FOREIGN KEY REFERENCES WeatherHistory(Id),\r\n\r\ntemp DECIMAL(18,2) NOT NULL,\r\nfeels_like DECIMAL(18,2) NOT NULL,\r\ntemp_min DECIMAL(18,2) NOT NULL,\r\ntemp_max DECIMAL(18,2) NOT NULL,\r\n\r\npressure int NOT NULL,\r\nhumidity int NOT NULL,\r\nsea_level int NOT NULL,\r\ngrnd_level int NOT NULL\r\n\r\nPRIMARY KEY (Id)\r\n);";
        public const string CheakMainCheak = "SELECT TABLE_NAME \r\nFROM InBootTest.INFORMATION_SCHEMA.TABLES \r\nWhere TABLE_NAME LIke 'main';";


        // wind Table inituate and cheak

        public const string CreateWindTable = "CREATE TABLE wind \r\n(\r\nId int IDENTITY(1,1),\r\nWeatherHistoryId int FOREIGN KEY REFERENCES WeatherHistory(Id),\r\n\r\nspeed DECIMAL(18,2) NOT NULL,\r\n\r\ndeg int NOT NULL\r\n\r\nPRIMARY KEY (Id)\r\n);";
        public const string CheakWindCheak = "SELECT TABLE_NAME \r\nFROM InBootTest.INFORMATION_SCHEMA.TABLES \r\nWhere TABLE_NAME LIke 'wind';";

        // weatherObject Table inituate and cheak

        public const string CreateWeatherObjectTable = "CREATE TABLE weather\r\n(\r\nId int IDENTITY(1,1),\r\nWeatherHistoryId int FOREIGN KEY REFERENCES WeatherHistory(Id),\r\nmain Nvarchar(max) NOT NULL,\r\ndescription Nvarchar(max) NOT NULL,\r\n\r\nPRIMARY KEY (Id)\r\n);";
        public const string CheakWeatherObjectCheak = "SELECT TABLE_NAME \r\nFROM InBootTest.INFORMATION_SCHEMA.TABLES \r\nWhere TABLE_NAME LIke 'weather';";


        //Base sql selects

        public const string CreateUser = "INSERT INTO Users (Id,isBot,FirstName,LastName,Username) Values (@Id,@isBot,@FirstName,@LastName,@Username);SELECT SCOPE_IDENTITY();";
        public const string CreateWeatherHistory = "INSERT INTO WeatherHistory (visibility,UserId) Values (@visibility,@UserId); SELECT SCOPE_IDENTITY();";
        public const string CreateCloudsObject = "INSERT INTO clouds (WeatherHistoryId,[all]) Values (@WeatherHistoryId,@all); SELECT SCOPE_IDENTITY();";
        public const string CreateMainObject = "INSERT INTO main (WeatherHistoryId,temp,feels_like,temp_min,temp_max,pressure,humidity,sea_level,grnd_level) Values (@WeatherHistoryId,@temp,@feels_like,@temp_min,@temp_max,@pressure,@humidity,@sea_level,@grnd_level); SELECT SCOPE_IDENTITY();";
        public const string CreateWindObject = "INSERT INTO wind (WeatherHistoryId,speed,deg) Values (@WeatherHistoryId,@speed,@deg); SELECT SCOPE_IDENTITY();";
        public const string CreateWeatherObject = "INSERT INTO weather (WeatherHistoryId,main,description) Values (@WeatherHistoryId,@main,@description); SELECT SCOPE_IDENTITY();";



        public const string GetUser = "SELECT * From Users Where Id = @id";
        public const string GetUserList = "Select * from Users";
        public const string GetHistoryOfUser = "SELECT * From WeatherHistory Where UserId = @UserId";
        public const string GetHistoryOfHistoryId = "SELECT * From WeatherHistory Where Id = @Id";

        public const string GetcloudsObjectOfHistoryId= "SELECT * From clouds Where WeatherHistoryId = @WeatherHistoryId";
        public const string GetMainWeatherObjectOfHistoryId = "SELECT * From main Where WeatherHistoryId = @WeatherHistoryId";
        public const string GetWeatherObjectOfHistoryId = "SELECT * From weather Where WeatherHistoryId = @WeatherHistoryId";
        public const string GetWindObjectOfHistoryId = "SELECT * From wind Where WeatherHistoryId = @WeatherHistoryId";





    }
}
