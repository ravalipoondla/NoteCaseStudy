using Microsoft.Synchronization;
using Microsoft.Synchronization.Files;
using OneNoteApplication.Logging.DiskLogging;
using OneNoteApplication.Logging.LoggingInterfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneNoteApplication.SyncService
{
    public class SyncHelper : ISyncHelper
    {
        //Create an instance of exception interface
        public IDiskLogging logException = new clsDiskLogging();
        public bool SyncNotes()
        {
            logException.LogExceptionToDisk("Inside Sync Notes...");
            string localPath = ConfigurationManager.AppSettings["localPath"]; //@"C:\Users\rpoondla\Documents\Visual Studio 2015\Projects\OneNoteApplication\OneNoteApplication\bin\Debug"; //args[0];
            string remotePath = ConfigurationManager.AppSettings["RemotePath"]; //@"\\10.213.154.154\f\Ravali"; //args[1];
            logException.LogExceptionToDisk("localPath: "+localPath + "RemotePath: "+ remotePath);

            try
            {
                // Set options for the synchronization operation
                FileSyncOptions options = FileSyncOptions.ExplicitDetectChanges |
                    FileSyncOptions.RecycleDeletedFiles |
                    FileSyncOptions.RecyclePreviousFileOnUpdates |
                    FileSyncOptions.RecycleConflictLoserFiles;

                FileSyncScopeFilter filter = new FileSyncScopeFilter();
                filter.FileNameIncludes.Add("*.txt");  //FileNameExcludes.Add("*.lnk"); // Exclude all *.lnk files

                // Explicitly detect changes on both replicas upfront, to avoid two change
                // detection passes for the two-way synchronization

                DetectChangesOnFileSystemReplica(
                        localPath, filter, options);
                DetectChangesOnFileSystemReplica(
                    remotePath, filter, options);

                // Synchronization in both directions
                SyncFileSystemReplicasOneWay(localPath, remotePath, null, options);
                SyncFileSystemReplicasOneWay(remotePath, localPath, null, options);
            }
            catch (Exception e)
            {
                logException.LogExceptionToDisk("\nException from File Synchronization Provider:\n" + e.ToString());
                return false;
            }
            return true;
        }


        public void DetectChangesOnFileSystemReplica(
                string replicaRootPath,
                FileSyncScopeFilter filter, FileSyncOptions options)
        {
            FileSyncProvider provider = null;

            try
            {
                provider = new FileSyncProvider(replicaRootPath, filter, options);
                provider.DetectChanges();
            }
            finally
            {
                // Release resources
                if (provider != null)
                    provider.Dispose();
            }
        }

        public void SyncFileSystemReplicasOneWay(
                string sourceReplicaRootPath, string destinationReplicaRootPath,
                FileSyncScopeFilter filter, FileSyncOptions options)
        {
            FileSyncProvider sourceProvider = null;
            FileSyncProvider destinationProvider = null;

            try
            {
                sourceProvider = new FileSyncProvider(
                    sourceReplicaRootPath, filter, options);
                destinationProvider = new FileSyncProvider(
                    destinationReplicaRootPath, filter, options);

                destinationProvider.AppliedChange +=
                    new EventHandler<AppliedChangeEventArgs>(OnAppliedChange);
                destinationProvider.SkippedChange +=
                    new EventHandler<SkippedChangeEventArgs>(OnSkippedChange);

                SyncOrchestrator agent = new SyncOrchestrator();
                agent.LocalProvider = sourceProvider;
                agent.RemoteProvider = destinationProvider;
                agent.Direction = SyncDirectionOrder.DownloadAndUpload; // Sync source to destination

                //Console.WriteLine("Synchronizing changes to replica: " +
                //    destinationProvider.RootDirectoryPath);
                logException.LogExceptionToDisk("Synchronizing changes to replica: " + destinationProvider.RootDirectoryPath);
                agent.Synchronize();
            }
            finally
            {
                // Release resources
                if (sourceProvider != null) sourceProvider.Dispose();
                if (destinationProvider != null) destinationProvider.Dispose();
            }
        }

        private void OnAppliedChange(object sender, AppliedChangeEventArgs args)
        {
            switch (args.ChangeType)
            {
                case ChangeType.Create:
                    logException.LogExceptionToDisk("-- Applied CREATE for file " + args.NewFilePath);
                    break;
                case ChangeType.Delete:
                    logException.LogExceptionToDisk("-- Applied DELETE for file " + args.OldFilePath);
                    break;
                //case ChangeType.Overwrite:
                //    logException.LogExceptionToDisk("-- Applied OVERWRITE for file " + args.OldFilePath);
                //    break;
                case ChangeType.Rename:
                    logException.LogExceptionToDisk("-- Applied RENAME for file " + args.OldFilePath +
                                      " as " + args.NewFilePath);
                    break;
            }
        }

        public void OnSkippedChange(object sender, SkippedChangeEventArgs args)
        {
            logException.LogExceptionToDisk("-- Skipped applying " + args.ChangeType.ToString().ToUpper()
                  + " for " + (!string.IsNullOrEmpty(args.CurrentFilePath) ?
                                args.CurrentFilePath : args.NewFilePath) + " due to error");

            if (args.Exception != null)
                logException.LogExceptionToDisk("   [" + args.Exception.Message + "]");
        }
    }
}
