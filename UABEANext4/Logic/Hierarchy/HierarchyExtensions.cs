using System.Collections.Generic;
using UABEANext4.Logic.Hierarchy;

namespace UABEANext4.Logic.Hierarchy;

public static class HierarchyExtensions
{
    public static IEnumerable<HierarchyItem> Flatten(this HierarchyItem item)
    {
        yield return item;
        foreach (var child in item.Children)
        {
            foreach (var descendant in child.Flatten())
            {
                yield return descendant;
            }
        }
    }
}
