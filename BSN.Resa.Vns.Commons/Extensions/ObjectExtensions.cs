namespace BSN.Resa.Vns.Commons.Extensions
{
    public static class ObjectExtensions
    {
        public static string SerializeToJson(this object obj)
        {
            return System.Text.Json.JsonSerializer.Serialize(obj);
        }
    }
}
