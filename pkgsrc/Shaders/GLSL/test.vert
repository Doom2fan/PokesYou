in vec3 aPosition;
in vec3 aNormal;
in vec2 aTexCoords;

out vec3 FragPos;
out vec2 TexCoords;

void main () {
    FragPos = vec3 (ModelMatrix * vec4 (aPosition, 1.));
    gl_Position = ProjectionMatrix * ViewMatrix * ModelMatrix * vec4 (aPosition, 1.0);
    TexCoords = aTexCoords;
}