using System.ComponentModel;

namespace Ahmed_mart.Helpers.v1
{
    public class EnumDescription
    {
        public static string GetEnumDescription(Enum value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            var attribute = value.GetType()
                .GetField(value.ToString())
                ?.GetCustomAttributes(typeof(DescriptionAttribute), false)
                .SingleOrDefault() as DescriptionAttribute;
            return attribute?.Description ?? value.ToString();
        }
    }
}
