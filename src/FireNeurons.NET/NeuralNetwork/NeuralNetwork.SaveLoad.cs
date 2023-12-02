using System.IO.Compression;
using System.Text;
using System.Text.Json;

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

    public NeuralNetwork Clone()
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
                    var zipped = Zip(ToJson(dto));
                    File.WriteAllBytes(file + ".nn", zipped);
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
                    var zipped = File.ReadAllBytes(file);
                    string json = UnZip(zipped);
                    dto = JsonSerializer.Deserialize<NeuralNetworkDto>(json, new JsonSerializerOptions() { WriteIndented = false, });
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

    public static NeuralNetworkDto FromJson(string json)
    {
        return JsonSerializer.Deserialize<NeuralNetworkDto>(json, new JsonSerializerOptions() { WriteIndented = true, })!;
    }
    public static string ToJson(NeuralNetworkDto neuralNetworkDto)
    {
        return JsonSerializer.Serialize(neuralNetworkDto, new JsonSerializerOptions() { WriteIndented = true })!;
    }

    private static byte[] Zip(string str)
    {
        var bytes = Encoding.UTF8.GetBytes(str);

        using var memoryStream = new MemoryStream();
        using (var gzipStream = new GZipStream(memoryStream, CompressionLevel.Optimal))
        {
            gzipStream.Write(bytes, 0, bytes.Length);
        }
        return memoryStream.ToArray();
    }

    private static string UnZip(byte[] bytes)
    {
        using var memoryStream = new MemoryStream(bytes);
        using var outputStream = new MemoryStream();
        using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
        {
            gzipStream.CopyTo(outputStream);
        }
        var output = outputStream.ToArray();
        return Encoding.UTF8.GetString(output, 0, output.Length);
    }
}
