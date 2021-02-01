namespace WhiteLabel.Domain.Extensions
{
    /// <summary>
    /// Extension for System.Array
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Add the elements on <param name="items"> at the end of <param name="target">
        /// </summary>
        /// <typeparam name="T">Type of the elements of the array</typeparam>
        /// <param name="target">Target where the items will be added</param>
        /// <param name="items">Items to be added</param>
        /// <returns><paramref name="target"/> with <paramref name="items"/> added</returns>
        public static T[] Add<T>(this T[] target, params T[] items)
        {
            // Validate the parameters
            if (target == null)
            {
                target = new T[] { };
            }
            if (items == null)
            {
                items = new T[] { };
            }

            // Join the arrays
            T[] result = new T[target.Length + items.Length];
            target.CopyTo(result, 0);
            items.CopyTo(result, target.Length);
            return result;
        }
    }
}