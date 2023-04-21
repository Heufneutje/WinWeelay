namespace WinWeelay.Core
{
    /// <summary>
    /// Header of a received relay message.
    /// </summary>
    public class RelayMessageHeader
    {
        /// <summary>
        /// ID of the relay message.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// The number of bytes in the data array.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Initialize a new header.
        /// </summary>
        /// <param name="id">ID of the relay message.</param>
        /// <param name="length">The number of bytes in the data array.</param>
        public RelayMessageHeader(string id, int length)
        {
            ID = id;
            Length = length;
        }
    }
}
