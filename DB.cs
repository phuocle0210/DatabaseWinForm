using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _20_15_qlbangiay_
{
    internal class DatabaseConnection
    {
        private string dataSource;
        private string database;
        private string tableName;
        private string query;
        private SqlConnection connect;

        //Nhớ config trước khi sử dụng
        public DatabaseConnection()
        {
            //điền vào server sql của bạn
            this.dataSource = "DESKTOP-FJ0Q38O\\SQLEXPRESS";

            //điền vào tên database
            this.database = "QLBanGiay";

            this.ketNoi(this.dataSource, this.database);
        }

        public void setTableName(string tableName)
        {
            this.tableName = tableName;
            this.query = $"SELECT * FROM {tableName}";
        }

        public Boolean ketNoi(String dataSource, String database)
        {
            try
            {
                this.connect = new SqlConnection();
                this.connect.ConnectionString = $"Data source = {dataSource}; Initial Catalog={database}; integrated Security=True";
            }
            catch (Exception)
            {

                return false;
            }

            return true;
        }

        protected DataSet getData(String sql = "")
        {
            SqlDataAdapter adapter = new SqlDataAdapter(sql, this.connect); ;
            DataSet data = new DataSet();
            adapter.Fill(data);
            return data;
        }

        protected int capNhatDuLieu()
        {
            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = this.query;
            command.Connection = this.connect;
            this.connect.Open();

            return command.ExecuteNonQuery();
        }

        protected int capNhatDuLieu(string sql)
        {
            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = sql;
            command.Connection = this.connect;
            int result;
            try
            {
                this.connect.Open();
                result = command.ExecuteNonQuery();
                this.connect.Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Lỗi!");
                return -1;
            }
            finally { this.connect.Close(); }

            return result;
        }

        public DataTable all()
        {
            DataSet getAll = this.getData($"SELECT * FROM {this.tableName}");
            return getAll.Tables[0];
        }

        public DataTable get()
        {
            DataSet get = this.getData(this.query);
            return get.Tables[0];
        }

        public DataRow first()
        {
            return this.get().Rows[0];
        }

        private object chuyenDoiGiaTri(object value)
        {
            object data = null;

            if (value?.GetType() == typeof(string))
                data = (string)$"N'{value}'";
            else if (value?.GetType() == typeof(int))
                data = (int)value;
            else if (value?.GetType() == typeof(float))
                data = (float)value;
            else if (value?.GetType() == typeof(Boolean))
                data = (Boolean)value == true ? 1 : 0;

            return data;
        }

        private string convertFieldKey<T>(List<T[]> list)
        {
            string fieldKey = "";
            for (int i = 0; i < list.Count; i++)
            {
                fieldKey += (list[i][0]).ToString() + (i + 1 == list.Count ? "" : ", ");
            }
            return fieldKey;
        }

        private string convertFieldKey<T>(List<T> list)
        {
            string fieldKey = "";
            for (int i = 0; i < list.Count; i++)
            {
                fieldKey += (list[i]).ToString() + (i + 1 == list.Count ? "" : ", ");
            }
            return fieldKey;
        }

        private string convertFieldValue<T>(List<T[]> list)
        {
            string fieldValue = "";
            for (int i = 0; i < list.Count; i++)
            {
                object value = list[i][1].GetType() == typeof(string) ?
                    $"N'{list[i][1]}'" : list[i][1];

                fieldValue += value + (i + 1 == list.Count ? "" : ", ");
            }
            return fieldValue;
        }

        private string convertFieldValue<T>(List<T> list)
        {
            string fieldValue = "";
            for (int i = 0; i < list.Count; i++)
            {
                fieldValue += this.chuyenDoiGiaTri(list[i]) + (i + 1 == list.Count ? "" : ", ");
            }
            return fieldValue;
        }

        private string setQueryInsert(string fieldKey, string fieldValue)
        {
            return $"INSERT INTO {this.tableName}(_KEY_) VALUES(_VALUE_)"
                .Replace("_KEY_", fieldKey)
                .Replace("_VALUE_", fieldValue);
        }

        public DatabaseError create(DatabaseInsert dbInsert)
        {
            if (dbInsert == null)
                return new DatabaseError("Vui lòng nhập dữ liệu", true);
            string fieldKey = this.convertFieldKey<object>(dbInsert.get());
            string fieldValue = this.convertFieldValue<object>(dbInsert.get());

            string _query = this.setQueryInsert(fieldKey, fieldValue);

            int result = this.capNhatDuLieu(_query);
            MessageBox.Show(_query.ToString());

            return new DatabaseError(_query, false);
        }

        public DatabaseError create(List<string> fieldKey, List<object> fieldValue)
        {
            string _fieldKey = this.convertFieldKey<string>(fieldKey);
            string _fieldValue = this.convertFieldValue<object>(fieldValue);

            string _query = this.setQueryInsert(_fieldKey, _fieldValue);

            int result = this.capNhatDuLieu(_query);
            MessageBox.Show(_query);

            return new DatabaseError(_query, false);
        }

        private DatabaseConnection _where(string columnName, string condition, object _value, string _CONDITION)
        {
            if (_value is null)
                return this;

            string SET_CONDITION = "WHERE";

            if (this.query.Contains(SET_CONDITION))
                SET_CONDITION = _CONDITION;

            _value = this.chuyenDoiGiaTri(_value);
            this.query += $" {SET_CONDITION} {columnName} {condition} {_value}";

            return this;
        }

        public DatabaseConnection where(string columnName, string condition, object _value)
        {
            return this._where(columnName, condition, _value, "AND");
        }

        public DatabaseConnection where(string columnName, object _value)
        {
            return this.where(columnName, "=", _value);
        }

        public DatabaseConnection orWhere(string columnName, string condition, object _value)
        {
            return this._where(columnName, condition, _value, "OR");
        }

        public DatabaseConnection orWhere(string columnName, object _value)
        {
            return this.orWhere(columnName, "=", _value);
        }

        //Trả ra chuỗi Query, Ví dụ: SELECT * FROM danh_muc
        public string testQuery() { return this.query; }

    }

    internal class DatabaseInsert
    {
        private List<object[]> danhSachDuLieu;

        public DatabaseInsert()
        {
            this.danhSachDuLieu = new List<object[]>();
        }

        public void add(string key, object value)
        {
            this.danhSachDuLieu.Add(new object[] { key, value });
        }

        public List<object[]> get()
        {
            return this.danhSachDuLieu;
        }

        public Boolean kiemTraGiaTri(object value)
        {
            return true;

        }

        
    }

    internal class DatabaseError
    {
        public readonly string message;
        public readonly Boolean isError;

        public DatabaseError()
        {
            this.message = string.Empty;
            this.isError = true;
        }

        public DatabaseError(string message, Boolean isError)
        {
            this.message = message;
            this.isError = isError;
        }
    }

    internal static class DB
    {
        private static DatabaseConnection dbConnection;

        static DB() {
            dbConnection = new DatabaseConnection();
        }

        public static DatabaseConnection table(string tableName = "")
        {
            dbConnection.setTableName(tableName);
            return dbConnection;
        }

    }
}
