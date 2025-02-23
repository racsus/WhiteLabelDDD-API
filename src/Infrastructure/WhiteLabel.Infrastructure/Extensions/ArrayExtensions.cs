namespace WhiteLabel.Infrastructure.Data.Extensions
{
    /// <summary>
    /// Extension for System.Array
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Add the elements on
        /// at the end of
        /// </summary>
        /// <typeparam name="T">Type of the elements of the array</typeparam>
        /// Target where the items will be added
        /// Items to be added
        /// <returns><paramref name="target"/> with <paramref name="items"/> added</returns>
        public static T[] Add<T>(this T[] target, params T[] items)
        {
            // Validate the parameters
            target ??= [];
            items ??= [];

            // Join the arrays
            var result = new T[target.Length + items.Length];
            target.CopyTo(result, 0);
            items.CopyTo(result, target.Length);
            return result;
        }
    }
}
