// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Capture.CivPointCaptureColorResolver
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.Maths;
using System;

#nullable disable
namespace Content.Client._CIV14merka.Capture;

public static class CivPointCaptureColorResolver
{
  public static readonly Color FriendlyColor = Color.FromHex((ReadOnlySpan<char>) "#59d46a", new Color?());
  public static readonly Color EnemyColor = Color.FromHex((ReadOnlySpan<char>) "#d85d5d", new Color?());
  public static readonly Color NeutralColor = Color.FromHex((ReadOnlySpan<char>) "#8b96a3", new Color?());

  public static Color GetRelationColor(int viewerTeamId, int teamId)
  {
    if (teamId <= 0)
      return CivPointCaptureColorResolver.NeutralColor;
    return viewerTeamId == 0 || viewerTeamId != teamId ? CivPointCaptureColorResolver.EnemyColor : CivPointCaptureColorResolver.FriendlyColor;
  }

  public static Color GetCapturePulseColor(
    int viewerTeamId,
    int ownerTeamId,
    int capturingTeamId,
    float timeSeconds)
  {
    if (capturingTeamId <= 0)
      return CivPointCaptureColorResolver.GetRelationColor(viewerTeamId, ownerTeamId);
    Color relationColor = CivPointCaptureColorResolver.GetRelationColor(viewerTeamId, capturingTeamId);
    float capturePulseAmount = CivPointCaptureColorResolver.GetCapturePulseAmount(timeSeconds);
    float num = ownerTeamId == 0 || ownerTeamId == capturingTeamId ? 0.3f : 0.18f;
    Color white = Color.White;
    double amount = (double) num + (double) capturePulseAmount * (1.0 - (double) num);
    return CivPointCaptureColorResolver.Blend(relationColor, white, (float) amount);
  }

  public static float GetCapturePulseAmount(float timeSeconds)
  {
    return MathF.Pow((float) (0.5 + 0.5 * (double) MathF.Sin(timeSeconds * 6f)), 0.85f);
  }

  private static Color Blend(Color from, Color to, float amount)
  {
    amount = Math.Clamp(amount, 0.0f, 1f);
    return new Color(from.R + (to.R - from.R) * amount, from.G + (to.G - from.G) * amount, from.B + (to.B - from.B) * amount, from.A + (to.A - from.A) * amount);
  }
}
