﻿// ***********************************************************************
// Assembly         : IronyModManager.Tests
// Author           : Mario
// Created          : 04-25-2020
//
// Last Modified By : Mario
// Last Modified On : 06-06-2020
// ***********************************************************************
// <copyright file="DefinitionPriorityTextConverterTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using IronyModManager.Converters;
using IronyModManager.Localization;
using IronyModManager.Models;
using IronyModManager.Parser.Definitions;
using IronyModManager.Services.Common;
using IronyModManager.Tests.Common;
using IronyModManager.Shared.Models;
using Moq;
using Xunit;

namespace IronyModManager.Tests
{
    /// <summary>
    /// Class PatchModConverterTests.
    /// </summary>
    public class DefinitionModTextConverterTests
    {
        /// <summary>
        /// Defines the test method Text_should_be_empty_when_patch_mod.
        /// </summary>
        [Fact]
        public void Text_should_not_include_priority_type_for_patch_mod()
        {
            DISetup.SetupContainer();
            var converter = new DefinitionPriorityTextConverter();
            var service = new Mock<IModPatchCollectionService>();
            service.Setup(p => p.IsPatchMod(It.IsAny<string>())).Returns(true);
            DISetup.Container.RegisterInstance(service.Object);
            var def = new Definition() { ModName = "IronyModManager_fake", Id = "t1" };
            var result = converter.Convert(new List<object>() { new List<IDefinition>() { def }, def }, null, null, null);
            result.Should().Be("IronyModManager_fake - t1");
        }

        /// <summary>
        /// Defines the test method Text_should_be_load_order.
        /// </summary>
        [Fact]
        public void Text_should_be_load_order()
        {
            DISetup.SetupContainer();
            var converter = new DefinitionPriorityTextConverter();
            var service = new Mock<IModPatchCollectionService>();
            service.Setup(p => p.IsPatchMod(It.IsAny<string>())).Returns((string p) =>
            {
                if (p == "IronyModManager_fake3")
                {
                    return true;
                }
                return false;
            });
            DISetup.Container.RegisterInstance(service.Object);
            var locManager = new Mock<ILocalizationManager>();
            locManager.Setup(s => s.GetResource(It.Is<string>(s => s.EndsWith("Order")))).Returns("Order");
            locManager.Setup(s => s.GetResource(It.Is<string>(s => s.EndsWith("FIOS")))).Returns("FIOS");
            locManager.Setup(s => s.GetResource(It.Is<string>(s => s.EndsWith("LIOS")))).Returns("LIOS");
            DISetup.Container.RegisterInstance(locManager.Object);
            var def = new Definition() { ModName = "IronyModManager_fake1", File = "test1.txt", Id = "t1" };
            var def2 = new Definition() { ModName = "IronyModManager_fake2", File = "test1.txt", Id = "t1" };
            var def3 = new Definition() { ModName = "IronyModManager_fake3", File = "test.txt", Id = "t1" };
            service.Setup(p => p.EvalDefinitionPriority(It.IsAny<IEnumerable<IDefinition>>())).Returns(new PriorityDefinitionResult() { Definition = def, PriorityType = Models.Common.DefinitionPriorityType.ModOrder });
            var result = converter.Convert(new List<object>() { new List<IDefinition>() { def, def2, def3 }, def }, null, null, null);
            result.Should().Be("IronyModManager_fake1 - t1 Order");
        }

        /// <summary>
        /// Defines the test method Text_should_be_fios.
        /// </summary>
        [Fact]
        public void Text_should_be_fios()
        {
            DISetup.SetupContainer();
            var converter = new DefinitionPriorityTextConverter();
            var service = new Mock<IModPatchCollectionService>();
            service.Setup(p => p.IsPatchMod(It.IsAny<string>())).Returns((string p) =>
            {
                if (p == "IronyModManager_fake3")
                {
                    return true;
                }
                return false;
            });
            DISetup.Container.RegisterInstance(service.Object);
            var locManager = new Mock<ILocalizationManager>();
            locManager.Setup(s => s.GetResource(It.Is<string>(s => s.EndsWith("Order")))).Returns("Order");
            locManager.Setup(s => s.GetResource(It.Is<string>(s => s.EndsWith("FIOS")))).Returns("FIOS");
            locManager.Setup(s => s.GetResource(It.Is<string>(s => s.EndsWith("LIOS")))).Returns("LIOS");
            DISetup.Container.RegisterInstance(locManager.Object);
            var def = new Definition() { ModName = "IronyModManager_fake1", File = "test1.txt", Id = "t1" };
            var def2 = new Definition() { ModName = "IronyModManager_fake2", File = "test1.txt,", Id = "t1" };
            var def3 = new Definition() { ModName = "IronyModManager_fake3", File = "test.txt", Id = "t1" };
            service.Setup(p => p.EvalDefinitionPriority(It.IsAny<IEnumerable<IDefinition>>())).Returns(new PriorityDefinitionResult() { Definition = def, PriorityType = Models.Common.DefinitionPriorityType.FIOS });
            var result = converter.Convert(new List<object>() { new List<IDefinition>() { def, def2, def3 }, def }, null, null, null);
            result.Should().Be("IronyModManager_fake1 - t1 FIOS");
        }

        /// <summary>
        /// Defines the test method Text_should_be_override.
        /// </summary>
        [Fact]
        public void Text_should_be_override()
        {
            DISetup.SetupContainer();
            var converter = new DefinitionPriorityTextConverter();
            var service = new Mock<IModPatchCollectionService>();
            service.Setup(p => p.IsPatchMod(It.IsAny<string>())).Returns((string p) =>
            {
                if (p == "IronyModManager_fake3")
                {
                    return true;
                }
                return false;
            });
            DISetup.Container.RegisterInstance(service.Object);
            var locManager = new Mock<ILocalizationManager>();
            locManager.Setup(s => s.GetResource(It.Is<string>(s => s.EndsWith("Order")))).Returns("Order");
            locManager.Setup(s => s.GetResource(It.Is<string>(s => s.EndsWith("FIOS")))).Returns("FIOS");
            locManager.Setup(s => s.GetResource(It.Is<string>(s => s.EndsWith("LIOS")))).Returns("LIOS");
            locManager.Setup(s => s.GetResource(It.Is<string>(s => s.EndsWith("Override")))).Returns("Override");
            DISetup.Container.RegisterInstance(locManager.Object);
            var def = new Definition() { ModName = "IronyModManager_fake1", File = "test1.txt", Id = "t1", Dependencies = new List<string>() { "IronyModManager_fake2", "IronyModManager_fake3" } };
            var def2 = new Definition() { ModName = "IronyModManager_fake2", File = "test1.txt,", Id = "t1" };
            var def3 = new Definition() { ModName = "IronyModManager_fake3", File = "test.txt", Id = "t1" };
            service.Setup(p => p.EvalDefinitionPriority(It.IsAny<IEnumerable<IDefinition>>())).Returns(new PriorityDefinitionResult() { Definition = def, PriorityType = Models.Common.DefinitionPriorityType.ModOverride });
            var result = converter.Convert(new List<object>() { new List<IDefinition>() { def, def2, def3 }, def }, null, null, null);
            result.Should().Be("IronyModManager_fake1 - t1 Override");
        }

        /// <summary>
        /// Defines the test method Text_should_be_lios.
        /// </summary>
        [Fact]
        public void Text_should_be_lios()
        {
            DISetup.SetupContainer();
            var converter = new DefinitionPriorityTextConverter();
            var service = new Mock<IModPatchCollectionService>();
            service.Setup(p => p.IsPatchMod(It.IsAny<string>())).Returns((string p) =>
            {
                if (p == "IronyModManager_fake3")
                {
                    return true;
                }
                return false;
            });
            DISetup.Container.RegisterInstance(service.Object);
            var locManager = new Mock<ILocalizationManager>();
            locManager.Setup(s => s.GetResource(It.Is<string>(s => s.EndsWith("Order")))).Returns("Order");
            locManager.Setup(s => s.GetResource(It.Is<string>(s => s.EndsWith("FIOS")))).Returns("FIOS");
            locManager.Setup(s => s.GetResource(It.Is<string>(s => s.EndsWith("LIOS")))).Returns("LIOS");
            DISetup.Container.RegisterInstance(locManager.Object);
            var def = new Definition() { ModName = "IronyModManager_fake1", File = "test1.txt", Id = "t1" };
            var def2 = new Definition() { ModName = "IronyModManager_fake2", File = "test1.txt", Id = "t1" };
            var def3 = new Definition() { ModName = "IronyModManager_fake3", File = "test.txt", Id = "t1" };
            service.Setup(p => p.EvalDefinitionPriority(It.IsAny<IEnumerable<IDefinition>>())).Returns(new PriorityDefinitionResult() { Definition = def, PriorityType = Models.Common.DefinitionPriorityType.LIOS });
            var result = converter.Convert(new List<object>() { new List<IDefinition>() { def, def2, def3 }, def }, null, null, null);
            result.Should().Be("IronyModManager_fake1 - t1 LIOS");
        }

        /// <summary>
        /// Defines the test method Text_should_be_empty.
        /// </summary>
        [Fact]
        public void Text_should_include_priority_type()
        {
            DISetup.SetupContainer();
            var service = new Mock<IModPatchCollectionService>();
            service.Setup(p => p.IsPatchMod(It.IsAny<string>())).Returns(false);
            DISetup.Container.RegisterInstance(service.Object);
            var converter = new DefinitionPriorityTextConverter();
            var def = new Definition() { ModName = "IronyModManager_fake", Id = "t1" };
            var result = converter.Convert(new List<object>() { new List<IDefinition>() { def }, def }, null, null, null);
            result.Should().Be("IronyModManager_fake - t1");
        }
    }
}
