using Avalonia;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;
using Avalonia.Rendering;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using Avalonia.Media;
using UABEANext4.Logic.Mesh;

namespace UABEANext4.Controls.MeshPreviewer;

public class MeshPreviewerControl : OpenGlControlBase
{
    private GL? _gl;
    private bool _dirtyModel = false;

    private uint _shaderProgram;
    private uint _gridShaderProgram;

    public string GlVendor { get; private set; } = "Unknown";
    public string GlRenderer { get; private set; } = "Unknown";
    public string GlVersionFull { get; private set; } = "Unknown";

    // Cache GPU resources for meshes and textures
    private Dictionary<MeshObj, MeshEntry> _meshCache = new();
    private Dictionary<TextureObj, uint> _textureCache = new();
    
    private class MeshEntry
    {
        public uint vao, vbo, ibo, indexCount;
        public Vector3 min, max;
    }

    private uint _gridVao, _gridVbo;
    private int _gridVertexCount;

    public int ViewportWidth { get; private set; }
    public int ViewportHeight { get; private set; }
    public bool IsControlFocused => IsFocused;
    public string PressedKeysString => string.Join(", ", _pressedKeys);
    public string LastShaderError { get; private set; } = "None";

    private Vector3 _cameraPos = new(10f, 5f, 10f);
    private Vector2 _cameraRot = new(MathF.PI / 4, -MathF.PI / 6);
    private float _cameraZoom = 15f;
    private Vector3 _lookAt = new(0, 0, 0);
    private Vector2 _lastPos = new(-1f, -1f);
    private bool _isRightMouseDown = false;
    private HashSet<Key> _pressedKeys = new();
    private Stopwatch _frameTimer = Stopwatch.StartNew();
    private bool _isLeftMouseDown = false;
    private bool _isMiddleMouseDown = false;

    public MeshPreviewerControl()
    {
        Focusable = true;
        ActiveSceneProperty.Changed.Subscribe(_ => { _dirtyModel = true; RequestNextFrameRendering(); });
        
        PointerWheelChanged += (s, e) => {
            Vector3 forward = Vector3.Normalize(_lookAt - _cameraPos);
            float speed = _cameraZoom * 0.1f;
            _cameraPos += forward * (float)e.Delta.Y * speed;
            RecalculateCamera();
            e.Handled = true;
        };
        
        AddHandler(PointerPressedEvent, (s, e) => {
            var p = e.GetCurrentPoint(this);
            _lastPos = new((float)p.Position.X, (float)p.Position.Y);
            if (p.Properties.IsRightButtonPressed) _isRightMouseDown = true;
            if (p.Properties.IsLeftButtonPressed) _isLeftMouseDown = true;
            if (p.Properties.IsMiddleButtonPressed) _isMiddleMouseDown = true;
            e.Pointer.Capture(this);
            e.Handled = true;
            Focus();
        }, RoutingStrategies.Tunnel | RoutingStrategies.Bubble);

        AddHandler(PointerReleasedEvent, (s, e) => {
            _lastPos = new(-1f, -1f);
            var p = e.GetCurrentPoint(this);
            if (!p.Properties.IsRightButtonPressed) _isRightMouseDown = false;
            if (!p.Properties.IsLeftButtonPressed) _isLeftMouseDown = false;
            if (!p.Properties.IsMiddleButtonPressed) _isMiddleMouseDown = false;
            e.Pointer.Capture(null);
            e.Handled = true;
        }, RoutingStrategies.Tunnel | RoutingStrategies.Bubble);

        AddHandler(PointerMovedEvent, (s, e) => {
            var p = e.GetCurrentPoint(this);
            var cur = new Vector2((float)p.Position.X, (float)p.Position.Y);

            if (_lastPos.X == -1f) return;
            var delta = cur - _lastPos;
            _lastPos = cur;

            if (_isRightMouseDown) {
                _cameraRot.X -= delta.X * 0.003f;
                _cameraRot.Y = Math.Clamp(_cameraRot.Y - delta.Y * 0.003f, -1.5f, 1.5f);
                RecalculateCamera();
                e.Handled = true;
            } else if (_isLeftMouseDown || _isMiddleMouseDown) {
                Vector3 forward = Vector3.Normalize(_lookAt - _cameraPos);
                Vector3 right = Vector3.Normalize(Vector3.Cross(forward, Vector3.UnitY));
                Vector3 up = Vector3.Cross(right, forward);
                float panScale = _cameraZoom * 0.001f;
                Vector3 move = right * (-delta.X * panScale) + up * (delta.Y * panScale);
                _cameraPos += move;
                _lookAt += move;
                RecalculateCamera();
                e.Handled = true;
            }
        }, RoutingStrategies.Tunnel | RoutingStrategies.Bubble);

        RecalculateCamera();
        RequestNextFrameRendering();
    }

    public override void Render(Avalonia.Media.DrawingContext context)
    {
        base.Render(context);
        context.DrawRectangle(Avalonia.Media.Brushes.Transparent, null, new Rect(Bounds.Size));
    }

    protected override void OnKeyDown(KeyEventArgs e) { _pressedKeys.Add(e.Key); base.OnKeyDown(e); }
    protected override void OnKeyUp(KeyEventArgs e) { _pressedKeys.Remove(e.Key); base.OnKeyUp(e); }

    public static readonly DirectProperty<MeshPreviewerControl, SceneNode?> ActiveSceneProperty = AvaloniaProperty.RegisterDirect<MeshPreviewerControl, SceneNode?>(nameof(ActiveScene), o => o.ActiveScene, (o, v) => o.ActiveScene = v);
    private SceneNode? _activeScene;
    public SceneNode? ActiveScene { get => _activeScene; set => SetAndRaise(ActiveSceneProperty, ref _activeScene, value); }
    
    // Compatibility Properties
    public static readonly DirectProperty<MeshPreviewerControl, MeshObj?> ActiveMeshProperty = AvaloniaProperty.RegisterDirect<MeshPreviewerControl, MeshObj?>(nameof(ActiveMesh), o => o.ActiveMesh, (o, v) => o.ActiveMesh = v);
    private MeshObj? _activeMesh;
    public MeshObj? ActiveMesh 
    { 
        get => _activeMesh; 
        set 
        {
            SetAndRaise(ActiveMeshProperty, ref _activeMesh, value);
            if (value != null)
            {
                var node = new SceneNode() { Name = "Single Mesh" };
                node.Mesh = value;
                ActiveScene = node;
            }
            else
            {
                if (ActiveScene?.Name == "Single Mesh") ActiveScene = null;
            }
        } 
    }

    public static readonly DirectProperty<MeshPreviewerControl, List<MeshObj>?> ActiveMeshesProperty = AvaloniaProperty.RegisterDirect<MeshPreviewerControl, List<MeshObj>?>(nameof(ActiveMeshes), o => o.ActiveMeshes, (o, v) => o.ActiveMeshes = v);
    private List<MeshObj>? _activeMeshes;
    public List<MeshObj>? ActiveMeshes 
    { 
        get => _activeMeshes; 
        set 
        {
            SetAndRaise(ActiveMeshesProperty, ref _activeMeshes, value);
            if (value != null && value.Count > 0)
            {
                var root = new SceneNode() { Name = "Mesh List" };
                foreach (var m in value)
                {
                    var node = new SceneNode() { Name = "Mesh" };
                    node.Mesh = m;
                    root.AddChild(node);
                }
                ActiveScene = root;
            }
            else
            {
                if (ActiveScene?.Name == "Mesh List") ActiveScene = null;
            }
        } 
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    private struct Vertex { public Vector3 Pos, Norm; public Vector2 UV; }

    private void RecalculateCamera()
    {
        Vector3 dir = new Vector3(
             MathF.Cos(_cameraRot.Y) * MathF.Sin(_cameraRot.X),
             MathF.Sin(_cameraRot.Y),
             MathF.Cos(_cameraRot.Y) * MathF.Cos(_cameraRot.X));
        _lookAt = _cameraPos + dir;
    }


    protected override unsafe void OnOpenGlInit(GlInterface gl)
    {
        base.OnOpenGlInit(gl);
        _gl = GL.GetApi(gl.GetProcAddress);
        
        GlVendor = _gl.GetStringS(StringName.Vendor) ?? "Unknown";
        GlRenderer = _gl.GetStringS(StringName.Renderer) ?? "Unknown";
        GlVersionFull = _gl.GetStringS(StringName.Version) ?? "Unknown";
    }

    private unsafe void EnsureResources(GL api)
    {
        if (_shaderProgram != 0 && _gridShaderProgram != 0 && _gridVao != 0) return;
        
        try {
            if (_shaderProgram != 0) api.DeleteProgram(_shaderProgram);
            if (_gridShaderProgram != 0) api.DeleteProgram(_gridShaderProgram);
        } catch { }

        LastShaderError = "";
        _shaderProgram = CreateProgram(MeshPreviewerShaders.VERTEX_SOURCE, MeshPreviewerShaders.FRAGMENT_SOURCE);
        _gridShaderProgram = CreateProgram(MeshPreviewerShaders.GRID_VERTEX, MeshPreviewerShaders.GRID_FRAGMENT);
        
        if (_shaderProgram == 0) LastShaderError += "MainShader ";
        if (_gridShaderProgram == 0) LastShaderError += "GridShader ";
        
        if (string.IsNullOrEmpty(LastShaderError)) LastShaderError = "All Loaded Successfully";
        
        BuildGrid(api);
    }

    private void CheckError(string label)
    {
        var err = _gl!.GetError();
        if (err != GLEnum.NoError) Debug.WriteLine($"[VRCA Preview] GL Error at {label}: {err}");
    }

    private Matrix4x4 CreatePerspectiveGl(float fov, float aspect, float near, float far)
    {
        float tanHalfFov = MathF.Tan(fov / 2.0f);
        Matrix4x4 m = default; 
        m.M11 = 1.0f / (aspect * tanHalfFov);
        m.M22 = 1.0f / tanHalfFov;
        m.M33 = -(far + near) / (far - near);
        m.M34 = -1.0f;
        m.M43 = -(2.0f * far * near) / (far - near);
        return m;
    }

    private unsafe uint CreateProgram(string vsrc, string fsrc)
    {
        var api = _gl!;
        uint vs = api.CreateShader(ShaderType.VertexShader); api.ShaderSource(vs, vsrc); api.CompileShader(vs);
        if (!CheckShader(vs, "Vertex")) { api.DeleteShader(vs); return 0; }
        
        uint fs = api.CreateShader(ShaderType.FragmentShader); api.ShaderSource(fs, fsrc); api.CompileShader(fs);
        if (!CheckShader(fs, "Fragment")) { api.DeleteShader(vs); api.DeleteShader(fs); return 0; }

        uint prg = api.CreateProgram(); api.AttachShader(prg, vs); api.AttachShader(prg, fs); api.LinkProgram(prg);
        
        int status; api.GetProgram(prg, ProgramPropertyARB.LinkStatus, out status);
        if (status == 0) {
            Debug.WriteLine($"[VRCA Preview] Link failed: {api.GetProgramInfoLog(prg)}");
            api.DeleteProgram(prg); prg = 0;
        }
        
        api.DeleteShader(vs); api.DeleteShader(fs); return prg;
    }

    private bool CheckShader(uint shader, string name)
    {
        int status; _gl!.GetShader(shader, ShaderParameterName.CompileStatus, out status);
        if (status == 0) {
            var log = _gl.GetShaderInfoLog(shader);
            Debug.WriteLine($"[VRCA Preview] {name} Compile Error:\n{log}");
            return false;
        }
        return true;
    }

    private unsafe void BuildGrid(GL api)
    {
        if (_gridVao != 0) { api.DeleteBuffer(_gridVbo); api.DeleteVertexArray(_gridVao); }
        var verts = new List<Vector3>();
        for (int i = -10; i <= 10; i++) {
            verts.Add(new(i, 0, -10)); verts.Add(new(i, 0, 10));
            verts.Add(new(-10, 0, i)); verts.Add(new(10, 0, i));
        }
        verts.Add(new(0, 0, 0)); verts.Add(new(5, 0, 0)); // X (Red)
        verts.Add(new(0, 0, 0)); verts.Add(new(0, 5, 0)); // Y (Green)
        verts.Add(new(0, 0, 0)); verts.Add(new(0, 0, 5)); // Z (Blue)
        _gridVertexCount = verts.Count;
        _gridVao = api.GenVertexArray(); api.BindVertexArray(_gridVao);
        _gridVbo = api.GenBuffer(); api.BindBuffer(GLEnum.ArrayBuffer, _gridVbo);
        fixed (void* p = verts.ToArray()) api.BufferData(GLEnum.ArrayBuffer, (nuint)(verts.Count * 12), p, GLEnum.StaticDraw);
        api.VertexAttribPointer(0, 3, GLEnum.Float, false, 12, (void*)0); api.EnableVertexAttribArray(0);
    }

    private unsafe void RebuildMeshes(GL api)
    {
        foreach (var e in _meshCache.Values) { api.DeleteBuffer(e.vbo); api.DeleteBuffer(e.ibo); api.DeleteVertexArray(e.vao); }
        _meshCache.Clear();
        
        if (ActiveScene == null) return;
        
        var meshes = new HashSet<MeshObj>();
        CollectMeshes(ActiveScene, meshes);
        
        foreach (var m in meshes) {
            if (m.Vertices.Length == 0) continue;
            var vcount = m.Vertices.Length / 3;
            var vdata = new Vertex[vcount];
            var uv0 = m.UVs?[0] ?? new float[vcount * 2];
            for (int i = 0; i < vcount; i++) {
                Vector3 pos = new Vector3(m.Vertices[i * 3], m.Vertices[i * 3 + 1], m.Vertices[i * 3 + 2]);
                
                Vector3 norm = Vector3.UnitY;
                if (m.Normals != null && (i * 3 + 2) < m.Normals.Length) {
                    norm = new Vector3(m.Normals[i * 3], m.Normals[i * 3 + 1], m.Normals[i * 3 + 2]);
                }
                
                Vector2 uv = Vector2.Zero;
                if (i * 2 + 1 < uv0.Length) {
                    uv = new Vector2(uv0[i * 2], uv0[i * 2 + 1]);
                }

                vdata[i] = new Vertex { 
                    Pos = pos,
                    Norm = norm,
                    UV = uv
                };
            }
            var entry = new MeshEntry { vao = api.GenVertexArray(), vbo = api.GenBuffer(), ibo = api.GenBuffer(), indexCount = (uint)m.Indices.Length, min = m.MinBounds, max = m.MaxBounds };
            api.BindVertexArray(entry.vao);
            api.BindBuffer(GLEnum.ArrayBuffer, entry.vbo); fixed (void* p = vdata) api.BufferData(GLEnum.ArrayBuffer, (nuint)(vdata.Length * sizeof(Vertex)), p, GLEnum.StaticDraw);
            api.BindBuffer(GLEnum.ElementArrayBuffer, entry.ibo); fixed (void* p = m.Indices) api.BufferData(GLEnum.ElementArrayBuffer, (nuint)(m.Indices.Length * 4), p, GLEnum.StaticDraw);
            api.VertexAttribPointer(0, 3, GLEnum.Float, false, (uint)sizeof(Vertex), (void*)0); api.EnableVertexAttribArray(0);
            api.VertexAttribPointer(1, 3, GLEnum.Float, false, (uint)sizeof(Vertex), (void*)12); api.EnableVertexAttribArray(1);
            api.VertexAttribPointer(2, 2, GLEnum.Float, false, (uint)sizeof(Vertex), (void*)24); api.EnableVertexAttribArray(2);
            
            _meshCache[m] = entry;
        }
        
        RebuildTextures(api);

        FrameAll();
    }
    
    private unsafe void RebuildTextures(GL api)
    {
        foreach (var tex in _textureCache.Values) { api.DeleteTexture(tex); }
        _textureCache.Clear();
        
        if (ActiveScene == null) return;

        var textures = new HashSet<TextureObj>();
        CollectTextures(ActiveScene, textures);
        
        foreach (var t in textures)
        {
            uint tid = api.GenTexture();
            api.BindTexture(TextureTarget.Texture2D, tid);
            
            api.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);
            api.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);
            api.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
            api.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);
            
            if (t.Data != null && t.Data.Length > 0)
            {
                fixed (void* d = t.Data)
                    api.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)t.Width, (uint)t.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, d);
            }
            
            _textureCache[t] = tid;
        }
        
        api.BindTexture(TextureTarget.Texture2D, 0);
    }

    private void CollectMeshes(SceneNode node, HashSet<MeshObj> meshes)
    {
        if (node.Mesh != null) meshes.Add(node.Mesh);
        foreach (var child in node.Children) CollectMeshes(child, meshes);
    }
    
    private void CollectTextures(SceneNode node, HashSet<TextureObj> textures)
    {
        if (node.Texture != null) textures.Add(node.Texture);
        foreach (var child in node.Children) CollectTextures(child, textures);
    }

    private void FrameAll()
    {
        if (_meshCache.Count == 0 || ActiveScene == null) return;
        
        var min = new Vector3(float.MaxValue); var max = new Vector3(float.MinValue);
        bool any = false;
        TraverseBounds(ActiveScene, ref min, ref max, ref any);
        
        if (!any) return;

        Vector3 center = (min + max) / 2f;
        float diag = (max - min).Length();
        if (diag < 0.1f) diag = 1f;
        
        _cameraZoom = MathF.Max(0.5f, diag);
        _cameraPos = center + new Vector3(0, diag * 0.3f, diag * 1.5f);
        Vector3 dir = Vector3.Normalize(center - _cameraPos);
        _cameraRot.Y = MathF.Asin(dir.Y);
        _cameraRot.X = MathF.Atan2(dir.X, dir.Z);
        RecalculateCamera();
    }
    
    private void TraverseBounds(SceneNode node, ref Vector3 min, ref Vector3 max, ref bool any)
    {
        if (node.Mesh != null && _meshCache.TryGetValue(node.Mesh, out var entry))
        {
            var mat = node.WorldMatrix;
            var corners = new Vector3[] {
                new(entry.min.X, entry.min.Y, entry.min.Z), new(entry.max.X, entry.min.Y, entry.min.Z),
                new(entry.min.X, entry.max.Y, entry.min.Z), new(entry.max.X, entry.max.Y, entry.min.Z),
                new(entry.min.X, entry.min.Y, entry.max.Z), new(entry.max.X, entry.min.Y, entry.max.Z),
                new(entry.min.X, entry.max.Y, entry.max.Z), new(entry.max.X, entry.max.Y, entry.max.Z)
            };
            foreach (var c in corners)
            {
                var t = Vector3.Transform(c, mat);
                min = Vector3.Min(min, t);
                max = Vector3.Max(max, t);
            }
            any = true;
        }
        foreach (var child in node.Children) TraverseBounds(child, ref min, ref max, ref any);
    }

    protected override unsafe void OnOpenGlRender(GlInterface gl, int fb)
    {
        var api = _gl;
        if (api == null) return;
        
        EnsureResources(api);
        
        api.BindFramebuffer(FramebufferTarget.Framebuffer, (uint)fb);
        api.ClearColor(0.2f, 0.2f, 0.2f, 1.0f); 
        api.Clear((uint)(GLEnum.ColorBufferBit | GLEnum.DepthBufferBit));
        api.Enable(EnableCap.DepthTest);
        api.DepthFunc(DepthFunction.Less);
        api.Disable(EnableCap.Blend);
        api.Disable(EnableCap.CullFace);
        
        var scaling = VisualRoot?.RenderScaling ?? 1.0;
        ViewportWidth = (int)(Bounds.Width * scaling);
        ViewportHeight = (int)(Bounds.Height * scaling);
        if (ViewportWidth <= 0 || ViewportHeight <= 0) return;
        api.Viewport(0, 0, (uint)ViewportWidth, (uint)ViewportHeight);
        
        float dt = (float)_frameTimer.Elapsed.TotalSeconds; _frameTimer.Restart();
        float aspect = (float)ViewportWidth / (float)ViewportHeight;

        if (_isRightMouseDown || _isLeftMouseDown) Focus();
        
        float speed = _cameraZoom * 2f * dt;
        Vector3 camForward = Vector3.Normalize(_lookAt - _cameraPos);
        Vector3 camRight = Vector3.Normalize(Vector3.Cross(camForward, Vector3.UnitY));
        Vector3 move = Vector3.Zero;
        
        if (_pressedKeys.Contains(Key.W)) move += camForward;
        if (_pressedKeys.Contains(Key.S)) move -= camForward;
        if (_pressedKeys.Contains(Key.A)) move -= camRight;
        if (_pressedKeys.Contains(Key.D)) move += camRight;
        if (_pressedKeys.Contains(Key.E)) move += Vector3.UnitY;
        if (_pressedKeys.Contains(Key.Q)) move -= Vector3.UnitY;
        
        if (move != Vector3.Zero)
        {
            move = Vector3.Normalize(move) * speed;
            _cameraPos += move;
            _lookAt += move;
        }
        
        var proj = CreatePerspectiveGl(0.785f, aspect, 0.01f, 10000.0f);
        var view = Matrix4x4.CreateLookAt(_cameraPos, _lookAt, Vector3.UnitY);
        var mvpGrid = view * proj;
        
        if (api.IsProgram(_gridShaderProgram))
        {
            api.UseProgram(_gridShaderProgram);
            api.UniformMatrix4(api.GetUniformLocation(_gridShaderProgram, "uMVP"), 1, false, &mvpGrid.M11);
            api.BindVertexArray(_gridVao);
            api.DrawArrays(GLEnum.Lines, 0, (uint)_gridVertexCount);
        }

        if (_dirtyModel) { _dirtyModel = false; RebuildMeshes(api); }
        
        if (ActiveScene != null && api.IsProgram(_shaderProgram))
        {
            api.UseProgram(_shaderProgram);
            
            int uModel = api.GetUniformLocation(_shaderProgram, "uModel");
            int uMVP = api.GetUniformLocation(_shaderProgram, "uMVP");
            int uLightDir = api.GetUniformLocation(_shaderProgram, "uLightDir");
            int uCamPos = api.GetUniformLocation(_shaderProgram, "uCamPos");
            int uMainTex = api.GetUniformLocation(_shaderProgram, "uMainTex");
            int uHasTexture = api.GetUniformLocation(_shaderProgram, "uHasTexture");
            
            api.Uniform3(uLightDir, -0.5f, -1f, -0.5f);
            api.Uniform3(uCamPos, _cameraPos.X, _cameraPos.Y, _cameraPos.Z);
            api.Uniform1(uMainTex, 0); 
            
            RenderNode(api, ActiveScene, view * proj, uModel, uMVP, uHasTexture);
        }

        api.BindVertexArray(0);
        api.UseProgram(0);
        
        CheckError("OnOpenGlRender-End");
        RequestNextFrameRendering();
    }
    
    private unsafe void RenderNode(GL api, SceneNode node, Matrix4x4 vp, int uModel, int uMVP, int uHasTexture)
    {
        if (node.Mesh != null && _meshCache.TryGetValue(node.Mesh, out var entry))
        {
            var model = node.WorldMatrix;
            var mvp = model * vp;
            
            api.UniformMatrix4(uModel, 1, false, &model.M11);
            api.UniformMatrix4(uMVP, 1, false, &mvp.M11);
            
            bool hasTex = node.Texture != null && _textureCache.ContainsKey(node.Texture);
            api.Uniform1(uHasTexture, hasTex ? 1 : 0);
            
            if (hasTex)
            {
                api.ActiveTexture(TextureUnit.Texture0);
                api.BindTexture(TextureTarget.Texture2D, _textureCache[node.Texture!]);
            }
            
            api.BindVertexArray(entry.vao);
            api.DrawElements(GLEnum.Triangles, entry.indexCount, GLEnum.UnsignedInt, (void*)0); 
            
            if (hasTex) api.BindTexture(TextureTarget.Texture2D, 0);
        }
        
        foreach (var child in node.Children)
        {
            RenderNode(api, child, vp, uModel, uMVP, uHasTexture);
        }
    }
}
