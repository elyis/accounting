namespace accounting.src
{
    public static class Constants
    {
        //private static readonly string serverUrl = Environment.GetEnvironmentVariable("ASPNETCORE_URLS").Split(";").First();
        private static readonly string serverUrl = "https://3117-95-183-16-18.ngrok-free.app";

        //Локальные пути к хранилищам фото
        public static readonly string localPathToStorages = @"src\Resources\Storages\";
        public static readonly string localPathToProfileIcons = @$"{localPathToStorages}ProfileIcons";
        public static readonly string localPathToMaterialIcons = $@"{localPathToStorages}MaterialIcons";
        public static readonly string localPathToProductIcons = $@"{localPathToStorages}ProductIcons";
        public static readonly string localPathToMaterialAccounting = $@"{localPathToStorages}MaterialAccountingIcons";
        public static readonly string localPathToProductAccounting = $@"{localPathToStorages}ProductAccountingIcons";

        //Путь к файлу из сети
        public static readonly string webPathToProfileIcons = @$"{serverUrl}/api/profileIcon/";
        public static readonly string webPathToMaterialIcons = @$"{serverUrl}/api/materialIcon/";
        public static readonly string webPathToProductIcons = @$"{serverUrl}/api/productIcon/";
        public static readonly string webPathToMaterialAccountingIcons = $@"{serverUrl}/api/materialAccountingIcon/";
        public static readonly string webPathToProductAccountingIcons = $@"{serverUrl}/api/salesAccountingIcon/";
    }
}
