// Decompiled with JetBrains decompiler
// Type: Content.Shared.Humanoid.HumanoidProfileExport
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Preferences;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;

#nullable enable
namespace Content.Shared.Humanoid;

[DataDefinition]
public sealed class HumanoidProfileExport : 
  ISerializationGenerated<HumanoidProfileExport>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public string ForkId;
  [DataField(null, false, 1, false, false, null)]
  public int Version = 1;
  [DataField(null, false, 1, true, false, null)]
  public HumanoidCharacterProfile Profile;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref HumanoidProfileExport target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<HumanoidProfileExport>(this, ref target, hookCtx, false, context))
      return;
    string target1 = (string) null;
    if (this.ForkId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ForkId, ref target1, hookCtx, false, context))
      target1 = this.ForkId;
    target.ForkId = target1;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Version, ref target2, hookCtx, false, context))
      target2 = this.Version;
    target.Version = target2;
    HumanoidCharacterProfile target3 = (HumanoidCharacterProfile) null;
    if (this.Profile == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HumanoidCharacterProfile>(this.Profile, ref target3, hookCtx, false, context))
    {
      if (this.Profile == null)
        target3 = (HumanoidCharacterProfile) null;
      else
        serialization.CopyTo<HumanoidCharacterProfile>(this.Profile, ref target3, hookCtx, context, true);
    }
    target.Profile = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref HumanoidProfileExport target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    HumanoidProfileExport target1 = (HumanoidProfileExport) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public HumanoidProfileExport Instantiate() => new HumanoidProfileExport();
}
