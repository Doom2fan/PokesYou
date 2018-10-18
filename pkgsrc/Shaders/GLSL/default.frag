out vec4 FragColor;

in vec3 normal;
in vec3 FragPos;
in vec2 TexCoords;

vec3 CalcDirLight (Light_t light, vec3 normal, vec3 viewDir);
vec3 CalcLight (Light_t light, vec3 normal, vec3 fragPos, vec3 viewDir);

void main () {
    // properties
    vec3 norm = normalize (normal);
    vec3 viewDir = normalize (uCameraPos - FragPos);

    // phase 1: Directional lighting
    vec3 result = vec3 (0.0, 0.0, 0.0);

    for (int i = 0; i < MAX_LIGHTS; i++) {
        switch (lights [i].lightType) {
        case LIGHTTYPE_Point:
            result += CalcLight (lights [i], norm, FragPos, viewDir);
            break;
        case LIGHTTYPE_Directional:
            result += CalcDirLight (lights [i], norm, viewDir);
            break;
        }
    }

    // phase 3: Spot light
    //result += CalcSpotLight (spotLight, norm, FragPos, viewDir);

    FragColor = vec4 (result, texture (matDiffuse1, TexCoords).a);
}

vec3 CalcDirLight (Light_t light, vec3 normal, vec3 viewDir) {
    vec4 matDiff1 = texture (matDiffuse1, TexCoords);
    vec4 matSpec1 = texture (matSpecular1, TexCoords);
    vec3 lightDir = normalize (-light.direction);

    // diffuse shading
    float diff = max (dot (normal, lightDir), 0.0);

    // specular shading
    vec3 reflectDir = reflect (-lightDir, normal);
    float spec = pow (max (dot (viewDir, reflectDir), 0.0), matShininess);

    // combine results
    vec3 ambient  = light.ambient  * matDiff1.xyz;
    vec3 diffuse  = light.diffuse  * diff * matDiff1.xyz;
    vec3 specular = light.specular * spec * matSpec1.xyz;
    return (ambient + diffuse + specular);
}

vec3 CalcLight (Light_t light, vec3 normal, vec3 fragPos, vec3 viewDir) {
    vec3 lightDir = normalize (light.position - fragPos);

    // Diffuse shading
    float diff = max (dot (normal, lightDir), 0.0);

    // Specular shading
    vec3 reflectDir = reflect (-lightDir, normal);
    float spec = pow (max (dot (viewDir, reflectDir), 0.0), matShininess);

    // Attenuation
    float distance = length (light.position - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));

    // Combine results
    vec3 ambient  = light.ambient  * vec3 (texture (matDiffuse1, TexCoords));
    vec3 diffuse  = light.diffuse  * diff * vec3 (texture (matDiffuse1, TexCoords));
    vec3 specular = light.specular * spec * vec3 (texture (matSpecular1, TexCoords));
    ambient  *= attenuation;
    diffuse  *= attenuation;
    specular *= attenuation;
    return (ambient + diffuse + specular);
}