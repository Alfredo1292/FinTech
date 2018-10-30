using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using TwoFunTwoMe_Common;

namespace TwoFunTwoMe_DataAccess
{
    public class DynamicSqlDAO
    {
        public static DynamicDto ExecuterSpOld(DynamicDto xDynamicDto, string xConnectionString)
        {
            try
            {
                DataSet dataSet1 = new DataSet();
                if (xDynamicDto.ParameterList.Count >= 1)
                    dataSet1 = DynamicSqlDAO.GetStoreParameter(xDynamicDto, xConnectionString);
                using (SqlConnection connection = new SqlConnection(xConnectionString))
                {
                    connection.Open();
                    using (SqlCommand selectCommand = new SqlCommand(xDynamicDto.SPName, connection))
                    {
                        int result = 150000;

                        selectCommand.CommandTimeout = result;
                        selectCommand.CommandType = CommandType.StoredProcedure;
                        if (dataSet1 != null && dataSet1.Tables.Count > 0)
                        {
                            foreach (DataRow row in (InternalDataCollectionBase)dataSet1.Tables[0].Rows)
                            {
                                DataRow store = row;
                                foreach (SpParameter spParameter in xDynamicDto.ParameterList.Where<SpParameter>((Func<SpParameter, bool>)(parameter => store["column_name"].ToString().Replace("@", string.Empty).ToLower().Equals(parameter.Name.Replace("@", string.Empty).ToLower()))))
                                {
                                    SqlDbType sqlDbType = (SqlDbType)Enum.Parse(typeof(SqlDbType), row["type_name"].ToString(), true);
                                    selectCommand.Parameters.Add(spParameter.Name, sqlDbType, (int)row["LENGTH"]);
                                    selectCommand.Parameters[spParameter.Name].Value = (object)spParameter.Value;
                                }
                            }
                        }
                        using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(selectCommand))
                        {
                            DataSet dataSet2 = new DataSet();
                            sqlDataAdapter.Fill(dataSet2);
                            xDynamicDto.Result = dataSet2;
                            connection.Close();
                        }
                    }
                    return xDynamicDto;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DynamicDto ExecuterSp(DynamicDto xDynamicDto, string xConnectionString)
        {
            try
            {
                SqlDatabase sqlDatabase = new SqlDatabase(xConnectionString);
                using (DbCommand storedProcCommand = ((Database)sqlDatabase).GetStoredProcCommand(xDynamicDto.SPName))
                {
                    int result = 150000;
                    storedProcCommand.CommandTimeout = result;
                    ((Database)sqlDatabase).DiscoverParameters(storedProcCommand);
                    foreach (SqlParameter parameter in storedProcCommand.Parameters)
                    {
                        SqlParameter xParameter = parameter;
                        foreach (SpParameter spParameter in xDynamicDto.ParameterList.Where<SpParameter>((Func<SpParameter, bool>)(param => xParameter.ParameterName.Equals(string.Format("@{0}", (object)param.Name)))))
                            xParameter.Value = (object)spParameter.Value;
                    }
                    using (DataSet dataSet = ((Database)sqlDatabase).ExecuteDataSet(storedProcCommand))
                        xDynamicDto.Result = dataSet;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return xDynamicDto;
        }

        private static DataSet GetStoreParameter(DynamicDto xDynamicDto, string xConnectionString)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(xConnectionString))
                {
                    connection.Open();
                    DataSet dataSet = new DataSet();
                    SqlCommand sqlCommand = new SqlCommand("sp_sproc_columns", connection);
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    SqlCommand selectCommand = sqlCommand;
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(selectCommand))
                    {
                        selectCommand.Parameters.Add(new SqlParameter("procedure_name", (object)xDynamicDto.SPName));
                        sqlDataAdapter.Fill(dataSet);
                        xDynamicDto.Result = dataSet;
                        connection.Close();
                    }
                    connection.Close();
                    return dataSet;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ExecuteBulkCopy(DataTable xDataTable, string xDestinationTableName, string xConnectionString, int xBatchSize, int xBulkCopyTimeout, List<BulkCopyColumnMappings> xBulkCopyColumnMappings)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(xConnectionString))
                {
                    sqlConnection.Open();
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(sqlConnection.ConnectionString))
                    {
                        sqlBulkCopy.BatchSize = xBatchSize;
                        sqlBulkCopy.BulkCopyTimeout = xBulkCopyTimeout;
                        sqlBulkCopy.DestinationTableName = xDestinationTableName;
                        foreach (BulkCopyColumnMappings copyColumnMapping in xBulkCopyColumnMappings)
                            sqlBulkCopy.ColumnMappings.Add(copyColumnMapping.NombreColumnaOrigen, copyColumnMapping.NombreColumnaDestino);
                        sqlBulkCopy.WriteToServer(xDataTable);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
