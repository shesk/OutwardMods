using System.Reflection;

namespace OutwardMod_CoopUIScaler
{
    public static class Utils
    {
        public static T GetPrivatePart<T, S>(S owner, string memberName) where T : class where S : class
        {
            FieldInfo field = typeof(S).GetField(memberName, BindingFlags.Instance | BindingFlags.NonPublic);
            return field?.GetValue(owner) as T;
        }
    }
}
