// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.RMCBattleExecuteSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Chat;
using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Medical.Unrevivable;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Administration.Logs;
using Content.Shared.Camera;
using Content.Shared.Chat;
using Content.Shared.CombatMode;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Ranged;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Wieldable.Components;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged;

public sealed class RMCBattleExecuteSystem : EntitySystem
{
  [Dependency]
  private ISharedAdminLogManager _admin;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private SharedCMChatSystem _rmcChat;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private ISharedPlayerManager _player;
  [Dependency]
  private SharedCameraRecoilSystem _cameraRecoil;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SkillsSystem _skills;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private RMCUnrevivableSystem _unrevivable;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<MarineComponent, GetVerbsEvent<AlternativeVerb>>(new EntityEventRefHandler<MarineComponent, GetVerbsEvent<AlternativeVerb>>(this.AlternativeInteract));
    this.SubscribeLocalEvent<MarineComponent, RMCBattleExecuteEvent>(new EntityEventRefHandler<MarineComponent, RMCBattleExecuteEvent>(this.ExecuteDoAfter));
    this.SubscribeLocalEvent<RMCBattleExecutedComponent, ExaminedEvent>(new EntityEventRefHandler<RMCBattleExecutedComponent, ExaminedEvent>(this.ExamineBody));
    this.SubscribeLocalEvent<RMCBattleExecuteComponent, ExaminedEvent>(new EntityEventRefHandler<RMCBattleExecuteComponent, ExaminedEvent>(this.OnGunExecuteExamined));
  }

  private void OnGunExecuteExamined(Entity<RMCBattleExecuteComponent> ent, ref ExaminedEvent args)
  {
    using (args.PushGroup("RMCBattleExecuteComponent"))
      args.PushMarkup(this.Loc.GetString("rmc-examine-text-execute"));
  }

  private void AlternativeInteract(
    Entity<MarineComponent> ent,
    ref GetVerbsEvent<AlternativeVerb> args)
  {
    CombatModeComponent comp;
    RMCBattleExecuteComponent executionComponent;
    EntityUid? handHeldItem;
    if (!(args.User != args.Target) || !this._hands.TryGetActiveItem((Entity<HandsComponent>) args.User, out handHeldItem) || !this.TryComp<CombatModeComponent>(args.User, out comp) || !comp.IsInCombatMode || !this.TryComp<RMCBattleExecuteComponent>(handHeldItem, out executionComponent) || !this._skills.HasSkill((Entity<SkillsComponent>) args.User, executionComponent.Skill, 1))
      return;
    EntityUid target = args.Target;
    EntityUid user = args.User;
    SortedSet<AlternativeVerb> verbs = args.Verbs;
    AlternativeVerb alternativeVerb = new AlternativeVerb();
    alternativeVerb.Text = this.Loc.GetString("rmc-execution");
    alternativeVerb.Act = (Action) (() => this.Execute(user, target, executionComponent, handHeldItem.Value));
    alternativeVerb.Priority = 100;
    verbs.Add(alternativeVerb);
  }

  private void Execute(
    EntityUid user,
    EntityUid target,
    RMCBattleExecuteComponent executionComponent,
    EntityUid handHeldItem)
  {
    if (this._mobState.IsDead(target) && this._unrevivable.IsUnrevivable(target))
    {
      this._popup.PopupClient($"You decide to not Execute {this.Name(target)}, as they are already far beyond revival.", new EntityUid?(user), PopupType.MediumCaution);
    }
    else
    {
      RMCBattleExecuteEvent @event = new RMCBattleExecuteEvent(this.GetNetEntity(user), this.GetNetEntity(target), executionComponent.Damage);
      this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, executionComponent.BattleExecuteTimeSeconds, (DoAfterEvent) @event, new EntityUid?(target), new EntityUid?(target), new EntityUid?(handHeldItem)));
      this._popup.PopupPredicted(this.Loc.GetString("rmc-execute-start-self", (nameof (target), (object) this.Name(target)), ("gun", (object) this.Name(handHeldItem))), this.Loc.GetString("rmc-execute-start-others", (nameof (user), (object) this.Name(user)), (nameof (target), (object) this.Name(target)), ("gun", (object) this.Name(handHeldItem))), user, new EntityUid?(user), PopupType.LargeCaution);
    }
  }

  private void ExecuteDoAfter(Entity<MarineComponent> ent, ref RMCBattleExecuteEvent args)
  {
    EntityUid entity1 = this.GetEntity(args.User);
    EntityUid entity2 = this.GetEntity(args.Target);
    if (args.Cancelled)
    {
      ISharedAdminLogManager admin = this._admin;
      LogStringHandler logStringHandler = new LogStringHandler(31 /*0x1F*/, 2);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) entity1), "ToPrettyString(user)");
      logStringHandler.AppendLiteral("'s Execution of ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) entity2), "ToPrettyString(target)");
      logStringHandler.AppendLiteral(" was cancelled.");
      ref LogStringHandler local = ref logStringHandler;
      admin.Add(LogType.RMCExecution, LogImpact.High, ref local);
      this._popup.PopupClient($"You decide to not Execute {this.Name(entity2)}.", new EntityUid?(entity1), PopupType.MediumCaution);
    }
    else
    {
      if (args.Handled)
        return;
      args.Handled = true;
      GunComponent comp1;
      if (!this.Exists(args.Used) || !this.TryComp<GunComponent>(args.Used, out comp1))
        return;
      TakeAmmoEvent args1 = new TakeAmmoEvent(1, new List<(EntityUid?, IShootable)>(), this.Transform(entity1).Coordinates, new EntityUid?(entity1));
      EntityUid? used = args.Used;
      this.RaiseLocalEvent<TakeAmmoEvent>(used.Value, args1);
      if (args1.Ammo.Count == 0)
      {
        ISharedAdminLogManager admin = this._admin;
        LogStringHandler logStringHandler = new LogStringHandler(49, 2);
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) entity1), "ToPrettyString(user)");
        logStringHandler.AppendLiteral("'s Execution of ");
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) entity2), "ToPrettyString(target)");
        logStringHandler.AppendLiteral(" was cancelled from lack of ammo.");
        ref LogStringHandler local = ref logStringHandler;
        admin.Add(LogType.RMCExecution, LogImpact.High, ref local);
        SharedAudioSystem audio = this._audio;
        SoundSpecifier soundEmpty = comp1.SoundEmpty;
        used = args.Used;
        EntityUid source = used.Value;
        EntityUid? user = new EntityUid?(entity1);
        AudioParams? audioParams = new AudioParams?();
        audio.PlayPredicted(soundEmpty, source, user, audioParams);
      }
      else
      {
        foreach ((EntityUid? Entity, IShootable Shootable) tuple in args1.Ammo)
          this.Del(tuple.Entity);
        ISharedAdminLogManager admin = this._admin;
        LogStringHandler logStringHandler = new LogStringHandler(27, 2);
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) entity1), "ToPrettyString(user)");
        logStringHandler.AppendLiteral("'s Execution of ");
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) entity2), "ToPrettyString(target)");
        logStringHandler.AppendLiteral(" Succeeded!");
        ref LogStringHandler local = ref logStringHandler;
        admin.Add(LogType.RMCExecution, LogImpact.High, ref local);
        EntityUid? nullable = args.Used;
        WieldableComponent comp2;
        if (this.TryComp<WieldableComponent>(nullable.Value, out comp2) && !comp2.Wielded)
        {
          float recoilScalarModified = comp1.CameraRecoilScalarModified;
          Vector2 worldPosition = this._transform.GetWorldPosition(entity1);
          Vector2 vector2 = this._transform.GetWorldPosition(entity2) - worldPosition;
          if (vector2 == Vector2.Zero)
            vector2 = new Vector2(0.0f, -1f);
          Vector2 kickback = Vector2Helpers.Normalized(vector2) * recoilScalarModified;
          this._cameraRecoil.KickCamera(entity1, kickback);
        }
        DamageableSystem damageable = this._damageable;
        EntityUid? uid = new EntityUid?(entity2);
        DamageSpecifier battleExecuteDamage = args.BattleExecuteDamage;
        nullable = new EntityUid?();
        EntityUid? origin1 = nullable;
        nullable = new EntityUid?();
        EntityUid? tool = nullable;
        damageable.TryChangeDamage(uid, battleExecuteDamage, true, origin: origin1, tool: tool);
        MobStateSystem mobState = this._mobState;
        EntityUid entity3 = entity2;
        nullable = new EntityUid?();
        EntityUid? origin2 = nullable;
        mobState.ChangeMobState(entity3, MobState.Dead, origin: origin2);
        this._unrevivable.MakeUnrevivable((Entity<RMCRevivableComponent>) entity2);
        SharedAudioSystem audio = this._audio;
        SoundSpecifier soundGunshotModified = comp1.SoundGunshotModified;
        nullable = args.Used;
        EntityUid source = nullable.Value;
        EntityUid? user = new EntityUid?(entity1);
        AudioParams? audioParams = new AudioParams?();
        audio.PlayPredicted(soundGunshotModified, source, user, audioParams);
        this._popup.PopupPredicted($"{this.Name(entity2)} WAS EXECUTED BY {this.Name(entity1)}!", entity2, new EntityUid?(entity1), PopupType.LargeCaution);
        string str = $"[bold][font size=24][color=red]\n{this.Name(entity2)} WAS EXECUTED BY {this.Name(entity1)}!\n[/color][/font][/bold]";
        MapCoordinates mapCoordinates = this._transform.GetMapCoordinates(entity2);
        Filter filter = Filter.Empty().AddInRange(mapCoordinates, 12f, this._player, (IEntityManager) this.EntityManager);
        filter.RemoveWhereAttachedEntity(new Predicate<EntityUid>(((EntitySystem) this).HasComp<XenoComponent>));
        this._rmcChat.ChatMessageToMany(str, str, filter, ChatChannel.Local);
        this.EnsureComp<RMCBattleExecutedComponent>(entity2);
      }
    }
  }

  private void ExamineBody(Entity<RMCBattleExecutedComponent> ent, ref ExaminedEvent args)
  {
    args.PushMarkup(this.Loc.GetString((string) ent.Comp.ExecutedText, ("victim", (object) ent.Owner)));
  }
}
