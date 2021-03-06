﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

namespace Microsoft.RESTier.Cli.EFTools.EntityDesign
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Data.Entity.Infrastructure;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Diagnostics;
    using System.Globalization;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    public class CodeFirstModelBuilderEngine : ModelBuilderEngine
    {
        protected override void ProcessModel(DbModel model, string storeModelNamespace, ModelBuilderSettings settings, 
            List<EdmSchemaError> errors)
        {
            ValidateModel(model, errors);
        }

        private static string GetStoreNamespace(ModelBuilderSettings settings)
        {
            return
                string.IsNullOrEmpty(settings.StorageNamespace)
                    ? String.Format(
                        CultureInfo.CurrentCulture,
                        "{0}.Store",
                        settings.ModelNamespace)
                    : settings.StorageNamespace;
        }

        public override void GenerateModel(ModelBuilderSettings settings)
        {
            

            var generatingModelWatch = Stopwatch.StartNew();

            // Clear out the ModelGenErrorCache before ModelGen begins
      

            var errors = new List<EdmSchemaError>();

            try
            {
                var storeModelNamespace = GetStoreNamespace(settings);
                Model = GenerateModels(storeModelNamespace, settings, errors);
                ProcessModel(Model, storeModelNamespace, settings, errors);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Data);
                throw;
            }
            finally
            {
                generatingModelWatch.Stop();
            }
        }

        private static void ValidateModel(DbModel model, List<EdmSchemaError> errors)
        {
            var settings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment };
            using (var writer = XmlWriter.Create(new StringBuilder(), settings))
            {
                var ssdlSerializer = new SsdlSerializer();
                ssdlSerializer.OnError +=
                    CreateOnErrorEventHandler(errors, ErrorCodes.GenerateModelFromDbReverseEngineerStoreModelFailed);

                ssdlSerializer.Serialize(
                    model.StoreModel,
                    model.ProviderInfo.ProviderInvariantName,
                    model.ProviderInfo.ProviderManifestToken,
                    writer);

                var csdlSerializer = new CsdlSerializer();
                csdlSerializer.OnError +=
                    CreateOnErrorEventHandler(errors, ErrorCodes.GenerateModelFromDbInvalidConceptualModel);

                csdlSerializer.Serialize(model.ConceptualModel, writer);

                new MslSerializerWrapper().Serialize(model, writer);
            }
        }

        private static EventHandler<DataModelErrorEventArgs> CreateOnErrorEventHandler(List<EdmSchemaError> errors, int errorCode)
        {
            return (sender, errorEventArgs) =>
                    errors.Add(
                        new EdmSchemaError(
                            errorEventArgs.ErrorMessage,
                            errorCode,
                            EdmSchemaErrorSeverity.Error));
        }
    }
}
