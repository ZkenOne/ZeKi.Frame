<script runat="template">

private string NowData
{
	get
	{
		return DateTime.Now.ToString("yyyy-MM-dd");
	}
}


private string IBLLNameSpace
{
	get
	{	
		string space = String.Empty;
		string tbname = SourceTable.Name;
		if(tbname.Split('_')[0].ToLower() == "base")
		{
			space = "CySoft.IBLL";
		}
		if( tbname.Split('_')[0].ToLower() == "tb" || 
				 tbname.Split('_')[0].ToLower() == "td" || 
				 tbname.Split('_')[0].ToLower() == "ts" || 
				 tbname.Split('_')[0].ToLower() == "tp" || 
				 tbname.Split('_')[0].ToLower() == "tz" ||
				 tbname.Split('_')[0].ToLower() == "tr"
				)
		{
			space = "CySoft.IBLL." + tbname.Split('_')[0].Substring(0,1).ToUpper();
			if(tbname.Split('_')[0].Length > 1)
			{
				space += tbname.Substring(1,tbname.Split('_')[0].Length-1);
			}
		}
		return space;
	}
}




private string IDALNameSpace
{
	get
	{	
		string space = String.Empty;
		string tbname = SourceTable.Name;
		if(tbname.Split('_')[0].ToLower() == "base")
		{
			space = "CySoft.IDAL";
		}
		if( tbname.Split('_')[0].ToLower() == "tb" || 
				 tbname.Split('_')[0].ToLower() == "td" || 
				 tbname.Split('_')[0].ToLower() == "ts" || 
				 tbname.Split('_')[0].ToLower() == "tp" || 
				 tbname.Split('_')[0].ToLower() == "tz" ||
				 tbname.Split('_')[0].ToLower() == "tr"
				)
		{
			space = "CySoft.IDAL." + tbname.Split('_')[0].Substring(0,1).ToUpper();
			if(tbname.Split('_')[0].Length > 1)
			{
				space += tbname.Substring(1,tbname.Split('_')[0].Length-1);
			}
		}
		return space;
	}
}




private string BLLNameSpace
{
	get
	{	
		string space = String.Empty;
		string tbname = SourceTable.Name;
		if(tbname.Split('_')[0].ToLower() == "base")
		{
			space = "CySoft.BLL";
		}
		if( tbname.Split('_')[0].ToLower() == "tb" || 
				 tbname.Split('_')[0].ToLower() == "td" || 
				 tbname.Split('_')[0].ToLower() == "ts" || 
				 tbname.Split('_')[0].ToLower() == "tp" || 
				 tbname.Split('_')[0].ToLower() == "tz" ||
				 tbname.Split('_')[0].ToLower() == "tr"
				)
		{
			space = "CySoft.BLL." + tbname.Split('_')[0].Substring(0,1).ToUpper();
			if(tbname.Split('_')[0].Length > 1)
			{
				space += tbname.Substring(1,tbname.Split('_')[0].Length-1);
			}
		}
		return space;
	}
}




private string DALNameSpace
{
	get
	{	
		string space = String.Empty;
		string tbname = SourceTable.Name;
		if(tbname.Split('_')[0].ToLower() == "base")
		{
			space = "CySoft.DAL";
		}
		if( tbname.Split('_')[0].ToLower() == "tb" || 
				 tbname.Split('_')[0].ToLower() == "td" || 
				 tbname.Split('_')[0].ToLower() == "ts" || 
				 tbname.Split('_')[0].ToLower() == "tp" || 
				 tbname.Split('_')[0].ToLower() == "tz" ||
				 tbname.Split('_')[0].ToLower() == "tr"
				)
		{
			space = "CySoft.DAL." + tbname.Split('_')[0].Substring(0,1).ToUpper();
			if(tbname.Split('_')[0].Length > 1)
			{
				space += tbname.Substring(1,tbname.Split('_')[0].Length-1);
			}
		}
		return space;
	}
}




private string ModelNameSpaceTest
{
	get
	{
		string space = String.Empty;
		string tbname = SourceTable.Name;
        if(tbname.Split('_')[0].ToLower() == "base")
		{
			space = "CySoft.Model.BaseSys";
		}
        if(tbname.Contains("_"))
        {
        if( tbname.Split('_')[0].ToLower() == "tb" || 
				 tbname.Split('_')[0].ToLower() == "td" || 
				 tbname.Split('_')[0].ToLower() == "ts" || 
				 tbname.Split('_')[0].ToLower() == "tp" || 
				 tbname.Split('_')[0].ToLower() == "tz" ||
				 tbname.Split('_')[0].ToLower() == "tr"
				)
		{
			space = "CySoft.Model." + tbname.Split('_')[0].Substring(0,1).ToUpper();
			if(tbname.Split('_')[0].Length > 1)
			{
				space += tbname.Substring(1,tbname.Split('_')[0].Length-1);
			}
		}
        }else
        {
        space = "CySoft.Model." + tbname.Substring(0,1).ToUpper();
        if(tbname.Length > 1)
			{
				space += tbname.Substring(1,tbname.Length-1);
			}
        }
		
		return space;
	}
}


private string ModelNameSpace
{
	get
	{
		string space = String.Empty;
		string tbname = SourceTable.Name;
        if(tbname.Split('_')[0].ToLower() == "base")
		{
			space = "CySoft.Model.BaseSys";
		}
		if( tbname.Split('_')[0].ToLower() == "tb" || 
				 tbname.Split('_')[0].ToLower() == "td" || 
				 tbname.Split('_')[0].ToLower() == "ts" || 
				 tbname.Split('_')[0].ToLower() == "tp" || 
				 tbname.Split('_')[0].ToLower() == "tz" ||
				 tbname.Split('_')[0].ToLower() == "tr"
				)
		{
			space = "CySoft.Model." + tbname.Split('_')[0].Substring(0,1).ToUpper();
			if(tbname.Split('_')[0].Length > 1)
			{
				space += tbname.Substring(1,tbname.Split('_')[0].Length-1);
			}
		}
		return space;
	}
}

private string HelperNameSpace
{
	get
	{	
		string space = String.Empty;
		string tbname = SourceTable.Name;
		if(tbname.Split('_')[0].ToLower() == "base")
		{
			space = "CySoft.RS.MapHelper.BaseSys";
		}
		if( tbname.Split('_')[0].ToLower() == "tb" || 
				 tbname.Split('_')[0].ToLower() == "td" || 
				 tbname.Split('_')[0].ToLower() == "ts" || 
				 tbname.Split('_')[0].ToLower() == "tp" || 
				 tbname.Split('_')[0].ToLower() == "tz" ||
				 tbname.Split('_')[0].ToLower() == "tr"
				)
		{
			space = "CySoft.RS.MapHelper." + tbname.Split('_')[0].Substring(0,1).ToUpper();
			if(tbname.Split('_')[0].Length > 1)
			{
				space += tbname.Substring(1,tbname.Split('_')[0].Length-1);
			}
		}
		return space;
	}
}

private string ModelClassName
{
	get
	{
		string classname = String.Empty;
		string[] strs = SourceTable.Name.Split('_');
		foreach(string str in strs)
		{
			if(!String.IsNullOrEmpty(str))
			{
				if(!String.IsNullOrEmpty(classname))
				{
					classname += "_";
				}
				classname += str.Substring(0,1).ToUpper();
				if(str.Length > 1)
				{
					classname += str.Substring(1,str.Length -1);
				}
			}
		}
		return classname;
	}
}

private string DefaultSortColumn
{
	get
	{
		string column = String.Empty;
		if(SourceTable.HasPrimaryKey)
		{
			column = SourceTable.PrimaryKey.MemberColumns[0].Name;
		}
		else
		{
			column = SourceTable.Columns[0].Name;
		}
		return column;
	}
}

private string PrimaryKeyColumns
{
	get
	{
		string columns = String.Empty;
		foreach(ColumnSchema column in SourceTable.PrimaryKey.MemberColumns)
		{
			if(!String.IsNullOrEmpty(columns))
			{
				columns +=	",";
			}
			columns += column.Name;
		}
		return columns;
	}
}

private string GetModelClassName(string tableName)
{
	string classname = String.Empty;
	string[] strs = tableName.Split('_');
	foreach (string str in strs)
	{
		if (!String.IsNullOrEmpty(str))
		{
			if (!String.IsNullOrEmpty(classname))
			{
				classname += "_";
			}
			classname += str.Substring(0, 1).ToUpper();
			if (str.Length > 1)
			{
				classname += str.Substring(1, str.Length - 1);
			}
		}
	}
	return classname;
}

private string GetDescription(ColumnSchema column)
{
	string description = String.Empty;
	if(column.Description != null && !String.IsNullOrEmpty(column.Description.Trim()))
	{
		description = "//" + column.Description.Trim();
	}
	return description;
}

private string GetSystemType(ColumnSchema column)
{
	string systemDataType = column.SystemType.ToString();
	
	if(systemDataType == "System.DateTime")
	{
		systemDataType = "DateTime";
	}
	return systemDataType;
}

private string GetPropertyAttrStr(ColumnSchema column)
{
	string str = String.Empty;
	bool isIdentity = (bool)column.ExtendedProperties["CS_IsIdentity"].Value;  //是否自增列

	if (column.IsPrimaryKeyMember)
    {
		if (column.NativeType == "varchar")
        {
			str = "[Property(true, false, DbIgnore.Update, DbType.AnsiString, " + column.Size + ")]";
		}
		else if (column.NativeType == "nvarchar")
		{
			str = "[Property(true, false, DbIgnore.Update, DbType.String, " + column.Size + ")]";
		}
        else
        {
			str = "[Property(true, false, DbIgnore.Update)]";
		}
	}
	else if(isIdentity)
    {
		str = "[Property(DbIgnore.InsertAndUpdate)]";
	}
	else if(column.Name =="rq" || column.Name == "rq_create")
    {
		str = "[Property(DbIgnore.Update)]";
	}
	else if (column.Name == "nlast" && column.NativeType=="timestamp")
	{
		str = "[Property(DbIgnore.InsertAndUpdate)]";
	}
	else if (column.NativeType == "varchar")
	{
		str = "[Property(DbType.AnsiString, " + column.Size + ")]";
	}
	else if (column.NativeType == "nvarchar")
	{
		str = "[Property(DbType.String, " + column.Size + ")]";
	}
	return str;
}

private string GetDefaultValue(ColumnSchema column)
{
	string str = String.Empty;
	string systemDataType = column.SystemType.ToString();
	switch(systemDataType)
	{
		case "System.DateTime":
			str = "new DateTime(1900,1,1)";
			break;
        case "System.Byte":
			str = "0";
			break;
		case "System.Int16":
			str = "0";
			break;
		case "System.Int32":
			str = "0";
			break;
		case "System.Int64":
			str = "0";
			break;
		case "System.Decimal":
			str = "0m";
			break;
		case "System.String":
			str = "String.Empty";
			break;
		case "System.Double":
			str = "0.0000000d";
			break;
        case "System.Guid":
			str = "Guid.Empty";
			break;
       case "type":
            str=null;
            break;
       case "System.Byte[]":
			str = null;
			break;
		default:
			throw new Exception(String.Format("can't transact [{0}] type, column name:{1}",systemDataType,column.Name));
	}
	return str;
}

private string GetDefaultValueStr(ColumnSchema column)
{
	string str = String.Empty;
	string systemDataType = column.SystemType.ToString();
	switch(systemDataType)
	{
		case "System.DateTime":
			str = " = DateTime.Now;";
			break;
        case "System.Byte":
			str = " = 0;";
			break;
		case "System.Int16":
			str = " = 0;";
			break;
		case "System.Int32":
			str = " = 0;";
			break;
		case "System.Int64":
			str = " = 0;";
			break;
		case "System.Decimal":
			str = " = 0m;";
			break;
		case "System.Double":
			str = " = 0d;";
            break;
		case "System.String":
			str = " = string.Empty;";
			break;
	}
	return str;
}

private string GetCanNullColumn(ColumnSchema column)
{
	string systemDataType = column.SystemType.ToString();
	bool allowBbNull = column.AllowDBNull;
	switch (systemDataType)
	{
		case "System.DateTime":
			systemDataType = allowBbNull ? "DateTime?" : "DateTime";
			break;
		case "System.Byte":
			systemDataType = allowBbNull ? "byte?" : "byte";
			break;
		case "System.Int32":
			systemDataType = allowBbNull ? "int?" : "int";
			break;
		case "System.Int16":
			systemDataType = allowBbNull ? "short?" : "short";
			break;
		case "System.Int64":
			systemDataType = allowBbNull ? "long?" : "long";
			break;
		case "System.Decimal":
			systemDataType = allowBbNull ? "decimal?" : "decimal";
			break;
		case "System.String":
			systemDataType = "string";
			break;
		case "System.Guid":
			systemDataType = "Guid";
			break;
		case "System.Byte[]":
			systemDataType = "byte[]";
			break;
		default:
			throw new Exception(String.Format("模板未设置类型{0}的处理方式", systemDataType));
	}

	return systemDataType;
}

private string UpdateSql
{
	get
	{
		string set = String.Empty;
		foreach(ColumnSchema column in SourceTable.NonKeyColumns)
		{
			string colName = column.Name;
			if(colName!="n_last" && colName!="xh" && colName != "sl_sum")
			{
				if(!String.IsNullOrEmpty(set))
				{
					set +=",";
				}
				set+= String.Format("db.[{0}]=#{0}#",colName);
			}
		}
		string where = String.Empty;
		foreach(ColumnSchema column in SourceTable.PrimaryKey.MemberColumns)
		{
			if(!String.IsNullOrEmpty(where))
			{
				where +=" and ";
			}
			where += String.Format("db.[{0}]=#{0}#",column.Name);
		}
		
		string sql = "update db set " + set + " from <include refid=\"Table\"/> as db where " + where;
		return sql;
	}
}

private string SelectSql
{
	get
	{
		string columnNames = String.Empty;
		foreach(ColumnSchema column in SourceTable.Columns)
		{
			if(column.Name != "n_last")
			{
				if(!String.IsNullOrEmpty(columnNames))
				{
					columnNames += ",";
				}
				if(column.NativeType == "time")
				{
					columnNames += "Convert(char(8),db.[" + column.Name+ "]) as " + column.Name;
				}
				else
				{
					columnNames += "db.["+column.Name+"]";
				}
			}
		}
		string sql = "select " + columnNames + " from <include refid=\"Table\"/> as db";
		return sql;
	}
}

private string IndexTableKeys
{
	get
	{
		string keyNames = String.Empty;
		foreach(ColumnSchema column in SourceTable.PrimaryKey.MemberColumns)
		{
			if(!String.IsNullOrEmpty(keyNames))
			{
				keyNames += ",";
			}
			keyNames+= String.Format("n{0} {1}",column.Name,GetDbTypeAndSize(column));
		}
		return keyNames;
	}
}

private string GetDbTypeAndSize(ColumnSchema column)
{
	string str = String.Empty;
	string nativeType = column.NativeType;
	switch (nativeType)
    {
        case "tinyint":
			str = "tinyint";
            break;
        case "int":
			str = "int";
            break;
        case "bigint":
			str = "bigint";
            break;
        case "smallint":
			str = "smallint";
            break;
		case "varchar":
			str = String.Format("varchar({0})",column.Size);
            break;
		case "nvarchar":
			str = String.Format("nvarchar({0})",column.Size);
            break;
		case "char":
			str = String.Format("char({0})",column.Size);
            break;
		case "nchar":
			str = String.Format("nchar({0})",column.Size);
            break;
		case "decimal":
			str = "decimal(18,7)";
            break;
		case "numeric":
			str = "numeric(20,4)";
            break;
		case "datetime":
			str = "datetime";
            break;
        default:
			throw new Exception(String.Format("模板未设置类型{0}的处理方式",nativeType));
    }
	
	return str;
}

private string IndexKeyNames
{
	get
	{
		string keyNames = String.Empty;
		if(SourceTable.HasPrimaryKey)
		{
			foreach(ColumnSchema column in SourceTable.PrimaryKey.MemberColumns)
			{
				if(!String.IsNullOrEmpty(keyNames))
				{
					keyNames += ",";
				}
				keyNames+= "n" + column.Name;
			}
		}
		else
		{
			foreach(ColumnSchema column in SourceTable.Columns)
			{
				if(!String.IsNullOrEmpty(keyNames))
				{
					keyNames += ",";
				}
				keyNames+= column.Name;
			}
		}
		return keyNames;
	}
}

private string TableKeyNames
{
	get
	{
		string keyNames = String.Empty;
		if(SourceTable.HasPrimaryKey)
		{
			foreach(ColumnSchema column in SourceTable.PrimaryKey.MemberColumns)
			{
				if(!String.IsNullOrEmpty(keyNames))
				{
					keyNames += ",";
				}
				keyNames+= column.Name;
			}
		}
		else
		{
			foreach(ColumnSchema column in SourceTable.Columns)
			{
				if(!String.IsNullOrEmpty(keyNames))
				{
					keyNames += ",";
				}
				keyNames+= column.Name;
			}
		}
		return keyNames;
	}
}


private string PageWhere
{
	get
	{
		string keyNames = String.Empty;
		if(SourceTable.HasPrimaryKey)
		{
			foreach(ColumnSchema column in SourceTable.PrimaryKey.MemberColumns)
			{
				if(!String.IsNullOrEmpty(keyNames))
				{
					keyNames += " and ";
				}
				keyNames+= String.Format("db.[{0}]=t.n{0}",column.Name);
			}
		}
		else
		{
			keyNames = "1=1";
		}
		return keyNames;
	}
}

private string BodyClassName
{
	get
	{
		string modelClassName = ModelClassName;
		string classname = modelClassName.Substring(0,modelClassName.Length -1)+"2";
		return classname;
	}
}

</script>