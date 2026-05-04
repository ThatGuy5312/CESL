#version 330 core
#define MAX_LIGHTS 8

in vec2 TexCoord;
in vec3 Normal;
in vec3 FragPos;
in vec4 Tangent;
out vec4 FragColor;

struct Light
{
    vec3 position;
    vec3 color;

    float constant;
    float linear;
    float quadratic;

    float intensity;
};

uniform sampler2D texture0;
uniform sampler2D normalMap;
uniform Light lights[MAX_LIGHTS];
uniform int lightCount;

uniform vec3 viewPos;
uniform vec3 objectColor;

uniform float specularStrength;
uniform float shininess;
uniform float normalStrength;

void main()
{
    vec3 N = normalize(Normal);
    vec3 T = normalize(Tangent.xyz);
    vec3 B = normalize(cross(N, T)) * Tangent.w;

    mat3 TBN = mat3(T, B, N);

    vec3 normalTex = texture(normalMap, TexCoord).rgb;
    normalTex = normalTex * 2.0 - 1.0;

    normalTex = mix(vec3(0.0, 0.0, 1.0), normalTex, normalStrength);

    vec3 norm = normalize(TBN * normalTex);

    vec3 viewDir = normalize(viewPos - FragPos);

    vec3 result = vec3(0.0);

    for (int i = 0; i < lightCount; i++)
    {
        vec3 lightDir = normalize(lights[i].position - FragPos);

        // Diffuse
        float diff = max(dot(norm, lightDir), 0.0);

        // Specular
        vec3 reflectDir = reflect(-lightDir, norm);
        float spec = pow(max(dot(viewDir, reflectDir), 0.0), shininess);

        // Attenuation
        float distance = length(lights[i].position - FragPos);
        float attenuation = 1.0 / (
            lights[i].constant +
            lights[i].linear * distance +
            lights[i].quadratic * distance * distance
        );

        vec3 ambient = 0.1 * lights[i].color * objectColor * lights[i].intensity;
        vec3 diffuse = diff * lights[i].color * objectColor * lights[i].intensity;
        vec3 specular = specularStrength * spec * lights[i].color * lights[i].intensity;

        ambient *= attenuation;
        diffuse *= attenuation;
        specular *= attenuation;

        result += ambient + diffuse + specular;
    }

    if (lightCount == 0)
    {
        result = vec3(objectColor);
    }

    vec4 texColor = texture(texture0, TexCoord);
    FragColor = texColor * vec4(objectColor, 1.0) * vec4(result, 1.0);
}