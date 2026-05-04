#version 330 core

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoord;
layout (location = 3) in vec4 aTangent;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

out vec2 TexCoord;
out vec3 Normal;
out vec3 FragPos;
out vec4 Tangent;

void main()
{
    FragPos = vec3(model * vec4(aPosition, 1.0));

    mat3 normalMatrix = mat3(transpose(inverse(model)));

    Normal = normalize(normalMatrix * aNormal);

    vec3 T = normalize(normalMatrix * aTangent.xyz);
    Tangent = vec4(T, aTangent.w);

    gl_Position = projection * view * vec4(FragPos, 1.0);
    TexCoord = aTexCoord;
}