using System.Numerics;
using Content.Shared._CIV14merka.Commander;

namespace Content.Client._CIV14merka.HeliSupply;

public readonly record struct HeliMapBlip(Vector2 Position, Vector2 Heading, CivAirstrikeSide Side);
