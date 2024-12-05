using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TimeTaskTracking.Infrastructure.Extensions
{
    public class ExecuteProcedureBioMatric
    {
        private readonly IConfiguration _configuration;

        private readonly string Conn_String;

        public ExecuteProcedureBioMatric(IConfiguration configuration)
        {
            _configuration = configuration;
            Conn_String = _configuration.GetConnectionString("BioMatricConnection");
        }


        private SqlDataAdapter? sqlDataAdapter = null;
        private SqlCommand? sqlCommand = null;

        private SqlConnection? sqlConnection = null;

        /// <summary>
        /// SQL Connection
        /// </summary>
        /// <returns></returns>
        private SqlConnection SQLConnection()
        {
            sqlConnection = new SqlConnection(Conn_String);
            if (sqlConnection.State != ConnectionState.Open)
            {
                try
                {
                    SqlConnection.ClearAllPools();
                    sqlConnection.Open();
                }
                catch (Exception)
                {
                    SqlConnection.ClearAllPools();
                    sqlConnection.Open();
                }
            }
            return sqlConnection;
        }

        /// <summary>
        /// Destroy Connection
        /// </summary>
        private void DestroyConnection()
        {
            sqlConnection.Close();
            sqlConnection.Dispose();
            SqlConnection.ClearAllPools();
        }
        public void SqlBulkyCopy(DataTable dtt, string tblName)
        {
            SqlConnection sqlConn = new(Conn_String);
            sqlConn.Open();
            SqlBulkCopy bulkCopy = new(sqlConn)
            {
                DestinationTableName = tblName
            };
            bulkCopy.WriteToServer(dtt);
            sqlConn.Close();
        }

        /// <summary>
        /// Login Stored Procedure Called
        /// </summary>
        /// <param name="storedProcedureName"></param>
        /// <param name="parameterNames"></param>
        /// <param name="parameterValues"></param>
        public async Task<DataTable?> Get_DataTableAsync(string storedProcedureName, string[] parameterNames, string[] parameterValues)
        {
            try
            {
                Task<DataTable> task1 = Task<DataTable>.Factory.StartNew(() =>
                {

                    DataTable dataTable = new();
                    sqlDataAdapter = new SqlDataAdapter(storedProcedureName, SQLConnection());
                    sqlDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                    if (parameterNames != null && parameterNames.Length > 0)
                    {
                        for (int i = 0; i < parameterNames.Length; i++)
                        {
                            _ = parameterValues[i] == null
                                ? sqlDataAdapter.SelectCommand.Parameters.AddWithValue(parameterNames[i], DBNull.Value)
                                : sqlDataAdapter.SelectCommand.Parameters.AddWithValue(parameterNames[i], parameterValues[i]);
                        }
                    }

                    sqlDataAdapter.SelectCommand.CommandTimeout = 300;
                    _ = sqlDataAdapter.Fill(dataTable);
                    sqlDataAdapter.Dispose();
                    DestroyConnection();

                    return dataTable;

                });
                return await task1.ConfigureAwait(false);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public DataTable Get_DataTable(string storedProcedureName, string[] parameterNames, string[] parameterValues)
        {
            try
            {
                DataTable dataTable = new();
                sqlDataAdapter = new SqlDataAdapter(storedProcedureName, SQLConnection());
                sqlDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                if (parameterNames != null && parameterNames.Length > 0)
                {
                    for (int i = 0; i < parameterNames.Length; i++)
                    {
                        _ = parameterValues[i] == null
                            ? sqlDataAdapter.SelectCommand.Parameters.AddWithValue(parameterNames[i], DBNull.Value)
                            : sqlDataAdapter.SelectCommand.Parameters.AddWithValue(parameterNames[i], parameterValues[i]);
                    }
                }

                sqlDataAdapter.SelectCommand.CommandTimeout = 300;
                _ = sqlDataAdapter.Fill(dataTable);
                sqlDataAdapter.Dispose();
                DestroyConnection();
                return dataTable;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Get Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <param name="columnsName"></param>
        /// <returns></returns>
        private static T GetObject<T>(DataRow row, List<string> columnsName) where T : new()
        {
            T obj = new();
            try
            {
                string columnname = "";
                string value = "";
                PropertyInfo[] Properties;
                Properties = typeof(T).GetProperties();
                foreach (PropertyInfo objProperty in Properties)
                {
                    columnname = columnsName.Find(name => name.ToLower() == objProperty.Name.ToLower());
                    if (!string.IsNullOrEmpty(columnname))
                    {
                        value = row[columnname].ToString();
                        if (!string.IsNullOrEmpty(value))
                        {
                            if (Nullable.GetUnderlyingType(objProperty.PropertyType) != null)
                            {
                                value = row[columnname].ToString().Replace("$", "").Replace(",", "");
                                objProperty.SetValue(obj, Convert.ChangeType(value, Type.GetType(Nullable.GetUnderlyingType(objProperty.PropertyType).ToString())), null);
                            }
                            else
                            {
                                value = row[columnname].ToString().Replace("%", "");
                                objProperty.SetValue(obj, Convert.ChangeType(value, Type.GetType(objProperty.PropertyType.ToString())), null);
                            }
                        }
                    }
                }
                return obj;
            }
            catch (Exception)
            {
                return obj;
            }
        }

        /// <summary>
        /// Convert Data Table To List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="datatable"></param>
        /// <returns></returns>

        public async Task<List<T>> DataTableToListAsync<T>(DataTable datatable) where T : new()
        {
            Task<List<T>> task1 = Task<List<T>>.Factory.StartNew(() =>
            {
                List<T> Temp = new();
                List<string> columnNames = datatable.Columns.Cast<DataColumn>().Select(c => c.ColumnName.ToLower()).ToList();
                Temp = datatable.AsEnumerable().ToList().ConvertAll(row => GetObject<T>(row, columnNames));
                return Temp;
            });

            return await task1.ConfigureAwait(false);

        }

        /// <summary>
        /// Execute Procedure With No Return Value. eg: Insert, Update, Delete 
        /// </summary>
        /// <param name="storedProcedureName">Stored Procedure Name.</param>
        /// <param name="parameterNames">Names of Parameter in String Array.</param>
        /// <param name="parameterValues">Values of Parameter in String Array.</param>
        public void ReturnEmpty(string storedProcedureName, string[] parameterNames, string[] parameterValues)
        {
            sqlCommand = new SqlCommand(storedProcedureName, SQLConnection())
            {
                CommandType = CommandType.StoredProcedure
            };
            if (parameterNames != null && parameterNames.Length > 0)
            {
                for (int i = 0; i < parameterNames.Length; i++)
                {
                    _ = parameterValues[i] == null
                        ? sqlCommand.Parameters.AddWithValue(parameterNames[i], DBNull.Value)
                        : sqlCommand.Parameters.AddWithValue(parameterNames[i], parameterValues[i]);
                }
            }
            _ = sqlCommand.ExecuteNonQuery();
            sqlCommand.Dispose();
            DestroyConnection();
        }

        public List<T> DataTableToList<T>(DataTable datatable) where T : new()
        {
            List<T> Temp = new();
            List<string> columnNames = datatable.Columns.Cast<DataColumn>().Select(c => c.ColumnName.ToLower()).ToList();
            Temp = datatable.AsEnumerable().ToList().ConvertAll(row => GetObject<T>(row, columnNames));
            return Temp;

        }

        /// <summary>
        /// convert datatable to onjet.   
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="datatable"></param>
        /// <returns></returns>
        public async Task<T> DataTableToModelAsync<T>(DataTable datatable) where T : new()
        {
            try
            {
                Task<T> task1 = Task<T>.Factory.StartNew(() =>
                {
                    List<T> Temp = new();
                    if (datatable != null)
                    {
                        List<string> columnNames = datatable.Columns.Cast<DataColumn>().Select(c => c.ColumnName.ToLower()).ToList();
                        Temp = datatable.AsEnumerable().ToList().ConvertAll(row => GetObject<T>(row, columnNames));
                        return Temp.First();
                    }
                    return Temp.First();
                });

                return await task1.ConfigureAwait(false);


            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.InnerException.ToString());
            }
        }

        public T DataTableToModel<T>(DataTable datatable) where T : new()
        {
            try
            {
                List<T> Temp = new();
                if (datatable != null)
                {
                    List<string> columnNames = datatable.Columns.Cast<DataColumn>().Select(c => c.ColumnName.ToLower()).ToList();
                    Temp = datatable.AsEnumerable().ToList().ConvertAll(row => GetObject<T>(row, columnNames));
                    return Temp.First();
                }
                return Temp.First();
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.InnerException.ToString());
            }
        }

        public async Task<DataTable> Get_DataTableAsync(string storedProcedureName, DataTable dt, string ColumnName, bool IsRef)
        {
            Task<DataTable> task1 = Task<DataTable>.Factory.StartNew(() =>
            {
                DataTable dataTable = new();
                sqlCommand = new SqlCommand(storedProcedureName, SQLConnection())
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 120
                };
                if (dt.Rows.Count > 0)
                {
                    _ = sqlCommand.Parameters.AddWithValue(ColumnName, dt);
                }

                using (SqlDataAdapter adp = new(sqlCommand))
                {
                    _ = adp.Fill(dataTable);
                }

                _ = sqlCommand.ExecuteNonQuery();
                sqlCommand.Dispose();
                DestroyConnection();
                return dataTable;


            });

            return await task1.ConfigureAwait(false);
        }

        public async Task<DataTable> ToDataTableAsync<T>(List<T> items)
        {
            Task<DataTable> task1 = Task<DataTable>.Factory.StartNew(() =>
            {

                DataTable dt = new(typeof(T).Name);
                PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo prop in Props)
                {
                    _ = dt.Columns.Add(prop.Name);
                }

                foreach (T item in items)
                {
                    object[] values = new object[Props.Length];
                    for (int i = 0; i < Props.Length; i++)
                    {
                        values[i] = Props[i].GetValue(item, null);
                    }

                    _ = dt.Rows.Add(values);
                }
                return dt;

            });
            return await task1.ConfigureAwait(false);
        }

        public async Task<DataSet?> Get_DataSetAsync(string storedProcedureName, string[] parameterNames, string[] parameterValues)
        {
            try
            {
                Task<DataSet> task1 = Task<DataSet>.Factory.StartNew(() =>
                {
                    DataSet dataSet = new();
                    sqlDataAdapter = new SqlDataAdapter(storedProcedureName, SQLConnection());
                    sqlDataAdapter.SelectCommand.CommandTimeout = 120;
                    sqlDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                    if (parameterNames != null && parameterNames.Length > 0)
                    {
                        for (int i = 0; i < parameterNames.Length; i++)
                        {
                            _ = parameterValues[i] == null
                                ? sqlDataAdapter.SelectCommand.Parameters.AddWithValue(parameterNames[i], DBNull.Value)
                                : sqlDataAdapter.SelectCommand.Parameters.AddWithValue(parameterNames[i], parameterValues[i]);
                        }
                    }
                    sqlDataAdapter.SelectCommand.CommandTimeout = 300;
                    _ = sqlDataAdapter.Fill(dataSet);
                    sqlDataAdapter.Dispose();
                    DestroyConnection();
                    return dataSet;
                });
                return await task1.ConfigureAwait(false);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public DataSet? Get_DataSet(string storedProcedureName, string[] parameterNames, string[] parameterValues)
        {
            try
            {
                DataSet dataSet = new();
                sqlDataAdapter = new SqlDataAdapter(storedProcedureName, SQLConnection());
                sqlDataAdapter.SelectCommand.CommandTimeout = 120;
                sqlDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                if (parameterNames != null && parameterNames.Length > 0)
                {
                    for (int i = 0; i < parameterNames.Length; i++)
                    {
                        _ = parameterValues[i] == null
                            ? sqlDataAdapter.SelectCommand.Parameters.AddWithValue(parameterNames[i], DBNull.Value)
                            : sqlDataAdapter.SelectCommand.Parameters.AddWithValue(parameterNames[i], parameterValues[i]);
                    }
                }
                sqlDataAdapter.SelectCommand.CommandTimeout = 300;
                _ = sqlDataAdapter.Fill(dataSet);
                sqlDataAdapter.Dispose();
                DestroyConnection();
                return dataSet;

            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
