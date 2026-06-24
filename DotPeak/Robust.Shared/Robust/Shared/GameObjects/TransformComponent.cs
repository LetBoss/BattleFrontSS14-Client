// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.TransformComponent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Animations;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.GameObjects;

[RegisterComponent]
[NetworkedComponent]
public sealed class TransformComponent : 
  Component,
  IComponentDebug,
  IComponent,
  ISerializationGenerated<IComponent>,
  ISerializationGenerated,
  ISerializationGenerated<IComponentDebug>,
  ISerializationGenerated<TransformComponent>
{
  [Robust.Shared.IoC.Dependency]
  private readonly IEntityManager _entMan;
  [DataField("parent", false, 1, false, false, null)]
  internal EntityUid _parent;
  [DataField("pos", false, 1, false, false, null)]
  internal Vector2 _localPosition = Vector2.Zero;
  [DataField("rot", false, 1, false, false, null)]
  internal Angle _localRotation;
  [DataField("noRot", false, 1, false, false, null)]
  internal bool _noLocalRotation;
  [DataField("anchored", false, 1, false, false, null)]
  internal bool _anchored;
  [DataField(null, false, 1, false, false, null)]
  public bool GridTraversal = true;
  [Robust.Shared.ViewVariables.ViewVariables]
  internal BroadphaseData? Broadphase;
  internal bool MatricesDirty = true;
  private Matrix3x2 _localMatrix = Matrix3x2.Identity;
  private Matrix3x2 _invLocalMatrix = Matrix3x2.Identity;
  [Robust.Shared.ViewVariables.ViewVariables]
  public bool ActivelyLerping;
  [Robust.Shared.ViewVariables.ViewVariables]
  public GameTick LastLerp = GameTick.Zero;
  [Robust.Shared.ViewVariables.ViewVariables]
  internal readonly HashSet<EntityUid> _children = new HashSet<EntityUid>();
  [Robust.Shared.IoC.Dependency]
  private readonly IMapManager _mapManager;
  internal bool _mapIdInitialized;
  internal bool _gridInitialized;
  [Access(new Type[] {typeof (SharedTransformSystem)})]
  internal EntityUid? _gridUid;
  [Robust.Shared.ViewVariables.ViewVariables]
  public EntityUid LerpParent;
  public bool PredictedLerp;

  [Robust.Shared.ViewVariables.ViewVariables]
  private NetEntity NetParent => this._entMan.GetNetEntity(this._parent);

  public Matrix3x2 LocalMatrix
  {
    get
    {
      if (this.MatricesDirty)
        this.RebuildMatrices();
      return this._localMatrix;
    }
  }

  public Matrix3x2 InvLocalMatrix
  {
    get
    {
      if (this.MatricesDirty)
        this.RebuildMatrices();
      return this._invLocalMatrix;
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  public Vector2? NextPosition { get; internal set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public Angle? NextRotation { get; internal set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public Vector2 PrevPosition { get; internal set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public Angle PrevRotation { get; internal set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public MapId MapID { get; internal set; }

  public EntityUid? MapUid { get; internal set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public EntityUid? GridUid => this._gridUid;

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public bool NoLocalRotation
  {
    get => this._noLocalRotation;
    set
    {
      if (value)
        this.LocalRotation = Angle.Zero;
      this._noLocalRotation = value;
      this._entMan.Dirty(this.Owner, (IComponent) this);
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [Animatable]
  public Angle LocalRotation
  {
    get => this._localRotation;
    set
    {
      if (this._noLocalRotation || ((Angle) ref this._localRotation).EqualsApprox(value))
        return;
      Angle localRotation = this._localRotation;
      this._localRotation = value;
      MetaDataComponent component = this._entMan.GetComponent<MetaDataComponent>(this.Owner);
      this._entMan.Dirty(this.Owner, (IComponent) this, component);
      this.MatricesDirty = true;
      if (!this.Initialized)
        return;
      this._entMan.System<SharedTransformSystem>().RaiseMoveEvent((Entity<TransformComponent, MetaDataComponent>) (this.Owner, this, component), this._parent, this._localPosition, localRotation, this.MapUid, false);
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [Obsolete("Use the system method instead")]
  public Angle WorldRotation
  {
    get
    {
      EntityUid uid = this._parent;
      EntityQuery<TransformComponent> entityQuery = this._entMan.GetEntityQuery<TransformComponent>();
      Angle worldRotation = this._localRotation;
      TransformComponent component;
      for (; uid.IsValid(); uid = component.ParentUid)
      {
        component = entityQuery.GetComponent(uid);
        worldRotation = Angle.op_Addition(worldRotation, component._localRotation);
      }
      return worldRotation;
    }
    set
    {
      if (this.NoLocalRotation)
        return;
      Angle worldRotation = this.WorldRotation;
      this.LocalRotation = Angle.op_Addition(this.LocalRotation, Angle.op_Subtraction(value, worldRotation));
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  private TransformComponent? _parentXform
  {
    get
    {
      return this._parent.IsValid() ? this._entMan.GetComponent<TransformComponent>(this._parent) : (TransformComponent) null;
    }
  }

  public EntityUid ParentUid => this._parent;

  [Obsolete("Use the system method instead")]
  public Matrix3x2 WorldMatrix
  {
    get
    {
      EntityQuery<TransformComponent> entityQuery = this._entMan.GetEntityQuery<TransformComponent>();
      EntityUid uid = this._parent;
      Matrix3x2 worldMatrix = this.LocalMatrix;
      while (uid.IsValid())
      {
        TransformComponent component = entityQuery.GetComponent(uid);
        Matrix3x2 localMatrix = component.LocalMatrix;
        uid = component.ParentUid;
        worldMatrix = Matrix3x2.Multiply(worldMatrix, localMatrix);
      }
      return worldMatrix;
    }
  }

  [Obsolete("Use the system method instead")]
  public Matrix3x2 InvWorldMatrix
  {
    get
    {
      EntityQuery<TransformComponent> entityQuery = this._entMan.GetEntityQuery<TransformComponent>();
      EntityUid uid = this._parent;
      Matrix3x2 invWorldMatrix = this.InvLocalMatrix;
      while (uid.IsValid())
      {
        TransformComponent component = entityQuery.GetComponent(uid);
        Matrix3x2 invLocalMatrix = component.InvLocalMatrix;
        uid = component.ParentUid;
        invWorldMatrix = Matrix3x2.Multiply(invLocalMatrix, invWorldMatrix);
      }
      return invWorldMatrix;
    }
  }

  [Animatable]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [Obsolete("Use the system method instead")]
  public Vector2 WorldPosition
  {
    get
    {
      return this._parent.IsValid() ? Vector2.Transform(this._localPosition, this._entMan.GetComponent<TransformComponent>(this.ParentUid).WorldMatrix) : Vector2.Zero;
    }
    set
    {
      if (!this._parent.IsValid())
        return;
      this.LocalPosition = Vector2.Transform(value, this._entMan.GetComponent<TransformComponent>(this.ParentUid).InvWorldMatrix);
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public EntityCoordinates Coordinates
  {
    get
    {
      bool flag = this._parent.IsValid();
      return new EntityCoordinates(flag ? this._parent : this.Owner, flag ? this.LocalPosition : Vector2.Zero);
    }
    [Obsolete("Use the system's setter method instead.")] set
    {
      this._entMan.EntitySysManager.GetEntitySystem<SharedTransformSystem>().SetCoordinates(this.Owner, this, value);
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [Obsolete("Use TransformSystem.GetMapCoordinates")]
  public MapCoordinates MapPosition => new MapCoordinates(this.WorldPosition, this.MapID);

  [Animatable]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public Vector2 LocalPosition
  {
    get => this._localPosition;
    [Obsolete("Use the system method instead")] set
    {
      if (this.Anchored || Vector2Helpers.EqualsApprox(this._localPosition, value))
        return;
      EntityUid parent = this._parent;
      Vector2 localPosition = this._localPosition;
      this._localPosition = value;
      MetaDataComponent component = this._entMan.GetComponent<MetaDataComponent>(this.Owner);
      this._entMan.Dirty(this.Owner, (IComponent) this, component);
      this.MatricesDirty = true;
      if (!this.Initialized)
        return;
      this._entMan.System<SharedTransformSystem>().RaiseMoveEvent((Entity<TransformComponent, MetaDataComponent>) (this.Owner, this, component), parent, localPosition, this._localRotation, this.MapUid);
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public bool Anchored
  {
    get => this._anchored;
    [Obsolete("Use the SharedTransformSystem.AnchorEntity/Unanchor methods instead.")] set
    {
      if (!this.Initialized)
      {
        this._anchored = value;
      }
      else
      {
        MapGridComponent grid;
        if (value && !this._anchored && this._mapManager.TryFindGridAt(this.MapPosition, out EntityUid _, out grid))
        {
          this._anchored = this._entMan.EntitySysManager.GetEntitySystem<SharedTransformSystem>().AnchorEntity(this.Owner, this, grid);
        }
        else
        {
          if (value || !this._anchored)
            return;
          this._entMan.EntitySysManager.GetEntitySystem<SharedTransformSystem>().Unanchor(this.Owner, this);
        }
      }
    }
  }

  public TransformChildrenEnumerator ChildEnumerator
  {
    get => new TransformChildrenEnumerator(this._children.GetEnumerator());
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  public int ChildCount => this._children.Count;

  [Obsolete("Use the system's method instead.")]
  public void AttachToGridOrMap()
  {
    this._entMan.EntitySysManager.GetEntitySystem<SharedTransformSystem>().AttachToGridOrMap(this.Owner, this);
  }

  [Obsolete("Use TransformSystem.SetParent() instead")]
  public void AttachParent(EntityUid parent)
  {
    this._entMan.EntitySysManager.GetEntitySystem<SharedTransformSystem>().SetParent(this.Owner, this, parent, this._entMan.GetEntityQuery<TransformComponent>());
  }

  [Obsolete("Use the system method instead")]
  public (Vector2 WorldPosition, Angle WorldRotation) GetWorldPositionRotation()
  {
    (Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 WorldMatrix) positionRotationMatrix = this.GetWorldPositionRotationMatrix();
    return (positionRotationMatrix.WorldPosition, positionRotationMatrix.WorldRotation);
  }

  [Obsolete("Use the system method instead")]
  public (Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 WorldMatrix) GetWorldPositionRotationMatrix(
    EntityQuery<TransformComponent> xforms)
  {
    EntityUid uid = this._parent;
    Angle angle = this._localRotation;
    Matrix3x2 matrix3x2 = this.LocalMatrix;
    TransformComponent component;
    for (; uid.IsValid(); uid = component.ParentUid)
    {
      component = xforms.GetComponent(uid);
      angle = Angle.op_Addition(angle, component.LocalRotation);
      Matrix3x2 localMatrix = component.LocalMatrix;
      matrix3x2 = Matrix3x2.Multiply(matrix3x2, localMatrix);
    }
    return (matrix3x2.Translation, angle, matrix3x2);
  }

  [Obsolete("Use the system method instead")]
  public (Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 WorldMatrix) GetWorldPositionRotationMatrix()
  {
    return this.GetWorldPositionRotationMatrix(this._entMan.GetEntityQuery<TransformComponent>());
  }

  [Obsolete("Use the system method instead")]
  public (Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 InvWorldMatrix) GetWorldPositionRotationInvMatrix(
    EntityQuery<TransformComponent> xformQuery)
  {
    (Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 WorldMatrix, Matrix3x2 InvWorldMatrix) rotationMatrixWithInv = this.GetWorldPositionRotationMatrixWithInv(xformQuery);
    return (rotationMatrixWithInv.WorldPosition, rotationMatrixWithInv.WorldRotation, rotationMatrixWithInv.InvWorldMatrix);
  }

  [Obsolete("Use the system method instead")]
  public (Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 WorldMatrix, Matrix3x2 InvWorldMatrix) GetWorldPositionRotationMatrixWithInv()
  {
    return this.GetWorldPositionRotationMatrixWithInv(this._entMan.GetEntityQuery<TransformComponent>());
  }

  [Obsolete("Use the system method instead")]
  public (Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 WorldMatrix, Matrix3x2 InvWorldMatrix) GetWorldPositionRotationMatrixWithInv(
    EntityQuery<TransformComponent> xformQuery)
  {
    EntityUid uid = this._parent;
    Angle angle = this._localRotation;
    Matrix3x2 matrix3x2_1 = this.InvLocalMatrix;
    Matrix3x2 matrix3x2_2 = this.LocalMatrix;
    TransformComponent component;
    for (; uid.IsValid(); uid = component.ParentUid)
    {
      component = xformQuery.GetComponent(uid);
      angle = Angle.op_Addition(angle, component.LocalRotation);
      Matrix3x2 localMatrix = component.LocalMatrix;
      matrix3x2_2 = Matrix3x2.Multiply(matrix3x2_2, localMatrix);
      matrix3x2_1 = Matrix3x2.Multiply(component.InvLocalMatrix, matrix3x2_1);
    }
    return (matrix3x2_2.Translation, angle, matrix3x2_2, matrix3x2_1);
  }

  public void RebuildMatrices()
  {
    this.MatricesDirty = false;
    if (!this._parent.IsValid())
    {
      this._localMatrix = Matrix3x2.Identity;
      this._invLocalMatrix = Matrix3x2.Identity;
    }
    this._localMatrix = Matrix3Helpers.CreateTransform(ref this._localPosition, ref this._localRotation);
    this._invLocalMatrix = Matrix3Helpers.CreateInverseTransform(ref this._localPosition, ref this._localRotation);
  }

  public string GetDebugString()
  {
    return $"pos/rot/wpos/wrot: {this.Coordinates}/{this.LocalRotation}/{this.WorldPosition}/{this.WorldRotation}";
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref TransformComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (TransformComponent) target1;
    if (serialization.TryCustomCopy<TransformComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid target2 = new EntityUid();
    if (!serialization.TryCustomCopy<EntityUid>(this._parent, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid>(this._parent, hookCtx, context);
    target._parent = target2;
    Vector2 target3 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this._localPosition, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<Vector2>(this._localPosition, hookCtx, context);
    target._localPosition = target3;
    Angle target4 = new Angle();
    if (!serialization.TryCustomCopy<Angle>(this._localRotation, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<Angle>(this._localRotation, hookCtx, context);
    target._localRotation = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this._noLocalRotation, ref target5, hookCtx, false, context))
      target5 = this._noLocalRotation;
    target._noLocalRotation = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this._anchored, ref target6, hookCtx, false, context))
      target6 = this._anchored;
    target._anchored = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.GridTraversal, ref target7, hookCtx, false, context))
      target7 = this.GridTraversal;
    target.GridTraversal = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref TransformComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    TransformComponent target1 = (TransformComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    TransformComponent target1 = (TransformComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IComponentDebug target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    TransformComponent target1 = (TransformComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponentDebug) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref IComponentDebug target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    TransformComponent target1 = (TransformComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual TransformComponent Component.Instantiate() => new TransformComponent();

  IComponentDebug IComponentDebug.Instantiate() => (IComponentDebug) this.Instantiate();

  IComponentDebug ISerializationGenerated<IComponentDebug>.Instantiate()
  {
    return (IComponentDebug) this.Instantiate();
  }
}
