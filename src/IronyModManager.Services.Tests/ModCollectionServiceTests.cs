﻿// ***********************************************************************
// Assembly         : IronyModManager.Services.Tests
// Author           : Mario
// Created          : 03-04-2020
//
// Last Modified By : Mario
// Last Modified On : 11-27-2020
// ***********************************************************************
// <copyright file="ModCollectionServiceTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using IronyModManager.IO;
using IronyModManager.IO.Common;
using IronyModManager.IO.Common.Mods;
using IronyModManager.IO.Common.Readers;
using IronyModManager.Models;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Shared.Cache;
using IronyModManager.Shared.MessageBus;
using IronyModManager.Storage.Common;
using IronyModManager.Tests.Common;
using Moq;
using Xunit;

namespace IronyModManager.Services.Tests
{
    /// <summary>
    /// Class ModCollectionServiceTests.
    /// </summary>
    public class ModCollectionServiceTests
    {
        /// <summary>
        /// Setups the mock case.
        /// </summary>
        /// <param name="storageProvider">The storage provider.</param>
        /// <param name="gameService">The game service.</param>
        private static void SetupMockCase(Mock<IStorageProvider> storageProvider, Mock<IGameService> gameService)
        {
            var collections = new List<IModCollection>()
            {
                new ModCollection()
                {
                    Mods = new List<string>() { "1", "2"},
                    Name = "test",
                    Game = "test"
                },
                new ModCollection()
                {
                    Mods = new List<string>() { "2"},
                    Name = "test2",
                    Game = "test"
                },
                new ModCollection()
                {
                    Mods = new List<string>() { "3"},
                    Name = "test",
                    Game = "test2"
                }
            };
            storageProvider.Setup(s => s.GetModCollections()).Returns(() =>
            {
                return collections;
            });
            storageProvider.Setup(s => s.SetModCollections(It.IsAny<IEnumerable<IModCollection>>())).Returns((IEnumerable<IModCollection> col) =>
            {
                collections = col.ToList();
                return true;
            });
            gameService.Setup(s => s.GetSelected()).Returns(new Game()
            {
                Type = "test"
            });
        }

        /// <summary>
        /// Defines the test method Should_create_empty_mod_collection_object.
        /// </summary>
        [Fact]
        public void Should_create_empty_mod_collection_object()
        {
            DISetup.SetupContainer();
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var gameService = new Mock<IGameService>();
            var modExport = new Mock<IModCollectionExporter>();
            SetupMockCase(storageProvider, gameService);

            var service = new ModCollectionService(null, null, new Cache(), null, null, null, null, gameService.Object, modExport.Object, storageProvider.Object, mapper.Object);
            var result = service.Create();
            result.Name.Should().BeNullOrEmpty();
            result.Mods.Should().NotBeNull();
            result.Mods.Count().Should().Be(0);
            result.IsSelected.Should().BeFalse();
            result.Game.Should().Be("test");
        }

        /// <summary>
        /// Defines the test method Should_delete_mod_collection.
        /// </summary>
        [Fact]
        public void Should_delete_mod_collection()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var gameService = new Mock<IGameService>();
            var modExport = new Mock<IModCollectionExporter>();
            SetupMockCase(storageProvider, gameService);

            var service = new ModCollectionService(null, null, new Cache(), null, null, null, null, gameService.Object, modExport.Object, storageProvider.Object, mapper.Object);
            var result = service.Delete("test");
            result.Should().BeTrue();
            service.GetAll().Count().Should().Be(1);
        }

        /// <summary>
        /// Defines the test method Should_not_delete_mod_collection.
        /// </summary>
        [Fact]
        public void Should_not_delete_mod_collection()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var gameService = new Mock<IGameService>();
            var modExport = new Mock<IModCollectionExporter>();
            SetupMockCase(storageProvider, gameService);

            var service = new ModCollectionService(null, null, new Cache(), null, null, null, null, gameService.Object, modExport.Object, storageProvider.Object, mapper.Object);
            var result = service.Delete("test3");
            result.Should().BeFalse();
            service.GetAll().Count().Should().Be(2);
        }

        /// <summary>
        /// Defines the test method Should_not_delete_mod_collection_when_collection_empty.
        /// </summary>
        [Fact]
        public void Should_not_delete_mod_collection_when_collection_empty()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var gameService = new Mock<IGameService>();
            var modExport = new Mock<IModCollectionExporter>();
            SetupMockCase(storageProvider, gameService);
            storageProvider.Setup(s => s.GetModCollections()).Returns(new List<IModCollection>());

            var service = new ModCollectionService(null, null, new Cache(), null, null, null, null, gameService.Object, modExport.Object, storageProvider.Object, mapper.Object);
            var result = service.Delete("test");
            result.Should().BeFalse();
            service.GetAll().Count().Should().Be(0);
        }

        /// <summary>
        /// Defines the test method Should_not_delete_mod_collection_when_no_selected_game.
        /// </summary>
        [Fact]
        public void Should_not_delete_mod_collection_when_no_selected_game()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var gameService = new Mock<IGameService>();
            var modExport = new Mock<IModCollectionExporter>();
            SetupMockCase(storageProvider, gameService);
            gameService.Setup(s => s.GetSelected()).Returns((IGame)null);

            var service = new ModCollectionService(null, null, new Cache(), null, null, null, null, gameService.Object, modExport.Object, storageProvider.Object, mapper.Object);
            var result = service.Delete("test");
            result.Should().BeFalse();
            service.GetAll().Count().Should().Be(0);
        }

        /// <summary>
        /// Defines the test method Should_not_delete_mod_collection_when_game_has_no_items.
        /// </summary>
        [Fact]
        public void Should_not_delete_mod_collection_when_game_has_no_items()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var gameService = new Mock<IGameService>();
            var modExport = new Mock<IModCollectionExporter>();
            SetupMockCase(storageProvider, gameService);
            storageProvider.Setup(s => s.GetModCollections()).Returns(new List<IModCollection>());
            gameService.Setup(s => s.GetSelected()).Returns(new Game()
            {
                Type = "no-items"
            });

            var service = new ModCollectionService(null, null, new Cache(), null, null, null, null, gameService.Object, modExport.Object, storageProvider.Object, mapper.Object);
            var result = service.Delete("test");
            result.Should().BeFalse();
            service.GetAll().Count().Should().Be(0);
        }

        /// <summary>
        /// Defines the test method Should_return_mod_collection_object.
        /// </summary>
        [Fact]
        public void Should_return_mod_collection_object()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var gameService = new Mock<IGameService>();
            var modExport = new Mock<IModCollectionExporter>();
            SetupMockCase(storageProvider, gameService);

            var service = new ModCollectionService(null, null, new Cache(), null, null, null, null, gameService.Object, modExport.Object, storageProvider.Object, mapper.Object);
            var result = service.Get("test");
            result.Should().NotBeNull();
            result.Name.Should().Be("test");
            result.Mods.Count().Should().Be(2);
        }

        /// <summary>
        /// Defines the test method Should_not_return_mod_collection_object.
        /// </summary>
        [Fact]
        public void Should_not_return_mod_collection_object()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var gameService = new Mock<IGameService>();
            var modExport = new Mock<IModCollectionExporter>();
            SetupMockCase(storageProvider, gameService);

            var service = new ModCollectionService(null, null, new Cache(), null, null, null, null, gameService.Object, modExport.Object, storageProvider.Object, mapper.Object);
            var result = service.Get("test3");
            result.Should().BeNull();
        }

        /// <summary>
        /// Defines the test method Should_not_return_mod_collection_object_when_collection_empty.
        /// </summary>
        [Fact]
        public void Should_not_return_mod_collection_object_when_collection_empty()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var gameService = new Mock<IGameService>();
            var modExport = new Mock<IModCollectionExporter>();
            SetupMockCase(storageProvider, gameService);
            storageProvider.Setup(s => s.GetModCollections()).Returns(new List<IModCollection>());

            var service = new ModCollectionService(null, null, new Cache(), null, null, null, null, gameService.Object, modExport.Object, storageProvider.Object, mapper.Object);
            var result = service.Get("test2");
            result.Should().BeNull();
        }

        /// <summary>
        /// Defines the test method Should_not_return_mod_collection_object_when_no_selected_game.
        /// </summary>
        [Fact]
        public void Should_not_return_mod_collection_object_when_no_selected_game()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var gameService = new Mock<IGameService>();
            var modExport = new Mock<IModCollectionExporter>();
            SetupMockCase(storageProvider, gameService);
            gameService.Setup(s => s.GetSelected()).Returns((IGame)null);

            var service = new ModCollectionService(null, null, new Cache(), null, null, null, null, gameService.Object, modExport.Object, storageProvider.Object, mapper.Object);
            var result = service.Get("test2");
            result.Should().BeNull();
        }

        /// <summary>
        /// Defines the test method Should_not_return_mod_collection_object_when_game_has_no_items.
        /// </summary>
        [Fact]
        public void Should_not_return_mod_collection_object_when_game_has_no_items()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var gameService = new Mock<IGameService>();
            var modExport = new Mock<IModCollectionExporter>();
            SetupMockCase(storageProvider, gameService);
            storageProvider.Setup(s => s.GetModCollections()).Returns(new List<IModCollection>());
            gameService.Setup(s => s.GetSelected()).Returns(new Game()
            {
                Type = "no-items"
            });

            var service = new ModCollectionService(null, null, new Cache(), null, null, null, null, gameService.Object, modExport.Object, storageProvider.Object, mapper.Object);
            var result = service.Get("test2");
            result.Should().BeNull();
        }

        /// <summary>
        /// Defines the test method Should_throw_exception_when_saving_empty_mod_object.
        /// </summary>
        [Fact]
        public void Should_throw_exception_when_saving_empty_mod_object()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var gameService = new Mock<IGameService>();
            var modExport = new Mock<IModCollectionExporter>();
            SetupMockCase(storageProvider, gameService);
            var service = new ModCollectionService(null, null, new Cache(), null, null, null, null, gameService.Object, modExport.Object, storageProvider.Object, mapper.Object);
            Exception exception = null;
            try
            {
                service.Save(null);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            exception.GetType().Should().Be(typeof(ArgumentNullException));
        }

        /// <summary>
        /// Defines the test method Should_throw_exception_when_saving_mod_object_with_no_game.
        /// </summary>
        [Fact]
        public void Should_throw_exception_when_saving_mod_object_with_no_game()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var gameService = new Mock<IGameService>();
            var modExport = new Mock<IModCollectionExporter>();
            SetupMockCase(storageProvider, gameService);
            var service = new ModCollectionService(null, null, new Cache(), null, null, null, null, gameService.Object, modExport.Object, storageProvider.Object, mapper.Object);
            Exception exception = null;
            try
            {
                service.Save(new ModCollection());
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            exception.GetType().Should().Be(typeof(ArgumentNullException));
        }

        /// <summary>
        /// Defines the test method Should_save_new_mod_object.
        /// </summary>
        [Fact]
        public void Should_save_new_mod_object()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var gameService = new Mock<IGameService>();
            var modExport = new Mock<IModCollectionExporter>();
            SetupMockCase(storageProvider, gameService);

            var service = new ModCollectionService(null, null, new Cache(), null, null, null, null, gameService.Object, modExport.Object, storageProvider.Object, mapper.Object);
            var result = service.Save(new ModCollection()
            {
                Name = "fake",
                Game = "test"
            });
            result.Should().BeTrue();
            service.GetAll().Count().Should().Be(3);
        }

        /// <summary>
        /// Defines the test method Should_overwrite_existing_mod_object.
        /// </summary>
        [Fact]
        public void Should_overwrite_existing_mod_object()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var gameService = new Mock<IGameService>();
            var modExport = new Mock<IModCollectionExporter>();
            SetupMockCase(storageProvider, gameService);

            var service = new ModCollectionService(null, null, new Cache(), null, null, null, null, gameService.Object, modExport.Object, storageProvider.Object, mapper.Object);
            var result = service.Save(new ModCollection()
            {
                Name = "test",
                Game = "test"
            });
            result.Should().BeTrue();
            service.GetAll().Count().Should().Be(2);
        }

        /// <summary>
        /// Defines the test method Should_return_mod_names.
        /// </summary>
        [Fact]
        public void Should_return_mod_names()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var gameService = new Mock<IGameService>();
            var modExport = new Mock<IModCollectionExporter>();
            SetupMockCase(storageProvider, gameService);

            var service = new ModCollectionService(null, null, new Cache(), null, null, null, null, gameService.Object, modExport.Object, storageProvider.Object, mapper.Object);
            service.GetAll().Count().Should().Be(2);
        }

        /// <summary>
        /// Defines the test method Should_not_return_mod_names.
        /// </summary>
        [Fact]
        public void Should_not_return_mod_names()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var gameService = new Mock<IGameService>();
            var modExport = new Mock<IModCollectionExporter>();
            SetupMockCase(storageProvider, gameService);
            storageProvider.Setup(s => s.GetModCollections()).Returns(new List<IModCollection>());

            var service = new ModCollectionService(null, null, new Cache(), null, null, null, null, gameService.Object, modExport.Object, storageProvider.Object, mapper.Object);
            service.GetAll().Count().Should().Be(0);
        }

        /// <summary>
        /// Defines the test method Should_not_return_mod_names_when_no_selected_game.
        /// </summary>
        [Fact]
        public void Should_not_return_mod_names_when_no_selected_game()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var gameService = new Mock<IGameService>();
            var modExport = new Mock<IModCollectionExporter>();
            SetupMockCase(storageProvider, gameService);
            gameService.Setup(s => s.GetSelected()).Returns((IGame)null);

            var service = new ModCollectionService(null, null, new Cache(), null, null, null, null, gameService.Object, modExport.Object, storageProvider.Object, mapper.Object);
            service.GetAll().Count().Should().Be(0);
        }

        /// <summary>
        /// Defines the test method Should_not_return_mod_names_when_game_has_no_items.
        /// </summary>
        [Fact]
        public void Should_not_return_mod_names_when_game_has_no_items()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var gameService = new Mock<IGameService>();
            var modExport = new Mock<IModCollectionExporter>();
            SetupMockCase(storageProvider, gameService);
            gameService.Setup(s => s.GetSelected()).Returns(new Game()
            {
                Type = "no-items"
            });


            var service = new ModCollectionService(null, null, new Cache(), null, null, null, null, gameService.Object, modExport.Object, storageProvider.Object, mapper.Object);
            service.GetAll().Count().Should().Be(0);
        }

        /// <summary>
        /// Defines the test method Should_export_mod.
        /// </summary>
        [Fact]
        public async Task Should_export_mod()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var gameService = new Mock<IGameService>();
            var modExport = new Mock<IModCollectionExporter>();
            var isValid = false;
            gameService.Setup(s => s.GetSelected()).Returns(new Game()
            {
                Type = "no-items",
                UserDirectory = "C:\\fake"
            });
            modExport.Setup(p => p.ExportAsync(It.IsAny<ModCollectionExporterParams>())).Callback((ModCollectionExporterParams p) =>
            {
                if (p.File.Equals("file") && p.Mod.Name.Equals("fake"))
                {
                    isValid = true;
                }
            });
            var modWriter = new Mock<IModWriter>();
            modWriter.Setup(p => p.ModDirectoryExists(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return false;
            });

            var service = new ModCollectionService(null, null, new Cache(), null, null, modWriter.Object, null, gameService.Object, modExport.Object, storageProvider.Object, mapper.Object);
            await service.ExportAsync("file", new ModCollection()
            {
                Name = "fake"
            });
            isValid.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_export_mod_with_order_only.
        /// </summary>
        [Fact]
        public async Task Should_export_mod_with_order_only()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var gameService = new Mock<IGameService>();
            var modExport = new Mock<IModCollectionExporter>();
            var isValid = false;
            gameService.Setup(s => s.GetSelected()).Returns(new Game()
            {
                Type = "no-items",
                UserDirectory = "C:\\fake"
            });
            modExport.Setup(p => p.ExportAsync(It.IsAny<ModCollectionExporterParams>())).Callback((ModCollectionExporterParams p) =>
            {
                if (p.File.Equals("file") && p.Mod.Name.Equals("fake") && p.ExportModOrderOnly)
                {
                    isValid = true;
                }
            });
            var modWriter = new Mock<IModWriter>();
            modWriter.Setup(p => p.ModDirectoryExists(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return false;
            });

            var service = new ModCollectionService(null, null, new Cache(), null, null, modWriter.Object, null, gameService.Object, modExport.Object, storageProvider.Object, mapper.Object);
            await service.ExportAsync("file", new ModCollection()
            {
                Name = "fake"
            }, true);
            isValid.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_import_mod.
        /// </summary>
        [Fact]
        public async Task Should_import_mod()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var gameService = new Mock<IGameService>();
            var modExport = new Mock<IModCollectionExporter>();
            DISetup.SetupContainer();
            gameService.Setup(s => s.GetSelected()).Returns(new Game()
            {
                Type = "no-items",
                UserDirectory = "C:\\fake"
            });
            modExport.Setup(p => p.ImportAsync(It.IsAny<ModCollectionExporterParams>())).Returns((ModCollectionExporterParams p) =>
            {
                p.Mod.Name = "fake";
                return Task.FromResult(true);
            });
            modExport.Setup(p => p.ImportModDirectoryAsync(It.IsAny<ModCollectionExporterParams>())).Returns((ModCollectionExporterParams p) =>
            {
                return Task.FromResult(true);
            });
            var modWriter = new Mock<IModWriter>();
            modWriter.Setup(p => p.ModDirectoryExists(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return false;
            });

            var service = new ModCollectionService(null, null, new Cache(), null, null, modWriter.Object, null, gameService.Object, modExport.Object, storageProvider.Object, mapper.Object);
            var result = await service.ImportAsync("file");
            result.Name.Should().Be("fake");
        }

        /// <summary>
        /// Defines the test method Should_not_import_mod.
        /// </summary>
        [Fact]
        public async Task Should_not_import_mod()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var gameService = new Mock<IGameService>();
            var modExport = new Mock<IModCollectionExporter>();
            DISetup.SetupContainer();
            gameService.Setup(s => s.GetSelected()).Returns(new Game()
            {
                Type = "no-items",
                UserDirectory = "C:\\fake"
            });
            modExport.Setup(p => p.ImportAsync(It.IsAny<ModCollectionExporterParams>())).Returns((ModCollectionExporterParams p) =>
            {
                p.Mod.Name = "fake";
                return Task.FromResult(true);
            });
            modExport.Setup(p => p.ImportModDirectoryAsync(It.IsAny<ModCollectionExporterParams>())).Returns((ModCollectionExporterParams p) =>
            {
                return Task.FromResult(false);
            });
            var modWriter = new Mock<IModWriter>();
            modWriter.Setup(p => p.ModDirectoryExists(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return false;
            });

            var service = new ModCollectionService(null, null, new Cache(), null, null, modWriter.Object, null, gameService.Object, modExport.Object, storageProvider.Object, mapper.Object);
            var result = await service.ImportAsync("file");
            result.Should().BeNull();
        }

        /// <summary>
        /// Defines the test method Should_not_return_mod_collection_details.
        /// </summary>
        [Fact]
        public async Task Should_not_return_mod_collection_details()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var gameService = new Mock<IGameService>();
            var modExport = new Mock<IModCollectionExporter>();
            DISetup.SetupContainer();
            gameService.Setup(s => s.GetSelected()).Returns(new Game()
            {
                Type = "no-items",
                UserDirectory = "C:\\fake"
            });
            modExport.Setup(p => p.ImportAsync(It.IsAny<ModCollectionExporterParams>())).Returns((ModCollectionExporterParams p) =>
            {
                p.Mod.Name = "fake";
                return Task.FromResult(false);
            });

            var service = new ModCollectionService(null, null, new Cache(), null, null, null, null, gameService.Object, modExport.Object, storageProvider.Object, mapper.Object);
            var result = await service.GetImportedCollectionDetailsAsync("file");
            result.Should().BeNull();
        }

        /// <summary>
        /// Defines the test method Should_return_mod_collection_details.
        /// </summary>
        [Fact]
        public async Task Should_return_mod_collection_details()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var gameService = new Mock<IGameService>();
            var modExport = new Mock<IModCollectionExporter>();
            DISetup.SetupContainer();
            gameService.Setup(s => s.GetSelected()).Returns(new Game()
            {
                Type = "no-items",
                UserDirectory = "C:\\fake"
            });
            modExport.Setup(p => p.ImportAsync(It.IsAny<ModCollectionExporterParams>())).Returns((ModCollectionExporterParams p) =>
            {
                p.Mod.Name = "fake";
                return Task.FromResult(true);
            });

            var service = new ModCollectionService(null, null, new Cache(), null, null, null, null, gameService.Object, modExport.Object, storageProvider.Object, mapper.Object);
            var result = await service.GetImportedCollectionDetailsAsync("file");
            result.Should().NotBeNull();
        }

        /// <summary>
        /// Defines the test method Should_return_true_when_collection_exists.
        /// </summary>
        [Fact]
        public void Should_return_true_when_collection_exists()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var gameService = new Mock<IGameService>();
            var modExport = new Mock<IModCollectionExporter>();
            SetupMockCase(storageProvider, gameService);

            var service = new ModCollectionService(null, null, new Cache(), null, null, null, null, gameService.Object, modExport.Object, storageProvider.Object, mapper.Object);
            var result = service.Exists("test");
            result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_not_return_true_when_collection_exists.
        /// </summary>
        [Fact]
        public void Should_not_return_true_when_collection_exists()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var gameService = new Mock<IGameService>();
            var modExport = new Mock<IModCollectionExporter>();
            SetupMockCase(storageProvider, gameService);

            var service = new ModCollectionService(null, null, new Cache(), null, null, null, null, gameService.Object, modExport.Object, storageProvider.Object, mapper.Object);
            var result = service.Exists("test101");
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_import_paradoxos_mod.
        /// </summary>
        [Fact]
        public async Task Should_import_paradoxos_mod()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var gameService = new Mock<IGameService>();
            var modExport = new Mock<IModCollectionExporter>();
            DISetup.SetupContainer();
            gameService.Setup(s => s.GetSelected()).Returns(new Game()
            {
                Type = "no-items",
                UserDirectory = "C:\\fake"
            });
            modExport.Setup(p => p.ImportParadoxosAsync(It.IsAny<ModCollectionExporterParams>())).Returns((ModCollectionExporterParams p) =>
            {
                p.Mod.Name = "fake";
                return Task.FromResult(true);
            });

            var service = new ModCollectionService(null, null, new Cache(), null, null, null, null, gameService.Object, modExport.Object, storageProvider.Object, mapper.Object);
            var result = await service.ImportParadoxosAsync("file");
            result.Name.Should().Be("fake");
        }

        /// <summary>
        /// Defines the test method Should_not_import_paradoxos_mod.
        /// </summary>
        [Fact]
        public async Task Should_not_import_paradoxos_mod()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var gameService = new Mock<IGameService>();
            var modExport = new Mock<IModCollectionExporter>();
            DISetup.SetupContainer();
            gameService.Setup(s => s.GetSelected()).Returns(new Game()
            {
                Type = "no-items",
                UserDirectory = "C:\\fake"
            });
            modExport.Setup(p => p.ImportParadoxosAsync(It.IsAny<ModCollectionExporterParams>())).Returns((ModCollectionExporterParams p) =>
            {
                p.Mod.Name = "fake";
                return Task.FromResult(false);
            });

            var service = new ModCollectionService(null, null, new Cache(), null, null, null, null, gameService.Object, modExport.Object, storageProvider.Object, mapper.Object);
            var result = await service.ImportParadoxosAsync("file");
            result.Should().BeNull();
        }

        /// <summary>
        /// Defines the test method Should_import_paradox_mod.
        /// </summary>
        [Fact]
        public async Task Should_import_paradox_mod()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var gameService = new Mock<IGameService>();
            var modExport = new Mock<IModCollectionExporter>();
            DISetup.SetupContainer();
            gameService.Setup(s => s.GetSelected()).Returns(new Game()
            {
                Type = "no-items",
                UserDirectory = "C:\\fake"
            });
            modExport.Setup(p => p.ImportParadoxAsync(It.IsAny<ModCollectionExporterParams>())).Returns((ModCollectionExporterParams p) =>
            {
                p.Mod.Name = "fake";
                return Task.FromResult(true);
            });

            var service = new ModCollectionService(null, null, new Cache(), null, null, null, null, gameService.Object, modExport.Object, storageProvider.Object, mapper.Object);
            var result = await service.ImportParadoxAsync();
            result.Name.Should().Be("fake");
        }

        /// <summary>
        /// Defines the test method Should_not_import_paradox_mod.
        /// </summary>
        [Fact]
        public async Task Should_not_import_paradox_mod()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var gameService = new Mock<IGameService>();
            var modExport = new Mock<IModCollectionExporter>();
            DISetup.SetupContainer();
            gameService.Setup(s => s.GetSelected()).Returns(new Game()
            {
                Type = "no-items",
                UserDirectory = "C:\\fake"
            });
            modExport.Setup(p => p.ImportParadoxosAsync(It.IsAny<ModCollectionExporterParams>())).Returns((ModCollectionExporterParams p) =>
            {
                p.Mod.Name = "fake";
                return Task.FromResult(false);
            });

            var service = new ModCollectionService(null, null, new Cache(), null, null, null, null, gameService.Object, modExport.Object, storageProvider.Object, mapper.Object);
            var result = await service.ImportParadoxAsync();
            result.Should().BeNull();
        }

        /// <summary>
        /// Defines the test method Should_import_paradox_launcher_mod.
        /// </summary>
        [Fact]
        public async Task Should_import_paradox_launcher_mod()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var gameService = new Mock<IGameService>();
            var modExport = new Mock<IModCollectionExporter>();
            DISetup.SetupContainer();
            gameService.Setup(s => s.GetSelected()).Returns(new Game()
            {
                Type = "no-items",
                UserDirectory = "C:\\fake"
            });
            modExport.Setup(p => p.ImportParadoxLauncherAsync(It.IsAny<ModCollectionExporterParams>())).Returns((ModCollectionExporterParams p) =>
            {
                p.Mod.Name = "fake";
                return Task.FromResult(true);
            });

            var service = new ModCollectionService(null, null, new Cache(), null, null, null, null, gameService.Object, modExport.Object, storageProvider.Object, mapper.Object);
            var result = await service.ImportParadoxLauncherAsync();
            result.Name.Should().Be("fake");
        }

        /// <summary>
        /// Defines the test method Should_not_import_paradox_launcher_mod.
        /// </summary>
        [Fact]
        public async Task Should_not_import_paradox_launcher_mod()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var gameService = new Mock<IGameService>();
            var modExport = new Mock<IModCollectionExporter>();
            DISetup.SetupContainer();
            gameService.Setup(s => s.GetSelected()).Returns(new Game()
            {
                Type = "no-items",
                UserDirectory = "C:\\fake"
            });
            modExport.Setup(p => p.ImportParadoxLauncherAsync(It.IsAny<ModCollectionExporterParams>())).Returns((ModCollectionExporterParams p) =>
            {
                p.Mod.Name = "fake";
                return Task.FromResult(false);
            });

            var service = new ModCollectionService(null, null, new Cache(), null, null, null, null, gameService.Object, modExport.Object, storageProvider.Object, mapper.Object);
            var result = await service.ImportParadoxLauncherAsync();
            result.Should().BeNull();
        }

        /// <summary>
        /// Defines the test method Should_not_export_hash_report.
        /// </summary>
        [Fact]
        public async Task Should_not_export_hash_report()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var gameService = new Mock<IGameService>();
            var modExport = new Mock<IModCollectionExporter>();
            var hashExport = new Mock<IReportExportService>();
            var messageBus = new Mock<IMessageBus>();
            messageBus.Setup(p => p.PublishAsync(It.IsAny<IMessageBusEvent>()));
            messageBus.Setup(p => p.Publish(It.IsAny<IMessageBusEvent>()));
            DISetup.SetupContainer();

            var service = new ModCollectionService(messageBus.Object, hashExport.Object, new Cache(), null, null, null, null, gameService.Object, modExport.Object, storageProvider.Object, mapper.Object);
            var result = await service.ExportHashReportAsync(null, "test");
            result.Should().BeFalse();

            await service.ExportHashReportAsync(new List<IMod>(), string.Empty);
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_export_hash_report.
        /// </summary>
        [Fact]
        public async Task Should_export_hash_report()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var gameService = new Mock<IGameService>();
            gameService.Setup(s => s.GetSelected()).Returns(new Game()
            {
                Type = "no-items",
                UserDirectory = "C:\\fake",
                GameFolders = new List<string>() { "test" }
            });
            var collections = new List<IModCollection>()
            {
                new ModCollection()
                {
                    IsSelected = true,
                    Mods = new List<string>() { "mod/fakemod.mod"},
                    Name = "test",
                    Game = "no-items"
                }
            };
            storageProvider.Setup(s => s.GetModCollections()).Returns(() =>
            {
                return collections;
            });
            var modExport = new Mock<IModCollectionExporter>();
            var hashExport = new Mock<IReportExportService>();
            hashExport.Setup(p => p.ExportAsync(It.IsAny<IEnumerable<IHashReport>>(), It.IsAny<string>())).ReturnsAsync((IEnumerable<IHashReport> report, string path) =>
            {
                if (report.Count() == 1 && report.FirstOrDefault().Reports.Count == 1 && report.FirstOrDefault().Reports.FirstOrDefault().File == "test\\test" && report.FirstOrDefault().Reports.FirstOrDefault().Hash == "2")
                {
                    return true;
                }
                return false;
            });
            var messageBus = new Mock<IMessageBus>();
            messageBus.Setup(p => p.PublishAsync(It.IsAny<IMessageBusEvent>()));
            messageBus.Setup(p => p.Publish(It.IsAny<IMessageBusEvent>()));
            var reader = new Mock<IReader>();
            reader.Setup(p => p.GetFileInfo(It.IsAny<string>(), It.IsAny<string>())).Returns(new FileInfo()
            {
                ContentSHA = "2"
            });
            var modWriter = new Mock<IModWriter>();
            modWriter.Setup(p => p.ModDirectoryExistsAsync(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return Task.FromResult(false);
            });
            DISetup.SetupContainer();

            var service = new ModCollectionService(messageBus.Object, hashExport.Object, new Cache(), null, reader.Object, modWriter.Object, null, gameService.Object, modExport.Object, storageProvider.Object, mapper.Object);
            var result = await service.ExportHashReportAsync(new List<IMod>() {
                new Mod()
                {
                    Name = "test",
                    Files = new List<string>() {"test\\test"}
                } }, "test");
            result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_not_import_hash.
        /// </summary>
        [Fact]
        public async Task Should_not_import_hash()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var gameService = new Mock<IGameService>();
            gameService.Setup(s => s.GetSelected()).Returns(new Game()
            {
                Type = "no-items",
                UserDirectory = "C:\\fake",
                GameFolders = new List<string>() { "test" }
            });
            var collections = new List<IModCollection>()
            {
                new ModCollection()
                {
                    IsSelected = true,
                    Mods = new List<string>() { "mod/fakemod.mod"},
                    Name = "test",
                    Game = "no-items"
                }
            };
            storageProvider.Setup(s => s.GetModCollections()).Returns(() =>
            {
                return collections;
            });
            var modExport = new Mock<IModCollectionExporter>();
            var hashExport = new Mock<IReportExportService>();
            hashExport.Setup(p => p.GetCollectionReports(It.IsAny<IReadOnlyCollection<IHashReport>>())).Returns((IReadOnlyCollection<IHashReport> hashReports) =>
            {
                if (hashReports != null)
                {
                    return hashReports.Where(p => p.ReportType == HashReportType.Collection).ToList();
                }
                return null;
            });
            var messageBus = new Mock<IMessageBus>();
            messageBus.Setup(p => p.PublishAsync(It.IsAny<IMessageBusEvent>()));
            messageBus.Setup(p => p.Publish(It.IsAny<IMessageBusEvent>()));
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            modWriter.Setup(p => p.ModDirectoryExistsAsync(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return Task.FromResult(false);
            });
            DISetup.SetupContainer();

            var service = new ModCollectionService(messageBus.Object, hashExport.Object, new Cache(), null, reader.Object, modWriter.Object, null, gameService.Object, modExport.Object, storageProvider.Object, mapper.Object);
            var result = await service.ImportHashReportAsync(new List<IMod>() {
                new Mod()
                {
                    Name = "test",
                    Files = new List<string>() {"test\\test"}
                } }, null);
            result.Should().BeNull();
        }

        /// <summary>
        /// Defines the test method Should_import_hash.
        /// </summary>
        [Fact]
        public async Task Should_import_hash_from_both_sources()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var gameService = new Mock<IGameService>();
            gameService.Setup(s => s.GetSelected()).Returns(new Game()
            {
                Type = "no-items",
                UserDirectory = "C:\\fake",
                GameFolders = new List<string>() { "test" }
            });
            var collections = new List<IModCollection>()
            {
                new ModCollection()
                {
                    IsSelected = true,
                    Mods = new List<string>() { "mod/fakemod.mod"},
                    Name = "test",
                    Game = "no-items"
                }
            };
            storageProvider.Setup(s => s.GetModCollections()).Returns(() =>
            {
                return collections;
            });
            var modExport = new Mock<IModCollectionExporter>();
            var hashExport = new Mock<IReportExportService>();
            hashExport.Setup(p => p.GetCollectionReports(It.IsAny<IReadOnlyCollection<IHashReport>>())).Returns((IReadOnlyCollection<IHashReport> hashReports) =>
            {
                if (hashReports != null)
                {
                    return hashReports.Where(p => p.ReportType == HashReportType.Collection).ToList();
                }
                return null;
            });
            var messageBus = new Mock<IMessageBus>();
            messageBus.Setup(p => p.PublishAsync(It.IsAny<IMessageBusEvent>()));
            messageBus.Setup(p => p.Publish(It.IsAny<IMessageBusEvent>()));
            var reader = new Mock<IReader>();
            reader.Setup(p => p.GetFileInfo(It.IsAny<string>(), It.IsAny<string>())).Returns(new FileInfo()
            {
                FileName = "2",
                ContentSHA = "3"
            });
            var modWriter = new Mock<IModWriter>();
            modWriter.Setup(p => p.ModDirectoryExistsAsync(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return Task.FromResult(false);
            });
            DISetup.SetupContainer();

            var innerReports = new List<IHashFileReport>() { new HashFileReport() { File = "test\\1", Hash = "2" } };
            var outerReports = new List<IHashReport>() { new HashReport() { Name = "testreport", Reports = innerReports } };
            var service = new ModCollectionService(messageBus.Object, hashExport.Object, new Cache(), null, reader.Object, modWriter.Object, null, gameService.Object, modExport.Object, storageProvider.Object, mapper.Object);
            var result = await service.ImportHashReportAsync(new List<IMod>() {
                new Mod()
                {
                    Name = "testreport",
                    Files = new List<string>() {"test\\test"}
                } }, outerReports);
            result.Should().NotBeNull();
            result.Count().Should().Be(1);
            result.FirstOrDefault().Reports.Count.Should().Be(2);
        }

        /// <summary>
        /// Defines the test method Should_import_hash_with_diff_only.
        /// </summary>
        [Fact]
        public async Task Should_import_hash_with_diff_only()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var gameService = new Mock<IGameService>();
            gameService.Setup(s => s.GetSelected()).Returns(new Game()
            {
                Type = "no-items",
                UserDirectory = "C:\\fake",
                GameFolders = new List<string>() { "test" }
            });
            var collections = new List<IModCollection>()
            {
                new ModCollection()
                {
                    IsSelected = true,
                    Mods = new List<string>() { "mod/fakemod.mod"},
                    Name = "test",
                    Game = "no-items"
                }
            };
            storageProvider.Setup(s => s.GetModCollections()).Returns(() =>
            {
                return collections;
            });
            var modExport = new Mock<IModCollectionExporter>();
            var hashExport = new Mock<IReportExportService>();
            hashExport.Setup(p => p.GetCollectionReports(It.IsAny<IReadOnlyCollection<IHashReport>>())).Returns((IReadOnlyCollection<IHashReport> hashReports) =>
            {
                if (hashReports != null)
                {
                    return hashReports.Where(p => p.ReportType == HashReportType.Collection).ToList();
                }
                return null;
            });
            var messageBus = new Mock<IMessageBus>();
            messageBus.Setup(p => p.PublishAsync(It.IsAny<IMessageBusEvent>()));
            messageBus.Setup(p => p.Publish(It.IsAny<IMessageBusEvent>()));
            var reader = new Mock<IReader>();
            reader.Setup(p => p.GetFileInfo(It.IsAny<string>(), It.IsAny<string>())).Returns(new FileInfo()
            {
                FileName = "2",
                ContentSHA = "3"
            });
            var modWriter = new Mock<IModWriter>();
            modWriter.Setup(p => p.ModDirectoryExistsAsync(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return Task.FromResult(false);
            });
            DISetup.SetupContainer();

            var innerReport = new List<IHashFileReport>() { new HashFileReport() { File = "test\\test", Hash = "2" } };
            var outerReport = new List<IHashReport>() { new HashReport() { Name = "testreport", Reports = innerReport } };
            var service = new ModCollectionService(messageBus.Object, hashExport.Object, new Cache(), null, reader.Object, modWriter.Object, null, gameService.Object, modExport.Object, storageProvider.Object, mapper.Object);
            var result = await service.ImportHashReportAsync(new List<IMod>() {
                new Mod()
                {
                    Name = "testreport",
                    Files = new List<string>() {"test\\test"}
                } }, outerReport);
            result.Should().NotBeNull();
            result.Count().Should().Be(1);
            result.FirstOrDefault().Reports.Count.Should().Be(1);
        }

        /// <summary>
        /// Defines the test method Should_not_import_hash_when_hashes_same.
        /// </summary>
        [Fact]
        public async Task Should_not_import_hash_when_hashes_same()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var gameService = new Mock<IGameService>();
            gameService.Setup(s => s.GetSelected()).Returns(new Game()
            {
                Type = "no-items",
                UserDirectory = "C:\\fake",
                GameFolders = new List<string>() { "test" }
            });
            var collections = new List<IModCollection>()
            {
                new ModCollection()
                {
                    IsSelected = true,
                    Mods = new List<string>() { "mod/fakemod.mod"},
                    Name = "test",
                    Game = "no-items"
                }
            };
            storageProvider.Setup(s => s.GetModCollections()).Returns(() =>
            {
                return collections;
            });
            var modExport = new Mock<IModCollectionExporter>();
            var hashExport = new Mock<IReportExportService>();
            hashExport.Setup(p => p.GetCollectionReports(It.IsAny<IReadOnlyCollection<IHashReport>>())).Returns((IReadOnlyCollection<IHashReport> hashReports) =>
            {
                if (hashReports != null)
                {
                    return hashReports.Where(p => p.ReportType == HashReportType.Collection).ToList();
                }
                return null;
            });
            var messageBus = new Mock<IMessageBus>();
            messageBus.Setup(p => p.PublishAsync(It.IsAny<IMessageBusEvent>()));
            messageBus.Setup(p => p.Publish(It.IsAny<IMessageBusEvent>()));
            var reader = new Mock<IReader>();
            reader.Setup(p => p.GetFileInfo(It.IsAny<string>(), It.IsAny<string>())).Returns(new FileInfo()
            {
                FileName = "2",
                ContentSHA = "2"
            });
            var modWriter = new Mock<IModWriter>();
            modWriter.Setup(p => p.ModDirectoryExistsAsync(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return Task.FromResult(false);
            });
            DISetup.SetupContainer();

            var innterReport = new List<IHashFileReport>() { new HashFileReport() { File = "test\\test", Hash = "2" } };
            var outerReport = new List<IHashReport>() { new HashReport() { Name = "testreport", Reports = innterReport } };
            var service = new ModCollectionService(messageBus.Object, hashExport.Object, new Cache(), null, reader.Object, modWriter.Object, null, gameService.Object, modExport.Object, storageProvider.Object, mapper.Object);
            var result = await service.ImportHashReportAsync(new List<IMod>() {
                new Mod()
                {
                    Name = "testreport",
                    Files = new List<string>() {"test\\test"}
                } }, outerReport);
            result.Should().NotBeNull();
            result.Count().Should().Be(0);
        }
    }
}
