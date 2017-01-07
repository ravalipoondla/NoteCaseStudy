using Microsoft.Synchronization;
using Microsoft.Synchronization.Files;
using OneNoteApplication;
using OneNoteApplication.Logging.DiskLogging;
using OneNoteApplication.Logging.LoggingInterfaces;
using OneNoteApplication.NoteService;
using OneNoteApplication.SyncService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OneNoteApplication
{
    public partial class Form1 : Form
    {
        //Create an instance of exception interface
        private IDiskLogging logException; //= new clsDiskLogging();
        private INoteHelper noteHelper; //= new NoteHelper();
        private ISyncHelper syncHelper;
        public Form1()
        {
            InitializeComponent();
            logException = new clsDiskLogging();
            noteHelper = new NoteHelper();
            syncHelper = new SyncHelper();
            //Calls sub-routine to log exception to disk file
            logException.LogExceptionToDisk("calling LoadNotes...");
            LoadNotes();
            tmSyncTimer.Start();
        }

        private void LoadNotes()
        {
            try
            {
                logException.LogExceptionToDisk("Inside LoadNotes...");
                DirectoryInfo info = new DirectoryInfo(ConfigurationManager.AppSettings["localPath"]);
                FileInfo[] files = info.GetFiles("*.txt").OrderBy(p => p.LastWriteTime).ToArray();
                foreach (FileInfo fileInfo in files)
                {
                    ListViewItem lstItem = new ListViewItem();
                    lstItem.Name = Path.GetFileNameWithoutExtension(fileInfo.Name);
                    lstItem.Tag = fileInfo;
                    lstNotes.Items.Add(lstItem);
                }
                lstNotes.SelectedIndex = -1;
            }
            catch(Exception ex)
            {
                logException.LogExceptionToDisk(ex.ToString());
            }
        }
        

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                logException.LogExceptionToDisk("Inside save button click...");
                if (string.IsNullOrEmpty(txtTitle.Text))
                {
                    MessageBox.Show("Title can't be Empty. Please provide the Title name");
                    return;
                }
                string oldFileName = string.Empty;
                string fileName = txtTitle.Text + ".txt";
                string fileFullPath = ConfigurationManager.AppSettings["localPath"] + "\\" + fileName;

                oldFileName = lstNotes.SelectedItem != null ? ((lstNotes.SelectedItem as ListViewItem).Text + ".txt") : string.Empty;
                if (oldFileName != fileName)
                {
                    DirectoryInfo info = new DirectoryInfo(ConfigurationManager.AppSettings["localPath"]);

                    if (info.GetFiles("*.txt").FirstOrDefault(x => x.Name == fileName) != null)
                    {
                        MessageBox.Show("There's already a note with that title. Please try a different title");
                        return;
                    }
                }

                // this is to move the files if their title is changed to recycle folder. or even deleted Note
                RecylceNoteFile(fileName);

                using (StreamWriter file = new StreamWriter(fileFullPath))
                {
                    file.WriteLine(txtBody.Text);
                }

                //ReloadNotes once again
                ReloadNotes();

                lblStatus.Text = "Note Saved Successfully!";
            }
            catch(Exception ex)
            {
                logException.LogExceptionToDisk(ex.ToString());
            }
        }

        private void ReloadNotes()
        {
            //Load Notes OnceAgain
            lstNotes.Items.Clear();
            LoadNotes();
        }

        private void RecylceNoteFile(string fileName)
        {
            logException.LogExceptionToDisk("Recycle Old Notes ...");
            if (lstNotes.SelectedIndex != -1)
            {
                var oldFileName = (lstNotes.SelectedItem as ListViewItem).Name + ".txt";
                if (fileName != oldFileName)
                {
                    noteHelper.RecycleNotes(oldFileName);
                }
            }
        }

        private void lstNotes_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (lstNotes.SelectedIndex != -1)
                {
                    ListViewItem lstItem = lstNotes.SelectedItem as ListViewItem;
                    LoadNoteDetails(lstItem);
                }
            }
            catch(Exception ex)
            {
                logException.LogExceptionToDisk(ex.ToString());
            }
        }

        private void LoadNoteDetails(ListViewItem lstItem)
        {
            logException.LogExceptionToDisk("Inside load note details...");
            string fileName = (lstItem.Tag as FileInfo).Name;
            txtTitle.Text = Path.GetFileNameWithoutExtension(fileName);
            txtBody.Text = noteHelper.LoadNotes(fileName);
        }

        private void btnShareOverEmail_Click(object sender, EventArgs e)
        {
            noteHelper.SendEmail(txtTitle.Text,txtBody.Text);
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            clearSelection();
        }

        private void clearSelection()
        {
            lstNotes.SelectedIndex = -1;
            lblStatus.Text = string.Empty;
            txtTitle.Text = string.Empty;
            txtBody.Text = string.Empty;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result.Equals(DialogResult.Yes))
                {
                    string fileName = (lstNotes.SelectedItem as ListViewItem).Text + ".txt";
                    lblStatus.Text = "Selected Note deleted successfully!";
                    RecylceNoteFile(fileName);
                    ReloadNotes();
                    clearSelection();
                }
            }
            catch(Exception ex)
            {
                logException.LogExceptionToDisk(ex.ToString());
            }
        }

        private void tmSyncTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (CommonUtility.CheckForInternetConnection())
                {
                    syncHelper.SyncNotes();
                }
            }
            catch(Exception ex)
            {
                logException.LogExceptionToDisk(ex.ToString());
            }
        }
    }
}
