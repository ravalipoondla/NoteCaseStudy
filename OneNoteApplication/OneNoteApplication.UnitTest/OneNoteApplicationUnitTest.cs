using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OneNoteApplication.SyncService;
using Microsoft.Synchronization.Files;

namespace OneNoteApplication.UnitTest
{ 
     [TestClass]
    public class OneNoteApplicationUnitTest
    {
        private static Mock<ISyncHelper> mocObj;
        private ISyncHelper objHelper;
        public OneNoteApplicationUnitTest()
        {
            mocObj = new Mock<ISyncHelper>();
            mocObj.Setup(x => x.SyncFileSystemReplicasOneWay(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<FileSyncScopeFilter>(), It.IsAny<FileSyncOptions>()));
            mocObj.Setup(x => x.DetectChangesOnFileSystemReplica(It.IsAny<string>(), It.IsAny<FileSyncScopeFilter>(), It.IsAny<FileSyncOptions>()));
            objHelper = new SyncHelper();
        }

        [TestMethod]
        public void SyncNotes()
        {
            var result = objHelper.SyncNotes();
            Assert.IsNotNull(result);
            Assert.IsTrue(result);
        }
    }
}


