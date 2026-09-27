namespace UABEANext4.Controls.MeshPreviewer;

public static class MeshPreviewerShaders
{
    public const string VERTEX_SOURCE = "#version 300 es\nlayout(location=0) in vec3 aP;\nlayout(location=1) in vec3 aN;\nlayout(location=2) in vec2 aU;\nuniform mat4 uMVP, uModel;\nout vec3 vN, vP;\nout vec2 vU;\nvoid main() {\n    vP = vec3(uModel * vec4(aP, 1.0));\n    vN = normalize(mat3(uModel) * aN);\n    vU = aU;\n    gl_Position = uMVP * vec4(aP, 1.0);\n}";

    public const string FRAGMENT_SOURCE = @"#version 300 es
precision highp float;
in vec3 vN, vP;
in vec2 vU;
uniform vec3 uLightDir, uCamPos;
uniform sampler2D uMainTex;
uniform int uHasTexture;
out vec4 FragColor;
void main() {
    vec3 n = normalize(vN);
    if (!gl_FrontFacing) n = -n;
    vec3 lightDir = normalize(-uLightDir);
    vec3 viewDir = normalize(uCamPos - vP);
    float diff = max(dot(n, lightDir), 0.0);
    float rim = pow(1.0 - max(dot(viewDir, n), 0.0), 3.0);
    
    vec3 baseColor = vec3(0.7, 0.73, 0.8);
    if (uHasTexture == 1) {
        vec4 tex = texture(uMainTex, vU);
        // Simple alpha cutout
        if (tex.a < 0.1) discard;
        baseColor = tex.rgb;
    }

    vec3 col = baseColor * (0.3 + diff * 0.7) + vec3(0.4, 0.5, 0.7) * rim * 0.4;
    FragColor = vec4(col, 1.0);
}";

    public const string GRID_VERTEX = "#version 300 es\nlayout(location=0) in vec3 aP;\nuniform mat4 uMVP;\nout vec3 vP;\nvoid main() {\n    vP = aP;\n    gl_Position = uMVP * vec4(aP, 1.0);\n}";

    public const string GRID_FRAGMENT = "#version 300 es\nprecision mediump float;\nin vec3 vP;\nout vec4 FragColor;\nvoid main() {\n    float d = length(vP);\n    vec3 c = vec3(0.5);\n    if (abs(vP.x) < 0.01 && abs(vP.z) < 5.1) c = vec3(1.0, 0.0, 0.0);\n    if (abs(vP.z) < 0.01 && abs(vP.x) < 5.1) c = vec3(0.0, 0.0, 1.0);\n    float alpha = 1.0 - smoothstep(10.0, 30.0, d);\n    FragColor = vec4(c, 0.5 * alpha);\n}";
}
