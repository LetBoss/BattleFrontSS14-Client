// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Analyzers.AccessAttribute
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable enable
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

  public AccessAttribute(params Type[] friends) => this.Friends = friends;
}
