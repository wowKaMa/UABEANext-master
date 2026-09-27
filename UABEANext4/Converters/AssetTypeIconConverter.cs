using AssetsTools.NET.Extra;
// 引用 AssetsTools.NET 的扩展命名空间，用于访问扩展类型（原名：AssetsTools.NET.Extra）

using Avalonia.Data;
// 引用 Avalonia 的数据绑定命名空间，提供 BindingNotification、BindingErrorType 等（原名：Avalonia.Data）

using Avalonia.Data.Converters;
// 引用 Avalonia 的数据转换器命名空间，包含 IValueConverter 接口（原名：Avalonia.Data.Converters）

using Avalonia.Media.Imaging;
// 引用 Avalonia 的位图/图像命名空间，提供 Bitmap 类型（原名：Avalonia.Media.Imaging）

using Avalonia.Platform;
// 引用 Avalonia 平台抽象命名空间，提供 AssetLoader 等平台相关 API（原名：Avalonia.Platform）

using System;
// 引用基础系统命名空间，提供 Uri、Exception 等基础类型（原名：System）

using System.Collections.Generic;
// 引用泛型集合命名空间，提供 Dictionary<TKey,TValue> 等集合类型（原名：System.Collections.Generic）

using System.Globalization;
// 引用文化信息命名空间，提供 CultureInfo（原名：System.Globalization）

namespace UABEANext4.Converters;
// 定义命名空间 UABEANext4.Converters，用于组织转换器类（原名：UABEANext4.Converters）

public class AssetTypeIconConverter : IValueConverter
// 定义公共类 AssetTypeIconConverter，实现 IValueConverter，用于将资产类型映射为图标（原名：AssetTypeIconConverter / IValueConverter）
{
    // 类体开始（AssetTypeIconConverter）

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    // 实现 IValueConverter.Convert（原名：Convert）：将绑定源的值转换为目标（此处为 Bitmap）
    {
        // 方法体开始（Convert）

        if (value is AssetClassID assetClass)
        // 检查传入的 value 是否为 AssetClassID 枚举（原名：AssetClassID）
        {
            // 条件成立块开始

            if ((int)assetClass < 0)
            // 如果 assetClass 的整数值小于 0（表示特殊/自定义或 MonoBehaviour 类型等）
            {
                return GetBitmap("UABEANext4/Assets/Icons/asset-mono-behaviour.png");
                // 返回默认的 MonoBehaviour 图标（通过 GetBitmap 加载），路径为 "UABEANext4/Assets/Icons/asset-mono-behaviour.png"
            }

            return assetClass switch
            // 使用 switch 表达式根据不同的 AssetClassID 返回对应图标（原名：switch / AssetClassID）
            {
                AssetClassID.Animation => GetBitmap("UABEANext4/Assets/Icons/asset-animation.png"),
                // AssetClassID.Animation 对应的图标路径 "asset-animation.png"

                AssetClassID.AnimationClip => GetBitmap("UABEANext4/Assets/Icons/asset-animation-clip.png"),
                // AssetClassID.AnimationClip 对应的图标路径 "asset-animation-clip.png"

                AssetClassID.Animator => GetBitmap("UABEANext4/Assets/Icons/asset-animator.png"),
                // AssetClassID.Animator 对应的图标路径 "asset-animator.png"

                AssetClassID.AnimatorController => GetBitmap("UABEANext4/Assets/Icons/asset-animator-controller.png"),
                // AssetClassID.AnimatorController 对应的图标路径 "asset-animator-controller.png"

                AssetClassID.AnimatorOverrideController => GetBitmap("UABEANext4/Assets/Icons/asset-animator-override-controller.png"),
                // AssetClassID.AnimatorOverrideController 对应的图标路径 "asset-animator-override-controller.png"

                AssetClassID.AudioClip => GetBitmap("UABEANext4/Assets/Icons/asset-audio-clip.png"),
                // AssetClassID.AudioClip 对应的图标路径 "asset-audio-clip.png"

                AssetClassID.AudioListener => GetBitmap("UABEANext4/Assets/Icons/asset-audio-listener.png"),
                // AssetClassID.AudioListener 对应的图标路径 "asset-audio-listener.png"

                AssetClassID.AudioMixer => GetBitmap("UABEANext4/Assets/Icons/asset-audio-mixer.png"),
                // AssetClassID.AudioMixer 对应的图标路径 "asset-audio-mixer.png"

                AssetClassID.AudioMixerGroup => GetBitmap("UABEANext4/Assets/Icons/asset-audio-mixer-group.png"),
                // AssetClassID.AudioMixerGroup 对应的图标路径 "asset-audio-mixer-group.png"

                AssetClassID.AudioSource => GetBitmap("UABEANext4/Assets/Icons/asset-audio-source.png"),
                // AssetClassID.AudioSource 对应的图标路径 "asset-audio-source.png"

                AssetClassID.Avatar => GetBitmap("UABEANext4/Assets/Icons/asset-avatar.png"),
                // AssetClassID.Avatar 对应的图标路径 "asset-avatar.png"

                AssetClassID.BillboardAsset => GetBitmap("UABEANext4/Assets/Icons/asset-billboard.png"),
                // AssetClassID.BillboardAsset 对应的图标路径 "asset-billboard.png"

                AssetClassID.BillboardRenderer => GetBitmap("UABEANext4/Assets/Icons/asset-billboard-renderer.png"),
                // AssetClassID.BillboardRenderer 对应的图标路径 "asset-billboard-renderer.png"

                AssetClassID.BoxCollider => GetBitmap("UABEANext4/Assets/Icons/asset-box-collider.png"),
                // AssetClassID.BoxCollider 对应的图标路径 "asset-box-collider.png"

                AssetClassID.Camera => GetBitmap("UABEANext4/Assets/Icons/asset-camera.png"),
                // AssetClassID.Camera 对应的图标路径 "asset-camera.png"

                AssetClassID.Canvas => GetBitmap("UABEANext4/Assets/Icons/asset-canvas.png"),
                // AssetClassID.Canvas 对应的图标路径 "asset-canvas.png"

                AssetClassID.CanvasGroup => GetBitmap("UABEANext4/Assets/Icons/asset-canvas-group.png"),
                // AssetClassID.CanvasGroup 对应的图标路径 "asset-canvas-group.png"

                AssetClassID.CanvasRenderer => GetBitmap("UABEANext4/Assets/Icons/asset-canvas-renderer.png"),
                // AssetClassID.CanvasRenderer 对应的图标路径 "asset-canvas-renderer.png"

                AssetClassID.CapsuleCollider => GetBitmap("UABEANext4/Assets/Icons/asset-capsule-collider.png"),
                // AssetClassID.CapsuleCollider 对应的图标路径 "asset-capsule-collider.png"

                AssetClassID.CapsuleCollider2D => GetBitmap("UABEANext4/Assets/Icons/asset-capsule-collider.png"),
                // AssetClassID.CapsuleCollider2D 复用胶囊碰撞器图标，路径 "asset-capsule-collider.png"

                AssetClassID.ComputeShader => GetBitmap("UABEANext4/Assets/Icons/asset-compute-shader.png"),
                // AssetClassID.ComputeShader 对应的图标路径 "asset-compute-shader.png"

                AssetClassID.Cubemap => GetBitmap("UABEANext4/Assets/Icons/asset-cubemap.png"),
                // AssetClassID.Cubemap 对应的图标路径 "asset-cubemap.png"

                AssetClassID.Flare => GetBitmap("UABEANext4/Assets/Icons/asset-flare.png"),
                // AssetClassID.Flare 对应的图标路径 "asset-flare.png"

                AssetClassID.FlareLayer => GetBitmap("UABEANext4/Assets/Icons/asset-flare-layer.png"),
                // AssetClassID.FlareLayer 对应的图标路径 "asset-flare-layer.png"

                AssetClassID.Font => GetBitmap("UABEANext4/Assets/Icons/asset-font.png"),
                // AssetClassID.Font 对应的图标路径 "asset-font.png"

                AssetClassID.GameObject => GetBitmap("UABEANext4/Assets/Icons/asset-game-object.png"),
                // AssetClassID.GameObject 对应的图标路径 "asset-game-object.png"

                AssetClassID.Light => GetBitmap("UABEANext4/Assets/Icons/asset-light.png"),
                // AssetClassID.Light 对应的图标路径 "asset-light.png"

                AssetClassID.LightmapSettings => GetBitmap("UABEANext4/Assets/Icons/asset-lightmap-settings.png"),
                // AssetClassID.LightmapSettings 对应的图标路径 "asset-lightmap-settings.png"

                AssetClassID.LODGroup => GetBitmap("UABEANext4/Assets/Icons/asset-lod-group.png"),
                // AssetClassID.LODGroup 对应的图标路径 "asset-lod-group.png"

                AssetClassID.Material => GetBitmap("UABEANext4/Assets/Icons/asset-material.png"),
                // AssetClassID.Material 对应的图标路径 "asset-material.png"

                AssetClassID.Mesh => GetBitmap("UABEANext4/Assets/Icons/asset-mesh.png"),
                // AssetClassID.Mesh 对应的图标路径 "asset-mesh.png"

                AssetClassID.MeshCollider => GetBitmap("UABEANext4/Assets/Icons/asset-mesh-collider.png"),
                // AssetClassID.MeshCollider 对应的图标路径 "asset-mesh-collider.png"

                AssetClassID.MeshFilter => GetBitmap("UABEANext4/Assets/Icons/asset-mesh-filter.png"),
                // AssetClassID.MeshFilter 对应的图标路径 "asset-mesh-filter.png"

                AssetClassID.MeshRenderer => GetBitmap("UABEANext4/Assets/Icons/asset-mesh-renderer.png"),
                // AssetClassID.MeshRenderer 对应的图标路径 "asset-mesh-renderer.png"

                AssetClassID.MonoBehaviour => GetBitmap("UABEANext4/Assets/Icons/asset-mono-behaviour.png"),
                // AssetClassID.MonoBehaviour 对应的图标路径 "asset-mono-behaviour.png"

                AssetClassID.MonoScript => GetBitmap("UABEANext4/Assets/Icons/asset-mono-script.png"),
                // AssetClassID.MonoScript 对应的图标路径 "asset-mono-script.png"

                AssetClassID.NavMeshSettings => GetBitmap("UABEANext4/Assets/Icons/asset-nav-mesh-settings.png"),
                // AssetClassID.NavMeshSettings 对应的图标路径 "asset-nav-mesh-settings.png"

                AssetClassID.ParticleSystem => GetBitmap("UABEANext4/Assets/Icons/asset-particle-system.png"),
                // AssetClassID.ParticleSystem 对应的图标路径 "asset-particle-system.png"

                AssetClassID.ParticleSystemRenderer => GetBitmap("UABEANext4/Assets/Icons/asset-particle-system-renderer.png"),
                // AssetClassID.ParticleSystemRenderer 对应的图标路径 "asset-particle-system-renderer.png"

                AssetClassID.RectTransform => GetBitmap("UABEANext4/Assets/Icons/asset-rect-transform.png"),
                // AssetClassID.RectTransform 对应的图标路径 "asset-rect-transform.png"

                AssetClassID.ReflectionProbe => GetBitmap("UABEANext4/Assets/Icons/asset-reflection-probe.png"),
                // AssetClassID.ReflectionProbe 对应的图标路径 "asset-reflection-probe.png"

                AssetClassID.Rigidbody => GetBitmap("UABEANext4/Assets/Icons/asset-rigidbody.png"),
                // AssetClassID.Rigidbody 对应的图标路径 "asset-rigidbody.png"

                AssetClassID.Shader => GetBitmap("UABEANext4/Assets/Icons/asset-shader.png"),
                // AssetClassID.Shader 对应的图标路径 "asset-shader.png"

                AssetClassID.ShaderVariantCollection => GetBitmap("UABEANext4/Assets/Icons/asset-shader-collection.png"),
                // AssetClassID.ShaderVariantCollection 对应的图标路径 "asset-shader-collection.png"

                AssetClassID.SkinnedMeshRenderer => GetBitmap("UABEANext4/Assets/Icons/asset-mesh-renderer.png"), // todo
                                                                                                                  // AssetClassID.SkinnedMeshRenderer 暂时复用 mesh renderer 图标（注：代码中标记为 todo，可能需要专用图标）

                AssetClassID.Sprite => GetBitmap("UABEANext4/Assets/Icons/asset-sprite.png"),
                // AssetClassID.Sprite 对应的图标路径 "asset-sprite.png"

                AssetClassID.SpriteRenderer => GetBitmap("UABEANext4/Assets/Icons/asset-sprite-renderer.png"),
                // AssetClassID.SpriteRenderer 对应的图标路径 "asset-sprite-renderer.png"

                AssetClassID.Terrain => GetBitmap("UABEANext4/Assets/Icons/asset-terrain.png"),
                // AssetClassID.Terrain 对应的图标路径 "asset-terrain.png"

                AssetClassID.TerrainCollider => GetBitmap("UABEANext4/Assets/Icons/asset-terrain-collider.png"),
                // AssetClassID.TerrainCollider 对应的图标路径 "asset-terrain-collider.png"

                AssetClassID.TextAsset => GetBitmap("UABEANext4/Assets/Icons/asset-text-asset.png"),
                // AssetClassID.TextAsset 对应的图标路径 "asset-text-asset.png"

                AssetClassID.Texture2D => GetBitmap("UABEANext4/Assets/Icons/asset-texture2d.png"),
                // AssetClassID.Texture2D 对应的图标路径 "asset-texture2d.png"

                AssetClassID.Texture3D => GetBitmap("UABEANext4/Assets/Icons/asset-texture2d.png"),
                // AssetClassID.Texture3D 暂时复用 2D 纹理图标（路径 "asset-texture2d.png"）

                AssetClassID.Transform => GetBitmap("UABEANext4/Assets/Icons/asset-transform.png"),
                // AssetClassID.Transform 对应的图标路径 "asset-transform.png"

                _ => GetBitmap("UABEANext4/Assets/Icons/asset-unknown.png"),
                // 默认分支：未知或未列出的 AssetClassID 使用通用未知图标 "asset-unknown.png"
            };
        }

        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
        // 如果传入的 value 不是 AssetClassID，则返回一个绑定错误通知（BindingNotification），表示类型不匹配
    }

    Dictionary<string, Bitmap> cache = new();
    // 字典缓存（cache）：按路径缓存已加载的 Bitmap，避免重复加载（键：路径，值：Bitmap）

    private Bitmap GetBitmap(string path)
    // 私有方法 GetBitmap：根据资源路径加载并缓存 Bitmap（原名：GetBitmap）
    {
        Bitmap? bitmap;
        // 局部变量 bitmap，用于尝试从缓存读取或保存新加载的位图

        if (cache.TryGetValue(path, out bitmap))
        // 如果缓存中存在该路径的 Bitmap，则直接返回缓存的实例
        {
            return bitmap;
        }
        else
        {
            bitmap = new Bitmap(AssetLoader.Open(new Uri($"avares://{path}")));
            // 使用 AssetLoader.Open 打开 avares URI（"avares://{path}"）并创建新的 Bitmap（原名：AssetLoader / Bitmap）
            cache[path] = bitmap;
            // 将新加载的 Bitmap 存入缓存（cache）以便后续复用
            return bitmap;
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    // 实现 IValueConverter.ConvertBack（原名：ConvertBack）：此转换器不支持反向转换
    {
        throw new NotImplementedException();
        // 抛出 NotImplementedException 表示未实现 ConvertBack（调用者不应使用反向绑定）
    }
}
// 类体结束（AssetTypeIconConverter）
