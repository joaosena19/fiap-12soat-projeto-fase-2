using System.Reflection;
using System.Runtime.Serialization;

namespace Shared
{
    public static class EnumHelper
    {
        public static IEnumerable<string> GetEnumMemberValues<T>() where T : Enum
        {
            var type = typeof(T);

            return Enum.GetValues(type)
                       .Cast<T>()
                       .Select(val =>
                       {
                           var member = type.GetMember(val.ToString()).FirstOrDefault();
                           var attribute = member?.GetCustomAttribute<EnumMemberAttribute>();
                           return attribute?.Value ?? val.ToString(); // fallback para nome do enum
                       });
        }
    }
}
