using System.Text.RegularExpressions; // 引用正则表达式命名空间，用于执行正则匹配（保留原名 System.Text.RegularExpressions）

namespace UABEANext4.Util; // 定义命名空间 UABEANext4.Util，用于组织工具类（保留原名 UABEANext4.Util）

public static class SearchUtils // 定义公共静态类 SearchUtils，作为搜索相关工具方法的容器（保留原名 SearchUtils）
{
    // cheap * search check
    // 注释：廉价的通配符 '*' 匹配检查（作者原注），用于说明下面方法的用途

    public static bool WildcardMatches(string test, string pattern, bool caseSensitive = true) // 定义公共静态方法 WildcardMatches，参数：要测试的字符串 test，通配模式 pattern，是否区分大小写 caseSensitive（默认 true）
    {
        RegexOptions options = 0; // 声明并初始化 RegexOptions 变量 options 为 0（无特殊选项），用于后续传入正则匹配

        if (!caseSensitive) // 如果不区分大小写
            options |= RegexOptions.IgnoreCase; // 将 IgnoreCase 标志加入 options，使正则匹配忽略大小写

        return Regex.IsMatch(test, "^" + Regex.Escape(pattern).Replace("\\*", ".*") + "$", options); // 将通配符模式转换为正则并执行匹配：
                                                                                                     // 1. Regex.Escape(pattern) 对 pattern 中的特殊字符进行转义，保证字面匹配；
                                                                                                     // 2. Replace("\\*", ".*") 将转义后的星号模式 "\*" 替换为 ".*"（正则中表示任意长度任意字符），实现通配符功能；
                                                                                                     // 3. 在前后加上 ^ 和 $，确保整个 test 字符串完全匹配该模式；
                                                                                                     // 4. 使用上面构造的 options（可能包含 IgnoreCase）进行匹配；
                                                                                                     // 最终返回布尔值，表示 test 是否匹配 pattern（保留原名 Regex.IsMatch）
    }
}
