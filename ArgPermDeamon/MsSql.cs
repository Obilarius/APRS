using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARPSMSSQL
{
    public class MsSql
    {
        #region Tabellennamen
        public static string TBL_tmp_AD_Users { get { return "temp.adusers"; } }
        public static string TBL_tmp_AD_UserInGroup { get { return "temp.grp_user"; } }
        public static string TBL_tmp_AD_Groups { get { return "temp.adgroups"; } }
        public static string TBL_tmp_AD_Computers { get { return "temp.adcomputers"; } }
        public static string TBL_tmp_FS_Dirs { get { return "temp.dirs"; } }
        public static string TBL_tmp_FS_Shares { get { return "temp.shares"; } }
        public static string TBL_tmp_FS_ACLs { get { return "temp.acls"; } }
        public static string TBL_tmp_FS_ACEs { get { return "temp.aces"; } }
        #endregion

        /// <summary>
        /// Connection String
        /// </summary>
        private string conString = @"Data Source=8MAN\SQLEXPRESS;Initial Catalog=ARPS;User Id=LokalArps;Password=nopasswd;MultipleActiveResultSets=True";

        /// <summary>
        /// Hält die MSSQL Vebindung
        /// </summary>
        public SqlConnection Con { get; private set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public MsSql()
        {
            // Erstellt die Verbindung zum MsSQL Server
            //Con = new SqlConnection(@"Data Source=PC-W10-SW\MSSQLSERVER_DEV;Initial Catalog=ArgesPerm;Integrated Security=True;MultipleActiveResultSets=True");
            try
            {
                Con = new SqlConnection(conString);
            }
            catch (Exception ex)
            {
                throw new Exception("Es konnte keine Vebindung zum MSSQL Server hergestellt werden! /n/r" + ex.Message);
            }
        }

        /// <summary>
        /// Öffnet die Verbindung zum MSSQL Server
        /// </summary>
        public void Open ()
        {
            if (Con.State == System.Data.ConnectionState.Open)
                return;
            Con.Open();
        }

        /// <summary>
        /// Schließt die Verbindung zum MSSQL Server
        /// </summary>
        public void Close ()
        {
            Con.Close();
        }


        /// <summary>
        /// Erstellt die Tabellen die der Deamon temporär befüllt
        /// </summary>
        internal static void CreateTempTables()
        {
            #region SQL Befehl
            string sql = $"CREATE TABLE {TBL_tmp_AD_Computers} " +
                    $"( " +
                    $"	[SID] [nvarchar](100) NOT NULL, " +
                    $"	[Name] [nvarchar](100) NULL, " +
                    $"	[SamAccountName] [nvarchar](100) NULL, " +
                    $"	[DistinguishedName] [nvarchar](300) NULL, " +
                    $"	[DisplayName] [nvarchar](100) NULL, " +
                    $"	[Description] [nchar](300) NULL, " +
                    $"	[Enabled] [smallint] NULL, " +
                    $"	[LastLogon] [datetime] NULL, " +
                    $"	[LastPasswordSet] [datetime] NULL, " +
                    $"CONSTRAINT [PK_adcomputers] PRIMARY KEY CLUSTERED  " +
                    $"	( " +
                    $"		[SID] ASC " +
                    $"	) " +
                    $"); " +
                    $" " +
                    $" " +
                    $"CREATE TABLE {TBL_tmp_AD_Groups} " +
                    $"( " +
                    $"	[SID] [nvarchar](100) NOT NULL, " +
                    $"	[SamAccountName] [nvarchar](100) NULL, " +
                    $"	[DistinguishedName] [nvarchar](300) NULL, " +
                    $"	[Name] [nvarchar](100) NULL, " +
                    $"	[Description] [nvarchar](max) NULL, " +
                    $"	[IsSecurityGroup] [smallint] NULL, " +
                    $"	[GroupScope] [nvarchar](50) NULL, " +
                    $"CONSTRAINT [PK_adgroups] PRIMARY KEY CLUSTERED  " +
                    $"	( " +
                    $"		[SID] ASC " +
                    $"	) " +
                    $"); " +
                    $" " +
                    $" " +
                    $"CREATE TABLE {TBL_tmp_AD_Users} " +
                    $"( " +
                    $"	[SID] [nvarchar](100) NOT NULL, " +
                    $"	[DisplayName] [nvarchar](100) NULL, " +
                    $"	[SamAccountName] [nvarchar](100) NULL, " +
                    $"	[DistinguishedName] [nvarchar](300) NULL, " +
                    $"	[UserPrincipalName] [nvarchar](100) NULL, " +
                    $"	[Enabled] [smallint] NOT NULL, " +
                    $" CONSTRAINT [PK_adusers] PRIMARY KEY CLUSTERED  " +
                    $"	( " +
                    $"		[SID] ASC " +
                    $"	) " +
                    $"); " +
                    $" " +
                    $" " +
                    $"CREATE TABLE {TBL_tmp_AD_UserInGroup} " +
                    $"( " +
                    $"	[grpSID] [nvarchar](100) NOT NULL, " +
                    $"	[userSID] [nvarchar](100) NOT NULL " +
                    $"); " +
                    $" " +
                    $" " +
                    $"CREATE TABLE {TBL_tmp_FS_ACEs} " +
                    $"( " +
                    $"	[_ace_id] [int] IDENTITY(1,1) NOT NULL, " +
                    $"	[_sid] [varchar](100) NOT NULL, " +
                    $"	[_rights] [int] NOT NULL, " +
                    $"	[_type] [bit] NOT NULL, " +
                    $"	[_fsr] [varchar](500) NOT NULL, " +
                    $"	[_is_inherited] [bit] NOT NULL, " +
                    $"	[_inheritance_flags] [int] NOT NULL, " +
                    $"	[_propagation_flags] [int] NOT NULL, " +
                    $"	[_ace_hash] [varchar](50) NOT NULL, " +
                    $" CONSTRAINT [PK_aces] PRIMARY KEY CLUSTERED  " +
                    $"	( " +
                    $"		[_ace_id] ASC " +
                    $"	) " +
                    $"); " +
                    $" " +
                    $" " +
                    $"CREATE TABLE {TBL_tmp_FS_ACLs} " +
                    $"( " +
                    $"	[_path_id] [int] NOT NULL, " +
                    $"	[_ace_id] [int] NOT NULL, " +
                    $"	[_type] [int] NOT NULL, " +
                    $" CONSTRAINT [PK_acls] PRIMARY KEY CLUSTERED  " +
                    $"	( " +
                    $"		[_path_id] ASC, " +
                    $"		[_ace_id] ASC, " +
                    $"		[_type] ASC " +
                    $"	) " +
                    $"); " +
                    $" " +
                    $" " +
                    $"CREATE TABLE {TBL_tmp_FS_Dirs}( " +
                    $"	[_path_id] [int] IDENTITY(1,1) NOT NULL, " +
                    $"	[_path_name] [nvarchar](max) NOT NULL, " +
                    $"	[_owner_sid] [nvarchar](100) NOT NULL, " +
                    $"	[_parent_path_id] [int] NOT NULL, " +
                    $"	[_is_root] [bit] NOT NULL, " +
                    $"	[_has_children] [bit] NOT NULL, " +
                    $"	[_scan_deepth] [int] NOT NULL, " +
                    $"	[_size] [bigint] DEFAULT 0, " +
                    $"	[_path_hash] [nchar](40) NOT NULL, " +
                    $" CONSTRAINT [PK_path_name] PRIMARY KEY CLUSTERED  " +
                    $"	( " +
                    $"		[_path_id] ASC " +
                    $"	), " +
                    $" CONSTRAINT [IX_path_hash] UNIQUE NONCLUSTERED  " +
                    $"	( " +
                    $"		[_path_hash] ASC " +
                    $"	) " +
                    $"); " +
                    $" " +
                    $" " +
                    $"CREATE TABLE {TBL_tmp_FS_Shares} ( " +
                    $"	[_path_id] [int] IDENTITY(1,1) NOT NULL, " +
                    $"	[_unc_path_name] [nvarchar](max) NOT NULL, " +
                    $"	[_owner_sid] [nvarchar](100) NOT NULL, " +
                    $"	[_has_children] [bit] NOT NULL, " +
                    $"	[_size] [bigint] NOT NULL, " +
                    $"	[_path_hash] [nchar](40) NOT NULL, " +
                    $"	[_path_name] [nvarchar](max) NULL, " +
                    $"	[_display_name] [nvarchar](50) NULL, " +
                    $"	[_remark] [nvarchar](300) NULL, " +
                    $"	[_share_type] [nchar](30) NOT NULL, " +
                    $"	[_hidden] [bit] NOT NULL, " +
                    $" CONSTRAINT [PK_shares] PRIMARY KEY CLUSTERED  " +
                    $"	( " +
                    $"		[_path_id] ASC " +
                    $"	) " +
                    $"); ";
            #endregion


            var mssql = new MsSql();
            mssql.Open();

            SqlCommand cmd = new SqlCommand(sql, mssql.Con);
            cmd.ExecuteNonQuery();

        }


        /// <summary>
        /// Löscht die Temporären Tabellen
        /// </summary>
        internal static void DeleteTempTables()
        {
            string sql = $"DROP TABLE IF EXISTS {TBL_tmp_AD_Computers}; " +
                $"DROP TABLE IF EXISTS {TBL_tmp_AD_Groups}; " +
                $"DROP TABLE IF EXISTS {TBL_tmp_AD_UserInGroup}; " +
                $"DROP TABLE IF EXISTS {TBL_tmp_AD_Users}; " +
                $"DROP TABLE IF EXISTS {TBL_tmp_FS_ACEs}; " +
                $"DROP TABLE IF EXISTS {TBL_tmp_FS_ACLs}; " +
                $"DROP TABLE IF EXISTS {TBL_tmp_FS_Dirs}; " +
                $"DROP TABLE IF EXISTS {TBL_tmp_FS_Shares}; ";

            var mssql = new MsSql();
            mssql.Open();

            SqlCommand cmd = new SqlCommand(sql, mssql.Con);
            cmd.ExecuteNonQuery();
        }
    }

}
