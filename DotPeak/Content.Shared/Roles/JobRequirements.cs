// Decompiled with JetBrains decompiler
// Type: Content.Shared.Roles.JobRequirements
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Preferences;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Shared.Roles;

public static class JobRequirements
{
  public static bool TryRequirementsMet(
    JobPrototype job,
    IReadOnlyDictionary<string, TimeSpan> playTimes,
    [NotNullWhen(false)] out FormattedMessage? reason,
    IEntityManager entManager,
    IPrototypeManager protoManager,
    HumanoidCharacterProfile? profile)
  {
    HashSet<JobRequirement> jobRequirement1 = entManager.System<SharedRoleSystem>().GetJobRequirement(job);
    reason = (FormattedMessage) null;
    if (jobRequirement1 == null)
      return true;
    foreach (JobRequirement jobRequirement2 in jobRequirement1)
    {
      if (!jobRequirement2.Check(entManager, protoManager, profile, playTimes, out reason))
        return false;
    }
    return true;
  }
}
