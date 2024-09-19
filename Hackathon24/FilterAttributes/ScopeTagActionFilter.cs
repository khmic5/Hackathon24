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
    public class ScopeTagActionFilter : ActionFilterAttribute
    {
        private readonly Type? _type;
        private readonly PropertyInfo? _scopeTagsPropertyInfo;

        public ScopeTagActionFilter(Type type, string scopeTagsPropertyName)
        {
            _type = type;
            PropertyInfo? propertyInfo = type.GetProperty(scopeTagsPropertyName);
            _scopeTagsPropertyInfo = propertyInfo;
        }   

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Code to execute before the action executes
            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            //  If _type or _scopeTagsPropertyInfo is null, return
            if (_type == null || _scopeTagsPropertyInfo == null)
            {
                base.OnActionExecuted(context);
                return;
            }

            // Code to execute after the action executes
            //  Filter Scope Tags for each entity
            if (context.Result is Microsoft.AspNetCore.Mvc.ObjectResult objectResult)
            {
                if (objectResult.Value != null && objectResult.Value.GetType() == _type)
                {
                    var entityWithScopeTags = objectResult.Value;
                    var existingTags = _scopeTagsPropertyInfo.GetValue(entityWithScopeTags) as List<string>;
                    if (existingTags != null)
                    {
                        var filteredTags = ScopeTagHelpers.GetIntersectingScopeTags(existingTags, context.HttpContext);
                        _scopeTagsPropertyInfo.SetValue(entityWithScopeTags, filteredTags);
                    }
                }
                else if (objectResult.Value != null && IsEnumerableOfType(objectResult.Value, _type))
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
