namespace MyMDB_WebApp
{
    public static class Secrets
    {
        /// <summary>
        /// Conneciton string with a user who can select, execute, update, delete all objects
        /// </summary>
        public static string BasicConnectionString =
            @"Data Source=NIKOSNOTEBOOK;Initial Catalog=MyMDB;User ID=mymdbinserter;Password=password;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
    }
}
