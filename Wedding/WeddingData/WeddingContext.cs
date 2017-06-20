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

namespace WeddingData
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
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="WEDDING")]
	public partial class WeddingContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertGuest(Guest instance);
    partial void UpdateGuest(Guest instance);
    partial void DeleteGuest(Guest instance);
    #endregion
		
		public WeddingContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public WeddingContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public WeddingContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public WeddingContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<Guest> Guest
		{
			get
			{
				return this.GetTable<Guest>();
			}
		}
		
		[global::System.Data.Linq.Mapping.FunctionAttribute(Name="dbo.fn_diagramobjects", IsComposable=true)]
		[return: global::System.Data.Linq.Mapping.ParameterAttribute(DbType="Int")]
		public System.Nullable<int> Fn_diagramobjects()
		{
			return ((System.Nullable<int>)(this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod()))).ReturnValue));
		}
		
		[global::System.Data.Linq.Mapping.FunctionAttribute(Name="dbo.sp_alterdiagram")]
		[return: global::System.Data.Linq.Mapping.ParameterAttribute(DbType="Int")]
		public int Sp_alterdiagram([global::System.Data.Linq.Mapping.ParameterAttribute(DbType="NVarChar(128)")] string diagramname, [global::System.Data.Linq.Mapping.ParameterAttribute(DbType="Int")] System.Nullable<int> owner_id, [global::System.Data.Linq.Mapping.ParameterAttribute(DbType="Int")] System.Nullable<int> version, [global::System.Data.Linq.Mapping.ParameterAttribute(DbType="VarBinary(MAX)")] System.Data.Linq.Binary definition)
		{
			IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), diagramname, owner_id, version, definition);
			return ((int)(result.ReturnValue));
		}
		
		[global::System.Data.Linq.Mapping.FunctionAttribute(Name="dbo.sp_creatediagram")]
		[return: global::System.Data.Linq.Mapping.ParameterAttribute(DbType="Int")]
		public int Sp_creatediagram([global::System.Data.Linq.Mapping.ParameterAttribute(DbType="NVarChar(128)")] string diagramname, [global::System.Data.Linq.Mapping.ParameterAttribute(DbType="Int")] System.Nullable<int> owner_id, [global::System.Data.Linq.Mapping.ParameterAttribute(DbType="Int")] System.Nullable<int> version, [global::System.Data.Linq.Mapping.ParameterAttribute(DbType="VarBinary(MAX)")] System.Data.Linq.Binary definition)
		{
			IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), diagramname, owner_id, version, definition);
			return ((int)(result.ReturnValue));
		}
		
		[global::System.Data.Linq.Mapping.FunctionAttribute(Name="dbo.sp_dropdiagram")]
		[return: global::System.Data.Linq.Mapping.ParameterAttribute(DbType="Int")]
		public int Sp_dropdiagram([global::System.Data.Linq.Mapping.ParameterAttribute(DbType="NVarChar(128)")] string diagramname, [global::System.Data.Linq.Mapping.ParameterAttribute(DbType="Int")] System.Nullable<int> owner_id)
		{
			IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), diagramname, owner_id);
			return ((int)(result.ReturnValue));
		}
		
		[global::System.Data.Linq.Mapping.FunctionAttribute(Name="dbo.sp_helpdiagramdefinition")]
		public ISingleResult<Sp_helpdiagramdefinitionResult> Sp_helpdiagramdefinition([global::System.Data.Linq.Mapping.ParameterAttribute(DbType="NVarChar(128)")] string diagramname, [global::System.Data.Linq.Mapping.ParameterAttribute(DbType="Int")] System.Nullable<int> owner_id)
		{
			IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), diagramname, owner_id);
			return ((ISingleResult<Sp_helpdiagramdefinitionResult>)(result.ReturnValue));
		}
		
		[global::System.Data.Linq.Mapping.FunctionAttribute(Name="dbo.sp_helpdiagrams")]
		public ISingleResult<Sp_helpdiagramsResult> Sp_helpdiagrams([global::System.Data.Linq.Mapping.ParameterAttribute(DbType="NVarChar(128)")] string diagramname, [global::System.Data.Linq.Mapping.ParameterAttribute(DbType="Int")] System.Nullable<int> owner_id)
		{
			IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), diagramname, owner_id);
			return ((ISingleResult<Sp_helpdiagramsResult>)(result.ReturnValue));
		}
		
		[global::System.Data.Linq.Mapping.FunctionAttribute(Name="dbo.sp_renamediagram")]
		[return: global::System.Data.Linq.Mapping.ParameterAttribute(DbType="Int")]
		public int Sp_renamediagram([global::System.Data.Linq.Mapping.ParameterAttribute(DbType="NVarChar(128)")] string diagramname, [global::System.Data.Linq.Mapping.ParameterAttribute(DbType="Int")] System.Nullable<int> owner_id, [global::System.Data.Linq.Mapping.ParameterAttribute(DbType="NVarChar(128)")] string new_diagramname)
		{
			IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), diagramname, owner_id, new_diagramname);
			return ((int)(result.ReturnValue));
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Guest")]
	public partial class Guest : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private System.Guid _GuestID;
		
		private string _GuestName;
		
		private string _ZipCode;
		
		private byte _MaxSize;
		
		private bool _Confirmed;
		
		private bool _InviteSent;
		
		private bool _AllowLodging;
		
		private System.Nullable<byte> @__Count;
		
		private System.Nullable<byte> @__NeedAccommodations;
		
		private string @__Notes;
		
		private System.Nullable<System.DateTime> @__RsvpDate;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnGuestIDChanging(System.Guid value);
    partial void OnGuestIDChanged();
    partial void OnGuestNameChanging(string value);
    partial void OnGuestNameChanged();
    partial void OnZipCodeChanging(string value);
    partial void OnZipCodeChanged();
    partial void OnMaxSizeChanging(byte value);
    partial void OnMaxSizeChanged();
    partial void OnConfirmedChanging(bool value);
    partial void OnConfirmedChanged();
    partial void OnInviteSentChanging(bool value);
    partial void OnInviteSentChanged();
    partial void OnAllowLodgingChanging(bool value);
    partial void OnAllowLodgingChanged();
    partial void On_CountChanging(System.Nullable<byte> value);
    partial void On_CountChanged();
    partial void On_NeedAccommodationsChanging(System.Nullable<byte> value);
    partial void On_NeedAccommodationsChanged();
    partial void On_NotesChanging(string value);
    partial void On_NotesChanged();
    partial void On_RsvpDateChanging(System.Nullable<System.DateTime> value);
    partial void On_RsvpDateChanged();
    #endregion
		
		public Guest()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_GuestID", DbType="UniqueIdentifier NOT NULL", IsPrimaryKey=true)]
		public System.Guid GuestID
		{
			get
			{
				return this._GuestID;
			}
			set
			{
				if ((this._GuestID != value))
				{
					this.OnGuestIDChanging(value);
					this.SendPropertyChanging();
					this._GuestID = value;
					this.SendPropertyChanged("GuestID");
					this.OnGuestIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_GuestName", DbType="VarChar(256) NOT NULL", CanBeNull=false)]
		public string GuestName
		{
			get
			{
				return this._GuestName;
			}
			set
			{
				if ((this._GuestName != value))
				{
					this.OnGuestNameChanging(value);
					this.SendPropertyChanging();
					this._GuestName = value;
					this.SendPropertyChanged("GuestName");
					this.OnGuestNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ZipCode", DbType="VarChar(16)")]
		public string ZipCode
		{
			get
			{
				return this._ZipCode;
			}
			set
			{
				if ((this._ZipCode != value))
				{
					this.OnZipCodeChanging(value);
					this.SendPropertyChanging();
					this._ZipCode = value;
					this.SendPropertyChanged("ZipCode");
					this.OnZipCodeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_MaxSize", DbType="TinyInt NOT NULL")]
		public byte MaxSize
		{
			get
			{
				return this._MaxSize;
			}
			set
			{
				if ((this._MaxSize != value))
				{
					this.OnMaxSizeChanging(value);
					this.SendPropertyChanging();
					this._MaxSize = value;
					this.SendPropertyChanged("MaxSize");
					this.OnMaxSizeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Confirmed", DbType="Bit NOT NULL")]
		public bool Confirmed
		{
			get
			{
				return this._Confirmed;
			}
			set
			{
				if ((this._Confirmed != value))
				{
					this.OnConfirmedChanging(value);
					this.SendPropertyChanging();
					this._Confirmed = value;
					this.SendPropertyChanged("Confirmed");
					this.OnConfirmedChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_InviteSent", DbType="Bit NOT NULL")]
		public bool InviteSent
		{
			get
			{
				return this._InviteSent;
			}
			set
			{
				if ((this._InviteSent != value))
				{
					this.OnInviteSentChanging(value);
					this.SendPropertyChanging();
					this._InviteSent = value;
					this.SendPropertyChanged("InviteSent");
					this.OnInviteSentChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_AllowLodging", DbType="Bit NOT NULL")]
		public bool AllowLodging
		{
			get
			{
				return this._AllowLodging;
			}
			set
			{
				if ((this._AllowLodging != value))
				{
					this.OnAllowLodgingChanging(value);
					this.SendPropertyChanging();
					this._AllowLodging = value;
					this.SendPropertyChanged("AllowLodging");
					this.OnAllowLodgingChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="__Count", DbType="TinyInt")]
		public System.Nullable<byte> _Count
		{
			get
			{
				return this.@__Count;
			}
			set
			{
				if ((this.@__Count != value))
				{
					this.On_CountChanging(value);
					this.SendPropertyChanging();
					this.@__Count = value;
					this.SendPropertyChanged("_Count");
					this.On_CountChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="__NeedAccommodations", DbType="TinyInt")]
		public System.Nullable<byte> _NeedAccommodations
		{
			get
			{
				return this.@__NeedAccommodations;
			}
			set
			{
				if ((this.@__NeedAccommodations != value))
				{
					this.On_NeedAccommodationsChanging(value);
					this.SendPropertyChanging();
					this.@__NeedAccommodations = value;
					this.SendPropertyChanged("_NeedAccommodations");
					this.On_NeedAccommodationsChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="__Notes", DbType="NVarChar(MAX)", UpdateCheck=UpdateCheck.Never)]
		public string _Notes
		{
			get
			{
				return this.@__Notes;
			}
			set
			{
				if ((this.@__Notes != value))
				{
					this.On_NotesChanging(value);
					this.SendPropertyChanging();
					this.@__Notes = value;
					this.SendPropertyChanged("_Notes");
					this.On_NotesChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="__RsvpDate", DbType="DateTime2(0)")]
		public System.Nullable<System.DateTime> _RsvpDate
		{
			get
			{
				return this.@__RsvpDate;
			}
			set
			{
				if ((this.@__RsvpDate != value))
				{
					this.On_RsvpDateChanging(value);
					this.SendPropertyChanging();
					this.@__RsvpDate = value;
					this.SendPropertyChanged("_RsvpDate");
					this.On_RsvpDateChanged();
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
	
	public partial class Sp_helpdiagramdefinitionResult
	{
		
		private System.Nullable<int> _Version;
		
		private System.Data.Linq.Binary _Definition;
		
		public Sp_helpdiagramdefinitionResult()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Name="version", Storage="_Version", DbType="Int")]
		public System.Nullable<int> Version
		{
			get
			{
				return this._Version;
			}
			set
			{
				if ((this._Version != value))
				{
					this._Version = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Name="definition", Storage="_Definition", DbType="VarBinary(MAX)", CanBeNull=true)]
		public System.Data.Linq.Binary Definition
		{
			get
			{
				return this._Definition;
			}
			set
			{
				if ((this._Definition != value))
				{
					this._Definition = value;
				}
			}
		}
	}
	
	public partial class Sp_helpdiagramsResult
	{
		
		private string _Database;
		
		private string _Name;
		
		private System.Nullable<int> _ID;
		
		private string _Owner;
		
		private System.Nullable<int> _OwnerID;
		
		public Sp_helpdiagramsResult()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Database", DbType="NVarChar(128)")]
		public string Database
		{
			get
			{
				return this._Database;
			}
			set
			{
				if ((this._Database != value))
				{
					this._Database = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Name", DbType="NVarChar(128)")]
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
					this._Name = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ID", DbType="Int")]
		public System.Nullable<int> ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				if ((this._ID != value))
				{
					this._ID = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Owner", DbType="NVarChar(128)")]
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
					this._Owner = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_OwnerID", DbType="Int")]
		public System.Nullable<int> OwnerID
		{
			get
			{
				return this._OwnerID;
			}
			set
			{
				if ((this._OwnerID != value))
				{
					this._OwnerID = value;
				}
			}
		}
	}
}
#pragma warning restore 1591