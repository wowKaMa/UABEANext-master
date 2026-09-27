namespace System.Collections.ObjectModel // 定义命名空间 System.Collections.ObjectModel（保留原名），用于放置集合相关类型
{ // 命名空间开始

    // Licensed to the .NET Foundation under one or more agreements. // 原始版权声明行（保留英文原文），表示该文件受 .NET 基金会许可
    // The .NET Foundation licenses this file to you under the MIT license. // 原始版权声明行（保留英文原文），说明许可为 MIT
    // See the LICENSE file in the project root for more information. // 原始版权声明行（保留英文原文），提示查看 LICENSE 文件

    using System.Collections.Generic; // 引用泛型集合命名空间（List<T>、Dictionary 等），保留原名
    using System.Collections.Specialized; // 引用特殊集合通知命名空间（NotifyCollectionChangedEventArgs 等），保留原名
    using System.ComponentModel; // 引用组件模型命名空间（INotifyPropertyChanged 等），保留原名
    using System.Diagnostics; // 引用调试工具命名空间（Debug 等），保留原名
    using System.Linq; // 引用 LINQ 扩展方法命名空间（Where、Any 等），保留原名

    /// <summary> // XML 注释开始：描述类的用途（保留英文原文结构）
    /// Implementation of a dynamic data collection based on generic Collection&lt;T&gt;, // 英文注释：基于泛型 Collection<T> 的动态数据集合实现
    /// implementing INotifyCollectionChanged to notify listeners // 英文注释：实现 INotifyCollectionChanged，用于在项添加/移除/刷新时通知监听者
    /// when items get added, removed or the whole list is refreshed. // 英文注释：说明何时触发通知
    /// </summary> // XML 注释结束

    public class RangeObservableCollection<T> : ObservableCollection<T> // 定义公共泛型类 RangeObservableCollection<T>，继承自 ObservableCollection<T>（保留原名）
    { // 类开始

        //------------------------------------------------------ // 分隔注释（保留原文结构）
        // // 空行分隔
        //  Private Fields // 区块注释：私有字段部分（保留英文原名）
        // // 空行分隔
        //------------------------------------------------------ // 分隔注释（保留原文结构）

        #region Private Fields    // 区域开始：Private Fields（保留原名）
        [NonSerialized] // 特性：标记字段在序列化时不被序列化（NonSerialized）
        private DeferredEventsCollection? _deferredEvents; // 私有可空字段：用于延迟事件收集的 DeferredEventsCollection 实例（_deferredEvents）
        #endregion Private Fields // 区域结束：Private Fields（保留原名）


        //------------------------------------------------------ // 分隔注释（保留原文结构）
        // // 空行分隔
        //  Constructors // 区块注释：构造函数部分（保留英文原名）
        // // 空行分隔
        //------------------------------------------------------ // 分隔注释（保留原文结构）

        #region Constructors // 区域开始：Constructors（保留原名）
        /// <summary> // XML 注释开始：构造函数说明（保留英文原文结构）
        /// Initializes a new instance of ObservableCollection that is empty and has default initial capacity. // 英文注释：初始化一个空的 ObservableCollection 实例
        /// </summary> // XML 注释结束
        public RangeObservableCollection() { } // 无参构造函数：初始化空集合（调用基类默认构造），空实现体

        /// <summary> // XML 注释开始：构造函数说明（保留英文原文结构）
        /// Initializes a new instance of the ObservableCollection class that contains // 英文注释：初始化一个包含指定集合元素的 ObservableCollection
        /// elements copied from the specified collection and has sufficient capacity // 英文注释：并具有足够容量以容纳复制的元素
        /// to accommodate the number of elements copied. // 英文注释：继续说明
        /// </summary> // XML 注释结束
        /// <param name="collection">The collection whose elements are copied to the new list.</param> // 参数注释：collection 为要复制的集合
        /// <remarks> // 备注开始
        /// The elements are copied onto the ObservableCollection in the // 英文注释：元素按枚举器读取顺序复制
        /// same order they are read by the enumerator of the collection. // 继续说明
        /// </remarks> // 备注结束
        /// <exception cref="ArgumentNullException"> collection is a null reference </exception> // 异常注释：collection 为 null 时抛出 ArgumentNullException
        public RangeObservableCollection(IEnumerable<T> collection) : base(collection) { } // 构造函数：接受 IEnumerable<T> 并传递给基类构造器以初始化集合

        /// <summary> // XML 注释开始：构造函数说明（保留英文原文结构）
        /// Initializes a new instance of the ObservableCollection class // 英文注释：初始化一个 ObservableCollection 实例
        /// that contains elements copied from the specified list // 英文注释：从指定列表复制元素
        /// </summary> // XML 注释结束
        /// <param name="list">The list whose elements are copied to the new list.</param> // 参数注释：list 为要复制的 List<T>
        /// <remarks> // 备注开始
        /// The elements are copied onto the ObservableCollection in the // 英文注释：元素按枚举器读取顺序复制
        /// same order they are read by the enumerator of the list. // 继续说明
        /// </remarks> // 备注结束
        /// <exception cref="ArgumentNullException"> list is a null reference </exception> // 异常注释：list 为 null 时抛出 ArgumentNullException
        public RangeObservableCollection(List<T> list) : base(list) { } // 构造函数：接受 List<T> 并传递给基类构造器以初始化集合

        #endregion Constructors // 区域结束：Constructors（保留原名）

        //------------------------------------------------------ // 分隔注释（保留原文结构）
        // // 空行分隔
        //  Public Properties // 区块注释：公共属性部分（保留英文原名）
        // // 空行分隔
        //------------------------------------------------------ // 分隔注释（保留原文结构）

        #region Public Properties // 区域开始：Public Properties（保留原名）
        EqualityComparer<T>? _Comparer; // 私有字段：可空的 EqualityComparer<T>，用于比较元素相等性（_Comparer）
        public EqualityComparer<T> Comparer // 公共属性 Comparer：返回或设置比较器
        {
            get => _Comparer ??= EqualityComparer<T>.Default; // getter：如果 _Comparer 为 null 则赋默认比较器并返回
            private set => _Comparer = value; // 私有 setter：设置 _Comparer（外部不可写）
        }

        /// <summary> // XML 注释开始：AllowDuplicates 属性说明（保留英文原文结构）
        /// Gets or sets a value indicating whether this collection acts as a <see cref="HashSet{T}"/>, // 英文注释：指示集合是否像 HashSet<T> 一样工作
        /// disallowing duplicate items, based on <see cref="Comparer"/>. // 英文注释：基于 Comparer 禁止重复项
        /// This might indeed consume background performance, but in the other hand, // 英文注释：说明性能权衡
        /// it will pay off in UI performance as less required UI updates are required. // 英文注释：减少 UI 更新带来好处
        /// </summary> // XML 注释结束
        public bool AllowDuplicates { get; set; } = true; // 公共属性 AllowDuplicates：是否允许重复项，默认 true（允许重复）

        #endregion Public Properties // 区域结束：Public Properties（保留原名）

        //------------------------------------------------------ // 分隔注释（保留原文结构）
        // // 空行分隔
        //  Public Methods // 区块注释：公共方法部分（保留英文原名）
        // // 空行分隔
        //------------------------------------------------------ // 分隔注释（保留原文结构）

        #region Public Methods // 区域开始：Public Methods（保留原名）

        /// <summary> // XML 注释开始：AddRange 方法说明（保留英文原文结构）
        /// Adds the elements of the specified collection to the end of the <see cref="ObservableCollection{T}"/>. // 英文注释：将指定集合的元素添加到 ObservableCollection 末尾
        /// </summary> // XML 注释结束
        /// <param name="collection"> // 参数注释开始
        /// The collection whose elements should be added to the end of the <see cref="ObservableCollection{T}"/>. // 英文注释：collection 为要添加的集合
        /// The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type. // 英文注释：collection 本身不能为 null，但可以包含 null 元素（若 T 为引用类型）
        /// </param> // 参数注释结束
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception> // 异常注释：collection 为 null 时抛出 ArgumentNullException
        public void AddRange(IEnumerable<T> collection) // 公共方法 AddRange：将集合追加到末尾
        {
            InsertRange(Count, collection); // 调用 InsertRange 在索引 Count（末尾）插入集合
        }

        /// <summary> // XML 注释开始：InsertRange 方法说明（保留英文原文结构）
        /// Inserts the elements of a collection into the <see cref="ObservableCollection{T}"/> at the specified index. // 英文注释：在指定索引插入集合元素
        /// </summary> // XML 注释结束
        /// <param name="index">The zero-based index at which the new elements should be inserted.</param> // 参数注释：index 为插入起始索引（从 0 开始）
        /// <param name="collection">The collection whose elements should be inserted into the List<T>. // 参数注释：collection 为要插入的集合
        /// The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.</param>                // 参数注释继续
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception> // 异常注释：collection 为 null 时抛出 ArgumentNullException
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not in the collection range.</exception> // 异常注释：index 超出范围时抛出 ArgumentOutOfRangeException
        public void InsertRange(int index, IEnumerable<T> collection) // 公共方法 InsertRange：在指定索引插入集合
        {
            if (collection == null) // 检查 collection 是否为 null
                throw new ArgumentNullException(nameof(collection)); // 若为 null 则抛出 ArgumentNullException
            if (index < 0) // 检查 index 是否小于 0
                throw new ArgumentOutOfRangeException(nameof(index)); // 若小于 0 则抛出 ArgumentOutOfRangeException
            if (index > Count) // 检查 index 是否大于 Count（超出末尾）
                throw new ArgumentOutOfRangeException(nameof(index)); // 若超出则抛出 ArgumentOutOfRangeException

            if (!AllowDuplicates) // 如果不允许重复项
                collection = // 对传入集合进行去重与过滤，赋回 collection
                  collection
                  .Distinct(Comparer) // 使用 Comparer 去重
                  .Where(item => !Items.Contains(item, Comparer)) // 过滤掉已存在于 Items 的项
                  .ToList(); // 转为列表以避免多次枚举

            if (collection is ICollection<T> countable) // 如果 collection 实现 ICollection<T>
            {
                if (countable.Count == 0) // 如果集合为空
                    return; // 直接返回（无需插入）
            }
            else if (!collection.Any()) // 否则如果 collection 不包含任何元素（使用 LINQ Any 检查）
                return; // 直接返回

            CheckReentrancy(); // 检查重入（ObservableCollection 的保护机制，防止在事件触发期间修改集合）

            //expand the following couple of lines when adding more constructors. // 注释：提示如果添加更多构造函数需扩展下面几行
            var target = (List<T>)Items; // 将内部 Items 强制转换为 List<T>（基于构造函数保证 Items 为 List<T>）
            target.InsertRange(index, collection); // 在目标列表的指定索引插入集合

            OnEssentialPropertiesChanged(); // 触发属性变更通知（Count 与索引器）

            if (!(collection is IList list)) // 如果 collection 不是 IList
                list = new List<T>(collection); // 将 collection 包装为 List<T> 以便后续通知使用

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list, index)); // 触发集合更改事件，通知监听者添加了 list（从 index 开始）
        }


        /// <summary>  // XML 注释开始：RemoveRange 方法说明（保留英文原文结构）
        /// Removes the first occurence of each item in the specified collection from the <see cref="ObservableCollection{T}"/>. // 英文注释：从集合中移除指定集合中每个项的首次出现
        /// </summary> // XML 注释结束
        /// <param name="collection">The items to remove.</param>        // 参数注释：collection 为要移除的项集合
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception> // 异常注释：collection 为 null 时抛出 ArgumentNullException
        public void RemoveRange(IEnumerable<T> collection) // 公共方法 RemoveRange：移除集合中指定项的首次出现
        {
            if (collection == null) // 检查 collection 是否为 null
                throw new ArgumentNullException(nameof(collection)); // 若为 null 则抛出 ArgumentNullException

            if (Count == 0) // 如果当前集合为空
                return; // 直接返回
            else if (collection is ICollection<T> countable) // 否则如果 collection 实现 ICollection<T>
            {
                if (countable.Count == 0) // 如果 collection 为空
                    return; // 直接返回
                else if (countable.Count == 1) // 如果 collection 只有一个元素
                    using (IEnumerator<T> enumerator = countable.GetEnumerator()) // 使用枚举器获取该单个元素
                    {
                        enumerator.MoveNext(); // 移动到第一个元素
                        Remove(enumerator.Current); // 调用 Remove 移除该元素
                        return; // 返回
                    }
            }
            else if (!collection.Any()) // 否则如果 collection 没有任何元素
                return; // 直接返回

            CheckReentrancy(); // 检查重入保护

            var clusters = new Dictionary<int, List<T>>(); // 创建字典 clusters：键为索引，值为在该索引处被移除的项列表（用于分组通知）
            var lastIndex = -1; // 记录上一次处理的索引
            List<T>? lastCluster = null; // 记录上一次的聚簇列表
            foreach (T item in collection) // 遍历要移除的每个项
            {
                var index = IndexOf(item); // 在当前集合中查找该项的索引
                if (index < 0) // 如果未找到
                    continue; // 跳过该项

                Items.RemoveAt(index); // 从 Items 中移除该索引处的项

                if (lastIndex == index && lastCluster != null) // 如果当前索引与上一次相同且 lastCluster 非空
                    lastCluster.Add(item); // 将该项加入 lastCluster（聚簇）
                else
                    clusters[lastIndex = index] = lastCluster = new List<T> { item }; // 否则创建新的聚簇并记录到 clusters 中
            }

            OnEssentialPropertiesChanged(); // 触发 Count 与索引器属性变更通知

            if (Count == 0) // 如果集合已为空
                OnCollectionReset(); // 触发集合重置通知（Reset）
            else
                foreach (KeyValuePair<int, List<T>> cluster in clusters) // 否则遍历每个聚簇
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, cluster.Value, cluster.Key)); // 触发移除通知，包含被移除的项与索引

        }

        /// <summary> // XML 注释开始：RemoveAll 方法说明（保留英文原文结构）
        /// Iterates over the collection and removes all items that satisfy the specified match. // 英文注释：遍历集合并移除满足匹配条件的所有项
        /// </summary> // XML 注释结束
        /// <remarks>The complexity is O(n).</remarks> // 备注：复杂度为 O(n)
        /// <param name="match"></param> // 参数注释：match 为判断是否移除的谓词
        /// <returns>Returns the number of elements that where </returns> // 返回值注释：返回被移除的元素数量
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception> // 异常注释：match 为 null 时抛出 ArgumentNullException
        public int RemoveAll(Predicate<T> match) // 公共方法 RemoveAll：移除所有满足 match 的项（整个集合范围）
        {
            return RemoveAll(0, Count, match); // 调用重载方法 RemoveAll 从索引 0 开始处理 Count 个元素
        }

        /// <summary> // XML 注释开始：RemoveAll 重载方法说明（保留英文原文结构）
        /// Iterates over the specified range within the collection and removes all items that satisfy the specified match. // 英文注释：在指定范围内移除满足 match 的项
        /// </summary> // XML 注释结束
        /// <remarks>The complexity is O(n).</remarks> // 备注：复杂度为 O(n)
        /// <param name="index">The index of where to start performing the search.</param> // 参数注释：index 为起始索引
        /// <param name="count">The number of items to iterate on.</param> // 参数注释：count 为要检查的项数
        /// <param name="match"></param> // 参数注释：match 为判断是否移除的谓词
        /// <returns>Returns the number of elements that where </returns> // 返回值注释：返回被移除的元素数量
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is out of range.</exception> // 异常注释：index 越界时抛出
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is out of range.</exception> // 异常注释：count 越界时抛出
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception> // 异常注释：match 为 null 时抛出
        public int RemoveAll(int index, int count, Predicate<T> match) // 公共方法 RemoveAll（带范围）：在指定范围内移除满足 match 的项
        {
            if (index < 0) // 检查 index 是否小于 0
                throw new ArgumentOutOfRangeException(nameof(index)); // 抛出异常
            if (count < 0) // 检查 count 是否小于 0
                throw new ArgumentOutOfRangeException(nameof(count)); // 抛出异常
            if (index + count > Count) // 检查 index + count 是否超出集合范围
                throw new ArgumentOutOfRangeException(nameof(index)); // 抛出异常
            if (match == null) // 检查 match 是否为 null
                throw new ArgumentNullException(nameof(match)); // 抛出异常

            if (Count == 0) // 如果集合为空
                return 0; // 返回 0（无项被移除）

            List<T>? cluster = null; // 局部变量 cluster：当前聚簇（可能为 null）
            var clusterIndex = -1; // 当前聚簇的起始索引
            var removedCount = 0; // 已移除项计数

            using (BlockReentrancy()) // 使用 BlockReentrancy 保护，防止在事件触发期间重入
            using (DeferEvents()) // 使用 DeferEvents 延迟事件收集，最后一次性触发
            {
                for (var i = 0; i < count; i++, index++) // 遍历指定范围内的每个位置
                {
                    T item = Items[index]; // 获取当前位置的项
                    if (match(item)) // 如果匹配谓词为真
                    {
                        Items.RemoveAt(index); // 从 Items 中移除该项
                        removedCount++; // 移除计数加一

                        if (clusterIndex == index) // 如果当前聚簇索引等于当前索引
                        {
                            Debug.Assert(cluster != null); // 调试断言：cluster 不应为 null
                            cluster!.Add(item); // 将该项加入当前聚簇
                        }
                        else
                        {
                            cluster = new List<T> { item }; // 否则创建新的聚簇并包含该项
                            clusterIndex = index; // 更新聚簇索引
                        }

                        index--; // 因为移除了当前索引的项，索引回退以便下次循环检查新的当前位置
                    }
                    else if (clusterIndex > -1) // 如果当前不匹配且存在未提交的聚簇
                    {
                        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, cluster, clusterIndex)); // 触发移除通知，提交聚簇
                        clusterIndex = -1; // 重置聚簇索引
                        cluster = null; // 清空聚簇
                    }
                }

                if (clusterIndex > -1) // 循环结束后如果仍有未提交的聚簇
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, cluster, clusterIndex)); // 提交该聚簇的移除通知
            }

            if (removedCount > 0) // 如果有项被移除
                OnEssentialPropertiesChanged(); // 触发 Count 与索引器属性变更通知

            return removedCount; // 返回被移除的项数
        }

        /// <summary> // XML 注释开始：RemoveRange（索引、计数）说明（保留英文原文结构）
        /// Removes a range of elements from the <see cref="ObservableCollection{T}"/>>. // 英文注释：从集合中移除一段元素
        /// </summary> // XML 注释结束
        /// <param name="index">The zero-based starting index of the range of elements to remove.</param> // 参数注释：起始索引
        /// <param name="count">The number of elements to remove.</param> // 参数注释：要移除的元素数量
        /// <exception cref="ArgumentOutOfRangeException">The specified range is exceeding the collection.</exception> // 异常注释：范围越界时抛出
        public void RemoveRange(int index, int count) // 公共方法 RemoveRange（索引、计数）：移除一段元素
        {
            if (index < 0) // 检查 index 是否小于 0
                throw new ArgumentOutOfRangeException(nameof(index)); // 抛出异常
            if (count < 0) // 检查 count 是否小于 0
                throw new ArgumentOutOfRangeException(nameof(count)); // 抛出异常
            if (index + count > Count) // 检查范围是否超出集合
                throw new ArgumentOutOfRangeException(nameof(index)); // 抛出异常

            if (count == 0) // 如果要移除的数量为 0
                return; // 直接返回

            if (count == 1) // 如果只移除一个元素
            {
                RemoveItem(index); // 调用基类的 RemoveItem 移除单个元素（触发相应事件）
                return; // 返回
            }

            //Items will always be List<T>, see constructors // 注释：Items 总是 List<T>（参见构造函数）
            var items = (List<T>)Items; // 将 Items 强制转换为 List<T>
            List<T> removedItems = items.GetRange(index, count); // 获取将被移除的项的副本列表

            CheckReentrancy(); // 检查重入保护

            items.RemoveRange(index, count); // 从底层列表中移除指定范围

            OnEssentialPropertiesChanged(); // 触发 Count 与索引器属性变更通知

            if (Count == 0) // 如果集合现在为空
                OnCollectionReset(); // 触发集合重置通知
            else
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItems, index)); // 否则触发移除通知，包含被移除的项与起始索引
        }

        /// <summary>  // XML 注释开始：ReplaceRange 方法说明（保留英文原文结构）
        /// Clears the current collection and replaces it with the specified collection, // 英文注释：清空当前集合并用指定集合替换
        /// using <see cref="Comparer"/>. // 英文注释：使用 Comparer 进行比较
        /// </summary>             // XML 注释结束
        /// <param name="collection">The items to fill the collection with, after clearing it.</param> // 参数注释：替换用的集合
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception> // 异常注释：collection 为 null 时抛出
        public void ReplaceRange(IEnumerable<T> collection) // 公共方法 ReplaceRange：用指定集合替换整个集合
        {
            ReplaceRange(0, Count, collection); // 调用重载 ReplaceRange 从索引 0 开始替换 Count 个元素（即全部替换）
        }

        /// <summary> // XML 注释开始：ReplaceRange（带索引与计数）说明（保留英文原文结构）
        /// Removes the specified range and inserts the specified collection in its position, leaving equal items in equal positions intact. // 英文注释：移除指定范围并在该位置插入指定集合，保持相等项在相同位置不变
        /// </summary> // XML 注释结束
        /// <param name="index">The index of where to start the replacement.</param> // 参数注释：起始索引
        /// <param name="count">The number of items to be replaced.</param> // 参数注释：要替换的项数
        /// <param name="collection">The collection to insert in that location.</param> // 参数注释：要插入的集合
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is out of range.</exception> // 异常注释：index 越界时抛出
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is out of range.</exception> // 异常注释：count 越界时抛出
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception> // 异常注释：collection 为 null 时抛出
        /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is null.</exception> // 异常注释：comparer 为 null 时抛出（注：此处文档提及 comparer，但方法使用 Comparer 属性）
        public void ReplaceRange(int index, int count, IEnumerable<T> collection) // 公共方法 ReplaceRange（带索引与计数）：在指定位置替换指定数量的元素为 collection
        {
            if (index < 0) // 检查 index 是否小于 0
                throw new ArgumentOutOfRangeException(nameof(index)); // 抛出异常
            if (count < 0) // 检查 count 是否小于 0
                throw new ArgumentOutOfRangeException(nameof(count)); // 抛出异常
            if (index + count > Count) // 检查范围是否超出集合
                throw new ArgumentOutOfRangeException(nameof(index)); // 抛出异常

            if (collection == null) // 检查 collection 是否为 null
                throw new ArgumentNullException(nameof(collection)); // 抛出异常

            if (!AllowDuplicates) // 如果不允许重复项
                collection = // 对 collection 去重
                  collection
                  .Distinct(Comparer)
                  .ToList();

            if (collection is ICollection<T> countable) // 如果 collection 实现 ICollection<T>
            {
                if (countable.Count == 0) // 如果 collection 为空
                {
                    RemoveRange(index, count); // 移除指定范围
                    return; // 返回
                }
            }
            else if (!collection.Any()) // 否则如果 collection 没有元素
            {
                RemoveRange(index, count); // 移除指定范围
                return; // 返回
            }

            if (index + count == 0) // 如果要替换的范围起始为 0 且 count 为 0（即在空集合开头插入）
            {
                InsertRange(0, collection); // 直接插入集合
                return; // 返回
            }

            if (!(collection is IList<T> list)) // 如果 collection 不是 IList<T>
                list = new List<T>(collection); // 将其包装为 List<T>

            using (BlockReentrancy()) // 使用 BlockReentrancy 保护
            using (DeferEvents()) // 使用 DeferEvents 延迟事件收集
            {
                var rangeCount = index + count; // 计算替换范围的结束索引（exclusive）
                var addedCount = list.Count; // 要插入的元素数量

                var changesMade = false; // 标记是否有实际更改发生
                List<T>? // 声明两个聚簇列表用于记录替换前后的项
                  newCluster = null,
                  oldCluster = null;


                int i = index; // 从起始索引开始
                for (; i < rangeCount && i - index < addedCount; i++) // 遍历并比较并行位置，直到到达范围末尾或新列表末尾
                {
                    //parallel position // 注释：并行位置比较
                    T old = this[i], @new = list[i - index]; // 获取旧项与对应的新项
                    if (Comparer.Equals(old, @new)) // 如果旧项与新项相等（使用 Comparer）
                    {
                        OnRangeReplaced(i, newCluster!, oldCluster!); // 提交之前的聚簇替换通知（如果有）
                        continue; // 继续下一个位置
                    }
                    else
                    {
                        Items[i] = @new; // 将旧项替换为新项

                        if (newCluster == null) // 如果当前没有聚簇
                        {
                            Debug.Assert(oldCluster == null); // 调试断言：oldCluster 应为 null
                            newCluster = new List<T> { @new }; // 创建新的新项聚簇
                            oldCluster = new List<T> { old }; // 创建对应的旧项聚簇
                        }
                        else
                        {
                            newCluster.Add(@new); // 否则将新项加入 newCluster
                            oldCluster!.Add(old); // 将旧项加入 oldCluster
                        }

                        changesMade = true; // 标记发生更改
                    }
                }

                OnRangeReplaced(i, newCluster!, oldCluster!); // 在循环结束后提交最后的聚簇替换通知（如果有）

                //exceeding position // 注释：处理超出并行比较范围的情况（新增或移除）
                if (count != addedCount) // 如果原范围长度与新增长度不相等
                {
                    var items = (List<T>)Items; // 将 Items 强制转换为 List<T>
                    if (count > addedCount) // 如果原来要替换的更多（需要移除多余项）
                    {
                        var removedCount = rangeCount - addedCount; // 计算要移除的数量
                        T[] removed = new T[removedCount]; // 创建数组保存被移除的项
                        items.CopyTo(i, removed, 0, removed.Length); // 复制要移除的项到数组
                        items.RemoveRange(i, removedCount); // 从底层列表移除这些项
                        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removed, i)); // 触发移除通知
                    }
                    else // 否则新增更多（需要插入额外项）
                    {
                        var k = i - index; // 计算已处理的新项数量偏移
                        T[] added = new T[addedCount - k]; // 创建数组保存要新增的项
                        for (int j = k; j < addedCount; j++) // 将剩余的新项复制到数组
                        {
                            T @new = list[j]; // 获取新项
                            added[j - k] = @new; // 存入数组
                        }
                        items.InsertRange(i, added); // 在底层列表插入新增项
                        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, added, i)); // 触发添加通知
                    }

                    OnEssentialPropertiesChanged(); // 触发 Count 与索引器属性变更通知
                }
                else if (changesMade) // 如果长度相同但发生了替换更改
                {
                    OnIndexerPropertyChanged(); // 触发索引器属性变更通知（Item[]）
                }
            }
        }

        #endregion Public Methods // 区域结束：Public Methods（保留原名）


        //------------------------------------------------------ // 分隔注释（保留原文结构）
        // // 空行分隔
        //  Protected Methods // 区块注释：受保护方法部分（保留英文原名）
        // // 空行分隔
        //------------------------------------------------------ // 分隔注释（保留原文结构）

        #region Protected Methods // 区域开始：Protected Methods（保留原名）

        /// <summary> // XML 注释开始：ClearItems 说明（保留英文原文结构）
        /// Called by base class Collection&lt;T&gt; when the list is being cleared; // 英文注释：当基类 Collection<T> 清空列表时调用
        /// raises a CollectionChanged event to any listeners. // 英文注释：向监听者触发 CollectionChanged 事件
        /// </summary> // XML 注释结束
        protected override void ClearItems() // 受保护重写方法 ClearItems：在清空集合时调用
        {
            if (Count == 0) // 如果集合已为空
                return; // 直接返回

            CheckReentrancy(); // 检查重入保护
            base.ClearItems(); // 调用基类实现以实际清空 Items
            OnEssentialPropertiesChanged(); // 触发 Count 与索引器属性变更通知
            OnCollectionReset(); // 触发集合重置通知（Reset）
        }

        /// <inheritdoc/> // 文档继承标记（保留原文）
        protected override void InsertItem(int index, T item) // 受保护重写方法 InsertItem：插入单个项
        {
            if (!AllowDuplicates && Items.Contains(item)) // 如果不允许重复且 Items 已包含该项
                return; // 则忽略插入

            base.InsertItem(index, item); // 否则调用基类实现插入项（会触发相应事件）
        }

        /// <inheritdoc/> // 文档继承标记（保留原文）
        protected override void SetItem(int index, T item) // 受保护重写方法 SetItem：设置索引处的项（替换）
        {
            if (AllowDuplicates) // 如果允许重复
            {
                if (Comparer.Equals(this[index], item)) // 如果新旧项相等（使用 Comparer）
                    return; // 则无需替换
            }
            else
              if (Items.Contains(item, Comparer)) // 否则如果不允许重复且 Items 已包含该项（使用 Comparer）
                return; // 则忽略替换

            CheckReentrancy(); // 检查重入保护
            T oldItem = this[index]; // 保存旧项
            base.SetItem(index, item); // 调用基类实现替换项

            OnIndexerPropertyChanged(); // 触发索引器属性变更通知（Item[]）
            OnCollectionChanged(NotifyCollectionChangedAction.Replace, oldItem!, item!, index); // 触发替换通知，包含旧项、新项与索引
        }

        /// <summary> // XML 注释开始：OnCollectionChanged 说明（保留英文原文结构）
        /// Raise CollectionChanged event to any listeners. // 英文注释：向监听者触发 CollectionChanged 事件
        /// Properties/methods modifying this ObservableCollection will raise // 英文注释：修改集合的方法会通过此虚方法触发事件
        /// a collection changed event through this virtual method. // 继续说明
        /// </summary> // XML 注释结束
        /// <remarks> // 备注开始
        /// When overriding this method, either call its base implementation // 英文注释：重写此方法时应调用基类实现或使用 BlockReentrancy
        /// or call <see cref="BlockReentrancy"/> to guard against reentrant collection changes. // 继续说明
        /// </remarks> // 备注结束
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) // 受保护重写方法 OnCollectionChanged：处理集合更改事件触发
        {
            if (_deferredEvents != null) // 如果存在延迟事件集合
            {
                _deferredEvents.Add(e); // 将当前事件加入延迟集合，稍后统一触发
                return; // 返回，不立即触发基类事件
            }
            base.OnCollectionChanged(e); // 否则调用基类实现立即触发事件
        }

        protected virtual IDisposable DeferEvents() => new DeferredEventsCollection(this); // 受保护虚方法 DeferEvents：创建并返回一个 DeferredEventsCollection，用于延迟事件收集

        #endregion Protected Methods // 区域结束：Protected Methods（保留原名）


        //------------------------------------------------------ // 分隔注释（保留原文结构）
        // // 空行分隔
        //  Private Methods // 区块注释：私有方法部分（保留英文原名）
        // // 空行分隔
        //------------------------------------------------------ // 分隔注释（保留原文结构）

        #region Private Methods // 区域开始：Private Methods（保留原名）

        /// <summary> // XML 注释开始：OnEssentialPropertiesChanged 说明（保留英文原文结构）
        /// Helper to raise Count property and the Indexer property. // 英文注释：辅助方法，用于触发 Count 与索引器属性变更
        /// </summary> // XML 注释结束
        void OnEssentialPropertiesChanged() // 私有方法 OnEssentialPropertiesChanged：触发关键属性变更通知
        {
            OnPropertyChanged(EventArgsCache.CountPropertyChanged); // 触发 Count 属性变更通知（使用缓存的 PropertyChangedEventArgs）
            OnIndexerPropertyChanged(); // 触发索引器属性变更通知
        }

        /// <summary> // XML 注释开始：OnIndexerPropertyChanged 说明（保留英文原文结构）
        /// /// Helper to raise a PropertyChanged event for the Indexer property // 英文注释：辅助方法，触发索引器属性的 PropertyChanged 事件
        /// /// </summary> // XML 注释结束
        void OnIndexerPropertyChanged() => // 私有方法 OnIndexerPropertyChanged：单行表达式体
         OnPropertyChanged(EventArgsCache.IndexerPropertyChanged); // 触发索引器（Item[]）的 PropertyChanged 事件（使用缓存的 EventArgs）

        /// <summary> // XML 注释开始：OnCollectionChanged 辅助重载说明（保留英文原文结构）
        /// Helper to raise CollectionChanged event to any listeners // 英文注释：辅助方法，触发集合更改事件
        /// </summary> // XML 注释结束
        void OnCollectionChanged(NotifyCollectionChangedAction action, object oldItem, object newItem, int index) => // 私有方法重载：根据参数构造 NotifyCollectionChangedEventArgs 并调用 OnCollectionChanged
         OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index)); // 调用 OnCollectionChanged 并传入构造好的事件参数（替换动作）

        /// <summary> // XML 注释开始：OnCollectionReset 说明（保留英文原文结构）
        /// Helper to raise CollectionChanged event with action == Reset to any listeners // 英文注释：辅助方法，触发 Reset 类型的集合更改事件
        /// </summary> // XML 注释结束
        void OnCollectionReset() => // 私有方法 OnCollectionReset：单行表达式体
         OnCollectionChanged(EventArgsCache.ResetCollectionChanged); // 触发 Reset 类型的集合更改事件（使用缓存的 EventArgs）

        /// <summary> // XML 注释开始：OnRangeReplaced 说明（保留英文原文结构）
        /// Helper to raise event for clustered action and clear cluster. // 英文注释：辅助方法，用于对聚簇替换操作触发事件并清空聚簇
        /// </summary> // XML 注释结束
        /// <param name="followingItemIndex">The index of the item following the replacement block.</param> // 参数注释：followingItemIndex 为替换块之后的项索引
        /// <param name="newCluster"></param> // 参数注释：newCluster 为新项聚簇
        /// <param name="oldCluster"></param> // 参数注释：oldCluster 为旧项聚簇
        //TODO should have really been a local method inside ReplaceRange(int index, int count, IEnumerable<T> collection, IEqualityComparer<T> comparer), // TODO 注释：作者建议此方法应为局部方法
        //move when supported language version updated. // TODO 注释：当语言版本支持时移动该方法
        void OnRangeReplaced(int followingItemIndex, ICollection<T> newCluster, ICollection<T> oldCluster) // 私有方法 OnRangeReplaced：处理聚簇替换事件触发
        {
            if (oldCluster == null || oldCluster.Count == 0) // 如果旧聚簇为空或计数为 0
            {
                Debug.Assert(newCluster == null || newCluster.Count == 0); // 调试断言：新聚簇也应为空或计数为 0
                return; // 返回，不触发事件
            }

            OnCollectionChanged( // 触发替换事件，构造新的列表副本以传递给事件监听者
              new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Replace,
                new List<T>(newCluster),
                new List<T>(oldCluster),
                followingItemIndex - oldCluster.Count));

            oldCluster.Clear(); // 清空旧聚簇集合
            newCluster.Clear(); // 清空新聚簇集合
        }

        #endregion Private Methods // 区域结束：Private Methods（保留原名）

        //------------------------------------------------------ // 分隔注释（保留原文结构）
        // // 空行分隔
        //  Private Types // 区块注释：私有类型部分（保留英文原名）
        // // 空行分隔
        //------------------------------------------------------ // 分隔注释（保留原文结构）

        #region Private Types // 区域开始：Private Types（保留原名）
        sealed class DeferredEventsCollection : List<NotifyCollectionChangedEventArgs>, IDisposable // 定义封闭类 DeferredEventsCollection，继承 List<NotifyCollectionChangedEventArgs> 并实现 IDisposable（用于延迟事件收集）
        {
            readonly RangeObservableCollection<T> _collection; // 只读字段：引用外部集合实例（_collection）
            public DeferredEventsCollection(RangeObservableCollection<T> collection) // 构造函数：接受外部集合实例
            {
                Debug.Assert(collection != null); // 调试断言：collection 不应为 null
                Debug.Assert(collection._deferredEvents == null); // 调试断言：外部集合当前没有延迟事件集合
                _collection = collection; // 保存引用
                _collection._deferredEvents = this; // 将外部集合的 _deferredEvents 指向当前实例，表示开始延迟事件收集
            }

            public void Dispose() // IDisposable.Dispose 实现：在 using 结束时调用以提交延迟事件
            {
                _collection._deferredEvents = null; // 清空外部集合的延迟事件引用
                foreach (var args in this) // 遍历当前延迟事件集合中的每个事件参数
                    _collection.OnCollectionChanged(args); // 逐个调用外部集合的 OnCollectionChanged 以触发实际事件
            }
        }

        #endregion Private Types // 区域结束：Private Types（保留原名）

    } // RangeObservableCollection<T> 类结束

    /// <remarks> // XML 注释开始：EventArgsCache 说明（保留英文原文结构）
    /// To be kept outside <see cref="ObservableCollection{T}"/>, since otherwise, a new instance will be created for each generic type used. // 英文注释：将缓存放在 ObservableCollection<T> 外部以避免为每个泛型类型创建新实例
    /// </remarks> // XML 注释结束
    internal static class EventArgsCache // 定义内部静态类 EventArgsCache，用于缓存常用的事件参数实例（避免重复分配）
    {
        internal static readonly PropertyChangedEventArgs CountPropertyChanged = new PropertyChangedEventArgs("Count"); // 缓存 Count 属性变更的 PropertyChangedEventArgs 实例
        internal static readonly PropertyChangedEventArgs IndexerPropertyChanged = new PropertyChangedEventArgs("Item[]"); // 缓存索引器（Item[]）属性变更的 PropertyChangedEventArgs 实例
        internal static readonly NotifyCollectionChangedEventArgs ResetCollectionChanged = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset); // 缓存 Reset 类型的 NotifyCollectionChangedEventArgs 实例
    } // EventArgsCache 类结束
} // 命名空间结束
