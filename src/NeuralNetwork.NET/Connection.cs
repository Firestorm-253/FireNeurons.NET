﻿namespace NeuralNetwork.NET;

public class Connection
{
    public Neuron InputNeuron { get; init; }
    public Neuron OutputNeuron { get; init; }

    public double Weight { get; set; }

    public Connection(Neuron inputNeuron, Neuron outputNeuron)
    {
        this.InputNeuron = inputNeuron;
        this.OutputNeuron = outputNeuron;
    }

    public void Randomize(Activation activation)
    {
        this.Weight = GetRandom(activation, this.OutputNeuron.Connections.Count, this.OutputNeuron.Layer.Neurons.Count);
    }

    public double GetValue()
    {
        return this.InputNeuron.Value * this.Weight;
    }
}