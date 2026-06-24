using System;

namespace Robust.Shared.Analyzers;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Interface)]
public sealed class AccessAttribute : Attribute
{
	public readonly Type[] Friends;

	public const AccessPermissions SelfDefaultPermissions = AccessPermissions.ReadWriteExecute;

	public const AccessPermissions FriendDefaultPermissions = AccessPermissions.ReadWriteExecute;

	public const AccessPermissions OtherDefaultPermissions = AccessPermissions.Read;

	public AccessPermissions Self { get; set; } = AccessPermissions.ReadWriteExecute;

	public AccessPermissions Friend { get; set; } = AccessPermissions.ReadWriteExecute;

	public AccessPermissions Other { get; set; } = AccessPermissions.Read;

	public AccessAttribute(params Type[] friends)
	{
		Friends = friends;
	}
}
