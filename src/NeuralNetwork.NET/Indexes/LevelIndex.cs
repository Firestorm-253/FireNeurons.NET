using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork.NET.Indexes;

public struct LevelIndex : IEquatable<LevelIndex>
{
    public int Index { get; init; }
    public LayerIndex LayerIndex { get; init; }

    public LevelIndex(int index, LayerIndex layerIndex)
    {
        this.Index = index;
        this.LayerIndex = layerIndex;
    }

    public override bool Equals([NotNullWhen(true)] object? value) => (value is LevelIndex index) && (Index.Equals(index.Index) && LayerIndex.Equals(index.LayerIndex));

    public bool Equals(LevelIndex other) => Index == other.Index;

    public static implicit operator LevelIndex((int, LayerIndex) index) => new(index.Item1, index.Item2);
}
