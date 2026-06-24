// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Intel.Tech.TechAnnounceEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Audio;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared._RMC14.Intel.Tech;

[DataRecord]
[NetSerializable]
[Serializable]
public sealed record TechAnnounceEvent(string Author, string Message, SoundSpecifier? Sound);
