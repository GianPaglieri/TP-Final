namespace PaginaRedSocial.Helpers
{
    public class Credentials
    {
        private const String SERVER = "localhost";
        private const String DATABASE = "RedSocial";
        private const String USER = "root";
        private const String PASSWORD = "";
        private const int PORT = 3307;

        public static String GetConnectionString()
        {
            return $"server={Credentials.SERVER};user={Credentials.USER};database={Credentials.DATABASE};port={Credentials.PORT};password={Credentials.PASSWORD}";
        }

    }
}
