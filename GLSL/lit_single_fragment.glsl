#version 330 core
#define MAX_LIGHTS 8

in vec3 Normal;
in vec3 FragPos;

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

uniform Light lights[MAX_LIGHTS];
uniform int lightCount;

uniform vec3 viewPos;
uniform vec3 objectColor;

uniform float specularStrength;
uniform float shininess;

void main()
{
    vec3 norm = normalize(Normal);
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

    FragColor = vec4(result, 1.0);
}