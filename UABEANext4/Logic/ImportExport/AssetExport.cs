using AssetsTools.NET;
// 引用 AssetsTools.NET 库，用于读取和操作 Unity 资产文件（原名：AssetsTools.NET）

using Newtonsoft.Json.Linq;
// 引用 Newtonsoft.Json 的 JSON 类型（JToken、JObject 等），用于 JSON 导出（原名：Newtonsoft.Json.Linq）

using System;
// 引用基础系统命名空间，提供常用类型与异常等（原名：System）

using System.Collections.Generic;
// 引用泛型集合命名空间，提供 List<T> 等集合类型（原名：System.Collections.Generic）

using System.IO;
// 引用文件与流操作命名空间，提供 Stream、StreamWriter、File 等（原名：System.IO）

using static System.FormattableString;
// 使用静态导入 FormattableString 的成员（如 Invariant），便于格式化字符串（原名：System.FormattableString）

namespace UABEANext4.Logic.ImportExport;
// 定义命名空间 UABEANext4.Logic.ImportExport，用于组织导入/导出相关逻辑（原名：UABEANext4.Logic.ImportExport）

public class AssetExport
// 定义公共类 AssetExport（原名：AssetExport），用于将资产导出为原始字节、文本或 JSON
{
    // 类体开始（AssetExport）

    private readonly Stream _stream;
    // 私有只读字段 _stream（Stream）：保存写入目标流（原名：_stream）

    private readonly StreamWriter _streamWriter;
    // 私有只读字段 _streamWriter（StreamWriter）：用于向 _stream 写入文本（原名：_streamWriter）

    public AssetExport(Stream writeStream)
    // 构造函数 AssetExport(Stream writeStream)：接收一个写入流（原名：AssetExport）
    {
        // 构造函数体开始

        _stream = writeStream;
        // 将传入的写入流赋值给字段 _stream（原名：writeStream / _stream）

        _streamWriter = new StreamWriter(_stream);
        // 使用 _stream 创建 StreamWriter 并赋值给 _streamWriter（原名：StreamWriter）
    }
    // 构造函数体结束

    public void DumpRawAsset(AssetsFileReader reader, long position, uint size)
    // 公共方法 DumpRawAsset：从 reader 的指定位置读取原始字节并写入目标流（原名：DumpRawAsset）
    {
        // 方法体开始

        var assetFs = reader.BaseStream;
        // 获取 reader 的基础流（BaseStream），用于直接读取字节（原名：assetFs / reader.BaseStream）

        assetFs.Position = position;
        // 将基础流的位置设置为传入的 position（原名：Position / position）

        var buf = new byte[4096];
        // 分配一个 4096 字节的缓冲区用于分块读取（原名：buf）

        var bytesLeft = (int)size;
        // 将要读取的总字节数转换为 int 并赋给 bytesLeft（原名：bytesLeft / size）

        while (bytesLeft > 0)
        // 循环直到读取完所有字节（原名：while）
        {
            // 循环体开始

            var readSize = assetFs.Read(buf, 0, Math.Min(bytesLeft, buf.Length));
            // 从 assetFs 读取最多 buf.Length 或剩余字节数的数据，返回实际读取字节数（原名：readSize / Read）

            _stream.Write(buf, 0, readSize);
            // 将读取到的字节写入目标流 _stream（原名：Write）

            bytesLeft -= readSize;
            // 减少剩余字节计数（原名：bytesLeft）
        }
        // 循环结束
    }
    // 方法体结束

    public void DumpTextAsset(AssetTypeValueField baseField)
    // 公共方法 DumpTextAsset：将结构化的资产字段以可读文本格式导出到目标流（原名：DumpTextAsset）
    {
        // 方法体开始

        RecurseTextDump(baseField, 0);
        // 调用递归方法 RecurseTextDump 从根字段开始导出文本（原名：RecurseTextDump，depth 初始为 0）

        _streamWriter.Flush();
        // 刷新 _streamWriter，确保所有文本写入底层流（原名：Flush）
    }
    // 方法体结束

    private void RecurseTextDump(AssetTypeValueField field, int depth)
    // 私有递归方法 RecurseTextDump：根据字段模板递归生成文本表示（原名：RecurseTextDump）
    {
        // 方法体开始

        var template = field.TemplateField;
        // 获取字段对应的模板（TemplateField），包含类型、名称、对齐等信息（原名：template）

        var align = template.IsAligned ? "1" : "0";
        // 根据模板的 IsAligned 决定对齐标志（"1" 或 "0"）（原名：align / IsAligned）

        var typeName = template.Type;
        // 获取模板的类型名（原名：typeName / template.Type）

        var fieldName = template.Name;
        // 获取模板的字段名（原名：fieldName / template.Name）

        var isArray = template.IsArray;
        // 判断模板是否表示数组（原名：isArray / template.IsArray）

        // string's field isn't aligned but its array is
        // 注释：字符串字段本身不对齐，但字符串数组的元素需要对齐（保留英文原注释）

        if (template.ValueType == AssetValueType.String)
            align = "1";
        // 如果字段类型是字符串，则强制将 align 设为 "1"（原名：AssetValueType.String）

        if (isArray)
        {
            // 如果当前字段是数组类型

            var sizeTemplate = template.Children[0];
            // 数组模板的第一个子项通常是表示大小的模板（原名：sizeTemplate）

            var sizeAlign = sizeTemplate.IsAligned ? "1" : "0";
            // 获取大小字段的对齐标志（原名：sizeAlign）

            var sizeTypeName = sizeTemplate.Type;
            // 获取大小字段的类型名（原名：sizeTypeName）

            var sizeFieldName = sizeTemplate.Name;
            // 获取大小字段的名称（原名：sizeFieldName）

            if (template.ValueType != AssetValueType.ByteArray)
            {
                // 如果数组元素不是字节数组（ByteArray），按元素递归导出

                var size = field.AsArray.size;
                // 获取数组的实际元素数量（原名：size / field.AsArray.size）

                _streamWriter.WriteLine(Invariant($"{new string(' ', depth)}{align} {typeName} {fieldName} ({size} items)"));
                // 写入数组头行，包含缩进、对齐标志、类型名、字段名和元素数量（使用 Invariant 格式化）

                _streamWriter.WriteLine(Invariant($"{new string(' ', depth + 1)}{sizeAlign} {sizeTypeName} {sizeFieldName} = {size}"));
                // 写入数组大小字段行（缩进更深一层）

                for (int i = 0; i < field.Children.Count; i++)
                {
                    _streamWriter.WriteLine(Invariant($"{new string(' ', depth + 1)}[{i}]"));
                    RecurseTextDump(field.Children[i], depth + 2);
                }
                // 遍历数组元素，写入索引行并递归导出每个元素
            }
            else
            {
                // 如果数组元素是字节数组（ByteArray），逐字节输出

                var data = field.AsByteArray;
                // 获取字节数组数据（原名：data）

                var size = data.Length;
                // 获取字节数组长度（原名：size）

                _streamWriter.WriteLine(Invariant($"{new string(' ', depth)}{align} {typeName} {fieldName} ({size} items)"));
                // 写入字节数组头行

                _streamWriter.WriteLine(Invariant($"{new string(' ', depth + 1)}{sizeAlign} {sizeTypeName} {sizeFieldName} = {size}"));
                // 写入字节数组大小字段行

                for (int i = 0; i < size; i++)
                {
                    _streamWriter.WriteLine(Invariant($"{new string(' ', depth + 1)}[{i}]"));
                    _streamWriter.WriteLine(Invariant($"{new string(' ', depth + 2)}0 UInt8 data = {data[i]}"));
                }
                // 逐字节写入每个元素的索引行和数据行（以 UInt8 格式显示）
            }
        }
        else
        {
            // 非数组字段的处理

            var value = "";
            // 初始化 value 字符串用于附加到字段行（原名：value）

            if (field.Value != null)
            {
                // 如果字段有值（是值字段而非复合对象）

                AssetValueType evt = field.Value.ValueType;
                // 获取字段值的具体类型（原名：evt / ValueType）

                if (evt == AssetValueType.String)
                {
                    var fixedStr = TextDumpEscapeString(field.AsString);
                    value = $" = \"{fixedStr}\"";
                }
                // 如果是字符串，进行转义并将其包裹在引号中作为 value

                else if (1 <= (int)evt && (int)evt <= 12)
                {
                    value = Invariant($" = {field.AsString}");
                }
                // 对于常见的数值类型，直接使用 field.AsString 作为 value（按 typetree 的约定）
            }
            _streamWriter.WriteLine(Invariant($"{new string(' ', depth)}{align} {typeName} {fieldName}{value}"));
            // 写入当前字段行，包含缩进、对齐标志、类型名、字段名和可选的值

            if (field.Value != null && field.Value.ValueType == AssetValueType.ManagedReferencesRegistry)
            {
                TextDumpManagedReferencesRegistry(field, depth);
            }
            else
            {
                foreach (var child in field)
                {
                    RecurseTextDump(child, depth + 1);
                }
            }
            // 如果字段是 ManagedReferencesRegistry，调用专门处理函数；否则递归处理子字段
        }
    }
    // RecurseTextDump 方法结束

    private void TextDumpManagedReferencesRegistry(AssetTypeValueField field, int depth)
    // 私有方法 TextDumpManagedReferencesRegistry：将 ManagedReferencesRegistry 类型以文本形式导出（原名：TextDumpManagedReferencesRegistry）
    {
        // 方法体开始

        var registry = field.Value.AsManagedReferencesRegistry;
        // 获取受管引用注册表对象（原名：registry）

        if (registry.version == 1)
        {
            // 处理版本 1 的格式（需要在末尾添加终止符）

            // we need to include this since text dumps are
            // essentially pretty raw dumps and need to include
            // that info so we know when to stop the list
            // 注释：说明为什么要添加终止符（保留英文原注释）

            var referencesWithTerm = new List<AssetTypeReferencedObject>(registry.references)
            {
                new AssetTypeReferencedObject()
                {
                    rid = 0,
                    type = AssetTypeReference.TERMINUS,
                    data = AssetTypeValueField.DUMMY_FIELD
                }
            };
            // 复制引用列表并在末尾添加一个终止符对象（TERMINUS），以便文本导出能识别结束

            _streamWriter.WriteLine(Invariant($"{new string(' ', depth + 1)}0 int version = {registry.version}"));
            // 写入版本号行

            for (int i = 0; i < referencesWithTerm.Count; i++)
            {
                var refObj = referencesWithTerm[i];
                var typeRef = refObj.type;
                _streamWriter.WriteLine(Invariant($"{new string(' ', depth + 1)}0 ReferencedObject {i:d8}"));
                _streamWriter.WriteLine($"{new string(' ', depth + 2)}0 ReferencedManagedType type");
                _streamWriter.WriteLine($"{new string(' ', depth + 3)}1 string class = \"{TextDumpEscapeString(typeRef.ClassName)}\"");
                _streamWriter.WriteLine($"{new string(' ', depth + 3)}1 string ns = \"{TextDumpEscapeString(typeRef.Namespace)}\"");
                _streamWriter.WriteLine($"{new string(' ', depth + 3)}1 string asm = \"{TextDumpEscapeString(typeRef.AsmName)}\"");
                _streamWriter.WriteLine($"{new string(' ', depth + 2)}0 ReferencedObjectData data");

                foreach (var child in refObj.data.Children)
                {
                    RecurseTextDump(child, depth + 3);
                }
            }
            // 遍历每个引用对象，写入其类型信息与数据，并递归导出数据字段
        }
        else if (registry.version == 2)
        {
            // 处理版本 2 的格式（包含 rid 与引用数组）

            _streamWriter.WriteLine(Invariant($"{new string(' ', depth + 1)}0 int version = {registry.version}"));
            _streamWriter.WriteLine($"{new string(' ', depth + 1)}0 vector RefIds");
            _streamWriter.WriteLine($"{new string(' ', depth + 2)}1 Array Array");
            _streamWriter.WriteLine(Invariant($"{new string(' ', depth + 3)}0 int size = {registry.references.Count}"));
            for (int i = 0; i < registry.references.Count; i++)
            {
                AssetTypeReferencedObject refObj = registry.references[i];
                AssetTypeReference typeRef = refObj.type;
                _streamWriter.WriteLine($"{new string(' ', depth + 3)}0 ReferencedObject data");
                _streamWriter.WriteLine(Invariant($"{new string(' ', depth + 4)}0 SInt64 rid = {refObj.rid}"));
                _streamWriter.WriteLine($"{new string(' ', depth + 4)}0 ReferencedManagedType type");
                _streamWriter.WriteLine($"{new string(' ', depth + 5)}1 string class = \"{TextDumpEscapeString(typeRef.ClassName)}\"");
                _streamWriter.WriteLine($"{new string(' ', depth + 5)}1 string ns = \"{TextDumpEscapeString(typeRef.Namespace)}\"");
                _streamWriter.WriteLine($"{new string(' ', depth + 5)}1 string asm = \"{TextDumpEscapeString(typeRef.AsmName)}\"");
                _streamWriter.WriteLine($"{new string(' ', depth + 4)}0 ReferencedObjectData data");

                foreach (AssetTypeValueField child in refObj.data.Children)
                {
                    RecurseTextDump(child, depth + 5);
                }
            }
            // 写入版本 2 的结构：版本号、RefIds 向量、每个引用的 rid、类型与数据，并递归导出数据字段
        }
        else
        {
            throw new NotSupportedException($"Registry version {registry.version} not supported.");
        }
        // 如果版本既不是 1 也不是 2，则抛出不支持异常
    }
    // TextDumpManagedReferencesRegistry 方法结束

    public void DumpJsonAsset(AssetTypeValueField baseField)
    // 公共方法 DumpJsonAsset：将资产字段导出为 JSON 并写入目标流（原名：DumpJsonAsset）
    {
        // 方法体开始

        var jBaseField = RecurseJsonDump(baseField, false);
        // 调用递归方法 RecurseJsonDump 将字段转换为 JToken（原名：jBaseField）

        _streamWriter.Write(jBaseField.ToString());
        // 将生成的 JSON 字符串写入流（原名：ToString / Write）

        _streamWriter.Flush();
        // 刷新写入器以确保数据写入底层流（原名：Flush）
    }
    // 方法体结束

    private JToken RecurseJsonDump(AssetTypeValueField field, bool uabeFlavor)
    // 私有递归方法 RecurseJsonDump：将字段转换为 JToken（JSON 表示），uabeFlavor 用于控制特定导出风格（原名：RecurseJsonDump）
    {
        // 方法体开始

        var template = field.TemplateField;
        // 获取字段模板（原名：template）

        var isArray = template.IsArray;
        // 判断模板是否为数组（原名：isArray）

        if (isArray)
        {
            var jArray = new JArray();
            // 创建一个 JArray 用于存放数组元素（原名：jArray）

            if (template.ValueType != AssetValueType.ByteArray)
            {
                for (int i = 0; i < field.Children.Count; i++)
                {
                    jArray.Add(RecurseJsonDump(field.Children[i], uabeFlavor));
                }
            }
            else
            {
                var byteArrayData = field.AsByteArray;
                for (int i = 0; i < byteArrayData.Length; i++)
                {
                    jArray.Add(byteArrayData[i]);
                }
            }

            return jArray;
            // 如果是数组，构建并返回 JArray（元素为递归结果或字节值）
        }
        else
        {
            if (field.Value != null)
            {
                var valueType = field.Value.ValueType;

                if (field.Value.ValueType != AssetValueType.ManagedReferencesRegistry)
                {
                    object value = valueType switch
                    {
                        AssetValueType.Bool => field.AsBool,
                        AssetValueType.Int8 or
                        AssetValueType.Int16 or
                        AssetValueType.Int32 => field.AsInt,
                        AssetValueType.Int64 => field.AsLong,
                        AssetValueType.UInt8 or
                        AssetValueType.UInt16 or
                        AssetValueType.UInt32 => field.AsUInt,
                        AssetValueType.UInt64 => field.AsULong,
                        AssetValueType.String => field.AsString,
                        AssetValueType.Float => field.AsFloat,
                        AssetValueType.Double => field.AsDouble,
                        _ => "invalid value"
                    };

                    return (JValue)JToken.FromObject(value);
                }
                else
                {
                    return JsonDumpManagedReferencesRegistry(field, uabeFlavor);
                }
            }
            else
            {
                var jObject = new JObject();
                foreach (AssetTypeValueField child in field)
                {
                    jObject.Add(child.FieldName, RecurseJsonDump(child, uabeFlavor));
                }

                return jObject;
            }
        }
    }
    // RecurseJsonDump 方法结束

    private JObject JsonDumpManagedReferencesRegistry(AssetTypeValueField field, bool uabeFlavor = false)
    // 私有方法 JsonDumpManagedReferencesRegistry：将 ManagedReferencesRegistry 转换为 JObject（原名：JsonDumpManagedReferencesRegistry）
    {
        // 方法体开始

        var registry = field.Value.AsManagedReferencesRegistry;
        // 获取受管引用注册表对象（原名：registry）

        if (registry.version >= 1 || registry.version <= 2)
        {
            var jArrayRefs = new JArray();
            foreach (var refObj in registry.references)
            {
                var typeRef = refObj.type;

                var jObjManagedType = new JObject
                {
                    { "class", typeRef.ClassName },
                    { "ns", typeRef.Namespace },
                    { "asm", typeRef.AsmName }
                };

                var jObjData = new JObject();
                foreach (var child in refObj.data)
                {
                    jObjData.Add(child.FieldName, RecurseJsonDump(child, uabeFlavor));
                }

                JObject jObjRefObject;
                if (registry.version == 1)
                {
                    jObjRefObject = new JObject
                    {
                        { "type", jObjManagedType },
                        { "data", jObjData }
                    };
                }
                else
                {
                    jObjRefObject = new JObject
                    {
                        { "rid", refObj.rid },
                        { "type", jObjManagedType },
                        { "data", jObjData }
                    };
                }

                jArrayRefs.Add(jObjRefObject);
            }

            var jObjReferences = new JObject
            {
                { "version", registry.version },
                { "RefIds", jArrayRefs }
            };

            return jObjReferences;
        }
        else
        {
            throw new NotSupportedException($"Registry version {registry.version} not supported.");
        }
    }
    // JsonDumpManagedReferencesRegistry 方法结束

    // only replace \ with \\ but not " with \"
    // you just have to find the last "
    // 注释：说明 TextDumpEscapeString 的行为（只替换反斜杠与换行回车，不转义双引号）

    private static string TextDumpEscapeString(string str)
    // 私有静态方法 TextDumpEscapeString：对文本导出中的字符串做简单转义（原名：TextDumpEscapeString）
    {
        // 方法体开始

        return str
            .Replace("\\", "\\\\")
            .Replace("\r", "\\r")
            .Replace("\n", "\\n");
        // 将反斜杠替换为双反斜杠，并将回车与换行替换为 \r 与 \n（保留英文原名：Replace）
    }
    // 方法体结束

}
// 类体结束（AssetExport）
