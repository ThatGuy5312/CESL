string fragmentShaderSource = @"private float testPrivateFloat;

[Range(0, 1, 1)]
public int testPublicInt;

public void main()
{
    // main body
}";

var frag = CESL.CESL.ParseFragmentShader(fragmentShaderSource);

Console.WriteLine($"GLSL:\n{frag.GLSL}\n");

// dont close immediately
Console.WriteLine("Press any key to exit...");
Console.ReadKey();
