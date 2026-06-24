// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Projectiles.DamageFalloffThreshold
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable disable
namespace Content.Shared._RMC14.Projectiles;

[DataRecord]
[NetSerializable]
[Serializable]
public record struct DamageFalloffThreshold(float Range, FixedPoint2 Falloff, bool IgnoreModifiers);
