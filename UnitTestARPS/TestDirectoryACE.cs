using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestARPS
{
    [TestClass]
    public class TestDirectoryACE
    {
        private FileSystemRights read = (FileSystemRights)131209;  //Read
        private FileSystemRights readSyncro = (FileSystemRights)1179785;  //Read & Syncro
        private FileSystemRights writeSyncro = (FileSystemRights)1048854;  //Write & Syncro

        private FileSystemRights readWriteSync = (FileSystemRights)1180063;  //Read & Write & Syncro
        private FileSystemRights readExecuteSyncro = (FileSystemRights)1179817;  //Read & Execute & Syncro
        private FileSystemRights modifySyncro = (FileSystemRights)1245631;  //Modify & Syncro
        private FileSystemRights fullcontrollTakeownerSyncro = (FileSystemRights)2032127;  //Full Control & Take Ownership & Syncro


        [TestMethod]
        public void Allow_ReadWriteS_Disallow_ReadS()
        {
            int allow = (int)readWriteSync;
            int disallow = (int)readSyncro;

            // Dreht die bits des erlaubten Rechts um 010 -> 101
            allow = ~allow;
            // Bitweises oder der beiden Rechte
            int result = allow | disallow;
            // Dteht die bits des Ergebnisses wieder um
            result = ~result;


            Assert.AreEqual(result, 278);
        }


        [TestMethod]
        public void Allow_ModifyS_ReadExecuteS_Read_Disallow_WriteS()
        {
            List<FileSystemRights> allow = new List<FileSystemRights>() { modifySyncro, readExecuteSyncro, read };
            FileSystemRights disallow = writeSyncro;

            int allAllow = 0;
            foreach (var allowRight in allow)
            {
                allAllow = allAllow | (int)allowRight;
            }

            // Dreht die bits des erlaubten Rechts um 010 -> 101
            allAllow = ~allAllow;
            // Bitweises oder der beiden Rechte
            int result = allAllow | (int)disallow;
            // Dteht die bits des Ergebnisses wieder um
            result = ~result;


            Assert.AreEqual(result, 196777);
        }


        [TestMethod]
        public void Allow_FullControlTakeownerS_Disallow_ReadWriteS_Read()
        {
            FileSystemRights allow = fullcontrollTakeownerSyncro;
            List<FileSystemRights> disallow = new List<FileSystemRights>() { readWriteSync, read };

            int allDisallow = 0;
            foreach (var disallowRight in disallow)
            {
                allDisallow = allDisallow | (int)disallowRight;
            }

            // Dreht die bits des erlaubten Rechts um 010 -> 101
            allow = ~allow;
            // Bitweises oder der beiden Rechte
            int result = (int)allow | allDisallow;
            // Dteht die bits des Ergebnisses wieder um
            result = ~result;


            Assert.AreEqual(result, 852064);
        }
    }
}
