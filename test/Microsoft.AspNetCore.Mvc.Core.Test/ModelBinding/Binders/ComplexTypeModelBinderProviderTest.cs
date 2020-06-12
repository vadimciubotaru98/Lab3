﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Moq;
using Xunit;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.Binders
{
    public class ComplexTypeModelBinderProviderTest
    {
        [Theory]
        [InlineData(typeof(string))]
        [InlineData(typeof(int))]
        [InlineData(typeof(List<int>))]
        public void Create_ForNonComplexType_ReturnsNull(Type modelType)
        {
            // Arrange
            var provider = new ComplexTypeModelBinderProvider();

            var context = new TestModelBinderProviderContext(modelType);

            // Act
            var result = provider.GetBinder(context);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Create_ForSupportedTypes_ReturnsBinder()
        {
            // Arrange
            var provider = new ComplexTypeModelBinderProvider();

            var context = new TestModelBinderProviderContext(typeof(Person));
            context.OnCreatingBinder(m =>
            {
                if (m.ModelType == typeof(int) || m.ModelType == typeof(string))
                {
                    return Mock.Of<IModelBinder>();
                }
                else
                {
                    Assert.False(true, "Not the right model type");
                    return null;
                }
            });

            // Act
            var result = provider.GetBinder(context);

            // Assert
            Assert.IsType<ComplexTypeModelBinder>(result);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Create_ForSupportedType_ReturnsBinder_WithExpectedAllowValidatingTopLevelNodes(
            bool allowValidatingTopLevelNodes)
        {
            // Arrange
            var provider = new ComplexTypeModelBinderProvider();

            var context = new TestModelBinderProviderContext(typeof(Person));
            context.MvcOptions.AllowValidatingTopLevelNodes = allowValidatingTopLevelNodes;
            context.OnCreatingBinder(m =>
            {
                if (m.ModelType == typeof(int) || m.ModelType == typeof(string))
                {
                    return Mock.Of<IModelBinder>();
                }
                else
                {
                    Assert.False(true, "Not the right model type");
                    return null;
                }
            });

            // Act
            var result = provider.GetBinder(context);

            // Assert
            var binder = Assert.IsType<ComplexTypeModelBinder>(result);
            Assert.Equal(allowValidatingTopLevelNodes, binder.AllowValidatingTopLevelNodes);
        }

        private class Person
        {
            public string Name { get; set; }

            public int Age { get; set; }
        }
    }
}
