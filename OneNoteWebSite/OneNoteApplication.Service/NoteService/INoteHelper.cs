using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneNoteApplication.NoteService
{
    public interface INoteHelper
    {
        void SendEmail(string subject, string body);
        void RecycleNotes(string oldTitle);
        string LoadNotes(string fileName);
    }
}
