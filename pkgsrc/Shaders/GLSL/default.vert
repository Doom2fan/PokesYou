layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;

out vec3 normal;
out vec3 FragPos;
out vec2 TexCoords;

void main () {
    FragPos = vec3 (ModelMatrix * vec4 (aPosition, 1.0));
    normal = NormalModelMatrix * aNormal;
    TexCoords = aTexCoords;

    gl_Position = ProjectionMatrix * ViewMatrix * vec4 (FragPos, 1.0);
}