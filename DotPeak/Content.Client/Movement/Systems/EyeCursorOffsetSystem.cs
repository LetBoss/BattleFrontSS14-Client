// Decompiled with JetBrains decompiler
// Type: Content.Client.Movement.Systems.EyeCursorOffsetSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Movement.Components;
using Content.Shared._RMC14.Scoping;
using Content.Shared.Camera;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.Movement.Systems;

public sealed class EyeCursorOffsetSystem : EntitySystem
{
  [Dependency]
  private IEyeManager _eyeManager;
  [Dependency]
  private IInputManager _inputManager;
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private IClyde _clyde;
  private static float _edgeOffset = 0.9f;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<EyeCursorOffsetComponent, GetEyeOffsetEvent>(new ComponentEventRefHandler<EyeCursorOffsetComponent, GetEyeOffsetEvent>((object) this, __methodptr(OnGetEyeOffsetEvent)), (Type[]) null, (Type[]) null);
  }

  private void OnGetEyeOffsetEvent(
    EntityUid uid,
    EyeCursorOffsetComponent component,
    ref GetEyeOffsetEvent args)
  {
    if (this.HasComp<ScopingComponent>(uid))
      return;
    Vector2? nullable = this.OffsetAfterMouse(uid, component);
    if (!nullable.HasValue)
      return;
    args.Offset += nullable.Value;
  }

  public Vector2? OffsetAfterMouse(EntityUid uid, EyeCursorOffsetComponent? component)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    ScreenCoordinates mouseScreenPosition = this._inputManager.MouseScreenPosition;
    Vector2i size = this._clyde.MainWindow.Size;
    float num = MathF.Min((float) (size.X / 2), (float) (size.Y / 2)) * EyeCursorOffsetSystem._edgeOffset;
    Vector2 vector2_1 = new Vector2((float) -((double) ((ScreenCoordinates) ref mouseScreenPosition).X - (double) (size.X / 2)) / num, (((ScreenCoordinates) ref mouseScreenPosition).Y - (float) (size.Y / 2)) / num);
    if (!localEntity.HasValue)
      return new Vector2?();
    this._transform.GetWorldPosition(localEntity.Value);
    if (component == null)
      component = this.EnsureComp<EyeCursorOffsetComponent>(uid);
    if (WindowId.op_Inequality(mouseScreenPosition.Window, WindowId.Invalid))
    {
      Angle rotation = this._eyeManager.CurrentEye.Rotation;
      Vector2 vector2_2 = Vector2.Transform(vector2_1, Quaternion.CreateFromAxisAngle(-Vector3.UnitZ, (float) ((Angle) ref rotation).Opposite().Theta)) * component.MaxOffset;
      if ((double) vector2_2.Length() > (double) component.MaxOffset)
        vector2_2 = Vector2Helpers.Normalized(vector2_2) * component.MaxOffset;
      component.TargetPosition = vector2_2;
      if (component.CurrentPosition != component.TargetPosition)
      {
        Vector2 vector2_3 = component.TargetPosition - component.CurrentPosition;
        if ((double) vector2_3.Length() > (double) component.OffsetSpeed)
          vector2_3 = Vector2Helpers.Normalized(vector2_3) * component.OffsetSpeed;
        component.CurrentPosition += vector2_3;
      }
    }
    return new Vector2?(component.CurrentPosition);
  }
}
