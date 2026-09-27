using System; // 引用基础系统命名空间，提供 Math 等基础功能

using System.Collections.Generic; // 引用泛型集合命名空间，提供 List<T> 等集合类型

using System.IO; // 引用文件与路径操作命名空间，提供 Directory 等 IO 功能

namespace UABEANext4.Util; // 定义命名空间 UABEANext4.Util，用于组织工具类

public static class FileUtils // 定义公共静态类 FileUtils，封装与文件/字节大小相关的实用方法
{
    private static readonly string[] BYTE_SIZE_SUFFIXES = new string[] { "字节 (B)", "千字节 (KB)", "兆字节 (MB)", "吉字节 (GB)", "太字节 (TB)", "拍字节 (PB)", "艾字节 (EB)" }; // 定义字节单位后缀数组，中文显示并在括号中保留英文原名

    public static string GetFormattedByteSize(long size) // 公共静态方法：将字节数格式化为带单位的字符串（例如 1.23MB）
    {
        int log = (int)Math.Log(size, 1024); // 计算以 1024 为底的对数，确定使用哪个单位（例如 0->B,1->KB,2->MB 等）

        double div = log == 0 ? 1 : Math.Pow(1024, log); // 计算除数：如果 log 为 0 则除数为 1，否则为 1024 的 log 次方

        double num = size / div; // 将原始字节数除以除数得到以目标单位表示的数值

        return $"{num:f2}{BYTE_SIZE_SUFFIXES[log]}"; // 返回格式化字符串，保留两位小数并附加对应单位后缀
    }

    public static List<string> GetFilesInDirectory(string path, List<string> extensions) // 公共静态方法：在指定目录中查找具有给定扩展名的文件并返回完整路径列表
    {
        List<string> files = new List<string>(); // 创建用于收集文件路径的列表

        foreach (string extension in extensions) // 遍历传入的扩展名列表（例如 "png","json"）
        {
            files.AddRange(Directory.GetFiles(path, "*." + extension)); // 使用 Directory.GetFiles 按模式 "*.<ext>" 获取匹配文件并加入结果列表
        }

        return files; // 返回收集到的文件路径列表
    }
}
