﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace Mapster.Tests
{
    [TestClass]
    public class WhenHandlingUnmappedMembers
    {
        [TestCleanup]
        public void TestCleanup()
        {
            TypeAdapterConfig.GlobalSettings.RequireDestinationMemberSource = false;
        }

        [TestMethod]
        public void No_Errors_Thrown_With_Default_Configuration_On_Unmapped_Primitive()
        {
            TypeAdapterConfig.GlobalSettings.RequireDestinationMemberSource = false;
            TypeAdapterConfig<ParentPoco, ParentDto>.NewConfig().Compile();

            var source = new SimplePoco {Id = Guid.NewGuid(), Name = "TestName"};

            var simpleDto = TypeAdapter.Adapt<SimplePoco, SimpleDto>(source);

            simpleDto.Name.ShouldBe("TestName");
            simpleDto.UnmappedMember.ShouldBeNull();
            simpleDto.UnmappedMember2.ShouldBe(0);
        }

        [TestMethod]
        public void Error_Thrown_With_Explicit_Configuration_On_Unmapped_Primitive()
        {
            try
            {
                TypeAdapterConfig.GlobalSettings.RequireDestinationMemberSource = true;
                TypeAdapterConfig<ParentPoco, ParentDto>.NewConfig().Compile();

                var source = new SimplePoco {Id = Guid.NewGuid(), Name = "TestName"};

                TypeAdapter.Adapt<SimplePoco, SimpleDto>(source);
            }
            catch (InvalidOperationException ex)
            {
                ex.ToString().ShouldContain("UnmappedMember");
            }
        }

        [TestMethod]
        public void No_Errors_Thrown_With_Default_Configuration_On_Unmapped_Child_Collection()
        {
            TypeAdapterConfig.GlobalSettings.RequireDestinationMemberSource = false;
            TypeAdapterConfig<ParentPoco, ParentDto>.NewConfig().Compile();

            var source = new ParentPoco { Id = Guid.NewGuid(), Name = "TestName", Children = new List<ChildPoco> { new ChildPoco { Id = Guid.NewGuid(), Name = "TestName" } } };

            var destination = TypeAdapter.Adapt<ParentPoco, ParentDto>(source);

            destination.Name.ShouldBe("TestName");
            destination.UnmappedChildren.ShouldBeNull();
            destination.Children.Count.ShouldBe(1);
        }

        [TestMethod]
        public void Error_Thrown_With_Explicit_Configuration_On_Unmapped_Child_Collection()
        {
            try
            {
                TypeAdapterConfig.GlobalSettings.RequireDestinationMemberSource = true;
                TypeAdapterConfig<ParentPoco, ParentDto>.NewConfig().Compile();

                var source = new ParentPoco {Id = Guid.NewGuid(), Name = "TestName", Children = new List<ChildPoco> {new ChildPoco {Id = Guid.NewGuid(), Name = "TestName"}}};

                TypeAdapter.Adapt<ParentPoco, ParentDto>(source);
                Assert.Fail();
            }
            catch (InvalidOperationException ex)
            {
                ex.ToString().ShouldContain("UnmappedChildren");
            }
        }


        #region TestClasses

        public class SimplePoco
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public class SimpleDto
        {
            public Guid Id { get; set; }
            public string Name { get; set; }

            public string UnmappedMember { get; set; }

            public int UnmappedMember2 { get; set; }
        }

        public class ChildPoco
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public class ChildDto
        {
            public Guid Id { get; set; }
            public string Name { get; set; }

            public string UnmappedChildMember { get; set; }
        }

        public class ParentPoco
        {
            public Guid Id { get; set; }
            public string Name { get; set; }

            public List<ChildPoco> Children { get; set; }
        }

        public class ParentDto
        {
            public Guid Id { get; set; }
            public string Name { get; set; }

            public List<ChildDto> Children { get; set; }

            public List<ChildDto> UnmappedChildren { get; set; } 
        }

        #endregion



    }


}