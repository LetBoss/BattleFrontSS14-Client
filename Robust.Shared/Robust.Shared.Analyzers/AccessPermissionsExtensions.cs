using System;

namespace Robust.Shared.Analyzers;

public static class AccessPermissionsExtensions
{
	public static string ToUnixPermissions(this AccessPermissions permissions)
	{
		return permissions switch
		{
			AccessPermissions.None => "---", 
			AccessPermissions.Read => "r--", 
			AccessPermissions.Write => "-w-", 
			AccessPermissions.Execute => "--x", 
			AccessPermissions.ReadWrite => "rw-", 
			AccessPermissions.ReadExecute => "r-x", 
			AccessPermissions.WriteExecute => "-wx", 
			AccessPermissions.ReadWriteExecute => "rwx", 
			_ => throw new ArgumentOutOfRangeException("permissions", permissions, null), 
		};
	}
}
