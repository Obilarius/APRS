﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ArgPerm
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="ArgesPerm")]
	public partial class DataClassesDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void Insertadgroup(adgroup instance);
    partial void Updateadgroup(adgroup instance);
    partial void Deleteadgroup(adgroup instance);
    partial void Insertaduser(aduser instance);
    partial void Updateaduser(aduser instance);
    partial void Deleteaduser(aduser instance);
    partial void Insertdir(dir instance);
    partial void Updatedir(dir instance);
    partial void Deletedir(dir instance);
    partial void Insertright(right instance);
    partial void Updateright(right instance);
    partial void Deleteright(right instance);
    #endregion
		
		public DataClassesDataContext() : 
				base(global::ArgPerm.Properties.Settings.Default.ArgesPermConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public DataClassesDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public DataClassesDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public DataClassesDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public DataClassesDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<adgroup> adgroups
		{
			get
			{
				return this.GetTable<adgroup>();
			}
		}
		
		public System.Data.Linq.Table<aduser> adusers
		{
			get
			{
				return this.GetTable<aduser>();
			}
		}
		
		public System.Data.Linq.Table<dir> dirs
		{
			get
			{
				return this.GetTable<dir>();
			}
		}
		
		public System.Data.Linq.Table<grp_user> grp_users
		{
			get
			{
				return this.GetTable<grp_user>();
			}
		}
		
		public System.Data.Linq.Table<right> rights
		{
			get
			{
				return this.GetTable<right>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.adgroups")]
	public partial class adgroup : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private string _SID;
		
		private string _SamAccountName;
		
		private string _DistinguishedName;
		
		private string _Name;
		
		private string _Description;
		
		private System.Nullable<short> _IsSecurityGroup;
		
		private string _GroupScope;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnSIDChanging(string value);
    partial void OnSIDChanged();
    partial void OnSamAccountNameChanging(string value);
    partial void OnSamAccountNameChanged();
    partial void OnDistinguishedNameChanging(string value);
    partial void OnDistinguishedNameChanged();
    partial void OnNameChanging(string value);
    partial void OnNameChanged();
    partial void OnDescriptionChanging(string value);
    partial void OnDescriptionChanged();
    partial void OnIsSecurityGroupChanging(System.Nullable<short> value);
    partial void OnIsSecurityGroupChanged();
    partial void OnGroupScopeChanging(string value);
    partial void OnGroupScopeChanged();
    #endregion
		
		public adgroup()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SID", DbType="NVarChar(100) NOT NULL", CanBeNull=false, IsPrimaryKey=true)]
		public string SID
		{
			get
			{
				return this._SID;
			}
			set
			{
				if ((this._SID != value))
				{
					this.OnSIDChanging(value);
					this.SendPropertyChanging();
					this._SID = value;
					this.SendPropertyChanged("SID");
					this.OnSIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SamAccountName", DbType="NVarChar(100)")]
		public string SamAccountName
		{
			get
			{
				return this._SamAccountName;
			}
			set
			{
				if ((this._SamAccountName != value))
				{
					this.OnSamAccountNameChanging(value);
					this.SendPropertyChanging();
					this._SamAccountName = value;
					this.SendPropertyChanged("SamAccountName");
					this.OnSamAccountNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DistinguishedName", DbType="NVarChar(300)")]
		public string DistinguishedName
		{
			get
			{
				return this._DistinguishedName;
			}
			set
			{
				if ((this._DistinguishedName != value))
				{
					this.OnDistinguishedNameChanging(value);
					this.SendPropertyChanging();
					this._DistinguishedName = value;
					this.SendPropertyChanged("DistinguishedName");
					this.OnDistinguishedNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Name", DbType="NVarChar(100)")]
		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				if ((this._Name != value))
				{
					this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Description", DbType="NVarChar(MAX)")]
		public string Description
		{
			get
			{
				return this._Description;
			}
			set
			{
				if ((this._Description != value))
				{
					this.OnDescriptionChanging(value);
					this.SendPropertyChanging();
					this._Description = value;
					this.SendPropertyChanged("Description");
					this.OnDescriptionChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IsSecurityGroup", DbType="SmallInt")]
		public System.Nullable<short> IsSecurityGroup
		{
			get
			{
				return this._IsSecurityGroup;
			}
			set
			{
				if ((this._IsSecurityGroup != value))
				{
					this.OnIsSecurityGroupChanging(value);
					this.SendPropertyChanging();
					this._IsSecurityGroup = value;
					this.SendPropertyChanged("IsSecurityGroup");
					this.OnIsSecurityGroupChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_GroupScope", DbType="NVarChar(50)")]
		public string GroupScope
		{
			get
			{
				return this._GroupScope;
			}
			set
			{
				if ((this._GroupScope != value))
				{
					this.OnGroupScopeChanging(value);
					this.SendPropertyChanging();
					this._GroupScope = value;
					this.SendPropertyChanged("GroupScope");
					this.OnGroupScopeChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.adusers")]
	public partial class aduser : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private string _SID;
		
		private string _DisplayName;
		
		private string _SamAccountName;
		
		private string _DistinguishedName;
		
		private string _UserPrincipalName;
		
		private short _Enabled;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnSIDChanging(string value);
    partial void OnSIDChanged();
    partial void OnDisplayNameChanging(string value);
    partial void OnDisplayNameChanged();
    partial void OnSamAccountNameChanging(string value);
    partial void OnSamAccountNameChanged();
    partial void OnDistinguishedNameChanging(string value);
    partial void OnDistinguishedNameChanged();
    partial void OnUserPrincipalNameChanging(string value);
    partial void OnUserPrincipalNameChanged();
    partial void OnEnabledChanging(short value);
    partial void OnEnabledChanged();
    #endregion
		
		public aduser()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SID", DbType="NVarChar(100) NOT NULL", CanBeNull=false, IsPrimaryKey=true)]
		public string SID
		{
			get
			{
				return this._SID;
			}
			set
			{
				if ((this._SID != value))
				{
					this.OnSIDChanging(value);
					this.SendPropertyChanging();
					this._SID = value;
					this.SendPropertyChanged("SID");
					this.OnSIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DisplayName", DbType="NVarChar(100)")]
		public string DisplayName
		{
			get
			{
				return this._DisplayName;
			}
			set
			{
				if ((this._DisplayName != value))
				{
					this.OnDisplayNameChanging(value);
					this.SendPropertyChanging();
					this._DisplayName = value;
					this.SendPropertyChanged("DisplayName");
					this.OnDisplayNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SamAccountName", DbType="NVarChar(100)")]
		public string SamAccountName
		{
			get
			{
				return this._SamAccountName;
			}
			set
			{
				if ((this._SamAccountName != value))
				{
					this.OnSamAccountNameChanging(value);
					this.SendPropertyChanging();
					this._SamAccountName = value;
					this.SendPropertyChanged("SamAccountName");
					this.OnSamAccountNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DistinguishedName", DbType="NVarChar(300)")]
		public string DistinguishedName
		{
			get
			{
				return this._DistinguishedName;
			}
			set
			{
				if ((this._DistinguishedName != value))
				{
					this.OnDistinguishedNameChanging(value);
					this.SendPropertyChanging();
					this._DistinguishedName = value;
					this.SendPropertyChanged("DistinguishedName");
					this.OnDistinguishedNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_UserPrincipalName", DbType="NVarChar(100)")]
		public string UserPrincipalName
		{
			get
			{
				return this._UserPrincipalName;
			}
			set
			{
				if ((this._UserPrincipalName != value))
				{
					this.OnUserPrincipalNameChanging(value);
					this.SendPropertyChanging();
					this._UserPrincipalName = value;
					this.SendPropertyChanged("UserPrincipalName");
					this.OnUserPrincipalNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Enabled", DbType="SmallInt NOT NULL")]
		public short Enabled
		{
			get
			{
				return this._Enabled;
			}
			set
			{
				if ((this._Enabled != value))
				{
					this.OnEnabledChanging(value);
					this.SendPropertyChanging();
					this._Enabled = value;
					this.SendPropertyChanged("Enabled");
					this.OnEnabledChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.dirs")]
	public partial class dir : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _ID;
		
		private string _Directory;
		
		private string _Owner;
		
		private string _Hash;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIDChanging(int value);
    partial void OnIDChanged();
    partial void OnDirectoryChanging(string value);
    partial void OnDirectoryChanged();
    partial void OnOwnerChanging(string value);
    partial void OnOwnerChanged();
    partial void OnHashChanging(string value);
    partial void OnHashChanged();
    #endregion
		
		public dir()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ID", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				if ((this._ID != value))
				{
					this.OnIDChanging(value);
					this.SendPropertyChanging();
					this._ID = value;
					this.SendPropertyChanged("ID");
					this.OnIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Directory", DbType="NVarChar(MAX) NOT NULL", CanBeNull=false)]
		public string Directory
		{
			get
			{
				return this._Directory;
			}
			set
			{
				if ((this._Directory != value))
				{
					this.OnDirectoryChanging(value);
					this.SendPropertyChanging();
					this._Directory = value;
					this.SendPropertyChanged("Directory");
					this.OnDirectoryChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Owner", DbType="NVarChar(100) NOT NULL", CanBeNull=false)]
		public string Owner
		{
			get
			{
				return this._Owner;
			}
			set
			{
				if ((this._Owner != value))
				{
					this.OnOwnerChanging(value);
					this.SendPropertyChanging();
					this._Owner = value;
					this.SendPropertyChanged("Owner");
					this.OnOwnerChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Hash", DbType="NChar(40) NOT NULL", CanBeNull=false)]
		public string Hash
		{
			get
			{
				return this._Hash;
			}
			set
			{
				if ((this._Hash != value))
				{
					this.OnHashChanging(value);
					this.SendPropertyChanging();
					this._Hash = value;
					this.SendPropertyChanged("Hash");
					this.OnHashChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.grp_user")]
	public partial class grp_user
	{
		
		private string _userSID;
		
		private string _grpSID;
		
		public grp_user()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_userSID", DbType="NVarChar(100) NOT NULL", CanBeNull=false)]
		public string userSID
		{
			get
			{
				return this._userSID;
			}
			set
			{
				if ((this._userSID != value))
				{
					this._userSID = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_grpSID", DbType="NVarChar(100) NOT NULL", CanBeNull=false)]
		public string grpSID
		{
			get
			{
				return this._grpSID;
			}
			set
			{
				if ((this._grpSID != value))
				{
					this._grpSID = value;
				}
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.rights")]
	public partial class right : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _ID;
		
		private int _DirID;
		
		private string _Hash;
		
		private string _IdentityReference;
		
		private short _AccessControlType;
		
		private string _FileSystemRights;
		
		private short _IsInherited;
		
		private string _InheritanceFlags;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIDChanging(int value);
    partial void OnIDChanged();
    partial void OnDirIDChanging(int value);
    partial void OnDirIDChanged();
    partial void OnHashChanging(string value);
    partial void OnHashChanged();
    partial void OnIdentityReferenceChanging(string value);
    partial void OnIdentityReferenceChanged();
    partial void OnAccessControlTypeChanging(short value);
    partial void OnAccessControlTypeChanged();
    partial void OnFileSystemRightsChanging(string value);
    partial void OnFileSystemRightsChanged();
    partial void OnIsInheritedChanging(short value);
    partial void OnIsInheritedChanged();
    partial void OnInheritanceFlagsChanging(string value);
    partial void OnInheritanceFlagsChanged();
    #endregion
		
		public right()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ID", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				if ((this._ID != value))
				{
					this.OnIDChanging(value);
					this.SendPropertyChanging();
					this._ID = value;
					this.SendPropertyChanged("ID");
					this.OnIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DirID", DbType="Int NOT NULL")]
		public int DirID
		{
			get
			{
				return this._DirID;
			}
			set
			{
				if ((this._DirID != value))
				{
					this.OnDirIDChanging(value);
					this.SendPropertyChanging();
					this._DirID = value;
					this.SendPropertyChanged("DirID");
					this.OnDirIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Hash", DbType="NChar(40) NOT NULL", CanBeNull=false)]
		public string Hash
		{
			get
			{
				return this._Hash;
			}
			set
			{
				if ((this._Hash != value))
				{
					this.OnHashChanging(value);
					this.SendPropertyChanging();
					this._Hash = value;
					this.SendPropertyChanged("Hash");
					this.OnHashChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IdentityReference", DbType="NVarChar(100) NOT NULL", CanBeNull=false)]
		public string IdentityReference
		{
			get
			{
				return this._IdentityReference;
			}
			set
			{
				if ((this._IdentityReference != value))
				{
					this.OnIdentityReferenceChanging(value);
					this.SendPropertyChanging();
					this._IdentityReference = value;
					this.SendPropertyChanged("IdentityReference");
					this.OnIdentityReferenceChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_AccessControlType", DbType="SmallInt NOT NULL")]
		public short AccessControlType
		{
			get
			{
				return this._AccessControlType;
			}
			set
			{
				if ((this._AccessControlType != value))
				{
					this.OnAccessControlTypeChanging(value);
					this.SendPropertyChanging();
					this._AccessControlType = value;
					this.SendPropertyChanged("AccessControlType");
					this.OnAccessControlTypeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FileSystemRights", DbType="NVarChar(300) NOT NULL", CanBeNull=false)]
		public string FileSystemRights
		{
			get
			{
				return this._FileSystemRights;
			}
			set
			{
				if ((this._FileSystemRights != value))
				{
					this.OnFileSystemRightsChanging(value);
					this.SendPropertyChanging();
					this._FileSystemRights = value;
					this.SendPropertyChanged("FileSystemRights");
					this.OnFileSystemRightsChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IsInherited", DbType="SmallInt NOT NULL")]
		public short IsInherited
		{
			get
			{
				return this._IsInherited;
			}
			set
			{
				if ((this._IsInherited != value))
				{
					this.OnIsInheritedChanging(value);
					this.SendPropertyChanging();
					this._IsInherited = value;
					this.SendPropertyChanged("IsInherited");
					this.OnIsInheritedChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_InheritanceFlags", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string InheritanceFlags
		{
			get
			{
				return this._InheritanceFlags;
			}
			set
			{
				if ((this._InheritanceFlags != value))
				{
					this.OnInheritanceFlagsChanging(value);
					this.SendPropertyChanging();
					this._InheritanceFlags = value;
					this.SendPropertyChanged("InheritanceFlags");
					this.OnInheritanceFlagsChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591