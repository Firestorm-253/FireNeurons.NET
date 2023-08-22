using System.Diagnostics.CodeAnalysis;

namespace FireNeurons.NET.Indexes;
using Objects;

public readonly struct ConnectionIndex : IIndex, IEquatable<ConnectionIndex>
{
    public int Index { get; init; }
    public NeuronIndex NeuronIndex { get; init; }

    public ConnectionIndex(int index, NeuronIndex neuronIndex)
    {
        this.Index = index;
        this.NeuronIndex = neuronIndex;
    }

    public override bool Equals([NotNullWhen(true)] object? value) => (value is ConnectionIndex index) && (this.Index.Equals(index.Index) && this.NeuronIndex.Equals(index.NeuronIndex));
    public bool Equals(ConnectionIndex other) => this.Index == other.Index;
    public override int GetHashCode() => HashCode.Combine(this.Index, this.NeuronIndex);

    public static bool operator ==(ConnectionIndex a, ConnectionIndex b) => a.Equals(b);
    public static bool operator !=(ConnectionIndex a, ConnectionIndex b) => !a.Equals(b);

    public static implicit operator ConnectionIndex((int, NeuronIndex) index) => new(index.Item1, index.Item2);
    public static implicit operator ConnectionIndex(Connection connection) => connection.Index;
}
