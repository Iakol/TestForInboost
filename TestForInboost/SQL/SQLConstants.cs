using Telegram.Bot.Types;

namespace TestForInboost.SQL
{
    public static class SQLConstants
    {
        //Ckeack Table
        public const string CheakTable = "SELECT CASE When EXISTS ( SELECT 1 FROM InBootTest.INFORMATION_SCHEMA.TABLES Where TABLE_NAME Like @TableName) THEN 1 else 0 end ;\r\n";

        // Create WeatherHisotryTable 
        public const string CreateWeatherHistoryTable = "CREATE TABLE WeatherHistory (\r\n\tId int IDENTITY(1,1),\r\n\tUserId bigINT REFERENCES Users(Id),\r\n\tMain Nvarchar(max) NOT NULL,\r\n\tDescription Nvarchar(max) NOT NULL,\r\n\ttemp DECIMAL(18,2) NOT NULL,\r\n\ttemp_feels_like DECIMAL(18,2) NOT NULL,\r\n\ttemp_min DECIMAL(18,2) NOT NULL,\r\n\ttemp_max DECIMAL(18,2) NOT NULL,\r\n\tpressure INT not null,\r\n\thumidity INT not null,\r\n\twind_speed DECIMAL(18,2) NOT NULL,\r\n\twind_deg INT NOT NULL,\r\n\tcloud INT not null,\r\n\tvisibility INT not null,\r\n\tcreateAt DATETIME2 not null,\r\n\tPRIMARY KEY (Id)\r\n);";

        // User Table inituate and cheak
        public const string CreateUserTable = "CREATE TABLE Users (\r\n\tId bigInt NOT NULL,\r\n\tIsBot bit NOT NULL,\r\n\tFirstName nvarchar(max) NOT NULL,\r\n\tLastName nvarchar(max) NULL,\r\n\tUsername nvarchar(max) NULL,\r\n\tPRIMARY KEY (ID)\r\n\r\n);";


        //Create Weather History
        public const string CreateWeatherHistory = "INSERT INTO WeatherHistory (UserId,Main,Description,temp,temp_feels_like,temp_min,temp_max,pressure,humidity,wind_speed,wind_deg,cloud,visibility,createAt) Values(@UserId, @Main, @Description, @temp, @temp_feels_like, @temp_min, @temp_max, @pressure, @humidity, @wind_speed, @wind_deg,@cloud, @visibility,@createAt); SELECT SCOPE_IDENTITY();";
        

        //Create User

        public const string CreateUser = "INSERT INTO Users (Id,isBot,FirstName,LastName,Username) Values (@Id,@isBot,@FirstName,@LastName,@Username);";


        // Get User SQL
        public const string GetUser = "SELECT * From Users Where Id = @id";
        public const string GetUserList = "Select * from Users";
        public const string GetHistoryByUserList = "SELECT * From WeatherHistory Where UserId = @UserId";


        //Get Weather History
        public const string GetWeatherHstoryById = "SELECT * From WeatherHistory Where UserId = 525262264 Order by createAt DESC";







    }
}
