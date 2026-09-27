using AssetsTools.NET;
// 引用 AssetsTools.NET 库，用于读取和操作 Unity 资产文件（保留英文原名：AssetsTools.NET）

using Newtonsoft.Json.Linq;
// 引用 Newtonsoft.Json 的 JToken/JObject 等类型，用于解析 JSON（保留英文原名：Newtonsoft.Json.Linq）

using System;
// 引用基础系统命名空间，提供常用类型与异常等（保留英文原名：System）

using System.Collections.Generic;
// 引用泛型集合命名空间，提供 List<T>、Stack<T> 等集合类型（保留英文原名：System.Collections.Generic）

using System.Globalization;
// 引用文化/格式化相关命名空间，用于数字/日期解析（保留英文原名：System.Globalization）

using System.IO;
// 引用文件与路径操作命名空间，提供 Stream、File、Path 等 IO 功能（保留英文原名：System.IO）

using System.Text;
// 引用文本处理命名空间，提供 StringBuilder 等（保留英文原名：System.Text）

namespace UABEANext4.Logic.ImportExport;
// 定义命名空间 UABEANext4.Logic.ImportExport（保留英文原名），用于组织导入/导出相关逻辑类型

public class AssetImport
// 定义公共类 AssetImport（保留英文原名：AssetImport），用于从不同文本/JSON/原始流导入并构建 Unity 资产二进制表示
{
    // 类体开始

    private readonly Stream _stream;
    // 私有只读字段 _stream（Stream）：保存传入的读取流（保留英文原名：_stream）

    private readonly StreamReader _streamReader;
    // 私有只读字段 _streamReader（StreamReader）：用于按行或按文本读取 _stream（保留英文原名：_streamReader）

    private readonly RefTypeManager _refMan;
    // 私有只读字段 _refMan（RefTypeManager）：用于管理受管引用类型模板（保留英文原名：_refMan）

    public AssetImport(Stream readStream, RefTypeManager refMan)
    // 构造函数 AssetImport(Stream readStream, RefTypeManager refMan)：接收要读取的流与引用类型管理器（保留英文原名：AssetImport）
    {
        // 构造函数体开始

        _stream = readStream;
        // 将传入的 readStream 赋值给字段 _stream（保留英文原名：readStream / _stream）

        _streamReader = new StreamReader(_stream);
        // 使用 _stream 创建 StreamReader 并赋值给 _streamReader，便于按文本读取（保留英文原名：StreamReader）

        _refMan = refMan;
        // 将传入的引用类型管理器 refMan 赋值给字段 _refMan（保留英文原名：refMan / _refMan）
    }
    // 构造函数体结束

    public byte[] ImportRawAsset()
    // 公共方法 ImportRawAsset()：直接将输入流的原始字节复制到内存并返回（保留英文原名：ImportRawAsset）
    {
        // 方法体开始

        using var ms = new MemoryStream();
        // 创建一个 MemoryStream（ms）用于缓存输入流的全部字节（保留英文原名：MemoryStream / ms）

        _stream.CopyTo(ms);
        // 将 _stream 的所有字节复制到 ms（保留英文原名：CopyTo）

        return ms.ToArray();
        // 将 MemoryStream 的内容转换为字节数组并返回（保留英文原名：ToArray）
    }
    // 方法体结束

    public byte[]? ImportTextAsset(out string? exceptionMessage)
    // 公共方法 ImportTextAsset(out string? exceptionMessage)：从文本导入资产（按自定义文本转二进制），返回字节数组或 null，并通过 out 返回异常信息（保留英文原名：ImportTextAsset）
    {
        // 方法体开始

        using var ms = new MemoryStream();
        // 创建 MemoryStream（ms）用于写入生成的二进制数据（保留英文原名：ms）

        var writer = new AssetsFileWriter(ms)
        {
            BigEndian = false
        };
        // 创建 AssetsFileWriter（writer）包装 ms，用于按 Unity 资产格式写入数据，并设置 BigEndian = false（保留英文原名：AssetsFileWriter / BigEndian）

        try
        // 使用 try/catch 捕获解析或写入过程中的异常（保留英文原名：try）
        {
            // try 块开始

            ImportTextAssetLoop(writer);
            // 调用私有方法 ImportTextAssetLoop 将文本逐行解析并写入 writer（保留英文原名：ImportTextAssetLoop）

            exceptionMessage = null;
            // 如果成功则将 exceptionMessage 设为 null（保留英文原名：exceptionMessage）
        }
        // try 块结束
        catch (Exception ex)
        // 捕获任意异常并记录异常信息（保留英文原名：catch / Exception）
        {
            exceptionMessage = ex.ToString();
            // 将异常信息转换为字符串并通过 out 参数返回（保留英文原名：ex）

            return null;
            // 返回 null 表示导入失败（保留英文原名：null）
        }
        return ms.ToArray();
        // 成功时返回 MemoryStream 中写入的字节数组（保留英文原名：ToArray）
    }
    // 方法体结束

    private void ImportTextAssetLoop(AssetsFileWriter writer)
    // 私有方法 ImportTextAssetLoop(AssetsFileWriter writer)：逐行解析自定义文本格式并写入 writer（保留英文原名：ImportTextAssetLoop）
    {
        // 方法体开始

        Stack<bool> alignStack = new Stack<bool>();
        // 创建一个布尔栈 alignStack，用于跟踪嵌套结构是否需要对齐（保留英文原名：Stack<bool> / alignStack）

        while (true)
        // 无限循环，直到读取到流末尾（保留英文原名：while）
        {
            // 循环体开始

            string? line = _streamReader.ReadLine();
            // 从 _streamReader 读取一行文本（可能为 null 表示 EOF），赋给 line（保留英文原名：ReadLine）

            if (line == null)
                return;
            // 如果读取到 EOF（line 为 null）则返回结束方法（保留英文原名：null / return）

            int thisDepth = 0;
            // 初始化 thisDepth，用于计算当前行的缩进深度（保留英文原名：thisDepth）

            while (line[thisDepth] == ' ')
                thisDepth++;
            // 通过计数前导空格计算缩进深度（保留英文原名：line / ' '）

            if (line[thisDepth] == '[') // array index, ignore
                continue;
            // 如果当前非空白字符是 '['（数组索引行），则忽略该行并继续下一行（保留英文原注释）

            if (thisDepth < alignStack.Count)
            {
                while (thisDepth < alignStack.Count)
                {
                    if (alignStack.Pop())
                        writer.Align();
                }
            }
            // 如果当前深度小于对齐栈高度，弹出栈并对需要对齐的层调用 writer.Align（保留英文原名：alignStack / writer.Align）

            bool align = line.Substring(thisDepth, 1) == "1";
            // 从当前深度位置读取一个字符判断是否为 "1"，用以决定该字段是否需要对齐（保留英文原名：align）

            int typeName = thisDepth + 2;
            // 计算类型名在行中的起始索引（保留英文原名：typeName）

            int eqSign = line.IndexOf('=');
            // 查找等号位置，用于分离类型/字段与值（保留英文原名：eqSign / IndexOf）

            string valueStr = line.Substring(eqSign + 1).Trim();
            // 提取等号右侧的值字符串并去除首尾空白（保留英文原名：valueStr / Trim）

            if (eqSign != -1)
            {
                // 如果存在等号（表示这是一个带值的字段）
                string check = line.Substring(typeName);
                // 从 typeName 位置截取剩余字符串用于判断字段类型（保留英文原名：check）

                // sorted by frequency
                // 注释：下面的判断按出现频率排序以提高匹配效率（保留英文原注释）

                if (StartsWithSpace(check, "int"))
                {
                    writer.Write(int.Parse(valueStr, NumberStyles.Integer, CultureInfo.InvariantCulture));
                }
                // 如果字段类型以 "int " 开头，则将 valueStr 解析为 int 并写入 writer（保留英文原名：StartsWithSpace / int.Parse）

                else if (StartsWithSpace(check, "float"))
                {
                    writer.Write(float.Parse(valueStr, NumberStyles.Float, CultureInfo.InvariantCulture));
                }
                // 如果类型为 float，则解析为 float 并写入（保留英文原名：float.Parse）

                else if (StartsWithSpace(check, "bool"))
                {
                    writer.Write(bool.Parse(valueStr));
                }
                // 如果类型为 bool，则解析并写入（保留英文原名：bool.Parse）

                else if (StartsWithSpace(check, "SInt64"))
                {
                    writer.Write(long.Parse(valueStr, NumberStyles.Integer, CultureInfo.InvariantCulture));
                }
                // 如果类型为 SInt64，则解析为 long 并写入（保留英文原名：SInt64 / long.Parse）

                else if (StartsWithSpace(check, "string"))
                {
                    int firstQuote = valueStr.IndexOf('"');
                    int lastQuote = valueStr.LastIndexOf('"');
                    string valueStrFix = valueStr.Substring(firstQuote + 1, lastQuote - firstQuote - 1);
                    valueStrFix = UnescapeDumpString(valueStrFix);
                    writer.WriteCountStringInt32(valueStrFix);
                }
                // 如果类型为 string，提取引号内字符串，反转义后使用 writer.WriteCountStringInt32 写入（保留英文原名：WriteCountStringInt32 / UnescapeDumpString）

                else if (StartsWithSpace(check, "UInt8"))
                {
                    writer.Write(byte.Parse(valueStr, NumberStyles.Integer, CultureInfo.InvariantCulture));
                }
                // UInt8：解析为 byte 并写入（保留英文原名：UInt8）

                else if (StartsWithSpace(check, "unsigned int"))
                {
                    writer.Write(uint.Parse(valueStr, NumberStyles.Integer, CultureInfo.InvariantCulture));
                }
                // unsigned int：解析为 uint 并写入（保留英文原名：unsigned int）

                else if (StartsWithSpace(check, "UInt16"))
                {
                    writer.Write(ushort.Parse(valueStr, NumberStyles.Integer, CultureInfo.InvariantCulture));
                }
                // UInt16：解析为 ushort 并写入（保留英文原名：UInt16）

                else if (StartsWithSpace(check, "SInt8"))
                {
                    writer.Write(sbyte.Parse(valueStr, NumberStyles.Integer, CultureInfo.InvariantCulture));
                }
                // SInt8：解析为 sbyte 并写入（保留英文原名：SInt8）

                else if (StartsWithSpace(check, "SInt16"))
                {
                    writer.Write(short.Parse(valueStr, NumberStyles.Integer, CultureInfo.InvariantCulture));
                }
                // SInt16：解析为 short 并写入（保留英文原名：SInt16）

                else if (StartsWithSpace(check, "UInt64"))
                {
                    writer.Write(ulong.Parse(valueStr, NumberStyles.Integer, CultureInfo.InvariantCulture));
                }
                // UInt64：解析为 ulong 并写入（保留英文原名：UInt64）

                else if (StartsWithSpace(check, "double"))
                {
                    writer.Write(double.Parse(valueStr, NumberStyles.Float, CultureInfo.InvariantCulture));
                }
                // double：解析为 double 并写入（保留英文原名：double）

                else if (StartsWithSpace(check, "char"))
                {
                    writer.Write(sbyte.Parse(valueStr, NumberStyles.Integer, CultureInfo.InvariantCulture));
                }
                // char：解析为 sbyte 并写入（保留英文原名：char）

                else if (StartsWithSpace(check, "FileSize"))
                {
                    writer.Write(ulong.Parse(valueStr, NumberStyles.Integer, CultureInfo.InvariantCulture));
                }
                // FileSize：解析为 ulong 并写入（保留英文原名：FileSize）

                // not seen in the wild? but still part of at
                // I'm not sure where this list is from
                // 注释：下面列出的一些类型可能不常见，但仍然支持（保留英文原注释）

                else if (StartsWithSpace(check, "short"))
                {
                    writer.Write(short.Parse(valueStr, NumberStyles.Integer, CultureInfo.InvariantCulture));
                }
                // short：解析为 short 并写入（保留英文原名：short）

                else if (StartsWithSpace(check, "long"))
                {
                    writer.Write(long.Parse(valueStr, NumberStyles.Integer, CultureInfo.InvariantCulture));
                }
                // long：解析为 long 并写入（保留英文原名：long）

                else if (StartsWithSpace(check, "SInt32"))
                {
                    writer.Write(int.Parse(valueStr, NumberStyles.Integer, CultureInfo.InvariantCulture));
                }
                // SInt32：解析为 int 并写入（保留英文原名：SInt32）

                else if (StartsWithSpace(check, "UInt32"))
                {
                    writer.Write(uint.Parse(valueStr, NumberStyles.Integer, CultureInfo.InvariantCulture));
                }
                // UInt32：解析为 uint 并写入（保留英文原名：UInt32）

                else if (StartsWithSpace(check, "unsigned char"))
                {
                    writer.Write(byte.Parse(valueStr, NumberStyles.Integer, CultureInfo.InvariantCulture));
                }
                // unsigned char：解析为 byte 并写入（保留英文原名：unsigned char）

                else if (StartsWithSpace(check, "unsigned short"))
                {
                    writer.Write(ushort.Parse(valueStr, NumberStyles.Integer, CultureInfo.InvariantCulture));
                }
                // unsigned short：解析为 ushort 并写入（保留英文原名：unsigned short）

                else if (StartsWithSpace(check, "unsigned long long"))
                {
                    writer.Write(ulong.Parse(valueStr, NumberStyles.Integer, CultureInfo.InvariantCulture));
                }
                // unsigned long long：解析为 ulong 并写入（保留英文原名：unsigned long long）

                if (align)
                {
                    writer.Align();
                }
                // 如果该字段需要对齐（align 为 true），则调用 writer.Align() 进行对齐（保留英文原名：Align）
            }
            else
            {
                alignStack.Push(align);
            }
            // 如果当前行没有等号（表示进入一个新的结构层级），将 align 标志压入 alignStack（保留英文原名：alignStack）
        }
    }
    // 方法体结束

    public byte[]? ImportJsonAsset(AssetTypeTemplateField tempField, out string? exceptionMessage)
    // 公共方法 ImportJsonAsset(AssetTypeTemplateField tempField, out string? exceptionMessage)：根据类型模板将 JSON 转换为资产二进制，返回字节数组或 null，并通过 out 返回异常信息（保留英文原名：ImportJsonAsset）
    {
        // 方法体开始

        using var ms = new MemoryStream();
        // 创建 MemoryStream（ms）用于写入生成的二进制数据（保留英文原名：ms）

        var writer = new AssetsFileWriter(ms)
        {
            BigEndian = false
        };
        // 创建 AssetsFileWriter（writer）包装 ms 并设置 BigEndian = false（保留英文原名：writer）

        try
        // 使用 try/catch 捕获解析或写入过程中的异常（保留英文原名：try）
        {
            // try 块开始

            string jsonText = _streamReader.ReadToEnd();
            // 读取输入流的全部文本作为 JSON 字符串（保留英文原名：ReadToEnd / jsonText）

            JToken token = JToken.Parse(jsonText);
            // 使用 Newtonsoft.Json 解析 JSON 文本为 JToken（保留英文原名：JToken.Parse / token）

            RecurseJsonImport(writer, tempField, token);
            // 调用递归方法 RecurseJsonImport 根据模板将 JSON 内容写入 writer（保留英文原名：RecurseJsonImport）

            exceptionMessage = null;
            // 成功时将 exceptionMessage 设为 null（保留英文原名：exceptionMessage）
        }
        // try 块结束
        catch (Exception ex)
        // 捕获异常并返回异常信息（保留英文原名：Exception / ex）
        {
            exceptionMessage = ex.ToString();
            // 将异常信息字符串化并通过 out 返回（保留英文原名：ex）

            return null;
            // 返回 null 表示导入失败（保留英文原名：null）
        }
        return ms.ToArray();
        // 成功时返回 MemoryStream 中写入的字节数组（保留英文原名：ToArray）
    }
    // 方法体结束

    private void RecurseJsonImport(AssetsFileWriter writer, AssetTypeTemplateField tempField, JToken token)
    // 私有方法 RecurseJsonImport：递归地根据类型模板（tempField）将 JSON（token）写入 writer（保留英文原名：RecurseJsonImport）
    {
        // 方法体开始

        bool align = tempField.IsAligned;
        // 读取模板字段的对齐标志（IsAligned），决定是否在写入后调用 Align（保留英文原名：align / IsAligned）

        if (tempField.Children.Count == 1 && tempField.Children[0].IsArray &&
            token.Type == JTokenType.Array)
        {
            RecurseJsonImport(writer, tempField.Children[0], token);
            return;
        }
        // 如果模板表示单一数组且 JSON 也是数组，则直接递归到子模板处理数组元素（保留英文原名：IsArray / JTokenType.Array）

        if (!tempField.HasValue && !tempField.IsArray)
        {
            foreach (AssetTypeTemplateField childTempField in tempField.Children)
            {
                JToken? childToken = token[childTempField.Name];

                if (childToken == null)
                {
                    if (tempField != null)
                    {
                        throw new Exception($"Missing field {childTempField.Name} in JSON. Parent field is {tempField.Type} {tempField.Name}.");
                    }
                    else
                    {
                        throw new Exception($"Missing field {childTempField.Name} in JSON.");
                    }
                }

                RecurseJsonImport(writer, childTempField, childToken);
            }

            if (align)
            {
                writer.Align();
            }
        }
        // 如果模板不是值字段且不是数组，遍历其子字段，从 JSON 中取对应子 token 并递归处理；处理完后根据 align 决定是否对齐（保留英文原名：HasValue / Children）

        else if (tempField.HasValue && tempField.ValueType == AssetValueType.ManagedReferencesRegistry)
        {
            JsonImportManagedReferencesRegistry(writer, tempField, token);
        }
        // 如果模板字段是受管引用注册表（ManagedReferencesRegistry），调用专门处理函数（保留英文原名：ManagedReferencesRegistry）

        else
        {
            switch (tempField.ValueType)
            {
                case AssetValueType.Bool:
                {
                    writer.Write((bool)token);
                    break;
                }
                case AssetValueType.UInt8:
                {
                    writer.Write((byte)token);
                    break;
                }
                case AssetValueType.Int8:
                {
                    writer.Write((sbyte)token);
                    break;
                }
                case AssetValueType.UInt16:
                {
                    writer.Write((ushort)token);
                    break;
                }
                case AssetValueType.Int16:
                {
                    writer.Write((short)token);
                    break;
                }
                case AssetValueType.UInt32:
                {
                    writer.Write((uint)token);
                    break;
                }
                case AssetValueType.Int32:
                {
                    writer.Write((int)token);
                    break;
                }
                case AssetValueType.UInt64:
                {
                    writer.Write((ulong)token);
                    break;
                }
                case AssetValueType.Int64:
                {
                    writer.Write((long)token);
                    break;
                }
                case AssetValueType.Float:
                {
                    writer.Write((float)token);
                    break;
                }
                case AssetValueType.Double:
                {
                    writer.Write((double)token);
                    break;
                }
                case AssetValueType.String:
                {
                    align = true;
                    writer.WriteCountStringInt32((string?)token ?? "");
                    break;
                }
                case AssetValueType.ByteArray:
                {
                    JArray byteArrayJArray = ((JArray?)token) ?? new JArray();
                    byte[] byteArrayData = new byte[byteArrayJArray.Count];
                    for (int i = 0; i < byteArrayJArray.Count; i++)
                    {
                        byteArrayData[i] = (byte)byteArrayJArray[i];
                    }
                    writer.Write(byteArrayData.Length);
                    writer.Write(byteArrayData);
                    break;
                }
            }

            // have to do this because of bug in MonoDeserializer
            if (tempField.IsArray && tempField.ValueType != AssetValueType.ByteArray)
            {
                // children[0] is size field, children[1] is the data field
                AssetTypeTemplateField childTempField = tempField.Children[1];

                JArray? tokenArray = (JArray?)token;

                if (tokenArray == null)
                    throw new Exception($"Field {tempField.Name} was not an array in json.");

                writer.Write(tokenArray.Count);
                foreach (JToken childToken in tokenArray.Children())
                {
                    RecurseJsonImport(writer, childTempField, childToken);
                }
            }

            if (align)
            {
                writer.Align();
            }
        }
    }
    // 方法体结束

    private void JsonImportManagedReferencesRegistry(AssetsFileWriter writer, AssetTypeTemplateField tempField, JToken token)
    // 私有方法 JsonImportManagedReferencesRegistry：专门处理 ManagedReferencesRegistry 类型的 JSON 导入（保留英文原名：JsonImportManagedReferencesRegistry）
    {
        // 方法体开始

        int version = (int)ExpectAndReadField(token, "version", tempField);
        // 从 token 中读取 "version" 字段并转换为 int（使用 ExpectAndReadField 验证存在），赋给 version（保留英文原名：version）

        if (version < 1 || version > 2)
        {
            throw new Exception($"ManagedReferencesRegistry version {version} is invalid.");
        }
        // 验证版本号有效性（仅支持 1 或 2），否则抛出异常（保留英文原名：ManagedReferencesRegistry）

        JArray refIdsArray = (JArray)ExpectAndReadField(token, "RefIds", tempField);
        // 读取 "RefIds" 字段并转换为 JArray（保留英文原名：RefIds / refIdsArray）

        writer.Write(version);
        // 将版本号写入 writer（保留英文原名：writer.Write）

        int childCount = refIdsArray.Count;
        // 获取引用条目数量（保留英文原名：childCount）

        // todo: can we not trust the typetree?
        // 注释：TODO 提示是否可以不信任 typetree（保留英文原注释）

        if (version != 1)
        {
            writer.Write(childCount);
        }
        // 如果版本不是 1，则写入条目数量（版本 1 的格式不包含该字段）（保留英文原名：childCount）

        for (int i = 0; i < childCount; i++)
        {
            JToken refdObjectToken = refIdsArray[i];
            long rid = (long)ExpectAndReadField(refdObjectToken, "rid", tempField);
            if (version == 1)
            {
                if (rid != i)
                {
                    throw new Exception($"Field rid must be consecutive. Expected {i}, found {rid}.");
                }
            }
            else
            {
                writer.Write(rid);
            }

            JToken typeToken = ExpectAndReadField(refdObjectToken, "type", tempField);
            AssetTypeReference typeRef = new AssetTypeReference()
            {
                ClassName = (string?)ExpectAndReadField(typeToken, "class", tempField) ?? string.Empty,
                Namespace = (string?)ExpectAndReadField(typeToken, "ns", tempField) ?? string.Empty,
                AsmName = (string?)ExpectAndReadField(typeToken, "asm", tempField) ?? string.Empty
            };

            JToken dataToken = ExpectAndReadField(refdObjectToken, "data", tempField);

            typeRef.WriteAsset(writer);
            if (typeRef.ClassName == string.Empty && typeRef.Namespace == string.Empty && typeRef.AsmName == string.Empty)
            {
                // this is a null entry which has no data after it
                continue;
            }

            AssetTypeTemplateField? objectTempField = _refMan.GetTemplateField(typeRef);
            if (objectTempField == null)
            {
                throw new Exception($"Failed to get managed reference type. Wanted {typeRef.ClassName}.{typeRef.Namespace}"
                    + $" in {typeRef.AsmName} but got a null result.");
            }

            RecurseJsonImport(writer, objectTempField, dataToken);
        }

        if (version == 1)
        {
            AssetTypeReference.TERMINUS.WriteAsset(writer);
        }
        else
        {
            writer.Align();
        }
    }
    // 方法体结束

    private JToken ExpectAndReadField(JToken token, string name, AssetTypeTemplateField? tempField)
    // 私有方法 ExpectAndReadField：从 token 中读取名为 name 的子字段并在缺失时抛出带上下文的异常（保留英文原名：ExpectAndReadField）
    {
        // 方法体开始

        JToken? versionToken = token[name];
        // 尝试从 token 中获取名为 name 的子 token（保留英文原名：versionToken）

        if (versionToken == null)
        {
            if (tempField == null)
            {
                throw new Exception($"Missing field {name} in JSON.");
            }
            else
            {
                throw new Exception($"Missing field {name} in JSON. Parent field is {tempField.Type} {tempField.Name}.");
            }
        }
        return versionToken;
        // 如果找到则返回该子 token，否则抛出异常（保留英文原名：tempField）
    }
    // 方法体结束

    private static bool StartsWithSpace(string str, string value)
    // 私有静态方法 StartsWithSpace：判断字符串 str 是否以 "value "（value 后跟空格）开头（保留英文原名：StartsWithSpace）
    {
        // 方法体开始

        return str.StartsWith(value + " ");
        // 使用 StartsWith 检查并返回布尔结果（保留英文原名：StartsWith）
    }
    // 方法体结束

    private static string UnescapeDumpString(string str)
    // 私有静态方法 UnescapeDumpString：对导出文本中的转义序列进行反转义（保留英文原名：UnescapeDumpString）
    {
        // 方法体开始

        StringBuilder sb = new StringBuilder(str.Length);
        // 创建 StringBuilder（sb）用于构建反转义后的字符串（保留英文原名：StringBuilder）

        bool escaping = false;
        // 布尔标志 escaping 表示当前是否处于转义状态（保留英文原名：escaping）

        foreach (char c in str)
        {
            if (!escaping && c == '\\')
            {
                escaping = true;
                continue;
            }

            if (escaping)
            {
                if (c == '\\')
                    sb.Append('\\');
                else if (c == 'r')
                    sb.Append('\r');
                else if (c == 'n')
                    sb.Append('\n');
                else
                    sb.Append(c);

                escaping = false;
            }
            else
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
        // 返回反转义后的字符串（保留英文原名：ToString）
    }
    // 方法体结束
}
// 类体结束（AssetImport）
