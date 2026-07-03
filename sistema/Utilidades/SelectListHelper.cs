using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace sistema.Utilidades
{
    public static class SelectListHelper
    {
        public static SelectList Create(IEnumerable items, string dataValueField, string dataTextField)
        {
            var filtered = FilterSourceItems(items, dataValueField, dataTextField);
            return new SelectList(filtered, dataValueField, dataTextField);
        }

        public static IEnumerable<SelectListItem> FilterItems(IEnumerable<SelectListItem> items)
        {
            return (items ?? Enumerable.Empty<SelectListItem>())
                .Where(i => i != null
                            && !string.IsNullOrWhiteSpace(i.Value)
                            && !string.IsNullOrWhiteSpace(i.Text));
        }

        private static IList FilterSourceItems(IEnumerable items, string dataValueField, string dataTextField)
        {
            var result = new ArrayList();
            if (items == null)
            {
                return result;
            }

            foreach (var item in items)
            {
                if (item == null)
                {
                    continue;
                }

                if (item is SelectListItem listItem)
                {
                    if (string.IsNullOrWhiteSpace(listItem.Value) || string.IsNullOrWhiteSpace(listItem.Text))
                    {
                        continue;
                    }

                    result.Add(listItem);
                    continue;
                }

                var value = ReadMember(item, dataValueField);
                var text = ReadMember(item, dataTextField);
                if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(text))
                {
                    continue;
                }

                result.Add(item);
            }

            return result;
        }

        private static string ReadMember(object item, string memberName)
        {
            if (item == null || string.IsNullOrWhiteSpace(memberName))
            {
                return null;
            }

            var type = item.GetType();
            var property = type.GetProperty(memberName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (property != null)
            {
                return property.GetValue(item)?.ToString()?.Trim();
            }

            var field = type.GetField(memberName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            return field?.GetValue(item)?.ToString()?.Trim();
        }
    }
}
