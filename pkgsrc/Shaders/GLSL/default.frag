out vec4 FragColor;

in vec3 normal;
in vec3 FragPos;
in vec2 TexCoords;

vec3 CalcDirLight (DirLight_t light, vec3 normal, vec3 viewDir);
vec3 CalcLight (Light_t light, vec3 normal, vec3 fragPos, vec3 viewDir);

void main () {
    // properties
    vec3 norm = normalize (normal);
    vec3 viewDir = normalize (uCameraPos - FragPos);

    // phase 1: Directional lighting
    vec3 result = CalcDirLight (dirLight, norm, viewDir);

    for (int i = 0; i < MAX_LIGHTS; i++) {
        if (lights [i].lightType == LIGHTTYPE_Point)
            result += CalcLight (lights [i], norm, FragPos, viewDir);
    }

    // phase 3: Spot light
    //result += CalcSpotLight (spotLight, norm, FragPos, viewDir);

    FragColor = vec4 (result, 1.0);
}

vec3 CalcDirLight (DirLight_t light, vec3 normal, vec3 viewDir) {
    vec3 lightDir = normalize (-light.direction);

    // diffuse shading
    float diff = max (dot (normal, lightDir), 0.0);

    // specular shading
    vec3 reflectDir = reflect (-lightDir, normal);
    float spec = pow (max (dot (viewDir, reflectDir), 0.0), matShininess);

    // combine results
    vec3 ambient  = light.ambient  * vec3 (texture (matDiffuse, TexCoords));
    vec3 diffuse  = light.diffuse  * diff * vec3 (texture (matDiffuse, TexCoords));
    vec3 specular = light.specular * spec * vec3 (texture (matSpecular, TexCoords));
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
    vec3 ambient  = light.ambient  * vec3 (texture (matDiffuse, TexCoords));
    vec3 diffuse  = light.diffuse  * diff * vec3 (texture (matDiffuse, TexCoords));
    vec3 specular = light.specular * spec * vec3 (texture (matSpecular, TexCoords));
    ambient  *= attenuation;
    diffuse  *= attenuation;
    specular *= attenuation;
    return (ambient + diffuse + specular);
}