// Decompiled with JetBrains decompiler
// Type: Content.Shared.Arcade.SharedSpaceVillainArcadeComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Arcade;

public abstract class SharedSpaceVillainArcadeComponent : 
  Component,
  ISerializationGenerated<SharedSpaceVillainArcadeComponent>,
  ISerializationGenerated
{
  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref SharedSpaceVillainArcadeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (SharedSpaceVillainArcadeComponent) component;
    serialization.TryCustomCopy<SharedSpaceVillainArcadeComponent>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref SharedSpaceVillainArcadeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SharedSpaceVillainArcadeComponent target1 = (SharedSpaceVillainArcadeComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SharedSpaceVillainArcadeComponent target1 = (SharedSpaceVillainArcadeComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    SharedSpaceVillainArcadeComponent target1 = (SharedSpaceVillainArcadeComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual SharedSpaceVillainArcadeComponent Component.Instantiate()
  {
    throw new NotImplementedException();
  }

  [NetSerializable]
  [Serializable]
  public enum Indicators
  {
    HealthManager,
    HealthLimiter,
  }

  [NetSerializable]
  [Serializable]
  public enum PlayerAction
  {
    Attack,
    Heal,
    Recharge,
    NewGame,
    RequestData,
  }

  [NetSerializable]
  [Serializable]
  public enum SpaceVillainArcadeVisualState
  {
    Normal,
    Off,
    Broken,
    Win,
    GameOver,
  }

  [NetSerializable]
  [Serializable]
  public enum SpaceVillainArcadeUiKey
  {
    Key,
  }

  [NetSerializable]
  [Serializable]
  public sealed class SpaceVillainArcadePlayerActionMessage : BoundUserInterfaceMessage
  {
    public readonly SharedSpaceVillainArcadeComponent.PlayerAction PlayerAction;

    public SpaceVillainArcadePlayerActionMessage(
      SharedSpaceVillainArcadeComponent.PlayerAction playerAction)
    {
      this.PlayerAction = playerAction;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class SpaceVillainArcadeMetaDataUpdateMessage : 
    SharedSpaceVillainArcadeComponent.SpaceVillainArcadeDataUpdateMessage
  {
    public readonly string GameTitle;
    public readonly string EnemyName;
    public readonly bool ButtonsDisabled;

    public SpaceVillainArcadeMetaDataUpdateMessage(
      int playerHp,
      int playerMp,
      int enemyHp,
      int enemyMp,
      string playerActionMessage,
      string enemyActionMessage,
      string gameTitle,
      string enemyName,
      bool buttonsDisabled)
      : base(playerHp, playerMp, enemyHp, enemyMp, playerActionMessage, enemyActionMessage)
    {
      this.GameTitle = gameTitle;
      this.EnemyName = enemyName;
      this.ButtonsDisabled = buttonsDisabled;
    }
  }

  [NetSerializable]
  [Virtual]
  [Serializable]
  public class SpaceVillainArcadeDataUpdateMessage : BoundUserInterfaceMessage
  {
    public readonly int PlayerHP;
    public readonly int PlayerMP;
    public readonly int EnemyHP;
    public readonly int EnemyMP;
    public readonly string PlayerActionMessage;
    public readonly string EnemyActionMessage;

    public SpaceVillainArcadeDataUpdateMessage(
      int playerHp,
      int playerMp,
      int enemyHp,
      int enemyMp,
      string playerActionMessage,
      string enemyActionMessage)
    {
      this.PlayerHP = playerHp;
      this.PlayerMP = playerMp;
      this.EnemyHP = enemyHp;
      this.EnemyMP = enemyMp;
      this.EnemyActionMessage = enemyActionMessage;
      this.PlayerActionMessage = playerActionMessage;
    }
  }
}
