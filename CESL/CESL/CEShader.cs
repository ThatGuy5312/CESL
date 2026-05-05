using CESL.Data;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace CESL;

public class CEShader
{
    public int Handle { get; private set; }

    public VertexShader VertexShader { get; private set; }
    public FragmentShader FragmentShader { get; private set; }

    public CEShader() { }

    public CEShader(string ceslVertexSource, string ceslFragmentSource)
    {
        var vertexData = CESL.ParseVertexShader(ceslVertexSource);
        var fragmentData = CESL.ParseFragmentShader(ceslFragmentSource);

        VertexShader = vertexData;
        FragmentShader = fragmentData;

        //Console.WriteLine("Parsed vertex shader:\n" + vertexData.GLSL_Vertex_Data);
        //Console.WriteLine("Parsed fragment shader:\n" + fragmentData.GLSL_Fragment_Data);

        int vertexShader = CompileShader(ShaderType.VertexShader, vertexData.GLSL);
        int fragmentShader = CompileShader(ShaderType.FragmentShader, fragmentData.GLSL);

        BuildShader(vertexShader, fragmentShader);
    }

    public CEShader(VertexShader vertexData, FragmentShader fragmentData)
    {
        int vertexShader = CompileShader(ShaderType.VertexShader, vertexData.GLSL);
        int fragmentShader = CompileShader(ShaderType.FragmentShader, fragmentData.GLSL);

        BuildShader(vertexShader, fragmentShader);
    }

    public CEShader(string combinedSource)
    {
        string vertexSource = string.Empty;
        string fragmentSource = string.Empty;

        ParseCombinedSource(combinedSource, out vertexSource, out fragmentSource);

        int vertex = CompileShader(ShaderType.VertexShader, vertexSource);
        int fragment = CompileShader(ShaderType.FragmentShader, fragmentSource);

        BuildShader(vertex, fragment);
    }

    int CompileShader(ShaderType type, string source)
    {
        int shader = GL.CreateShader(type);
        GL.ShaderSource(shader, source);
        GL.CompileShader(shader);
        GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
        if (success == 0)
            throw new Exception($"{type} compilation failed: {GL.GetShaderInfoLog(shader)}");
        return shader;
    }

    void ParseCombinedSource(string source, out string vertexSrc, out string fragmentSrc)
    {
        var lines = source.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
        var currentType = string.Empty;

        var vertex = new System.Text.StringBuilder();
        var fragment = new System.Text.StringBuilder();

        foreach (string line in lines)
        {
            if (line.StartsWith("#shader"))
            {
                if (line.Contains("vertex")) currentType = "vertex";
                else if (line.Contains("fragment")) currentType = "fragment";
            }
            else
            {
                if (currentType == "vertex") vertex.AppendLine(line);
                else if (currentType == "fragment") fragment.AppendLine(line);
            }
        }

        vertexSrc = vertex.ToString();
        fragmentSrc = fragment.ToString();
    }

    void BuildShader(int vertexShader, int fragmentShader)
    {
        Handle = GL.CreateProgram();

        GL.AttachShader(Handle, vertexShader);
        GL.AttachShader(Handle, fragmentShader);
        GL.LinkProgram(Handle);

        GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int success);
        if (success == 0)
            throw new Exception($"Shader link failed: {GL.GetProgramInfoLog(Handle)}");

        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
    }

    public void Rebuild()
    {
        GL.DeleteProgram(Handle);

        int vertexShader = CompileShader(ShaderType.VertexShader, VertexShader.GLSL);
        int fragmentShader = CompileShader(ShaderType.FragmentShader, FragmentShader.GLSL);

        BuildShader(vertexShader, fragmentShader);
    }

    public void Use() => GL.UseProgram(Handle);

    public void SetMatrix4(string name, Matrix4 value) =>
        GL.UniformMatrix4(GL.GetUniformLocation(Handle, name), false, ref value);

    public void SetVector3(string name, Vector3 value)
        => GL.Uniform3(GL.GetUniformLocation(Handle, name), value);

    public void SetVector2(string name, Vector2 value)
        => GL.Uniform2(GL.GetUniformLocation(Handle, name), value);

    public void SetVector4(string name, Vector4 value)
        => GL.Uniform4(GL.GetUniformLocation(Handle, name), value);

    public void SetInt(string name, int value)
        => GL.Uniform1(GL.GetUniformLocation(Handle, name), value);

    public void SetFloat(string name, float value)
        => GL.Uniform1(GL.GetUniformLocation(Handle, name), value);

    public void SetBool(string name, bool value)
        => GL.Uniform1(GL.GetUniformLocation(Handle, name), value ? 1 : 0);
}
