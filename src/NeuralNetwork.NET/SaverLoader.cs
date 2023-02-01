using System.Text.Json;

using NeuralNetwork.NET.Dto;

namespace NeuralNetwork.NET;

public partial class NeuralNetwork
{
    public void Save(string file)
    {
        var dto = new NeuralNetworkDto(this);
        string json = JsonSerializer.Serialize(dto, new JsonSerializerOptions() { WriteIndented = false, });
        File.WriteAllText(file, json);
    }


    public static NeuralNetwork Load(string file)
    {
        var dto = LoadFromJsonFile(file);

        return null;
    }

    private static NeuralNetworkDto LoadFromJsonFile(string file)
    {
        string json = File.ReadAllText(file);
        var model = JsonSerializer.Deserialize<NeuralNetworkDto>(json, new JsonSerializerOptions() { WriteIndented = false, });

        if (model == null || json == "{}")
        {
            throw new Exception("ERROR: Loading failed!");
        }
        return model;
    }
}
