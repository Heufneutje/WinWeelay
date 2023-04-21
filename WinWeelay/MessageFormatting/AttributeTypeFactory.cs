namespace WinWeelay
{
    /// <summary>
    /// Helper class for determining the type of an attribute based on a given formatting character.
    /// </summary>
    public static class AttributeTypeFactory
    {
        /// <summary>
        /// Determine attribute type from formatting character.
        /// </summary>
        /// <param name="attributeChar">The formatting character.</param>
        /// <returns>The type of the attribute.</returns>
        public static AttributeType GetAttribute(char attributeChar)
        {
            return attributeChar switch
            {
                '*' or '\u0001' => AttributeType.Bold,
                '!' or '\u0002' => AttributeType.Reverse,
                '/' or '\u0003' => AttributeType.Italic,
                '_' or '\u0004' => AttributeType.Underline,
                '\u0010' => AttributeType.Hyperlink, // Custom, might need to be replaced with something better.
                '|' => AttributeType.KeepExistingAttributes,
                _ => AttributeType.None,
            };
        }
    }
}
