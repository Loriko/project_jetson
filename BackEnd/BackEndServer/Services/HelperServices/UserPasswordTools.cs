namespace BackEndServer.Services.HelperServices
{
    public static class UserPasswordTools
    {
        // Returns a string representing a randomly generated Salt for a User Password.
        public static string GenerateRandomPasswordSalt()
        {
            return StringGenerator.GenerateRandomString(6, 12);
        }

        // Returns a string representing the password, but in a salted (using input salt) and hashed form.
        public static string HashAndSaltPassword(string unsalted_unhashed_password, string salt)
        {
            // Salted Password = First three characters of Salt + Password + Remaining Characters of Salt
            string salted_password = salt.Substring(0, 3) + unsalted_unhashed_password + salt.Substring(3);

            // Return the MD5 hash of the salted password.
            return HashingTools.MD5Hash(salted_password);
        }
    }
}
