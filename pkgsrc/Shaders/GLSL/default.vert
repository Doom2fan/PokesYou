in vec3 aPosition;
in vec3 aNormal;
in vec2 aTexCoords;

out vec3 normal;
out vec3 FragPos;
out vec2 TexCoords;

void main () {
    FragPos = vec3 (ModelMatrix * vec4 (aPosition, 1.0));
    normal = mat3 (NormalModelMatrix) * aNormal;
    TexCoords = aTexCoords;

    gl_Position = ProjectionMatrix * ViewMatrix * vec4 (FragPos, 1.0);
}