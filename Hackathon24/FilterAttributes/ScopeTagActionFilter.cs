using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.OData.Query.Wrapper;
using Microsoft.AspNetCore.OData.Results;
using Hackathon24.Models;
using System.Collections.Generic;
using System.Reflection;
using Hackathon24.Helpers;
using System.Collections;

namespace Hackathon24.CustomFilters
{
    public class ScopeTagActionFilter<T> : ActionFilterAttribute where T : class
    {
        private readonly PropertyInfo? _scopeTagsPropertyInfo;

        public ScopeTagActionFilter(string scopeTagsPropertyName)
        {
            _scopeTagsPropertyInfo = typeof(T).GetProperty(scopeTagsPropertyName);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Code to execute before the action executes
            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            //  If _scopeTagsPropertyInfo is null, return
            if (_scopeTagsPropertyInfo == null)
            {
                base.OnActionExecuted(context);
                return;
            }

            // Code to execute after the action executes
            //  Filter Scope Tags for each entity
            if (context.Result is Microsoft.AspNetCore.Mvc.ObjectResult objectResult)
            {
                if (objectResult.Value != null && objectResult.Value.GetType() == typeof(T))
                {
                    var entityWithScopeTags = objectResult.Value;
                    var existingTags = _scopeTagsPropertyInfo.GetValue(entityWithScopeTags) as List<string>;
                    if (existingTags != null)
                    {
                        var filteredTags = ScopeTagHelpers.GetIntersectingScopeTags(existingTags, context.HttpContext);
                        _scopeTagsPropertyInfo.SetValue(entityWithScopeTags, filteredTags);
                    }
                }
                else if (objectResult.Value != null && IsEnumerableOfType(objectResult.Value, typeof(T)))
                {
                    IEnumerable enumerable = (IEnumerable)objectResult.Value;
                    foreach (var item in enumerable)
                    {
                        var existingTags = _scopeTagsPropertyInfo.GetValue(item) as List<string>;
                        if (existingTags != null)
                        {
                            var filteredTags = ScopeTagHelpers.GetIntersectingScopeTags(existingTags, context.HttpContext);
                            _scopeTagsPropertyInfo.SetValue(item, filteredTags);
                        }
                    }
                }
            }

            base.OnActionExecuted(context);
        }

        private bool IsEnumerableOfType(object obj, Type type)
        {
            if (obj == null) return false;

            var objType = obj.GetType();
            var interfaces = objType.GetInterfaces();

            foreach (var iface in interfaces)
            {
                if (iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    var genericArgument = iface.GetGenericArguments().FirstOrDefault();
                    if (genericArgument == type)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
