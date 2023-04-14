namespace accounting.src.Utility
{
    public class RecoveryCodeGenerator
    {
        public static string Generate()
        {
            var rnd = new Random();
            return rnd.Next(100_000, 1_000_000).ToString();
        }
    }
}
