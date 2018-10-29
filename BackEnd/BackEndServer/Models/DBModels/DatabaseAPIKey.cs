namespace BackEndServer.Models.DBModels
{
    public class DatabaseAPIKey
    {
        // Table Name
        public static readonly string TABLE_NAME = "api_key";
        // Attributes of API Key table.
        public static readonly string API_KEY_ID_LABEL = "id";
        public static readonly string USER_ID_LABEL = "user_id";
        public static readonly string API_KEY_LABEL = "api_key";
        public static readonly string API_KEY_SALT_LABEL = "salt";
        public static readonly string API_KEY_ISACTIVE_LABEL = "is_active";
        
        // Values for API Key activity status
        public enum API_Key_Status
        {
            INACTIVE = 0,
            ACTIVE = 1
        }

        // Database Model Class Attributes
        public int APIKeyId { get; set; }
        public int UserId { get; set; }
        public string Key { get; set; }
        public string Salt { get; set; }
        public int IsActive { get; set; }
    }
}