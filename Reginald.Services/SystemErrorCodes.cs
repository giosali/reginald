namespace Reginald.Services
{
    /// <summary>
    /// The following list describes system error codes (errors 0 to 499). They are returned by the GetLastError function when many functions fail. To retrieve the description text for the error in your application, use the FormatMessage function with the FORMAT_MESSAGE_FROM_SYSTEM flag.
    /// </summary>
    public enum SystemErrorCode : int
    {
        /// <summary>
        /// Cannot create a file when that file already exists.
        /// </summary>
        ERROR_ALREADY_EXISTS = 0xB7,
    }
}
