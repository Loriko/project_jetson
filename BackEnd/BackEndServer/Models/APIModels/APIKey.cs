namespace BackEndServer.Models.APIModels
{
    public class APIKey
    {
        public string API_Key { get; set; }
        public string API_KeySalt { get; set; }
        public bool IsActive { get; set; }

        public APIKey(string salted_hashed_api_key, string api_key_salt)
        {
            this.API_Key = salted_hashed_api_key;
            this.API_KeySalt = api_key_salt;
            this.IsActive = true;
        }
    }
}
