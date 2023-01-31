using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork.NET.Indexes;

public struct LayerIndex : IEquatable<LayerIndex>
{
    public int Index { get; init; }

    public LayerIndex(int index)
    {
        this.Index = index;
    }

    public override bool Equals([NotNullWhen(true)] object? value) => (value is LayerIndex index) && (Index.Equals(index.Index) && Index.Equals(index.Index));

    public bool Equals(LayerIndex other) => Index == other.Index;

    public static implicit operator LayerIndex(int index) => new(index);
}
