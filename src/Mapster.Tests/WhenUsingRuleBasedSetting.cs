﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace Mapster.Tests
{
    [TestClass]
    public class WhenUsingRuleBasedSetting
    {

        [TestMethod]
        public void Rule_Base_Testing()
        {
            TypeAdapterConfig.GlobalSettings.When((srcType, destType, mapType) => srcType == destType)
                .Ignore("Id");

            var simplePoco = new SimplePoco {Id = Guid.NewGuid(), Name = "TestName"};

            var dto = TypeAdapter.Adapt<SimplePoco>(simplePoco);

            dto.Id.ShouldBe(Guid.Empty);
            dto.Name.ShouldBe(simplePoco.Name);
        }

        #region TestClasses

        public class SimplePoco
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        #endregion


    }
}