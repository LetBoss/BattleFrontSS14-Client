// Decompiled with JetBrains decompiler
// Type: Content.Shared.Configurable.SharedConfigurationSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Interaction;
using Content.Shared.Tools;
using Content.Shared.Tools.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Configurable;

public abstract class SharedConfigurationSystem : EntitySystem
{
  [Dependency]
  private SharedUserInterfaceSystem _uiSystem;
  [Dependency]
  private SharedToolSystem _toolSystem;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ConfigurationComponent, ConfigurationComponent.ConfigurationUpdatedMessage>(new ComponentEventHandler<ConfigurationComponent, ConfigurationComponent.ConfigurationUpdatedMessage>((object) this, __methodptr(OnUpdate)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ConfigurationComponent, InteractUsingEvent>(new ComponentEventHandler<ConfigurationComponent, InteractUsingEvent>((object) this, __methodptr(OnInteractUsing)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ConfigurationComponent, ContainerIsInsertingAttemptEvent>(new ComponentEventHandler<ConfigurationComponent, ContainerIsInsertingAttemptEvent>((object) this, __methodptr(OnInsert)), (Type[]) null, (Type[]) null);
  }

  private void OnInteractUsing(
    EntityUid uid,
    ConfigurationComponent component,
    InteractUsingEvent args)
  {
    if (args.Handled || !this._toolSystem.HasQuality(args.Used, ProtoId<ToolQualityPrototype>.op_Implicit(component.QualityNeeded)))
      return;
    args.Handled = this._uiSystem.TryOpenUi(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum) ConfigurationComponent.ConfigurationUiKey.Key, args.User, false);
  }

  private void OnUpdate(
    EntityUid uid,
    ConfigurationComponent component,
    ConfigurationComponent.ConfigurationUpdatedMessage args)
  {
    foreach (string key in component.Config.Keys)
    {
      string valueOrDefault = args.Config.GetValueOrDefault<string, string>(key);
      if (!string.IsNullOrWhiteSpace(valueOrDefault) && (component.Validation == null || component.Validation.IsMatch(valueOrDefault)))
        component.Config[key] = valueOrDefault;
    }
    this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
    ConfigurationUpdatedEvent configurationUpdatedEvent = new ConfigurationUpdatedEvent(component);
    this.RaiseLocalEvent<ConfigurationUpdatedEvent>(uid, configurationUpdatedEvent, false);
  }

  private void OnInsert(
    EntityUid uid,
    ConfigurationComponent component,
    ContainerIsInsertingAttemptEvent args)
  {
    if (!this._toolSystem.HasQuality(((ContainerAttemptEventBase) args).EntityUid, ProtoId<ToolQualityPrototype>.op_Implicit(component.QualityNeeded)))
      return;
    ((CancellableEntityEventArgs) args).Cancel();
  }
}
