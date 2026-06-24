// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.InterfaceData
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;

#nullable enable
namespace Robust.Shared.GameObjects;

[DataDefinition]
[NetSerializable]
[Serializable]
public sealed class InterfaceData : ISerializationGenerated<InterfaceData>, ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float InteractionRange = 2f;
  [DataField(null, false, 1, false, false, null)]
  public bool RequireInputValidation = true;

  [DataField("type", false, 1, true, false, null)]
  public string ClientType { get; private set; }

  public InterfaceData(string clientType, float interactionRange = 2f, bool requireInputValidation = true)
  {
    this.ClientType = clientType;
    this.InteractionRange = interactionRange;
    this.RequireInputValidation = requireInputValidation;
  }

  public InterfaceData(InterfaceData data)
  {
    this.ClientType = data.ClientType;
    this.InteractionRange = data.InteractionRange;
    this.RequireInputValidation = data.RequireInputValidation;
  }

  public InterfaceData()
  {
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref InterfaceData target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<InterfaceData>(this, ref target, hookCtx, false, context))
      return;
    string target1 = (string) null;
    if (this.ClientType == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ClientType, ref target1, hookCtx, false, context))
      target1 = this.ClientType;
    target.ClientType = target1;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.InteractionRange, ref target2, hookCtx, false, context))
      target2 = this.InteractionRange;
    target.InteractionRange = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.RequireInputValidation, ref target3, hookCtx, false, context))
      target3 = this.RequireInputValidation;
    target.RequireInputValidation = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref InterfaceData target,
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
    InterfaceData target1 = (InterfaceData) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public InterfaceData Instantiate() => new InterfaceData();
}
