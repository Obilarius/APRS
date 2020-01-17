using ARPSDeamon;
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

        #region Temp
        public static string TBL_tmp_AD_Users { get { return "temp.adusers"; } }
        public static string TBL_tmp_AD_UserInGroup { get { return "temp.grp_user"; } }
        public static string TBL_tmp_AD_Groups { get { return "temp.adgroups"; } }
        public static string TBL_tmp_AD_Computers { get { return "temp.adcomputers"; } }
        public static string TBL_tmp_FS_Dirs { get { return "temp.dirs"; } }
        public static string TBL_tmp_FS_Shares { get { return "temp.shares"; } }
        public static string TBL_tmp_FS_ACLs { get { return "temp.acls"; } }
        public static string TBL_tmp_FS_ACEs { get { return "temp.aces"; } }
        #endregion

        #region Live
        public static string TBL_AD_Users { get { return "dbo.adusers"; } }
        public static string TBL_AD_UserInGroup { get { return "dbo.grp_user"; } }
        public static string TBL_AD_Groups { get { return "dbo.adgroups"; } }
        public static string TBL_AD_Computers { get { return "dbo.adcomputers"; } }
        public static string TBL_FS_Dirs { get { return "fs.dirs"; } }
        public static string TBL_FS_Shares { get { return "fs.shares"; } }
        public static string TBL_FS_ACLs { get { return "fs.acls"; } }
        public static string TBL_FS_ACEs { get { return "fs.aces"; } }
        #endregion

        #endregion


        //TODO: Umgestellt auf Test Datenbank
        /// <summary>
        /// Connection String
        /// </summary>
        private string conString = @"Data Source=ARPS\SQLEXPRESS;Initial Catalog=ARPS_Test;User Id=LokalArps;Password=nopasswd;MultipleActiveResultSets=True";

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


        /// <summary>
        /// Löscht den Inhalt der live Tabelle, kopiert den Inhalt der temp Tabelle zur live Tabelle und löscht die temp Tabelle
        /// </summary>
        internal static void WriteTempToLive()
        {
            var mssql = new MsSql();
            mssql.Open();

            #region ADUsers
            SqlCommand cmd = new SqlCommand($"SELECT CASE WHEN OBJECT_ID('{TBL_tmp_AD_Users}', 'U') IS NOT NULL THEN 1 ELSE 0 END", mssql.Con);

            if ((int)cmd.ExecuteScalar() == 1)
            {
                string sql = $"TRUNCATE TABLE {TBL_AD_Users}; " +
                $"INSERT INTO {TBL_AD_Users} SELECT * FROM {TBL_tmp_AD_Users}; " +
                $"DROP TABLE IF EXISTS {TBL_tmp_AD_Users}; ";

                cmd = new SqlCommand(sql, mssql.Con);
                cmd.ExecuteNonQuery();
            }
            else
                Log.writeLine("Temp Users existiert nicht");
            #endregion

            #region ADUserInGroup
            cmd = new SqlCommand($"SELECT CASE WHEN OBJECT_ID('{TBL_tmp_AD_UserInGroup}', 'U') IS NOT NULL THEN 1 ELSE 0 END", mssql.Con);

            if ((int)cmd.ExecuteScalar() == 1)
            {
                string sql = $"TRUNCATE TABLE {TBL_AD_UserInGroup}; " +
                $"INSERT INTO {TBL_AD_UserInGroup} SELECT * FROM {TBL_tmp_AD_UserInGroup}; " +
                $"DROP TABLE IF EXISTS {TBL_tmp_AD_UserInGroup}; ";

                cmd = new SqlCommand(sql, mssql.Con);
                cmd.ExecuteNonQuery();
            }
            else
                Log.writeLine("Temp UsersInGroup existiert nicht");
            #endregion

            #region ADGroups
            cmd = new SqlCommand($"SELECT CASE WHEN OBJECT_ID('{TBL_tmp_AD_Groups}', 'U') IS NOT NULL THEN 1 ELSE 0 END", mssql.Con);

            if ((int)cmd.ExecuteScalar() == 1)
            {
                string sql = $"TRUNCATE TABLE {TBL_AD_Groups}; " +
                $"INSERT INTO {TBL_AD_Groups} SELECT * FROM {TBL_tmp_AD_Groups}; " +
                $"DROP TABLE IF EXISTS {TBL_tmp_AD_Groups}; ";

                cmd = new SqlCommand(sql, mssql.Con);
                cmd.ExecuteNonQuery();
            }
            else
                Log.writeLine("Temp Groups existiert nicht");
            #endregion

            #region ADComputers
            cmd = new SqlCommand($"SELECT CASE WHEN OBJECT_ID('{TBL_tmp_AD_Computers}', 'U') IS NOT NULL THEN 1 ELSE 0 END", mssql.Con);

            if ((int)cmd.ExecuteScalar() == 1)
            {
                string sql = $"TRUNCATE TABLE {TBL_AD_Computers}; " +
                $"INSERT INTO {TBL_AD_Computers} SELECT * FROM {TBL_tmp_AD_Computers}; " +
                $"DROP TABLE IF EXISTS {TBL_tmp_AD_Computers}; ";

                cmd = new SqlCommand(sql, mssql.Con);
                cmd.ExecuteNonQuery();
            }
            else
                Log.writeLine("Temp Computers existiert nicht");
            #endregion

            #region FSDirs
            cmd = new SqlCommand($"SELECT CASE WHEN OBJECT_ID('{TBL_tmp_FS_Dirs}', 'U') IS NOT NULL THEN 1 ELSE 0 END", mssql.Con);

            if ((int)cmd.ExecuteScalar() == 1)
            {
                string sql = $"TRUNCATE TABLE {TBL_FS_Dirs}; " +
                $"INSERT INTO {TBL_FS_Dirs} SELECT * FROM {TBL_tmp_FS_Dirs}; " +
                $"DROP TABLE IF EXISTS {TBL_tmp_FS_Dirs}; ";

                cmd = new SqlCommand(sql, mssql.Con);
                cmd.ExecuteNonQuery();
            }
            else
                Log.writeLine("Temp Dirs existiert nicht");
            #endregion

            #region FSShares
            cmd = new SqlCommand($"SELECT CASE WHEN OBJECT_ID('{TBL_tmp_FS_Shares}', 'U') IS NOT NULL THEN 1 ELSE 0 END", mssql.Con);

            if ((int)cmd.ExecuteScalar() == 1)
            {
                string sql = $"TRUNCATE TABLE {TBL_FS_Shares}; " +
                $"INSERT INTO {TBL_FS_Shares} SELECT * FROM {TBL_tmp_FS_Shares}; " +
                $"DROP TABLE IF EXISTS {TBL_tmp_FS_Shares}; ";

                cmd = new SqlCommand(sql, mssql.Con);
                cmd.ExecuteNonQuery();
            }
            else
                Log.writeLine("Temp Shares existiert nicht");
            #endregion

            #region FSACLs
            cmd = new SqlCommand($"SELECT CASE WHEN OBJECT_ID('{TBL_tmp_FS_ACLs}', 'U') IS NOT NULL THEN 1 ELSE 0 END", mssql.Con);

            if ((int)cmd.ExecuteScalar() == 1)
            {
                string sql = $"TRUNCATE TABLE {TBL_FS_ACLs}; " +
                $"INSERT INTO {TBL_FS_ACLs} SELECT * FROM {TBL_tmp_FS_ACLs}; " +
                $"DROP TABLE IF EXISTS {TBL_tmp_FS_ACLs}; ";

                cmd = new SqlCommand(sql, mssql.Con);
                cmd.ExecuteNonQuery();
            }
            else
                Log.writeLine("Temp ACLs existiert nicht");
            #endregion

            #region FSACEs
            cmd = new SqlCommand($"SELECT CASE WHEN OBJECT_ID('{TBL_tmp_FS_ACEs}', 'U') IS NOT NULL THEN 1 ELSE 0 END", mssql.Con);

            if ((int)cmd.ExecuteScalar() == 1)
            {
                string sql = $"TRUNCATE TABLE {TBL_FS_ACEs}; " +
                $"INSERT INTO {TBL_FS_ACEs} SELECT * FROM {TBL_tmp_FS_ACEs}; " +
                $"DROP TABLE IF EXISTS {TBL_tmp_FS_ACEs}; ";

                cmd = new SqlCommand(sql, mssql.Con);
                cmd.ExecuteNonQuery();
            }
            else
                Log.writeLine("Temp ACEs existiert nicht");
            #endregion

            mssql.Close();
            Log.writeLine("Alle Datenbanken ins live System übertragen");
        }
    }

}
