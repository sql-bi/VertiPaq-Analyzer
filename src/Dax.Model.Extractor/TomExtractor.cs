using Microsoft.AnalysisServices.AdomdClient;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tom = Microsoft.AnalysisServices.Tabular;

namespace Dax.Metadata.Extractor
{
    public class TomExtractor
    {
        protected Dax.Metadata.Model DaxModel { get; private set; }
        protected Tom.Model tomModel;
        private TomExtractor(Tom.Model model, string extractorApp = null, string extractorVersion = null)
        {
            tomModel = model;
            AssemblyName tomExtractorAssemblyName = this.GetType().Assembly.GetName();
            Version version = tomExtractorAssemblyName.Version;
            DaxModel = new Dax.Metadata.Model(tomExtractorAssemblyName.Name, version.ToString(), extractorApp, extractorVersion);
            if (tomModel != null) {
                PopulateModel();
            }
        }
        
        private void PopulateModel()
        {
            foreach (var table in tomModel.Tables) {
                AddTable(table);
            }
           
            foreach (Tom.SingleColumnRelationship relationship in tomModel.Relationships) {
                AddRelationship(relationship);
            }
            foreach (Tom.ModelRole role in tomModel.Roles) {
                AddRole(role);
            }

            // Compatibility Level and Mode
            DaxModel.CompatibilityLevel = tomModel.Database.CompatibilityLevel;
            DaxModel.CompatibilityMode = tomModel.Database.CompatibilityMode.ToString();

            // Database version and last update and process date and time
            DaxModel.LastProcessed = tomModel.Database.LastProcessed;
            DaxModel.LastUpdate = tomModel.Database.LastUpdate;
            DaxModel.Version = tomModel.Database.Version;

            // Update ExtractionDate
            DaxModel.ExtractionDate = DateTime.UtcNow;
        }

        private void AddRole( Tom.ModelRole role )
        {
            Dax.Metadata.Role daxRole = new Role(DaxModel)
            {
                RoleName = new DaxName(role.Name)
            };
            foreach ( var tablePermission in role.TablePermissions ) {
                Dax.Metadata.Table table = DaxModel.Tables.SingleOrDefault(t => t.TableName.Name == tablePermission.Table.Name);
                Dax.Metadata.TablePermission daxTablePermission = new TablePermission(daxRole)
                {
                    Table = table,
                    FilterExpression = DaxExpression.GetExpression(tablePermission.FilterExpression)
                };

                daxRole.TablePermissions.Add(daxTablePermission);
            }

            DaxModel.Roles.Add(daxRole);
        }

        private void AddRelationship(Tom.SingleColumnRelationship relationship)
        {
            Dax.Metadata.Table fromTable = DaxModel.Tables.SingleOrDefault(t => t.TableName.Name == relationship.FromTable.Name);
            Dax.Metadata.Column fromColumn = fromTable.Columns.SingleOrDefault(t => t.ColumnName.Name == relationship.FromColumn.Name);
            Dax.Metadata.Table toTable = DaxModel.Tables.SingleOrDefault(t => t.TableName.Name == relationship.ToTable.Name);
            Dax.Metadata.Column toColumn = toTable.Columns.SingleOrDefault(t => t.ColumnName.Name == relationship.ToColumn.Name);
            Dax.Metadata.Relationship daxRelationship = new Dax.Metadata.Relationship (fromColumn, toColumn )
            {
                FromCardinalityType = relationship.FromCardinality.ToString(),
                ToCardinalityType = relationship.ToCardinality.ToString(),
                RelyOnReferentialIntegrity = relationship.RelyOnReferentialIntegrity,
                JoinOnDateBehavior = relationship.JoinOnDateBehavior.ToString(),
                CrossFilteringBehavior = relationship.CrossFilteringBehavior.ToString(),
                Type = relationship.Type.ToString(),
                IsActive = relationship.IsActive,
                Name = relationship.Name,
                SecurityFilteringBehavior = relationship.SecurityFilteringBehavior.ToString()
            };
            DaxModel.Relationships.Add(daxRelationship);
        }

        private void AddTable(Tom.Table table)
        {
            var partitions = table.Partitions;
            Tom.PartitionSourceType tableType = (partitions?.Count > 0) ? (partitions[0].SourceType) : Tom.PartitionSourceType.None;
            bool isCalculatedTable = (tableType == Tom.PartitionSourceType.Calculated);
            bool isCalculationGroup = (tableType == Tom.PartitionSourceType.CalculationGroup);
            var partitionSource = (isCalculatedTable) ? partitions[0].Source as Tom.CalculatedPartitionSource : null;

            Dax.Metadata.Table daxTable = new Dax.Metadata.Table(DaxModel)
            {
                TableName = new Dax.Metadata.DaxName(table.Name),
                IsHidden = table.IsHidden,
                IsPrivate = table.IsPrivate,
                IsLocalDateTable = (table.Annotations.FirstOrDefault(a => a.Name == "__PBI_LocalDateTable" && a.Value == "true") != null),
                IsTemplateDateTable = (table.Annotations.FirstOrDefault(a => a.Name == "__PBI_TemplateDateTable" && a.Value == "true") != null),
                TableExpression = Dax.Metadata.DaxExpression.GetExpression(isCalculatedTable ? partitionSource.Expression : null),
                TableType = isCalculatedTable ? Table.TableSourceType.CalculatedTable.ToString() :
                       (isCalculationGroup ? Table.TableSourceType.CalculationGroup.ToString() : null),
                Description = table.Description
            };

            daxTable.IsDateTable = table.DataCategory == Microsoft.AnalysisServices.DimensionType.Time.ToString();
            if (daxTable.IsDateTable == false)
            {
                daxTable.IsDateTable = table.Columns.SingleOrDefault((c) => c.IsKey && c.DataType == Tom.DataType.DateTime) != null;
                if (daxTable.IsDateTable == false)
                {
                    daxTable.IsDateTable = table.Model.Relationships.OfType<Tom.SingleColumnRelationship>().Any((r) =>
                    {
                        return r.IsActive &&
                        (
                            (
                                r.ToTable == table &&
                                r.ToColumn.DataType == Tom.DataType.DateTime &&
                                r.ToCardinality == Tom.RelationshipEndCardinality.One
                            )
                            ||
                            (
                                r.FromTable == table &&
                                r.FromColumn.DataType == Tom.DataType.DateTime &&
                                r.FromCardinality == Tom.RelationshipEndCardinality.One
                            )
                        );
                    });
                }
            }

            foreach (var column in table.Columns) {
                AddColumn(daxTable, column);
            }
            foreach (var measure in table.Measures) {
                AddMeasure(daxTable, measure);
            }
            foreach (var hierarchy in table.Hierarchies) {
                AddUserHierarchy(daxTable, hierarchy);
            }

            // Add calculation groups and calculation items
            if (table.CalculationGroup != null) {
                var calcGroup = new CalculationGroup(daxTable)
                {
                    Precedence = table.CalculationGroup.Precedence
                };
                foreach ( var calcItem in table.CalculationGroup.CalculationItems) {
                    AddCalculationItem(calcGroup, calcItem);
                }
                daxTable.CalculationGroup = calcGroup;

                // Set the first column of the table that is not a RowNumber as a calculation group attribute
                foreach (var column in daxTable.Columns) {
                    if (!column.IsRowNumber) {
                        column.IsCalculationGroupAttribute = true;
                        break;
                    }
                }
            }

            DaxModel.Tables.Add(daxTable);

        }
        public void AddCalculationItem(CalculationGroup calcGroup, Tom.CalculationItem tomCalcItem)
        {
            Dax.Metadata.CalculationItem calcItem = new CalculationItem(calcGroup)
            {
                ItemExpression = DaxExpression.GetExpression(tomCalcItem.Expression),
                FormatStringDefinition = DaxExpression.GetExpression(tomCalcItem.FormatStringDefinition?.Expression),
                ItemName = new Dax.Metadata.DaxName(tomCalcItem.Name),
                State = tomCalcItem.State.ToString(),
                ErrorMessage = tomCalcItem.ErrorMessage,
                FormatStringState = tomCalcItem.FormatStringDefinition?.State.ToString(),
                FormatStringErrorMessage = tomCalcItem.FormatStringDefinition?.ErrorMessage,
                Description = !string.IsNullOrEmpty(tomCalcItem?.Description) ? new Dax.Metadata.DaxNote(tomCalcItem.Description) : null
            };
            calcGroup.CalculationItems.Add(calcItem);
        }
        public void AddUserHierarchy( Table daxTable, Tom.Hierarchy hierarchy )
        {
            Dax.Metadata.UserHierarchy daxUserHierarchy = new Dax.Metadata.UserHierarchy ( daxTable )
            {
                HierarchyName = new Dax.Metadata.DaxName(hierarchy.Name),
                IsHidden = hierarchy.IsHidden,
            };
            // Create the hierarchy from the top to the bottom level 
            foreach ( var level in hierarchy.Levels.OrderBy( t => t.Ordinal ) ) 
            {
                Dax.Metadata.Column levelColumn = daxTable.Columns.Find(t => t.ColumnName.Name == level.Column.Name);
                daxUserHierarchy.Levels.Add(levelColumn);
            }
            daxTable.UserHierarchies.Add(daxUserHierarchy);
        }
        private void AddMeasure(Table daxTable, Tom.Measure measure)
        {
            Dax.Metadata.Measure daxMeasure = new Dax.Metadata.Measure
            {
                Table = daxTable,
                MeasureName = new Dax.Metadata.DaxName(measure.Name),
                MeasureExpression = Dax.Metadata.DaxExpression.GetExpression(measure?.Expression),
                DisplayFolder = measure.DisplayFolder,
                Description = measure.Description,
                IsHidden = measure.IsHidden,
                DataType = measure.DataType.ToString(),
                DetailRowsExpression = Dax.Metadata.DaxExpression.GetExpression(measure.DetailRowsDefinition?.Expression),
                FormatString = measure.FormatString,
                KpiStatusExpression = Dax.Metadata.DaxExpression.GetExpression(measure.KPI?.StatusExpression),
                KpiTargetExpression = Dax.Metadata.DaxExpression.GetExpression(measure.KPI?.TargetExpression),
                KpiTargetFormatString = measure.KPI?.TargetFormatString,
                KpiTrendExpression = Dax.Metadata.DaxExpression.GetExpression(measure.KPI?.TrendExpression)
            };
            daxTable.Measures.Add(daxMeasure);
        }

        private void AddColumn(Table daxTable, Tom.Column column)
        {
            Dax.Metadata.Column daxColumn = CreateColumn(daxTable, column);
            daxTable.Columns.Add(daxColumn);
        }
        private Dax.Metadata.Column CreateColumn (Table daxTable, Tom.Column column)
        {
            string calculatedColumnExpression =
                (column.Type == Tom.ColumnType.Calculated) ? (column as Tom.CalculatedColumn)?.Expression : null;

            return new Dax.Metadata.Column(daxTable)
            {
                ColumnName = new Dax.Metadata.DaxName(column.Name),
                DataType = column.DataType.ToString(),
                IsHidden = column.IsHidden,
                EncodingHint = column.EncodingHint.ToString(),
                IsAvailableInMDX = column.IsAvailableInMDX,
                IsKey = column.IsKey,
                IsNullable = column.IsNullable,
                IsUnique = column.IsUnique,
                KeepUniqueRows = column.KeepUniqueRows,
                SortByColumnName = column.SortByColumn?.Name,
                IsRowNumber = (column.Type == Tom.ColumnType.RowNumber),
                State = column.State.ToString(),
                ColumnType = column.Type.ToString(),
                ColumnExpression = Dax.Metadata.DaxExpression.GetExpression(calculatedColumnExpression),
                DisplayFolder = new Dax.Metadata.DaxNote(column.DisplayFolder),
                FormatString = column.FormatString,
                Description = new Dax.Metadata.DaxNote(column.Description)
            };
        }
        public static Dax.Metadata.Model GetDaxModel(Tom.Model model, string extractorApp, string extractorVersion)
        {
            TomExtractor extractor = new TomExtractor(model, extractorApp, extractorVersion);
            return extractor.DaxModel;
        }

        public static Tom.Database GetDatabase(string serverName, string databaseName)
        {
            Tom.Server server = new Tom.Server();
            server.Connect(serverName);
            Tom.Database db = server.Databases.FindByName(databaseName);
            // if db is null either it does not exist or we do not have admin rights to it
            if (db == null) { throw new ArgumentException($"The database '{databaseName}' could not be found. Either it does not exist or you do not have admin rights to it."); }
            return db;
        }

        public static Dax.Metadata.Model GetDaxModel(string serverName, string databaseName, string applicationName, string applicationVersion, bool readStatisticsFromData = true, int sampleRows = 0)
        {
            Tom.Database db = GetDatabase(serverName, databaseName);
            Tom.Model tomModel = db.Model;

            var daxModel = Dax.Metadata.Extractor.TomExtractor.GetDaxModel(tomModel, applicationName, applicationVersion);

            var connectionString = GetConnectionString(serverName, databaseName);

            using (var connection = new AdomdConnection(connectionString))
            {
                // Populate statistics from DMV
                Dax.Metadata.Extractor.DmvExtractor.PopulateFromDmv(daxModel, connection, serverName, databaseName, applicationName, applicationVersion);

                // Populate statistics by querying the data model
                if (readStatisticsFromData)
                {
                    Dax.Metadata.Extractor.StatExtractor.UpdateStatisticsModel(daxModel, connection, sampleRows);
                }
            }
            return daxModel;
        }

        private static string GetConnectionString(string dataSourceOrConnectionString, string databaseName)
        {
            var csb = new OleDbConnectionStringBuilder();
            try
            {
                csb.ConnectionString = dataSourceOrConnectionString;
            }
            catch
            {
                // Assume servername
                csb.Provider = "MSOLAP";
                csb.DataSource = dataSourceOrConnectionString;
            }
            csb["Initial Catalog"] = databaseName;
            return csb.ConnectionString;
        }
    }
}
