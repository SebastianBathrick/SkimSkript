namespace JollyRoger
{
    /// <summary>
    /// Represents additional information that can be associated with command line flags.
    /// </summary>
    /// <remarks>
    /// This struct provides supplementary details about flags, such as examples,
    /// usage notes, or other contextual information displayed in help output.
    /// </remarks>
    public struct AdditionalInfo
    {
        /// <summary>
        /// The label or title for this additional information.
        /// </summary>
        public string label = string.Empty;
        
        /// <summary>
        /// The actual value or content of this additional information.
        /// </summary>
        public string value = string.Empty;
        
        /// <summary>
        /// Additional properties that can be used for formatting or context.
        /// </summary>
        public object[] properties = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="AdditionalInfo"/> struct.
        /// </summary>
        /// <param name="label">The label or title for this information.</param>
        /// <param name="value">The content or value of this information.</param>
        /// <param name="properties">Optional additional properties for formatting.</param>
        public AdditionalInfo(string label, string value, params object[] properties)
        {
            this.label = label;
            this.value = value;
            this.properties = properties;         
        }

        /// <summary>
        /// Sets the additional properties for this information.
        /// </summary>
        /// <param name="properties">The properties to set.</param>
        /// <returns>This instance for method chaining.</returns>
        public AdditionalInfo SetProperties(params object[] properties)
        {
            this.properties = properties;
            return this;
        }

        /// <summary>
        /// Sets the label for this information.
        /// </summary>
        /// <param name="label">The label to set.</param>
        /// <returns>This instance for method chaining.</returns>
        public AdditionalInfo SetLabel(string label)
        {
            this.label = label;
            return this;
        }

        /// <summary>
        /// Sets the value for this information.
        /// </summary>
        /// <param name="value">The value to set.</param>
        /// <returns>This instance for method chaining.</returns>
        public AdditionalInfo SetValue(string value)
        {
            this.value = value;
            return this;
        }
    }
}
