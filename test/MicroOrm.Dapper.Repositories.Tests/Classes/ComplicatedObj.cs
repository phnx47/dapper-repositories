namespace MicroOrm.Dapper.Repositories.Tests.Classes
{
    internal class ComplicatedObj
    {
        public const string ConstName = "CC-456";

        public int Id = 456;
        public string Name = "FF-456";
        public string PropertName { get; } = "PP-456";

        public static string StaticName = "SS-456";
        public static string StaticPropertyName { get; } = "SS-PP-789";

        public string[] FieldNames = { "456", "654" };
        public int[] FieldArIds = { 1, 2, 3 };
        public string[] PropertyNames { get; } = { "456", "654" };

        public static string[] StaticFieldNames = { "456", "654" };
        public static int[] StaticFieldArIds = { 1, 2, 3 };
        public static string[] StaticPropertyNames { get; } = { "456", "654" };
    }
}
