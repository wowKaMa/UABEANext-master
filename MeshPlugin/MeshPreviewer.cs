using AssetsTools.NET.Extra;
// 引用 AssetsTools.NET 的扩展命名空间，用于处理扩展类型与辅助方法（保留英文原名：AssetsTools.NET.Extra）

using Avalonia.Media.Imaging;
// 引用 Avalonia 的位图/图像类型命名空间，提供 Bitmap/WriteableBitmap 等用于图像显示（保留英文原名：Avalonia.Media.Imaging）

using UABEANext4.AssetWorkspace;
// 引用项目的工作区命名空间，提供 Workspace、AssetInst 等类型与方法（保留英文原名：UABEANext4.AssetWorkspace）

using UABEANext4.Logic.Mesh;
// 引用项目的网格逻辑命名空间，提供 MeshObj、UnityVersion 等（保留英文原名：UABEANext4.Logic.Mesh）

using UABEANext4.Plugins;
// 引用项目的插件接口命名空间，定义 IUavPluginPreviewer、IUavPluginFunctions 等（保留英文原名：UABEANext4.Plugins）

namespace MeshPlugin;
// 定义命名空间 MeshPlugin，用于组织网格预览器相关类型（保留英文原名：MeshPlugin）

public class MeshPreviewer : IUavPluginPreviewer
// 定义公共类 MeshPreviewer，实现 IUavPluginPreviewer 接口，表示一个网格预览插件（保留英文原名：MeshPreviewer / IUavPluginPreviewer）
{
    // 类体开始（MeshPreviewer）

    public string Name => "Preview Mesh";
    // 只读属性 Name：插件名称，用于 UI 显示（返回 "Preview Mesh"）（保留英文原名：Name）

    public string Description => "Preview Meshes";
    // 只读属性 Description：插件描述，用于 UI 提示（返回 "Preview Meshes"）（保留英文原名：Description）

    private static bool IsGameObjectWithMeshFilter(Workspace workspace, AssetInst goAsset)
    // 私有静态方法 IsGameObjectWithMeshFilter：判断给定的 GameObject（goAsset）是否包含 MeshFilter 组件（保留英文原名：IsGameObjectWithMeshFilter / Workspace / AssetInst）
    {
        if (goAsset.Type != AssetClassID.GameObject)
            return false;
        // 如果所选资产类型不是 GameObject，则直接返回 false（保留英文原名：goAsset.Type / AssetClassID.GameObject）

        var goBase = workspace.GetBaseField(goAsset);
        // 获取 GameObject 的 BaseField（反序列化后的字段结构），用于读取组件列表（保留英文原名：workspace.GetBaseField / goBase）

        if (goBase is null)
            return false;
        // 如果无法读取 BaseField，则返回 false（保留英文原名：goBase）

        var goComponents = goBase["m_Component.Array"];
        // 读取 GameObject 的组件数组字段 m_Component.Array（保留英文原名：goComponents / "m_Component.Array"）

        foreach (var componentPair in goComponents)
        {
            var component = componentPair[componentPair.Children.Count - 1];
            // 每个 componentPair 的最后一个子项通常是组件引用，取出该引用（保留英文原名：componentPair / Children）

            // cheaper to use AssetFileInfo rather than AssetInst
            // 注释：使用 AssetFileInfo 比直接构造 AssetInst 更便宜（保留英文原注释）

            var componentInf = workspace.GetAssetFileInfo(goAsset.FileInstance, component);
            // 使用 workspace.GetAssetFileInfo 获取组件引用对应的 AssetFileInfo（保留英文原名：GetAssetFileInfo / componentInf）

            if (componentInf is not null && componentInf.TypeId == (int)AssetClassID.MeshFilter)
            {
                return true;
            }
            // 如果该组件存在且类型为 MeshFilter，则说明 GameObject 包含 MeshFilter，返回 true（保留英文原名：MeshFilter / TypeId）
        }

        return false;
        // 遍历结束后未找到 MeshFilter，则返回 false（保留英文原名：return false）
    }

    private static AssetInst? GetMeshFromGameObject(Workspace workspace, AssetInst goAsset)
    // 私有静态方法 GetMeshFromGameObject：从 GameObject 中查找并返回关联的 Mesh 资产（若存在），否则返回 null（保留英文原名：GetMeshFromGameObject）
    {
        var goBase = workspace.GetBaseField(goAsset);
        // 获取 GameObject 的 BaseField（保留英文原名：GetBaseField / goBase）

        if (goBase is null)
            return null;
        // 如果无法读取 BaseField，则返回 null（保留英文原名：goBase）

        var goComponents = goBase["m_Component.Array"];
        // 读取组件数组（保留英文原名：goComponents）

        foreach (var componentPair in goComponents)
        {
            var component = componentPair[componentPair.Children.Count - 1];
            // 取出组件引用（保留英文原名：componentPair / Children）

            // cheaper to use AssetFileInfo rather than AssetInst
            // 注释：再次说明使用 AssetFileInfo 更高效（保留英文原注释）

            var componentInf = workspace.GetAssetFileInfo(goAsset.FileInstance, component);
            // 获取组件的 AssetFileInfo（保留英文原名：componentInf）

            if (componentInf is not null && componentInf.TypeId == (int)AssetClassID.MeshFilter)
            {
                var mfiltAsset = new AssetInst(goAsset.FileInstance, componentInf);
                // 使用 AssetFileInfo 构造一个 AssetInst（MeshFilter 组件实例），以便读取其 BaseField（保留英文原名：AssetInst / mfiltAsset）

                var mfiltBase = workspace.GetBaseField(mfiltAsset);
                // 获取 MeshFilter 组件的 BaseField（保留英文原名：mfiltBase）

                if (mfiltBase is null)
                    return null;
                // 如果无法读取 MeshFilter 的 BaseField，则返回 null（保留英文原名：mfiltBase）

                var meshAsset = workspace.GetAssetInst(mfiltAsset.FileInstance, mfiltBase["m_Mesh"]);
                // 从 MeshFilter 的字段 m_Mesh 中获取指向的 Mesh 资产（保留英文原名：GetAssetInst / "m_Mesh"）

                if (meshAsset is null)
                    return null;
                // 如果无法获取 Mesh 资产则返回 null（保留英文原名：meshAsset）

                return meshAsset;
                // 找到并返回 Mesh 资产（保留英文原名：meshAsset）
            }
        }

        return null;
        // 未找到 MeshFilter 或 Mesh，则返回 null（保留英文原名：return null）
    }

    public UavPluginPreviewerType SupportsPreview(Workspace workspace, AssetInst selection)
    // 公共方法 SupportsPreview：判断给定选择是否支持网格预览，返回 UavPluginPreviewerType（保留英文原名：SupportsPreview / UavPluginPreviewerType）
    {
        var previewType = selection.Type == AssetClassID.Mesh || IsGameObjectWithMeshFilter(workspace, selection)
            ? UavPluginPreviewerType.Mesh
            : UavPluginPreviewerType.None;
        // 如果选择本身是 Mesh，或是包含 MeshFilter 的 GameObject，则返回 Mesh 类型的预览支持，否则返回 None（保留英文原名：selection.Type / AssetClassID.Mesh / IsGameObjectWithMeshFilter）

        return previewType;
        // 返回计算得到的 previewType（保留英文原名：previewType）
    }

    public MeshObj? ExecuteMesh(Workspace workspace, IUavPluginFunctions funcs, AssetInst selection, out string? error)
    // 公共方法 ExecuteMesh：尝试构建并返回 MeshObj 以供渲染预览，若失败通过 out 参数返回错误信息（保留英文原名：ExecuteMesh / MeshObj / IUavPluginFunctions）
    {
        try
        {
            // if we selected a gameobject, do gameobject -> meshfilter -> mesh
            // 注释：如果用户选择的是 GameObject，则需要先解析到 MeshFilter 再到 Mesh（保留英文原注释）

            if (selection.Type == AssetClassID.GameObject)
            {
                // todo: make GetComponent helper function for all plugins
                // 注释：TODO：可以把组件查找逻辑抽成通用函数供所有插件使用（保留英文原注释）

                var maybeMeshAsset = GetMeshFromGameObject(workspace, selection);
                // 尝试从 GameObject 中获取 Mesh 资产（保留英文原名：GetMeshFromGameObject / maybeMeshAsset）

                if (maybeMeshAsset is null)
                {
                    error = "No preview available (mesh couldn't be loaded).";
                    return null;
                }
                // 如果无法获取 Mesh，则设置错误信息并返回 null（保留英文原名：error）

                selection = maybeMeshAsset;
                // 将 selection 替换为实际的 Mesh 资产以便后续处理（保留英文原名：selection）
            }

            var meshBf = workspace.GetBaseField(selection);
            // 获取 Mesh 的 BaseField（保留英文原名：meshBf / GetBaseField）

            if (meshBf == null)
            {
                error = "No preview available (mesh base field couldn't be loaded).";
                return null;
            }
            // 如果无法读取 Mesh 的 BaseField，则设置错误并返回 null（保留英文原名：meshBf）

            var version = new UnityVersion(selection.FileInstance.file.Metadata.UnityVersion);
            // 读取所属文件的 Unity 版本并构造 UnityVersion（保留英文原名：UnityVersion / selection.FileInstance.file.Metadata.UnityVersion）

            var meshObj = new MeshObj(selection.FileInstance, meshBf, version);
            // 使用 MeshObj 构造器创建可用于渲染/预览的网格对象（保留英文原名：MeshObj / meshObj）

            error = null;
            // 成功构建，清空错误信息（保留英文原名：error）

            return meshObj;
            // 返回构建好的 MeshObj（保留英文原名：meshObj）
        }
        catch (Exception ex)
        {
            error = $"Mesh failed to decode due to an error. Exception:\n{ex}";
            return null;
        }
        // 捕获异常，记录异常信息到 error 并返回 null（保留英文原名：Exception / ex）
    }

    public Bitmap? ExecuteImage(Workspace workspace, IUavPluginFunctions funcs, AssetInst selection, out string? error)
        => throw new InvalidOperationException();
    // 公共方法 ExecuteImage：接口要求的方法，但网格预览不支持图像输出，直接抛出 InvalidOperationException（保留英文原名：ExecuteImage / InvalidOperationException）

    public string? ExecuteText(Workspace workspace, IUavPluginFunctions funcs, AssetInst selection, out string? error)
        => throw new InvalidOperationException();
    // 公共方法 ExecuteText：接口要求的方法，但网格预览不支持文本输出，直接抛出 InvalidOperationException（保留英文原名：ExecuteText / InvalidOperationException）

    public void Cleanup() { }
    // 公共方法 Cleanup：清理资源的占位实现（当前实现为空，若有缓存或资源应在此释放）（保留英文原名：Cleanup）

}
// 类体结束（MeshPreviewer）
