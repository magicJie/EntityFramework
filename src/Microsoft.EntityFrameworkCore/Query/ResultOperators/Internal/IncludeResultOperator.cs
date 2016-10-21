// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Clauses.StreamedData;

namespace Microsoft.EntityFrameworkCore.Query.ResultOperators.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class IncludeResultOperator : SequenceTypePreservingResultOperatorBase, IQueryAnnotation
    {
        private List<PropertyInfo> _chainedNavigationProperties;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IncludeResultOperator([NotNull] MemberExpression navigationPropertyPath, [NotNull] Expression pathFromQuerySource)
        {
            NavigationPropertyPath = navigationPropertyPath;
            PathFromQuerySource = pathFromQuerySource;
            QuerySource = GetQuerySource(pathFromQuerySource);
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IncludeResultOperator([NotNull] string navigationPropertyPath, [NotNull] Expression pathFromQuerySource)
        {
            StringNavigationPropertyPath = navigationPropertyPath;
            PathFromQuerySource = pathFromQuerySource;
            QuerySource = GetQuerySource(pathFromQuerySource);
        }

        private static IQuerySource GetQuerySource(Expression expression)
        {
            var querySourceReferenceExpression = expression as QuerySourceReferenceExpression;
            if (querySourceReferenceExpression != null)
            {
                return querySourceReferenceExpression.ReferencedQuerySource;
            }

            var memberExpression = expression as MemberExpression;
            if (memberExpression != null)
            {
                return GetQuerySource(memberExpression.Expression.RemoveConvert());
            }

            return null;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual IQuerySource QuerySource { get; set; }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual QueryModel QueryModel { get; set; }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual MemberExpression NavigationPropertyPath { get; }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual string StringNavigationPropertyPath { get; }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual Expression PathFromQuerySource { get; [param: NotNull] set; }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual IReadOnlyList<PropertyInfo> ChainedNavigationProperties => _chainedNavigationProperties;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual void AppendToNavigationPath([NotNull] IReadOnlyList<PropertyInfo> propertyInfos)
        {
            if (_chainedNavigationProperties == null)
            {
                _chainedNavigationProperties = new List<PropertyInfo>();
            }

            _chainedNavigationProperties.AddRange(propertyInfos);
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override string ToString()
            => "Include("
               + (StringNavigationPropertyPath ??
                  NavigationPropertyPath
                  + (_chainedNavigationProperties != null
                      ? "." + _chainedNavigationProperties.Select(p => p.Name).Join(".")
                      : string.Empty))
               + ")";

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override ResultOperatorBase Clone(CloneContext cloneContext)
            => StringNavigationPropertyPath != null
                ? new IncludeResultOperator(StringNavigationPropertyPath, PathFromQuerySource)
                : new IncludeResultOperator(NavigationPropertyPath, PathFromQuerySource)
                {
                    _chainedNavigationProperties = _chainedNavigationProperties
                };

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override void TransformExpressions([NotNull] Func<Expression, Expression> transformation)
        {
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override StreamedSequence ExecuteInMemory<T>([NotNull] StreamedSequence input) => input;
    }
}
