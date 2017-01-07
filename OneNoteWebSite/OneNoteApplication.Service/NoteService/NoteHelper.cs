using OneNoteApplication.Logging.DiskLogging;
using OneNoteApplication.Logging.LoggingInterfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneNoteApplication.NoteService
{
    public class NoteHelper: INoteHelper
    {
        //Create an instance of exception interface
        public IDiskLogging logException = new clsDiskLogging();


        public void SendEmail(string subject,string body)
        {
            // %0D%0A is hexa decimal representation of \r\n to replace line breaks
            System.Diagnostics.Process.Start("mailto:" + " ? subject=" + subject + "&body=" + body.Replace("\r\n", "%0D%0A"));
        }

        public void RecycleNotes(string fileName)
        {
            string localPath = ConfigurationManager.AppSettings["localPath"] + "\\";
            string recyclePath = localPath + "OneNote_RecycleBin\\";
            if (!Directory.Exists(recyclePath))
            {
                Directory.CreateDirectory(recyclePath);
            }

            File.Move( localPath + fileName, recyclePath + fileName);
        }

        public string LoadNotes(string fileName)
        {
            string noteBody="",strBody = "";
            string fileNameWithFullPath = ConfigurationManager.AppSettings["localPath"] + "\\" + fileName;
            //// Open the file to read from.
            using (StreamReader sr = File.OpenText(fileNameWithFullPath))
            {
                while ((strBody = sr.ReadLine()) != null)
                {
                    noteBody += strBody + Environment.NewLine;
                }
            }
            return noteBody;
        }
    }
}
