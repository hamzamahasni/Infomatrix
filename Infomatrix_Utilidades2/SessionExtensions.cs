using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Infomatrix_Utilidades
{
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session,string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }
        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            //Devuelve un null si no hay ningun valor de sesion y si no manda el valor
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}
