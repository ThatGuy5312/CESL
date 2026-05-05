using CESL.Data;
using OpenTK.Mathematics;
using System.Text.RegularExpressions;

namespace CESL;

public static class CESL
{
    /// <summary>
    /// Parses a CESL vertex shader source code and converts it to GLSL format, returning a VertexShader struct containing the GLSL code.
    /// </summary>
    /// <param name="source">CESL vertex shader</param>
    /// <returns></returns>
    public static VertexShader ParseVertexShader(string source)
    {
        var vertex_data = new VertexShader();

        var pattern = @"\b(public|private)\b";

        var replaced = Regex.Replace(source, pattern, "uniform");

        vertex_data.GLSL = replaced;

        return vertex_data;
    }

    /// <summary>
    /// Parses a CESL fragment shader source code, extracts uniform fields and their attributes, 
    /// and converts it to GLSL format. Returns a FragmentShader struct containing the GLSL code and a list of 
    /// ShaderField structs representing the uniform fields and their attributes.
    /// </summary>
    /// <param name="source">CESL fragment shader</param>
    /// <returns></returns>
    public static FragmentShader ParseFragmentShader(string source)
    {
        var lines = source.Split('\n');

        var frag_data = new FragmentShader();
        frag_data.Fields = [];

        List<string> output = [];
        List<string> pendingAttributes = [];

        foreach (var rawLine in lines)
        {
            var line = rawLine.Trim();

            var field = new ShaderField();

            // checking for a attribute
            if (line.StartsWith('['))
            {
                pendingAttributes.Add(line);
                continue;
            }

            field.IsPrivate = line.Contains("private");

            // convert the words public or private to uniform
            line = Regex.Replace(line, @"\b(public|private)\b", "uniform");

            // checking for a uniform
            if (line.Contains("uniform"))
            {
                // process attributes before clearing
                if (pendingAttributes.Count > 0)
                {
                    foreach (var attr in pendingAttributes)
                    {
                        // glsl attribute indication
                        output.Add("// " + attr);
                        field.Attribute = attr;
                    }

                    pendingAttributes.Clear();
                }

                output.Add(line);

                var split = Regex.Split(line.Trim(), @"\s+|;");

                field.Type = split[1];

                //if (!field.noShow)
                //field.value = CreateInstance(split[1]);

                field.Name = split[2];

                frag_data.Fields.Add(field);
                continue;
            }

            output.Add(line);
        }

        frag_data.GLSL = string.Join("\n", output);

        return frag_data;
    }

    /// <summary>
    /// Parses a CESL attribute string (e.g. "[Range(0, 1, 0.1)]") and 
    /// extracts the attribute name and its arguments, 
    /// returning them as a tuple.
    /// </summary>
    /// <param name="attr">Attribute name and arguments</param>
    /// <returns></returns>
    public static (string name, List<string> args) ParseAttribute(string attr)
    {
        // remove [ and ]
        attr = attr.Trim('[', ']');

        var nameEnd = attr.IndexOf('(');

        if (nameEnd == -1)
        {
            return (attr.Trim(), new List<string>());
        }

        var name = attr[..nameEnd].Trim();

        var argsRaw = attr.Substring(nameEnd + 1, attr.LastIndexOf(')') - nameEnd - 1);

        var args = argsRaw
            .Split(',')
            .Select(a => a.Trim())
            .Where(a => a.Length > 0)
            .ToList();

        return (name, args);
    }

    /// <summary>
    /// Creates a default instance of a shader field based on its type.
    /// </summary>
    /// <param name="shaderType">shader type</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static object CreateInstance(string shaderType) => shaderType switch
    {
        "float" => 0f,
        "int" => 0,
        "bool" => false,
        "string" => string.Empty,
        "vec3" => new Vector3(0),
        _ => throw new ArgumentException($"Unsupported shader type: {shaderType}")
    };

    /// <summary>
    /// Converts a GLSL vertex shader to CESL format with private_fields to specify what uniforms should be private.
    /// </summary>
    /// <param name="glsl_source">glsl vertex source</param>
    /// <param name="private_fields">uniforms that should be private</param>
    /// <returns></returns>
    public static string ToCESLVertexShader(string glsl_source, params string[] private_fields)
    {
        var lines = glsl_source.Split('\n');
        List<string> output = [];

        foreach (var rawLine in lines)
        {
            var line = rawLine.Trim();
            if (line.StartsWith("uniform"))
            {
                var split = Regex.Split(line.Trim(), @"\s+|;");
                var type = split[1];
                var name = split[2];
                var accessModifier = private_fields.Contains(name) ? "private" : "public";
                output.Add($"{accessModifier} {type} {name};");
            }
            else 
                output.Add(line);
        }

        return string.Join("\n", output);
    }
}
