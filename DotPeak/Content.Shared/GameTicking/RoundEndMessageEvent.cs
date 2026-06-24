// Decompiled with JetBrains decompiler
// Type: Content.Shared.GameTicking.RoundEndMessageEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;

#nullable enable
namespace Content.Shared.GameTicking;

[NetSerializable]
[DataDefinition]
[Serializable]
public sealed class RoundEndMessageEvent : 
  EntityEventArgs,
  ISerializationGenerated<RoundEndMessageEvent>,
  ISerializationGenerated
{
  public ResolvedSoundSpecifier? RestartSound;

  public string GamemodeTitle { get; }

  public string RoundEndText { get; }

  public TimeSpan RoundDuration { get; }

  public int RoundId { get; }

  public int PlayerCount { get; }

  public RoundEndMessageEvent.RoundEndPlayerInfo[] AllPlayersEndInfo { get; }

  public RoundEndMessageEvent(
    string gamemodeTitle,
    string roundEndText,
    TimeSpan roundDuration,
    int roundId,
    int playerCount,
    RoundEndMessageEvent.RoundEndPlayerInfo[] allPlayersEndInfo,
    ResolvedSoundSpecifier? restartSound)
  {
    this.GamemodeTitle = gamemodeTitle;
    this.RoundEndText = roundEndText;
    this.RoundDuration = roundDuration;
    this.RoundId = roundId;
    this.PlayerCount = playerCount;
    this.AllPlayersEndInfo = allPlayersEndInfo;
    this.RestartSound = restartSound;
  }

  public RoundEndMessageEvent()
  {
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RoundEndMessageEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    serialization.TryCustomCopy<RoundEndMessageEvent>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RoundEndMessageEvent target,
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
    RoundEndMessageEvent target1 = (RoundEndMessageEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public RoundEndMessageEvent Instantiate() => new RoundEndMessageEvent();

  [NetSerializable]
  [DataDefinition]
  [Serializable]
  public struct RoundEndPlayerInfo : 
    ISerializationGenerated<RoundEndMessageEvent.RoundEndPlayerInfo>,
    ISerializationGenerated
  {
    [DataField(null, false, 1, false, false, null)]
    public string PlayerOOCName;
    [DataField(null, false, 1, false, false, null)]
    public string? PlayerICName;
    [DataField(null, false, 1, false, false, null)]
    [NonSerialized]
    public NetUserId? PlayerGuid;
    public string Role;
    [DataField(null, false, 1, false, false, null)]
    [NonSerialized]
    public string[] JobPrototypes;
    [DataField(null, false, 1, false, false, null)]
    [NonSerialized]
    public string[] AntagPrototypes;
    public NetEntity? PlayerNetEntity;
    [DataField(null, false, 1, false, false, null)]
    public bool Antag;
    [DataField(null, false, 1, false, false, null)]
    public bool Observer;
    public bool Connected;

    public RoundEndPlayerInfo()
    {
      this.PlayerOOCName = (string) null;
      this.PlayerICName = (string) null;
      this.PlayerGuid = new NetUserId?();
      this.Role = (string) null;
      this.JobPrototypes = (string[]) null;
      this.AntagPrototypes = (string[]) null;
      this.PlayerNetEntity = new NetEntity?();
      this.Antag = false;
      this.Observer = false;
      this.Connected = false;
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void InternalCopy(
      ref RoundEndMessageEvent.RoundEndPlayerInfo target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      if (serialization.TryCustomCopy<RoundEndMessageEvent.RoundEndPlayerInfo>(this, ref target, hookCtx, false, context))
        return;
      string target1 = (string) null;
      if (this.PlayerOOCName == null)
        throw new NullNotAllowedException();
      if (!serialization.TryCustomCopy<string>(this.PlayerOOCName, ref target1, hookCtx, false, context))
        target1 = this.PlayerOOCName;
      string target2 = (string) null;
      if (!serialization.TryCustomCopy<string>(this.PlayerICName, ref target2, hookCtx, false, context))
        target2 = this.PlayerICName;
      NetUserId? target3 = new NetUserId?();
      if (!serialization.TryCustomCopy<NetUserId?>(this.PlayerGuid, ref target3, hookCtx, false, context))
        target3 = serialization.CreateCopy<NetUserId?>(this.PlayerGuid, hookCtx, context);
      string[] target4 = (string[]) null;
      if (this.JobPrototypes == null)
        throw new NullNotAllowedException();
      if (!serialization.TryCustomCopy<string[]>(this.JobPrototypes, ref target4, hookCtx, true, context))
        target4 = serialization.CreateCopy<string[]>(this.JobPrototypes, hookCtx, context);
      string[] target5 = (string[]) null;
      if (this.AntagPrototypes == null)
        throw new NullNotAllowedException();
      if (!serialization.TryCustomCopy<string[]>(this.AntagPrototypes, ref target5, hookCtx, true, context))
        target5 = serialization.CreateCopy<string[]>(this.AntagPrototypes, hookCtx, context);
      bool target6 = false;
      if (!serialization.TryCustomCopy<bool>(this.Antag, ref target6, hookCtx, false, context))
        target6 = this.Antag;
      bool target7 = false;
      if (!serialization.TryCustomCopy<bool>(this.Observer, ref target7, hookCtx, false, context))
        target7 = this.Observer;
      target = target with
      {
        PlayerOOCName = target1,
        PlayerICName = target2,
        PlayerGuid = target3,
        JobPrototypes = target4,
        AntagPrototypes = target5,
        Antag = target6,
        Observer = target7
      };
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void Copy(
      ref RoundEndMessageEvent.RoundEndPlayerInfo target,
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
      RoundEndMessageEvent.RoundEndPlayerInfo target1 = (RoundEndMessageEvent.RoundEndPlayerInfo) target;
      this.Copy(ref target1, serialization, hookCtx, context);
      target = (object) target1;
    }

    [Obsolete("Use ISerializationManager.CreateCopy instead")]
    public RoundEndMessageEvent.RoundEndPlayerInfo Instantiate()
    {
      return new RoundEndMessageEvent.RoundEndPlayerInfo();
    }
  }
}
