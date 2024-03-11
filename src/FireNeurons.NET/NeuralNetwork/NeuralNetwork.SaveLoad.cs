using System.IO.Compression;
using System.Text;
using System.Text.Json;
using PacketManager.dotnet;

namespace FireNeurons.NET;

using Dto;
using Objects;
using Optimisation;

public enum SaveType
{
    Json,
    Binary
}

public partial class NeuralNetwork
{
    public static NeuralNetwork Load(string file)
    {
        return new(file);
    }

    public NeuralNetwork(string file)
        : this(LoadFromFile(file))
    { }
    public NeuralNetwork(NeuralNetworkDto dto)
        : this()
    {
        foreach (var layerDto in dto.Layers)
        {
            var layer = new Layer(layerDto, this);
            this.Layers.Add(layer.Index, layer);
        }
    }

    public NeuralNetwork Clone() // Json is faster but needs more space for saving
    {
        return new(FromJson(ToJson(new(this))));
    }

    public void Save(string file, SaveType saveType)
    {
        var dto = new NeuralNetworkDto(this);

        file = Path.GetFullPath(file);
        if (Path.HasExtension(file))
        {
            file = file.Replace(Path.GetExtension(file), string.Empty);
        }

        switch (saveType)
        {
            case SaveType.Json:
                {
                    File.WriteAllText(file + ".json", ToJson(dto));
                }
                break;
            case SaveType.Binary:
                {
                    File.WriteAllBytes(file + ".nn", ToBytes(dto));
                }
                break;
            default: throw new Exception("ERROR: Invalid SaveType!");
        }
    }

    private static NeuralNetworkDto LoadFromFile(string file)
    {
        if (!File.Exists(file))
        {
            throw new FileNotFoundException();
        }

        NeuralNetworkDto? dto;

        switch (Path.GetExtension(file))
        {
            case ".json":
                {
                    string json = File.ReadAllText(file);
                    dto = FromJson(json);
                }
                break;
            case ".nn":
                {
                    dto = FromBytes(File.ReadAllBytes(file));
                }
                break;
            default: throw new Exception("ERROR: Invalid file-format!");
        }

        if (dto == null)
        {
            throw new Exception("ERROR: Loading failed!");
        }
        return dto;
    }

    public static NeuralNetworkDto FromJson(string json, bool withSpaces = false)
    {
        return JsonSerializer.Deserialize<NeuralNetworkDto>(json, new JsonSerializerOptions() { WriteIndented = withSpaces, })!;
    }
    public static string ToJson(NeuralNetworkDto neuralNetworkDto, bool withSpaces = false)
    {
        return JsonSerializer.Serialize(neuralNetworkDto, new JsonSerializerOptions() { WriteIndented = withSpaces })!;
    }

    public static NeuralNetworkDto FromBytes(byte[] bytes)
    {
        var pr = new PacketReader(bytes);
        return pr.ReadObject<NeuralNetworkDto>();
    }
    public static byte[] ToBytes(NeuralNetworkDto dto)
    {
        var pw = new PacketWriter();
        pw.WriteT(dto);
        return pw.GetBytes();
    }
}
