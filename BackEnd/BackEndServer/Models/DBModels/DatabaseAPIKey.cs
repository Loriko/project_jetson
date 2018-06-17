namespace BackEndServer.Models.DBModels
{
    public class DatabaseAPIKey
    {
        // Table Name
        public static readonly string TABLE_NAME = "api_key";
        // Attributes of API Key table.
        public static readonly string API_KEY_ID_LABEL = "id";
        public static readonly string API_KEY = "key";
        public static readonly string API_KEY_SALT = "key_salt";
        public static readonly string API_KEY_ISACTIVE_LABEL = "key_is_active";

        // Database Model Class Attributes
        public int APIKeyId { get; set; }
        public string APIKey { get; set; }
        public string APIKeySalt { get; set; }
        public bool IsActive { get; set; }
    }
}