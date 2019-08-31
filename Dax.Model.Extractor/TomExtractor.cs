using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tom = Microsoft.AnalysisServices.Tabular;

namespace Dax.Model.Extractor
{
    public class TomExtractor
    {
        protected Dax.Model.Model DaxModel { get; private set; }
        protected Tom.Model tomModel;
        private TomExtractor(Tom.Model model) : this(model, null, null) { }
        private TomExtractor(Tom.Model model, string extractorApp, string extractorVersion)
        {
            tomModel = model;
            AssemblyName tomExtractorAssemblyName = this.GetType().Assembly.GetName();
            Version version = tomExtractorAssemblyName.Version;
            DaxModel = new Dax.Model.Model(tomExtractorAssemblyName.Name, tomExtractorAssemblyName.Version.ToString(), extractorApp, extractorVersion);
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
            
        }

        private void AddRole( Tom.ModelRole role )
        {
            Dax.Model.Role daxRole = new Role(DaxModel)
            {
                RoleName = new DaxName(role.Name)
            };
            foreach ( var tablePermission in role.TablePermissions ) {
                Dax.Model.Table table = DaxModel.Tables.SingleOrDefault(t => t.TableName.Name == tablePermission.Table.Name);
                Dax.Model.TablePermission daxTablePermission = new TablePermission(daxRole)
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
            Dax.Model.Table fromTable = DaxModel.Tables.SingleOrDefault(t => t.TableName.Name == relationship.FromTable.Name);
            Dax.Model.Column fromColumn = fromTable.Columns.SingleOrDefault(t => t.ColumnName.Name == relationship.FromColumn.Name);
            Dax.Model.Table toTable = DaxModel.Tables.SingleOrDefault(t => t.TableName.Name == relationship.ToTable.Name);
            Dax.Model.Column toColumn = toTable.Columns.SingleOrDefault(t => t.ColumnName.Name == relationship.ToColumn.Name);
            Dax.Model.Relationship daxRelationship = new Dax.Model.Relationship (fromColumn, toColumn )
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

            Dax.Model.Table daxTable = new Dax.Model.Table(DaxModel)
            {
                TableName = new Dax.Model.DaxName(table.Name),
                IsHidden = table.IsHidden,
                TableExpression = Dax.Model.DaxExpression.GetExpression(isCalculatedTable ? partitionSource.Expression : null),
                TableType = isCalculatedTable ? Table.TableSourceType.CalculatedTable.ToString() :
                       (isCalculationGroup ? Table.TableSourceType.CalculationGroup.ToString() : null),
                Description = table.Description
            };
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
            Dax.Model.CalculationItem calcItem = new CalculationItem(calcGroup)
            {
                ItemExpression = DaxExpression.GetExpression(tomCalcItem.Expression),
                FormatStringDefinition = DaxExpression.GetExpression(tomCalcItem.FormatStringDefinition?.Expression),
                ItemName = new Dax.Model.DaxName(tomCalcItem.Name),
                State = tomCalcItem.State.ToString(),
                ErrorMessage = tomCalcItem.ErrorMessage,
                FormatStringState = tomCalcItem.FormatStringDefinition?.State.ToString(),
                FormatStringErrorMessage = tomCalcItem.FormatStringDefinition?.ErrorMessage
            };
            calcGroup.CalculationItems.Add(calcItem);
        }
        public void AddUserHierarchy( Table daxTable, Tom.Hierarchy hierarchy )
        {
            Dax.Model.UserHierarchy daxUserHierarchy = new Dax.Model.UserHierarchy ( daxTable )
            {
                HierarchyName = new Dax.Model.DaxName(hierarchy.Name),
                IsHidden = hierarchy.IsHidden,
            };
            // Create the hierarchy from the top to the bottom level 
            foreach ( var level in hierarchy.Levels.OrderBy( t => t.Ordinal ) ) 
            {
                Dax.Model.Column levelColumn = daxTable.Columns.Find(t => t.ColumnName.Name == level.Column.Name);
                daxUserHierarchy.Levels.Add(levelColumn);
            }
            daxTable.UserHierarchies.Add(daxUserHierarchy);
        }
        private void AddMeasure(Table daxTable, Tom.Measure measure)
        {
            Dax.Model.Measure daxMeasure = new Dax.Model.Measure
            {
                Table = daxTable,
                MeasureName = new Dax.Model.DaxName(measure.Name),
                MeasureExpression = Dax.Model.DaxExpression.GetExpression(measure?.Expression),
                DisplayFolder = measure.DisplayFolder,
                Description = measure.Description,
                IsHidden = measure.IsHidden,
                DataType = measure.DataType.ToString(),
                DetailRowsExpression = Dax.Model.DaxExpression.GetExpression(measure.DetailRowsDefinition?.Expression),
                FormatString = measure.FormatString,
                KpiStatusExpression = Dax.Model.DaxExpression.GetExpression(measure.KPI?.StatusExpression),
                KpiTargetExpression = Dax.Model.DaxExpression.GetExpression(measure.KPI?.TargetExpression),
                KpiTargetFormatString = measure.KPI?.TargetFormatString,
                KpiTrendExpression = Dax.Model.DaxExpression.GetExpression(measure.KPI?.TrendExpression)
            };
            daxTable.Measures.Add(daxMeasure);
        }

        private void AddColumn(Table daxTable, Tom.Column column)
        {
            Dax.Model.Column daxColumn = CreateColumn(daxTable, column);
            daxTable.Columns.Add(daxColumn);
        }
        private Dax.Model.Column CreateColumn (Table daxTable, Tom.Column column)
        {
            string calculatedColumnExpression =
                (column.Type == Tom.ColumnType.Calculated) ? (column as Tom.CalculatedColumn)?.Expression : null;

            return new Dax.Model.Column(daxTable)
            {
                ColumnName = new Dax.Model.DaxName(column.Name),
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
                ColumnExpression = Dax.Model.DaxExpression.GetExpression(calculatedColumnExpression),
                DisplayFolder = new Dax.Model.DaxNote(column.DisplayFolder),
                FormatString = column.FormatString,
                Description = new Dax.Model.DaxNote(column.Description)
            };
        }
        public static Dax.Model.Model GetDaxModel(Tom.Model model, string extractorApp, string extractorVersion)
        {
            TomExtractor extractor = new TomExtractor(model, extractorApp, extractorVersion);
            return extractor.DaxModel;
        }

        public static Microsoft.AnalysisServices.Database GetDatabase(string serverName, string databaseName)
        {
            Microsoft.AnalysisServices.Server server = new Microsoft.AnalysisServices.Server();
            server.Connect(serverName);
            Microsoft.AnalysisServices.Database db = server.Databases.FindByName(databaseName);
            return db;
        }

        public static Dax.Model.Model GetDaxModel(string serverName, string databaseName, string applicationName, string applicationVersion, bool readStatisticsFromData = true)
        {
            Microsoft.AnalysisServices.Database db = GetDatabase(serverName, databaseName);
            Microsoft.AnalysisServices.Tabular.Model tomModel = db.Model;

            var daxModel = Dax.Model.Extractor.TomExtractor.GetDaxModel(tomModel, applicationName, applicationVersion);

            var connectionString = GetConnectionString(serverName, databaseName);

            using (var connection = new OleDbConnection(connectionString))
            {
                // Populate statistics from DMV
                Dax.Model.Extractor.DmvExtractor.PopulateFromDmv(daxModel, connection, databaseName, applicationName, applicationVersion);

                // Populate statistics by querying the data model
                if (readStatisticsFromData)
                {
                    Dax.Model.Extractor.StatExtractor.UpdateStatisticsModel(daxModel, connection);
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
