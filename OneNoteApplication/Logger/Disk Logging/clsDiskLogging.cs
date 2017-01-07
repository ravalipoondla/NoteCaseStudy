using OneNoteApplication.Logging.LoggingInterfaces;
using System;
using System.Configuration;
using System.IO;

namespace OneNoteApplication.Logging.DiskLogging 
{
    
    public class clsDiskLogging : IDiskLogging
    {
        string m_LogFilePath = string.Empty;
               
        /// <summary>
        ///	Creates log exception file on disk if not created
        /// </summary>
        public clsDiskLogging()
        {
            try
            {
                m_LogFilePath = ConfigurationManager.AppSettings["ExceptionLogFilePath"];

                if (!File.Exists(m_LogFilePath))
                {
                    FileStream LogFileRef = File.Create(m_LogFilePath);

                    //Close the file created.
                    LogFileRef.Close();
                }
            }
            catch(Exception ex)
            {

            }
        }
        
        
        /// <summary>
        ///	Logs exception to disk file as per the path mentioned in the web.config file.
        /// </summary>
        /// <remarks>
        ///	bool isLogged = IDiskLogging.LogExceptionToDisk();
        /// </remarks>
        /// <returns>
        ///	"True", if exception has been logged successfully to disk or "false", if not able to write to disk
        /// </returns>
        public void LogExceptionToDisk(string exceptionOrInfoDetails) 
        {
            //Content from disk file template
            string TemplateExceptionDetails = string.Empty;

            string FinalExceptionDetails = string.Empty;
           

            try
            {
                // Read the file as one string.
                if (!string.IsNullOrEmpty(m_LogFilePath))
                {
                    //Appends exception string to the file
                    StreamWriter ExceptionFileRef = File.AppendText(m_LogFilePath);

                    //  //Writes exception object to disk and Sets the line break in the file
                    ExceptionFileRef.Write(DateTime.Now.ToString() + " ==> " + exceptionOrInfoDetails + Environment.NewLine);

                    //Close the stream
                    ExceptionFileRef.Close();
                }
            }
            catch(Exception ex)
            {

            }
        }//end of function
    }
}
