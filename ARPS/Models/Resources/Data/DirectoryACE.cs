using System;
using System.Security.AccessControl;

namespace ARPS
{
    public class DirectoryACE : IEquatable<DirectoryACE>
    {
        public bool IsGroup { get; }
        public string IdentityName { get; }
        public string DistinguishedName { get; }
        public int ACEId { get; }
        public string SID { get; }
        public FileSystemRights Rights { get; }
        public bool Type { get; }
        public string FileSystemRight { get; }
        public bool IsInherited { get; }
        public int InheritanceFlags { get; }
        public int PropagationFlags { get; }


        public DirectoryACE(bool isGroup, string identityName, string distinguishedName, int aCEId, string sID, FileSystemRights rights, bool type, string fileSystemRight, bool isInherited, int inheritanceFlags, int propagationFlags)
        {
            IsGroup = isGroup;
            IdentityName = identityName;
            DistinguishedName = distinguishedName;
            ACEId = aCEId;
            SID = sID;
            Rights = rights;
            Type = type;
            FileSystemRight = fileSystemRight;
            IsInherited = isInherited;
            InheritanceFlags = inheritanceFlags;
            PropagationFlags = propagationFlags;
        }

        public DirectoryACE(string sID)
        {
            SID = sID;
        }

        public DirectoryACE(string sID, FileSystemRights rights, bool type, string fileSystemRight, bool isInherited, int inheritanceFlags, int propagationFlags)
        {
            SID = sID;
            Rights = rights;
            Type = type;
            FileSystemRight = fileSystemRight;
            IsInherited = isInherited;
            InheritanceFlags = inheritanceFlags;
            PropagationFlags = propagationFlags;
        }


        #region NTFS
        /// <summary>
        /// Prüft ob der ACE Full Control besitz
        /// </summary>
        public bool HasFullControl
        {
            get
            {
                return (Rights.HasFlag(FileSystemRights.FullControl));
            }
        }

        /// <summary>
        /// Prüft ob der ACE Modify besitz
        /// </summary>
        public bool HasModify
        {
            get
            {
                return (Rights.HasFlag(FileSystemRights.Modify));
            }
        }

        /// <summary>
        /// Prüft ob der ACE ReadAndExecute besitz
        /// </summary>
        public bool HasReadAndExecute
        {
            get
            {
                return (Rights.HasFlag(FileSystemRights.ReadAndExecute));
            }
        }

        /// <summary>
        /// Prüft ob der ACE Write besitz
        /// </summary>
        public bool HasWrite
        {
            get
            {
                return (Rights.HasFlag(FileSystemRights.Write));
            }
        }

        /// <summary>
        /// Prüft ob der ACE Read besitz
        /// </summary>
        public bool HasRead
        {
            get
            {
                return (Rights.HasFlag(FileSystemRights.Read));
            }
        }

        /// <summary>
        /// Prüft ob der ACE ListDirectory besitz
        /// </summary>
        public bool HasListDirectory
        {
            get
            {
                return (Rights.HasFlag(FileSystemRights.ListDirectory));
            }
        }
        #endregion

        public string Propagation
        {
            get
            {
                if (InheritanceFlags == 0 && PropagationFlags == 0)
                    return "Nur dieser Ordner";
                else if (InheritanceFlags == 3 && PropagationFlags == 0)
                    return "Diesen Ordner, Unterordner und Dateien";
                else if(InheritanceFlags == 1 && PropagationFlags == 0)
                    return "Diesen Ordner, Unterordner";
                else if(InheritanceFlags == 2 && PropagationFlags == 0)
                    return "Diesen Ordner, Dateien";
                else if(InheritanceFlags == 3 && PropagationFlags == 2)
                    return "Nur Unterordner und Dateien";
                else if(InheritanceFlags == 1 && PropagationFlags == 2)
                    return "Nur Unterordner";
                else if(InheritanceFlags == 2 && PropagationFlags == 2)
                    return "Nur Dateien";

                return "";
            }
        }


        public bool Equals(DirectoryACE other)
        {

            return other != null &&
                   SID == other.SID &&
                   Rights == other.Rights &&
                   Type == other.Type &&
                   IsInherited == other.IsInherited &&
                   InheritanceFlags == other.InheritanceFlags &&
                   PropagationFlags == other.PropagationFlags;
        }
    }
}