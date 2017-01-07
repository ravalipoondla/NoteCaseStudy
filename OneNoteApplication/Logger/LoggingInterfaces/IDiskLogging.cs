using System;
using System.Collections.Generic;
using System.Text;

namespace OneNoteApplication.Logging.LoggingInterfaces
{
    /// <summary>
    /// Contract for logging exception to disk
    /// </summary>                                                                                                                                                                                                                          
    public interface IDiskLogging
    {
        /// <summary>
        /// Logs exception to disk file.
        /// </summary>
        /// <param name="objException">The source to represent exception.</param>
        /// <returns>Indicates whether write has been done successfully to disk.</returns>
        void LogExceptionToDisk( string exceptionOrInfoDetails);
    }
}
