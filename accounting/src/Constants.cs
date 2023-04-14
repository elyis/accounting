namespace accounting.src
{
    public class Constants
    {
        public const string serverUrl = "http://192.168.101.123:8080/";

        //Локальные пути к хранилищам фото
        public const string localPathToStorages = @"src\Resources\Storages\";
        public const string localPathToProfileIcons = @$"{localPathToStorages}ProfileIcons";

        //Путь к файлу из сети
        public const string webPathToProfileIcons = @$"{serverUrl}api/profileIcon/";
    }
}
