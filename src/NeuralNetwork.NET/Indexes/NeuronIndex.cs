using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork.NET.Indexes;

public struct NeuronIndex : IEquatable<NeuronIndex>
{
    public int Index { get; init; }
    public LevelIndex LevelIndex { get; init; }

    public NeuronIndex(int index, LevelIndex levelIndex)
    {
        this.Index = index;
        this.LevelIndex = levelIndex;
    }

    public override bool Equals([NotNullWhen(true)] object? value) => (value is NeuronIndex index) && (Index.Equals(index.Index) && LevelIndex.Equals(index.LevelIndex));

    public bool Equals(NeuronIndex other) => Index == other.Index;

    public static implicit operator NeuronIndex((int, LevelIndex) index) => new(index.Item1, index.Item2);
    public static implicit operator NeuronIndex((int, int, LayerIndex) index) => new(index.Item1, (index.Item2, index.Item3));
}
