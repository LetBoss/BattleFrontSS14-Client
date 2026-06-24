// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Teams.CivTeamRunHelper
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Movement.Components;
using System;

#nullable enable
namespace Content.Shared._CIV14merka.Teams;

public static class CivTeamRunHelper
{
  public static bool WantsRun(CivTeamMemberComponent member, InputMoverComponent mover)
  {
    return member.RunHeld || !mover.Sprinting;
  }

  public static bool ShouldRun(
    CivTeamMemberComponent member,
    InputMoverComponent mover,
    CivStaminaComponent stamina)
  {
    float num = Math.Clamp(member.RunStaminaThresholdRatio, 0.0f, 1f);
    bool flag = (double) stamina.MaxStamina > 0.0 && (double) stamina.Stamina > (double) stamina.MaxStamina * (double) num;
    return CivTeamRunHelper.WantsRun(member, mover) && mover.HasDirectionalMovement && (double) member.RunSpeedModifier > (double) member.SprintSpeedModifier && (double) stamina.Stamina > 0.0 && member.RunActive | flag;
  }
}
