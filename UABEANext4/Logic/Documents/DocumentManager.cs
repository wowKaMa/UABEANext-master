using Dock.Model.Mvvm.Controls;
// 引用 Dock.Model 的 MVVM 控件命名空间（原名：Dock.Model.Mvvm.Controls），提供 Document 等窗口/文档相关类型

using System.Collections.Generic;
// 引用泛型集合命名空间（原名：System.Collections.Generic），提供 List<T> 等集合类型

namespace UABEANext4.Logic.Documents;
// 定义命名空间 UABEANext4.Logic.Documents（原名：UABEANext4.Logic.Documents），用于组织与文档管理相关的逻辑代码

// this is temporary until this gets mvvm'd
// 注释：说明该类是临时实现，未来会改为 MVVM 风格（保留英文原注释）

public class DocumentManager
// 定义公共类 DocumentManager（原名：DocumentManager），负责在应用内管理打开的文档集合与焦点文档
{
    // 类体开始（DocumentManager）

    public List<Document> Documents = [];
    // 公共字段 Documents（原名：Documents），类型为 List<Document>，用于保存当前管理的文档列表；此处初始化为空（保留英文原名：Document）
    // 说明：Document 来自 Dock.Model.Mvvm.Controls，表示一个可管理的文档/视图项

    public Document? LastFocusedDocument = null;
    // 公共可空字段 LastFocusedDocument（原名：LastFocusedDocument），用于记录最后获得焦点的文档（可能为 null）
    // 说明：当用户切换文档时，可将该字段更新为当前活动文档以便恢复或引用

    public void Clear()
    // 公共方法 Clear（原名：Clear），用于清空文档管理器的状态（清空文档列表并重置焦点文档）
    {
        // 方法体开始（Clear）

        Documents = [];
        // 将 Documents 重置为空列表（清除所有已注册/打开的文档）
        // 说明：调用 Clear 后，管理器不再持有任何文档引用

        LastFocusedDocument = null;
        // 将 LastFocusedDocument 设为 null（重置最后焦点文档记录）
        // 说明：清理状态以避免悬挂引用或错误恢复
    }
    // 方法体结束（Clear）

}
// 类体结束（DocumentManager）
