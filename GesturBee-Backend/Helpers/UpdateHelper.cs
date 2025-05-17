namespace GesturBee_Backend.Helpers
{
    public static class UpdateHelpers
    {
        public static void UpdateIfChanged<T>(this T? newValue, T currentValue, Action<T> setValue) where T : struct, IEquatable<T>
        {
            if (newValue.HasValue && !newValue.Value.Equals(currentValue))
            {
                setValue(newValue.Value);
            }
        }

        public static void UpdateIfChanged(this string? newValue, string currentValue, Action<string> setValue)
        {
            if (!string.IsNullOrEmpty(newValue) && !newValue.Equals(currentValue))
            {
                setValue(newValue);
            }
        }
    }

}
