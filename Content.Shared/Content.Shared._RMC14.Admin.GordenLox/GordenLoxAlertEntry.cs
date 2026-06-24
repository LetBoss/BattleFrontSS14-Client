using System;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Admin.GordenLox;

[Serializable]
[NetSerializable]
public readonly record struct GordenLoxAlertEntry(int Id, DateTime CreatedAt, string Message);
