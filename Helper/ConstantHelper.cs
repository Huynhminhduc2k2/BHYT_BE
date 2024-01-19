using System.ComponentModel;

namespace BHYT_BE.Helper
{
    public class ConstantHelper
    {
        public static bool IsValidValue<T>(string value)
        {
            Type constantType = typeof(T);

            // Get the fields of the constantType that are string literals
            var validFieldValues = constantType.GetFields()
                .Where(field => field.IsLiteral && !field.IsInitOnly && field.FieldType == typeof(string))
                .Select(field => field.GetValue(null).ToString());

            // Check if the value is valid
            return validFieldValues.Contains(value);
        }

        public static bool AreValidValues<T>(IEnumerable<string> values)
        {
            return values.All(value => IsValidValue<T>(value));
        }
    }
}
