using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Application.MediatRBehaviors
{
    internal static class BehaviorHelperExtensions
    {
        internal static string GetGenericTypeName(this object @object) {
            var typeName = string.Empty;
            var type = @object.GetType();

            if (type.IsGenericType) {
                var genericTypes = string.Join(",", type.GetGenericArguments().Select(t => t.Name).ToArray());
                typeName = $"{type.Name.Remove(type.Name.IndexOf('`'))}<{genericTypes}>";
            } else {
                typeName = type.Name;
            }

            return typeName;
        }
    }
}
