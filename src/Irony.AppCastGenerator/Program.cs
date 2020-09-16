﻿// ***********************************************************************
// Assembly         : Irony.AppCastGenerator
// Author           : NetSparkle
// Created          : 09-14-2020
//
// Last Modified By : Mario
// Last Modified On : 09-16-2020
// ***********************************************************************
// <copyright file="Program.cs" company="NetSparkle">
//     NetSparkle
// </copyright>
// <summary>
// NetSparkle sounds like a good idea but some implementations behind it are bad (most generous description).
// Such as having a tool as a nuget but not a dotnet tool and little to no documentation on how it was envisioned to be used...
// </summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using CommandLine;
using NetSparkleUpdater;
using NetSparkleUpdater.AppCastHandlers;

namespace Irony.AppCastGenerator
{
    /// <summary>
    /// Class Program.
    /// </summary>
    internal class Program
    {
        #region Fields

        /// <summary>
        /// The signature manager
        /// </summary>
        private static readonly SignatureManager _signatureManager = new SignatureManager();

        #endregion Fields

        #region Methods

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(Run)
                .WithNotParsed(HandleParseError);
        }

        /// <summary>
        /// Gets the version from assembly.
        /// </summary>
        /// <param name="fileInfo">The file information.</param>
        /// <returns>System.String.</returns>
        private static string GetVersionFromAssembly(FileInfo fileInfo)
        {
            return FileVersionInfo.GetVersionInfo(fileInfo.FullName).ProductVersion.Split("+", StringSplitOptions.RemoveEmptyEntries)[0];
        }

        /// <summary>
        /// Handles the parse error.
        /// </summary>
        /// <param name="errs">The errs.</param>
        private static void HandleParseError(IEnumerable<Error> errs)
        {
            errs.Output();
        }

        /// <summary>
        /// Runs the specified opts.
        /// </summary>
        /// <param name="opts">The opts.</param>
        private static void Run(Options opts)
        {
            // Really really lacks documentation
            _signatureManager.SetStorageDirectory(opts.KeyLocation);

            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            if (opts.Export)
            {
                Console.WriteLine("Private Key:");
                Console.WriteLine(Convert.ToBase64String(_signatureManager.GetPrivateKey()));
                Console.WriteLine("Public Key:");
                Console.WriteLine(Convert.ToBase64String(_signatureManager.GetPublicKey()));
                return;
            }

            if (opts.GenerateKeys)
            {
                var didSucceed = _signatureManager.Generate(opts.ForceRegeneration);
                if (didSucceed)
                {
                    Console.WriteLine("Keys successfully generated", Color.Green);
                }
                else
                {
                    Console.WriteLine("Keys failed to generate", Color.Red);
                }
                return;
            }

            if (opts.BinaryToSign != null)
            {
                var signature = _signatureManager.GetSignatureForFile(new FileInfo(opts.BinaryToSign));

                Console.WriteLine($"Signature: {signature}", Color.Green);

                return;
            }

            if (opts.BinaryToVerify != null)
            {
                var result = _signatureManager.VerifySignature(new FileInfo(opts.BinaryToVerify), opts.Signature);

                if (result)
                {
                    Console.WriteLine($"Signature valid", Color.Green);
                }
                else
                {
                    Console.WriteLine($"Signature invalid", Color.Red);
                }

                return;
            }

            var search = $"*.{opts.Extension}";

            if (opts.SourceBinaryDirectory == ".")
            {
                opts.SourceBinaryDirectory = Environment.CurrentDirectory;
            }

            var binaries = Directory.GetFiles(opts.SourceBinaryDirectory, search);

            if (binaries.Length == 0)
            {
                Console.WriteLine($"No files founds matching {search} in {opts.SourceBinaryDirectory}", Color.Yellow);
                Environment.Exit(1);
            }

            var outputDirectory = opts.SourceBinaryDirectory;

            Console.WriteLine("");
            Console.WriteLine($"Searching: {opts.SourceBinaryDirectory}", Color.Blue);
            Console.WriteLine($"Found {binaries.Count()} {opts.Extension} files(s)", Color.Blue);
            Console.WriteLine("");

            try
            {
                var productName = opts.ProductName;

                var items = new List<AppCastItem>();

                var hasChangelog = File.Exists(opts.ChangeLogPath);

                var versionInfo = GetVersionFromAssembly(new FileInfo(Path.Combine(opts.VersionExtractionPath, "IronyModManager.exe")));

                foreach (var binary in binaries)
                {
                    var fileInfo = new FileInfo(binary);

                    var productVersion = versionInfo;
                    var itemTitle = string.IsNullOrWhiteSpace(productName) ? productVersion : productName + " " + productVersion;
                    var remoteUpdateFile = $"{opts.BaseUrl}/{(opts.PrefixVersion ? $"{versionInfo}/" : "")}{HttpUtility.UrlEncode(fileInfo.Name)}";

                    string os = string.Empty;
                    if (binary.Contains("win", StringComparison.OrdinalIgnoreCase))
                    {
                        os = "windows";
                    }
                    else if (binary.Contains("linux", StringComparison.OrdinalIgnoreCase))
                    {
                        os = "linux";
                    }
                    else if (binary.Contains("osx", StringComparison.OrdinalIgnoreCase))
                    {
                        os = "osx";
                    }
                    //
                    var item = new AppCastItem()
                    {
                        Title = itemTitle,
                        DownloadLink = remoteUpdateFile,
                        Version = productVersion,
                        ShortVersion = productVersion.Substring(0, productVersion.LastIndexOf('.')),
                        PublicationDate = fileInfo.CreationTime,
                        UpdateSize = fileInfo.Length,
                        Description = "",
                        DownloadSignature = _signatureManager.KeysExist() ? _signatureManager.GetSignatureForFile(fileInfo) : null,
                        OperatingSystemString = os,
                        MIMEType = MimeTypes.GetMimeType(fileInfo.Name)
                    };

                    if (hasChangelog)
                    {
                        item.Description = File.ReadAllText(opts.ChangeLogPath);
                    }

                    items.Add(item);
                }

                var appcastXmlDocument = XMLAppCast.GenerateAppCastXml(items, productName);

                var appcastFileName = Path.Combine(outputDirectory, opts.AppCastFileName);

                var dirName = Path.GetDirectoryName(appcastFileName);

                if (!Directory.Exists(dirName))
                {
                    Console.WriteLine("Creating {0}", dirName);
                    Directory.CreateDirectory(dirName);
                }

                Console.WriteLine("Writing appcast to {0}", appcastFileName);

                using (var w = XmlWriter.Create(appcastFileName, new XmlWriterSettings { NewLineChars = "\n", Encoding = new UTF8Encoding(false) }))
                {
                    appcastXmlDocument.Save(w);
                }

                if (_signatureManager.KeysExist())
                {
                    var appcastFile = new FileInfo(appcastFileName);
                    var signatureFile = appcastFileName + ".signature";
                    var signature = _signatureManager.GetSignatureForFile(appcastFile);

                    var result = _signatureManager.VerifySignature(appcastFile, signature);

                    if (result)
                    {
                        File.WriteAllText(signatureFile, signature);
                        Console.WriteLine($"Wrote {signatureFile}", Color.Green);
                    }
                    else
                    {
                        Console.WriteLine($"Failed to verify {signatureFile}", Color.Red);
                    }
                }
                else
                {
                    Console.WriteLine("Skipped generating signature.  Generate keys with --generate-keys", Color.Red);
                    Environment.Exit(1);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine();
                Environment.Exit(1);
            }
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Class Options.
        /// </summary>
        public class Options
        {
            #region Properties

            /// <summary>
            /// Gets or sets the output directory.
            /// </summary>
            /// <value>The output directory.</value>
            [Option('a', "appcast-file-name", Required = false, HelpText = "Appcast filename")]
            public string AppCastFileName { get; set; }

            /// <summary>
            /// Gets or sets the base URL.
            /// </summary>
            /// <value>The base URL.</value>
            [Option('u', "base-url", SetName = "local", Required = false, HelpText = "Base URL for downloads", Default = null)]
            public Uri BaseUrl { get; set; }

            /// <summary>
            /// Gets or sets the binary to sign.
            /// </summary>
            /// <value>The binary to sign.</value>
            [Option("generate-signature", SetName = "signing", Required = false, HelpText = "Generate signature from binary")]
            public string BinaryToSign { get; set; }

            /// <summary>
            /// Gets or sets the binary to verify.
            /// </summary>
            /// <value>The binary to verify.</value>
            [Option("verify", SetName = "verify", Required = false, HelpText = "Binary to verify")]
            public string BinaryToVerify { get; set; }

            /// <summary>
            /// Gets or sets the change log path.
            /// </summary>
            /// <value>The change log path.</value>
            [Option('p', "change-log-path", SetName = "local", Required = false, HelpText = "File path to change log.", Default = "")]
            public string ChangeLogPath { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether this <see cref="Options" /> is export.
            /// </summary>
            /// <value><c>true</c> if export; otherwise, <c>false</c>.</value>
            [Option("export", SetName = "keys", Required = false, HelpText = "Export keys")]
            public bool Export { get; set; }

            /// <summary>
            /// Gets or sets the extension.
            /// </summary>
            /// <value>The extension.</value>
            [Option('e', "ext", SetName = "local", Required = false, HelpText = "Search for file extensions.", Default = "exe")]
            public string Extension { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether [force regeneration].
            /// </summary>
            /// <value><c>true</c> if [force regeneration]; otherwise, <c>false</c>.</value>
            [Option("force", SetName = "keys", Required = false, HelpText = "Force regeneration of keys")]
            public bool ForceRegeneration { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether [generate keys].
            /// </summary>
            /// <value><c>true</c> if [generate keys]; otherwise, <c>false</c>.</value>
            [Option("generate-keys", SetName = "keys", Required = false, HelpText = "Generate keys")]
            public bool GenerateKeys { get; set; }

            /// <summary>
            /// Gets or sets the key location.
            /// </summary>
            /// <value>The key location.</value>
            [Option("key-location", Required = false, HelpText = "Key location")]
            public string KeyLocation { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether [prefix version].
            /// </summary>
            /// <value><c>true</c> if [prefix version]; otherwise, <c>false</c>.</value>
            [Option('x', "url-prefix-version", SetName = "local", Required = false, HelpText = "Add the version as a prefix to the download url")]
            public bool PrefixVersion { get; set; }

            /// <summary>
            /// Gets or sets the name of the product.
            /// </summary>
            /// <value>The name of the product.</value>
            [Option('n', "product-name", Required = false, HelpText = "Product Name", Default = "Application")]
            public string ProductName { get; set; }

            /// <summary>
            /// Gets or sets the signature.
            /// </summary>
            /// <value>The signature.</value>
            [Option("signature", SetName = "verify", Required = false, HelpText = "Signature")]
            public string Signature { get; set; }

            /// <summary>
            /// Gets or sets the source binary directory.
            /// </summary>
            /// <value>The source binary directory.</value>
            [Option('b', "binaries", SetName = "local", Required = false, HelpText = "Directory containing binaries.", Default = ".")]
            public string SourceBinaryDirectory { get; set; }

            /// <summary>
            /// Gets or sets the version extraction path.
            /// </summary>
            /// <value>The version extraction path.</value>
            [Option('f', "file-extract-version", SetName = "local", Required = false, HelpText = "Location where to extract version info from.", Default = false)]
            public string VersionExtractionPath { get; set; }

            #endregion Properties
        }

        #endregion Classes
    }
}
