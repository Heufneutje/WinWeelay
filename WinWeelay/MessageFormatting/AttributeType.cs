namespace WinWeelay
{
    /// <summary>
    /// Text formatting attribute types.
    /// </summary>
    public enum AttributeType
    {
        /// <summary>
        /// Plain text with no formatting applied.
        /// </summary>
        None = 0,

        /// <summary>
        /// Make the following text bold.
        /// </summary>
        Bold = 1,

        /// <summary>
        /// Currently unused.
        /// </summary>
        Reverse = 2,

        /// <summary>
        /// Make the following text italic.
        /// </summary>
        Italic = 3,

        /// <summary>
        /// Underline the following text.
        /// </summary>
        Underline = 4,

        /// <summary>
        /// Keep the previously defined text attributes.
        /// </summary>
        KeepExistingAttributes = 8,

        /// <summary>
        /// Change the text color with the following color arguments.
        /// </summary>
        ForeColor = 5,

        /// <summary>
        /// Change the background text color with the following color arguments.
        /// </summary>
        BackColor = 6,

        /// <summary>
        /// Handle the following text as a hyperlink. It will serve as both the display text and the link destination.
        /// </summary>
        Hyperlink = 7
    }
}
