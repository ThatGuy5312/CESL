CESL shader language project that I will be using in my game engine project [Chondria](https://github.com/ThatGuy5312/Chondria)

Functionality:

Adds csharp-like functions like having public and private fields along with field attributes.

–

The public and private member values will be stored as a bool in a ShaderField as IsPrivate.

This can be used in many ways like how I use it with having a private value not display on the
UI as a material property.

The attributes can also be used in many ways along with it being expandable.
It is stored as two strings being Attribute and AttributeName inside of ShaderField.
Attribute holds the full attribute call and arguments, ex [Range(0, 10, .1f)]. And AttributeName just hold the call name, ex Range.

You can expand this within your project like if you want a vec3 to show as a color you can do [Color] and check its AttributeName and show as a color value instead of a vec3 value.

–

For the actual C# functionality you can use the CESL class for most functions.

CESL Functions:
ParseVertexShader(string source) - Returns a VertexShader holding the parsed GLSL code.

ParseFragmentShader(string source) - Returns a FragmentShader holding the parsed GLSL code, a list of ShaderFields, and functions for finding a specific ShaderField by name and a static helper for setting a shader uniform.

ParseAttribute(string attr) - turns an attribute, ex [Range(0, 10, 1)] and returns a tuple with a string and string list holding the name of the attribute (Range), and the arguments (0, 10, 1).

CreateInstance(string shaderType) - Creates an instance of a uniform value type, 
ex vec3 -> new Vector3(0).

ToCESLVertexShader(string glsl_source, params string[] private_fields) -  Turns a GLSL vertex shader (will honestly work with a fragment shader too) and returns the parsed CESL code. Adding private fields indicates what fields will be private when parsed.





The ShaderField:
A shader field holds many values mainly consisting of just strings.
Fields:
Name - the name of the field
Type - a string name of the field type
Attribute - the full attribute call, ex [Range(0, 10, .1f)]
AttributeName - just the attribute name, ex Range
IsPrivate - a true or false value which holds if the field was defined as private in the shader

It also contains a single function:
GetAttribute() - Returns a new instance of the current attribute (wont be used in a lot of cases) but for something like Range it will return a RangeValue.


Range / RangeValue
It gets called as Range in the shader but is really RangeValue in C#.
It holds three floats and a bool:
Min - Minimum value of field
Max - Maximum value of field
Step - The increment value of the field
IsInt - Returns true if all the floats are whole numbers

This is mainly used for UI purposes.
