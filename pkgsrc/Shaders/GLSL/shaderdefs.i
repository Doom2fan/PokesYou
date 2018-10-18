// This file contains common data definitions for both vertex and fragment shader
precision highp int;
precision highp float;

// Defines
#define MAX_LIGHTS 32

// Constants
const int LIGHTTYPE_Point = 1;
const int LIGHTTYPE_Directional = 2;

// Structs
struct Light_t {
    int lightType;
    vec3 position, direction;

    float constant, linear, quadratic;

    vec3 ambient, diffuse, specular;
};

// Uniforms
uniform vec3 uCameraPos; // Camera data
uniform vec3 uCameraDir;

uniform Light_t lights [MAX_LIGHTS]; // Lights data

uniform sampler2D matDiffuse1; // Material data
uniform sampler2D matDiffuse2;
uniform sampler2D matDiffuse3;
uniform sampler2D matSpecular1;
uniform sampler2D matSpecular2;
uniform sampler2D matSpecular3;
uniform float     matShininess;

// Matrices
uniform mat4 ProjectionMatrix;
uniform mat4 ViewMatrix;
uniform mat4 ModelMatrix;
uniform mat3 NormalModelMatrix;