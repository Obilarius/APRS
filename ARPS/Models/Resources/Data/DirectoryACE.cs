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
        public FileSystemRights Rights { get; set; }
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

        /// <summary>
        /// Prüft ob der ACE SpezielleBerechtigung besitz
        /// </summary>
        public bool HasSpecialPermissions
        {
            get
            {
                if ((HasWriteDataCreateFiles || HasCreateDirectoryAppendData || HasWriteExtendedAttributes || HasWriteAttributes) && !HasWrite ||
                    (HasReadData || HasReadExtendedAttributes || HasReadAttributes || HasReadPermissions) && !HasRead || 
                    (HasExecuteFileTraverse && !HasReadAndExecute) ||
                    (HasDelete && !HasModify) ||
                    (HasDeleteSubdirectoriesAndFiles || HasChangePermissions || HasTakeOwnership) && !HasFullControl)
                    return true;
                else
                    return false;
            }
        }

        #region Einzelne NTFS Rechte

        /// <summary>
        /// Prüft ob der ACE WriteData / CreateFiles besitz
        /// </summary>
        public bool HasWriteDataCreateFiles
        {
            get
            {
                return Rights.HasFlag(FileSystemRights.WriteData) || Rights.HasFlag(FileSystemRights.CreateFiles);
            }
        }

        /// <summary>
        /// Prüft ob der ACE CreateDirectory / AppendData besitz
        /// </summary>
        public bool HasCreateDirectoryAppendData
        {
            get
            {
                return Rights.HasFlag(FileSystemRights.CreateDirectories) || Rights.HasFlag(FileSystemRights.AppendData);
            }
        }

        /// <summary>
        /// Prüft ob der ACE WriteExtendedAttributes besitz
        /// </summary>
        public bool HasWriteExtendedAttributes
        {
            get
            {
                return Rights.HasFlag(FileSystemRights.WriteExtendedAttributes);
            }
        }

        /// <summary>
        /// Prüft ob der ACE WriteAttributes besitz
        /// </summary>
        public bool HasWriteAttributes
        {
            get
            {
                return Rights.HasFlag(FileSystemRights.WriteAttributes);
            }
        }

        /// <summary>
        /// Prüft ob der ACE ReadData besitz
        /// </summary>
        public bool HasReadData
        {
            get
            {
                return Rights.HasFlag(FileSystemRights.ReadData);
            }
        }

        /// <summary>
        /// Prüft ob der ACE ReadExtendedAttributes besitz
        /// </summary>
        public bool HasReadExtendedAttributes
        {
            get
            {
                return Rights.HasFlag(FileSystemRights.ReadExtendedAttributes);
            }
        }

        /// <summary>
        /// Prüft ob der ACE ReadAttributes besitz
        /// </summary>
        public bool HasReadAttributes
        {
            get
            {
                return Rights.HasFlag(FileSystemRights.ReadAttributes);
            }
        }

        /// <summary>
        /// Prüft ob der ACE ReadPermissions besitz
        /// </summary>
        public bool HasReadPermissions
        {
            get
            {
                return Rights.HasFlag(FileSystemRights.ReadPermissions);
            }
        }

        /// <summary>
        /// Prüft ob der ACE ExecuteFile / Traverse besitz
        /// </summary>
        public bool HasExecuteFileTraverse
        {
            get
            {
                return Rights.HasFlag(FileSystemRights.ExecuteFile) || Rights.HasFlag(FileSystemRights.Traverse);
            }
        }

        /// <summary>
        /// Prüft ob der ACE Delete besitz
        /// </summary>
        public bool HasDelete
        {
            get
            {
                return Rights.HasFlag(FileSystemRights.Delete);
            }
        }

        /// <summary>
        /// Prüft ob der ACE DeleteSubdirectoriesAndFiles besitz
        /// </summary>
        public bool HasDeleteSubdirectoriesAndFiles
        {
            get
            {
                return Rights.HasFlag(FileSystemRights.DeleteSubdirectoriesAndFiles);
            }
        }

        /// <summary>
        /// Prüft ob der ACE ChangePermissions besitz
        /// </summary>
        public bool HasChangePermissions
        {
            get
            {
                return Rights.HasFlag(FileSystemRights.ChangePermissions);
            }
        }

        /// <summary>
        /// Prüft ob der ACE TakeOwnership besitz
        /// </summary>
        public bool HasTakeOwnership
        {
            get
            {
                return Rights.HasFlag(FileSystemRights.TakeOwnership);
            }
        }


        #endregion
        #endregion

        /// <summary>
        /// Gibt zurück auf welche Ebene (Ordner, Unterordner, Dateien) diese Regel gilt
        /// </summary>
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

        public bool PropagationOnThisFolder
        {
            get
            {
                if ((InheritanceFlags == 0 && PropagationFlags == 0) ||
                    (InheritanceFlags == 3 && PropagationFlags == 0) ||
                    (InheritanceFlags == 1 && PropagationFlags == 0) ||
                    (InheritanceFlags == 2 && PropagationFlags == 0))
                    return true;
                else if ((InheritanceFlags == 3 && PropagationFlags == 2) ||
                        (InheritanceFlags == 1 && PropagationFlags == 2) ||
                        (InheritanceFlags == 2 && PropagationFlags == 2))
                    return false;
                else 
                    return false;
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