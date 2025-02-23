namespace WhiteLabel.Domain.Pagination
{
    /// <summary>
    ///  Indicates the filter options
    ///  Uses semicolon symbol ";" to combine multiple filters
    /// </summary>
    public class FilterOption
    {
        /// <summary>
        /// Property or field where the filter will be applied.
        /// It can also be used to indicate whether the Any function (with the | character) or the All function (with the & character) will be applied.
        /// </summary>
        public string Member { get; set; }

        /// <summary>
        /// String representing the value to filer or, comma-separated values in case of array.
        /// </summary>
        public string Value { get; set; }

        public FilterOperator Operator { get; set; }
    }
}
