using System; // 引用基础系统命名空间，提供基本类型与委托（保留原名 System）

using System.Collections.Generic; // 引用泛型集合命名空间，提供 Comparer<T> 等（保留原名 System.Collections.Generic）

using System.Collections.ObjectModel; // 引用可观察集合命名空间，提供 ObservableCollection<T>（保留原名 System.Collections.ObjectModel）

namespace UABEANext4.Util; // 定义命名空间 UABEANext4.Util，用于组织工具扩展方法（保留原名 UABEANext4.Util）

public static class ObservableCollectionExtensions // 定义公共静态类 ObservableCollectionExtensions，作为扩展方法容器（保留原名 ObservableCollectionExtensions）
{ // 类开始

    public static int BinarySearch<T>(this ObservableCollection<T> collection, T item, Func<T, T, int>? compare = null) // 定义扩展方法 BinarySearch：对 ObservableCollection<T> 执行二分查找，返回索引或按位取反的插入点（保留原名 BinarySearch）
    { // 方法开始

        if (collection == null || collection.Count == 0) // 如果集合为 null 或为空
        { // 条件块开始
            return -1; // 返回 -1 表示未找到或集合无效（保留原意）
        } // 条件块结束

        compare ??= ((x, y) => Comparer<T>.Default.Compare(x, y)); // 如果未提供比较函数，则使用默认 Comparer<T>.Default.Compare 作为比较器

        int low = 0; // 初始化二分查找的低位索引 low 为 0
        int high = collection.Count - 1; // 初始化高位索引 high 为集合最后一个元素的索引

        while (low <= high) // 当 low 小于等于 high 时循环（标准二分查找循环）
        { // 循环开始
            int mid = (low + high) / 2; // 计算中间索引 mid（向下取整）
            int comparison = compare(collection[mid], item); // 使用比较函数比较集合中 mid 位置的元素与目标 item

            if (comparison == 0) // 如果比较结果为 0，表示相等
            { // 分支开始
                return mid; // 返回找到的索引 mid
            } // 分支结束
            else if (comparison < 0) // 如果集合中元素小于目标（comparison < 0）
            { // 分支开始
                low = mid + 1; // 将 low 移到 mid 右侧，继续在右半区查找
            } // 分支结束
            else // 否则（comparison > 0）
            { // 分支开始
                high = mid - 1; // 将 high 移到 mid 左侧，继续在左半区查找
            } // 分支结束
        } // 循环结束

        return ~low; // 未找到时返回按位取反的插入点 ~low（与 .NET Array.BinarySearch 行为一致）
    } // 方法结束
} // 类结束
