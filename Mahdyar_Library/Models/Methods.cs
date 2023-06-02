using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Mahdyar_Library.Models
{
    public static class Methods
    {
        /// <summary>
        /// در صورتی که خصوصیات داخل یک شی از نوع رشته یا عددی باشند و حاوی مقدار صفر و یا رشته خالی باشند مقادیر 
        /// آن خصوصیات را تبدیل به نال میکند
        /// </summary>
        /// <param name="model">شی</param>
        /// <param name="emptyStringOnly">در صورت صحیح بودن فقط خصوصیات نوع رشته ای بررسی می شوند</param>
        /// <returns>شی جدید</returns>
        public static T AssignEmptyValuesWithNull<T>(T model, bool emptyStringOnly = false)
        {
            if (model == null) throw new Exception("Model Is null");
            var props = model.GetType().GetProperties();
            foreach (var prop in props)
            {
                if (prop.PropertyType == typeof(string))
                {
                    string val = (string)prop.GetValue(model);
                    if (val == "")
                        prop.SetValue(model, null);
                }
                else if (!emptyStringOnly && prop.PropertyType == typeof(long?))
                {
                    long? val = (long?)prop.GetValue(model);
                    if (val == 0)
                        prop.SetValue(model, null);
                }
                else if (!emptyStringOnly && prop.PropertyType == typeof(int?))
                {
                    int? val = (int?)prop.GetValue(model);
                    if (val == 0)
                        prop.SetValue(model, null);
                }
                else if (!emptyStringOnly && prop.PropertyType == typeof(short?))
                {
                    short? val = (short?)prop.GetValue(model);
                    if (val == 0)
                        prop.SetValue(model, null);
                }
            }
            return model;
        }
        /// <summary>
        /// تیدبل تاریخ شمسی به میلادی
        /// </summary>
        /// <param name="shamsiDate">تاریخ شمسی</param>
        /// <param name="isTotime">نیاز به زمان هست؟</param>
        /// <returns>تاریخ میلادی</returns>
        public static DateTime? ToMiladiDate(this string shamsiDate, bool isTotime = false)
        {
            if (string.IsNullOrEmpty(shamsiDate))
                return null;

            var dateSection = shamsiDate.Split('/');
            if (!dateSection.Any())
                return null;

            var pCal = new PersianCalendar();

            var dtNow = DateTime.Now;
            var dtmConvet = pCal.ToDateTime(int.Parse(dateSection[0]), int.Parse(dateSection[1]), int.Parse(dateSection[2]), 0, 0, 0, 0);

            //درصورتی که تاریخ ، تاریخ جاری باشد زمان جاری هم انجام می شود
            if (dtmConvet.Date == dtNow.Date)
                return dtmConvet.AddHours(dtNow.Hour).AddMinutes(dtNow.Minute).AddSeconds(dtNow.Second);

            //در صورتی که تاریخ ورودی از تاریخ جاری کمتر باشد آخرین زمان در نظر گرفته می شود
            if (isTotime)
                return dtmConvet.AddHours(23).AddMinutes(59).AddSeconds(59);

            return dtmConvet;
        }
        /// <summary>
        /// تبدیل تاریخ میلادی به شمسی مثال: 1396/01/01
        /// </summary>   
        public static string ToShamsiDate(this DateTime milladiDate)
        {
            var pa = new PersianCalendar();
            return pa.GetYear(milladiDate).ToString() + "/" + pa.GetMonth(milladiDate).ToString("00") + "/" +
                   pa.GetDayOfMonth(milladiDate).ToString("00");
        }
        /// <summary>
        /// تعیین معتبر بودن کد ملی
        /// </summary>
        /// <param name="nationalCode">کد ملی وارد شده</param>
        /// <returns>
        /// در صورتی که کد ملی صحیح باشد خروجی <c>true</c> و در صورتی که کد ملی اشتباه باشد خروجی <c>false</c> خواهد بود
        /// </returns>
        /// <exception cref="System.Exception"></exception>
        public static Boolean IsValidNationalCode(this String nationalCode)
        {
            //در صورتی که کد ملی وارد شده تهی باشد

            if (String.IsNullOrEmpty(nationalCode))
                return false;


            //در صورتی که کد ملی وارد شده طولش کمتر از 10 رقم باشد
            if (nationalCode.Length != 10)
                return false;

            //در صورتی که کد ملی ده رقم عددی نباشد
            var regex = new Regex(@"\d{10}");
            if (!regex.IsMatch(nationalCode))
                return false;

            //در صورتی که رقم‌های کد ملی وارد شده یکسان باشد
            var allDigitEqual = new[] { "0000000000", "1111111111", "2222222222", "3333333333", "4444444444", "5555555555", "6666666666", "7777777777", "8888888888", "9999999999" };
            if (allDigitEqual.Contains(nationalCode)) return false;


            //عملیات شرح داده شده در بالا
            var chArray = nationalCode.ToCharArray();
            var num0 = Convert.ToInt32(chArray[0].ToString()) * 10;
            var num2 = Convert.ToInt32(chArray[1].ToString()) * 9;
            var num3 = Convert.ToInt32(chArray[2].ToString()) * 8;
            var num4 = Convert.ToInt32(chArray[3].ToString()) * 7;
            var num5 = Convert.ToInt32(chArray[4].ToString()) * 6;
            var num6 = Convert.ToInt32(chArray[5].ToString()) * 5;
            var num7 = Convert.ToInt32(chArray[6].ToString()) * 4;
            var num8 = Convert.ToInt32(chArray[7].ToString()) * 3;
            var num9 = Convert.ToInt32(chArray[8].ToString()) * 2;
            var a = Convert.ToInt32(chArray[9].ToString());

            var b = (((((((num0 + num2) + num3) + num4) + num5) + num6) + num7) + num8) + num9;
            var c = b % 11;

            return (((c < 2) && (a == c)) || ((c >= 2) && ((11 - c) == a)));

        }


        //var inputObj = new { RequestNewIds = requestIds.TrimEnd(',') };
        //var outputObj = new[] { new { validate = false, BranchTypeId = 0, UsageTypeId = 0 } };
        //ErrorSpModel errorSpModel1 = new ErrorSpModel();
        //var item = BranchInfoModel.Global_Sp(QueryPurpose.UniqueBranchTypeAndUsageTypes, inputObj, outputObj, ref errorSpModel1).FirstOrDefault();

        //    if ((item?.validate ?? false) == false)
        //{
        //    return Json(new { errorCode = 1, message = "نوع انشعاب و نوع کاربری تمام درخواست های انتخاب شده باید یکسان باشد" }, JsonRequestBehavior.AllowGet);
        //}


        //public static T2 GlobalSp<T1, T2>(string purpose, T1 inputAnonymousObject, T2 outputAnonymousObject, ref ErrorSpModel spOut)
        //{
        //    using (var db = new dbSs_OstanOprEntities())
        //    {
        //        db.Pub_Sp_MultiPurpose(purpose
        //            , JsonConvert.SerializeObject(inputAnonymousObject)
        //            , spOut.Op_IntErrorCode
        //            , spOut.Op_StrErrorMessage);
        //        if (!spOut.CheckErrorSp())
        //            return JsonConvert.DeserializeAnonymousType(spOut.Str_ErrorMessage, outputAnonymousObject);
        //        return outputAnonymousObject;
        //    }

        //}
        ////public static T2 CallSp<T1, T2>(string spName, T1 inputAnonymousObject, T2 outputAnonymousObject, ref ErrorSpModel spOut)
        ////        {
        ////            var props = inputAnonymousObject.GetType().GetProperties();

        ////            List<ObjectParameter> parameters = new List<ObjectParameter>(props.Length);

        ////            for (int i = 0; i < props.Length; i++)
        ////            {
        ////                ObjectParameter p1 = props[i].GetValue(inputAnonymousObject) != null
        ////                    ? new ObjectParameter(props[i].Name, props[i].GetValue(inputAnonymousObject)) : new ObjectParameter(
        ////                        props[i].Name, props[i].PropertyType);
        ////                parameters.Add(p1);
        ////            }
        ////            using (var db = new dbSs_OstanOprEntities())
        ////            {
        ////                var objectContext = ((IObjectContextAdapter)db).ObjectContext;
        ////                var jsonret = objectContext.ExecuteFunction<string>(spName, parameters.ToArray());


        ////                //db.Pub_Sp_MultiPurpose(purpose
        ////                //    , JsonConvert.SerializeObject(inputAnonymousObject)
        ////                //    , spOut.Op_IntErrorCode
        ////                //    , spOut.Op_StrErrorMessage);
        ////                //if(!spOut.CheckErrorSp())
        ////                return JsonConvert.DeserializeAnonymousType(jsonret.FirstOrDefault(), outputAnonymousObject);
        ////            }
        ////        }
        //public static void GlobalSp<T1>(string purpose, T1 inputAnonymousObject, ref ErrorSpModel spOut)
        //{
        //    using (var db = new dbSs_OstanOprEntities())
        //    {
        //        db.Pub_Sp_MultiPurpose(purpose
        //            , JsonConvert.SerializeObject(inputAnonymousObject)
        //            , spOut.Op_IntErrorCode
        //            , spOut.Op_StrErrorMessage);
        //        spOut.CheckErrorSp();
        //    }
        //}
        //public static List<SelectListItem> FillChosen(string chosenOperation)
        //{
        //    var spOut = new ErrorSpModel();
        //    using (var db = new dbSs_OstanOprEntities())
        //    {
        //        var res = db.Pub_Sp_FillAllChosen(chosenOperation,
        //            ""
        //            , spOut.Op_IntErrorCode
        //            , spOut.Op_StrErrorMessage
        //        ).Select(s => new SelectListItem
        //        {
        //            Text = s.Text
        //            ,
        //            Value = s.Value
        //        }).ToList();
        //        return res;

        //    }
        //}
        //public static List<SelectListItem> FillChosen<T1>(string chosenOperation, T1 inputAnonymousObject)
        //{
        //    var spOut = new ErrorSpModel();
        //    using (var db = new dbSs_OstanOprEntities())
        //    {
        //        var res = db.Pub_Sp_FillAllChosen(chosenOperation,
        //            JsonConvert.SerializeObject(inputAnonymousObject)
        //            , spOut.Op_IntErrorCode
        //            , spOut.Op_StrErrorMessage
        //        ).Select(s => new SelectListItem
        //        {
        //            Text = s.Text
        //            ,
        //            Value = s.Value
        //        }).ToList();
        //        return res;

        //    }
        //}
        //public static List<SelectListItem> FillChosen<T1>(string chosenOperation, T1 inputAnonymousObject, ref ErrorSpModel spOut)
        //{
        //    using (var db = new dbSs_OstanOprEntities())
        //    {
        //        var res = db.Pub_Sp_FillAllChosen(chosenOperation,
        //             JsonConvert.SerializeObject(inputAnonymousObject)
        //             , spOut.Op_IntErrorCode
        //             , spOut.Op_StrErrorMessage
        //         ).Select(s => new SelectListItem
        //         {
        //             Text = s.Text
        //                 ,
        //             Value = s.Value
        //         }).ToList();
        //        spOut.CheckErrorSp();
        //        return res;

        //    }
        //}








    //    public static List<SqlParameter> PrepareSPParameters(object inputParameters, params ObjectParameter[] outParameters)
    //    {
    //        List<SqlParameter> sqlParameters = new List<SqlParameter>();
    //        foreach (var propertyInfo in inputParameters.GetType().GetProperties())
    //        {
    //            object value = propertyInfo.GetValue(inputParameters, null);
    //            sqlParameters.Add(new SqlParameter
    //            {
    //                ParameterName = propertyInfo.Name,
    //                Value = value ?? DBNull.Value,
    //                Size = int.MaxValue,
    //                SqlDbType = Extensions.GetSqlDbType(propertyInfo.PropertyType),
    //            });
    //        }

    //        foreach (object item in outParameters)
    //        {
    //            var objParameter = (ObjectParameter)item;
    //            sqlParameters.Add(new SqlParameter
    //            {
    //                Direction = ParameterDirection.Output,
    //                ParameterName = objParameter.Name,
    //                SqlDbType = Extensions.GetSqlDbType(objParameter.ParameterType),
    //                Size = int.MaxValue
    //            });
    //        }

    //        return sqlParameters;
    //    }

    //    public static SqlDbType GetSqlDbType(Type giveType)
    //    {
    //        var typeMap = new Dictionary<Type, SqlDbType>();

    //        typeMap[typeof(string)] = SqlDbType.NVarChar;
    //        typeMap[typeof(char[])] = SqlDbType.NVarChar;
    //        typeMap[typeof(char)] = SqlDbType.NVarChar;
    //        typeMap[typeof(byte)] = SqlDbType.TinyInt;
    //        typeMap[typeof(short)] = SqlDbType.SmallInt;
    //        typeMap[typeof(int)] = SqlDbType.Int;
    //        typeMap[typeof(long)] = SqlDbType.BigInt;
    //        typeMap[typeof(byte[])] = SqlDbType.Image;
    //        typeMap[typeof(bool)] = SqlDbType.Bit;
    //        typeMap[typeof(DateTime)] = SqlDbType.DateTime2;
    //        typeMap[typeof(DateTimeOffset)] = SqlDbType.DateTimeOffset;
    //        typeMap[typeof(decimal)] = SqlDbType.Money;
    //        typeMap[typeof(float)] = SqlDbType.Real;
    //        typeMap[typeof(double)] = SqlDbType.Float;
    //        typeMap[typeof(TimeSpan)] = SqlDbType.Time;
    //        // Allow nullable types to be handled
    //        giveType = Nullable.GetUnderlyingType(giveType) ?? giveType;

    //        if (typeMap.ContainsKey(giveType))
    //        {
    //            return typeMap[giveType];
    //        }

    //        throw new ArgumentException("not a supported .NET class");

    //    }

    //    public static List<TEntity> ExecuteStoredProcedure<TEntity>(DbContext dbContext, string spName, object inputParameters, params ObjectParameter[] outParameters) where TEntity : new()
    //    {
    //        List<TEntity> queryResult = new List<TEntity>();
    //        try
    //        {
    //            using (var connection = dbContext.Database.Connection)
    //            {
    //                connection.Open();
    //                var command = connection.CreateCommand();
    //                command.CommandText = spName;
    //                command.CommandType = CommandType.StoredProcedure;
    //                command.Parameters.AddRange(PrepareSPParameters(inputParameters, outParameters).ToArray());

    //                using (var reader = command.ExecuteReader())
    //                {
    //                    queryResult = reader.MapToList<TEntity>();
    //                }

    //                foreach (var item in command.Parameters)
    //                {
    //                    var sqlParam = (SqlParameter)item;
    //                    if (sqlParam.Direction == ParameterDirection.Output)
    //                    {
    //                        outParameters.FirstOrDefault(c => c.Name == sqlParam.ParameterName).Value = sqlParam.Value;
    //                    }
    //                }

    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            e.YaldaTranslateException();
    //        }

    //        return queryResult ?? new List<TEntity>();
    //    }

    //    public static void ExecuteStoredProcedure(DbContext dbContext, string spName, object inputParameters, params ObjectParameter[] outParameters)
    //    {
    //        try
    //        {
    //            using (var connection = dbContext.Database.Connection)
    //            {
    //                connection.Open();
    //                var command = connection.CreateCommand();
    //                command.CommandText = spName;
    //                command.CommandType = CommandType.StoredProcedure;
    //                command.Parameters.AddRange(PrepareSPParameters(inputParameters, outParameters).ToArray());
    //                var reader = command.ExecuteReader();
    //                foreach (var item in command.Parameters)
    //                {
    //                    var sqlParam = (SqlParameter)item;
    //                    if (sqlParam.Direction == ParameterDirection.Output)
    //                    {
    //                        outParameters.FirstOrDefault(c => c.Name == sqlParam.ParameterName).Value = sqlParam.Value;
    //                    }
    //                }

    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            e.YaldaTranslateException();
    //        }


    //    }

    //    public static List<T> MapToList<T>(this DbDataReader dr) where T : new()
    //    {
    //        if (dr != null && dr.HasRows)
    //        {
    //            var entity = typeof(T);
    //            var entities = new List<T>();
    //            var propDict = new Dictionary<string, PropertyInfo>();
    //            var props = entity.GetProperties(BindingFlags.Instance | BindingFlags.Public);
    //            bool isPrimitiveType = entity.IsPrimitive || entity.IsValueType || (entity == typeof(string) || entity == typeof(int) || entity == typeof(decimal));
    //            propDict = props.ToDictionary(p => p.Name.ToUpper(), p => p);

    //            while (dr.Read())
    //            {
    //                T newObject = new T();
    //                for (int index = 0; index < dr.FieldCount; index++)
    //                {
    //                    if (propDict.ContainsKey(dr.GetName(index).ToUpper()))
    //                    {
    //                        var info = propDict[dr.GetName(index).ToUpper()];
    //                        if ((info != null) && info.CanWrite)
    //                        {
    //                            var val = dr.GetValue(index);
    //                            info.SetValue(newObject, (val == DBNull.Value) ? null : val, null);
    //                        }
    //                    }
    //                    else if (isPrimitiveType)
    //                    {
    //                        newObject = (T)dr.GetValue(index);
    //                    }
    //                }

    //                entities.Add(newObject);
    //            }
    //            return entities;
    //        }
    //        return null;
    //    }

    //    public static string ToJson(this object obj)
    //    {
    //        JavaScriptSerializer jss = new JavaScriptSerializer();
    //        jss.MaxJsonLength = Int32.MaxValue;
    //        return jss.Serialize(obj);
    //    }

    //    public static string ToXml(this object obj)
    //    {
    //        if (obj == null) return null;
    //        var xmlDoc = new XmlDocument();
    //        var xmlSerializer = new XmlSerializer(obj.GetType());
    //        using (var xmlStream = new MemoryStream())
    //        {
    //            xmlSerializer.Serialize(xmlStream, obj);
    //            xmlStream.Position = 0;
    //            xmlDoc.Load(xmlStream);
    //            return xmlDoc.InnerXml;
    //        }
    //    }
    //}

    //public class MultipleResultSetWrapper
    //{
    //    private DbContext _db;
    //    private string _storedProcedure;
    //    private List<SqlParameter> _sqlParameters;
    //    public List<Func<IObjectContextAdapter, DbDataReader, IList>> _resultSets;
    //    private ObjectParameter[] _outputs;
    //    private object _inputs;
    //    public MultipleResultSetWrapper(DbContext db, string storedProcedure, object inputs, params ObjectParameter[] outpputs)
    //    {
    //        _db = db;
    //        _storedProcedure = storedProcedure;
    //        _resultSets = new List<Func<IObjectContextAdapter, DbDataReader, IList>>();
    //        _sqlParameters = new List<SqlParameter>();
    //        _outputs = outpputs;
    //        _inputs = inputs;

    //        if (_inputs != null)
    //        {
    //            foreach (var propertyInfo in _inputs.GetType().GetProperties())
    //            {
    //                object value = propertyInfo.GetValue(_inputs, null);
    //                _sqlParameters.Add(new SqlParameter
    //                {
    //                    ParameterName = propertyInfo.Name,
    //                    Value = value ?? DBNull.Value,
    //                    Size = int.MaxValue,
    //                    SqlDbType = Extensions.GetSqlDbType(propertyInfo.PropertyType),
    //                });
    //            }
    //        }

    //        foreach (object item in _outputs)
    //        {
    //            var objParameter = (ObjectParameter)item;
    //            _sqlParameters.Add(new SqlParameter
    //            {
    //                Direction = ParameterDirection.Output,
    //                ParameterName = objParameter.Name,
    //                SqlDbType = Extensions.GetSqlDbType(objParameter.ParameterType),
    //                Size = int.MaxValue
    //            });
    //        }
    //    }

    //    public T ExecuteProcedure<T>() where T : new()
    //    {
    //        var results = new List<IEnumerable>();
    //        var TEntity = new T();

    //        using (var connection = _db.Database.Connection)
    //        {
    //            connection.Open();
    //            var command = connection.CreateCommand();
    //            command.CommandText = _storedProcedure;
    //            command.CommandType = CommandType.StoredProcedure;
    //            command.Parameters.AddRange(_sqlParameters.ToArray());

    //            using (var reader = command.ExecuteReader())
    //            {
    //                var adapter = ((IObjectContextAdapter)_db);
    //                foreach (var resultSet in _resultSets)
    //                {
    //                    results.Add(resultSet(adapter, reader));
    //                    reader.NextResult();
    //                }
    //            }

    //            foreach (var item in command.Parameters)
    //            {
    //                var sqlParam = (SqlParameter)item;
    //                if (sqlParam.Direction == ParameterDirection.Output)
    //                {
    //                    _outputs.FirstOrDefault(c => c.Name == sqlParam.ParameterName).Value = sqlParam.Value;
    //                }
    //            }

    //        }

    //        var entity = TEntity.GetType();
    //        var props = entity.GetProperties(BindingFlags.Instance | BindingFlags.Public);
    //        var propDict = props.ToDictionary(p => p.Name.ToUpper(), p => p);
    //        bool isPrimitiveType = entity.IsPrimitive || entity.IsValueType || (entity == typeof(string) || entity == typeof(int) || entity == typeof(decimal));
    //        bool isList;
    //        foreach (var prop in props)
    //        {
    //            var info = propDict[prop.Name.ToUpper()];
    //            var baseType = typeof(List<>);
    //            Type genericType = baseType.MakeGenericType(prop.PropertyType);
    //            IList resultList;
    //            IList objValue = new ArrayList();
    //            if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
    //            {
    //                objValue = results.Where(c => c.GetType().FullName == prop.PropertyType.FullName).ToList();
    //                isList = true;
    //                resultList = (IList)Activator.CreateInstance(genericType.GetGenericArguments()[0]);
    //            }
    //            else
    //            {
    //                isList = false;
    //                objValue = results.Where(c => c.GetType().GetGenericArguments() != null && c.GetType().GetGenericArguments()[0].Name == prop.PropertyType.Name).ToList();
    //                resultList = (IList)Activator.CreateInstance(genericType);
    //            }

    //            if ((info != null) && info.CanWrite && objValue != null && objValue.Count > 0)
    //            {
    //                foreach (var item in (IList)objValue[0])
    //                {
    //                    resultList.Add(item);
    //                }

    //                info.SetValue(TEntity, isList ? resultList : resultList != null && resultList.Count > 0 ? resultList[0] : null, null);
    //            }
    //        }

    //        return TEntity;
    //    }
    //    public MultipleResultSetWrapper With<TResult>()
    //    {
    //        _resultSets.Add((adapter, reader) => adapter
    //            .ObjectContext
    //            .Translate<TResult>(reader)
    //            .ToList());

    //        return this;
    //    }

    //    public T Execute<T>() where T : new()
    //    {
    //        var results = new List<IEnumerable>();
    //        var TEntity = new T();
    //        using (var connection = _db.Database.Connection)
    //        {
    //            var entity = TEntity.GetType();
    //            var props = entity.GetProperties(BindingFlags.Instance | BindingFlags.Public);

    //            //if (_resultSets == null || !_resultSets.Any())
    //            //{
    //            //    var dbSets = typeof(T).GetProperties(BindingFlags.Public |
    //            //                               BindingFlags.Instance);
    //            //    foreach (var prop in dbSets)
    //            //    {
    //            //        var baseType = typeof(List<>);
    //            //        Type genericType = baseType.MakeGenericType(prop.PropertyType);
    //            //        _resultSets.Add((adapter, reader) => adapter
    //            //            .ObjectContext
    //            //            .Translate<prop.PropertyType.GetGenericArguments> (reader)
    //            //            .ToList());
    //            //    }
    //            //}

    //            connection.Open();
    //            var command = connection.CreateCommand();
    //            command.CommandText = _storedProcedure;
    //            command.CommandType = CommandType.StoredProcedure;
    //            command.Parameters.AddRange(_sqlParameters.ToArray());

    //            using (var reader = command.ExecuteReader())
    //            {
    //                var adapter = ((IObjectContextAdapter)_db);
    //                foreach (var resultSet in _resultSets)
    //                {
    //                    results.Add(resultSet(adapter, reader));
    //                    reader.NextResult();
    //                }
    //            }


    //            var propDict = props.ToDictionary(p => p.Name.ToUpper(), p => p);
    //            bool isPrimitiveType = entity.IsPrimitive || entity.IsValueType || (entity == typeof(string) || entity == typeof(int) || entity == typeof(decimal));
    //            bool isList;
    //            foreach (var prop in props)
    //            {
    //                var info = propDict[prop.Name.ToUpper()];
    //                var baseType = typeof(List<>);
    //                Type genericType = baseType.MakeGenericType(prop.PropertyType);
    //                IList resultList;
    //                IList objValue = new ArrayList();
    //                if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
    //                {
    //                    objValue = results.Where(c => c.GetType().FullName == prop.PropertyType.FullName).ToList();
    //                    isList = true;
    //                    resultList = (IList)Activator.CreateInstance(genericType.GetGenericArguments()[0]);
    //                }
    //                else
    //                {
    //                    isList = false;
    //                    objValue = results.Where(c => c.GetType().GetGenericArguments() != null && c.GetType().GetGenericArguments()[0].Name == prop.PropertyType.Name).ToList();
    //                    resultList = (IList)Activator.CreateInstance(genericType);
    //                }

    //                if ((info != null) && info.CanWrite)
    //                {
    //                    foreach (var item in (IList)objValue[0])
    //                    {
    //                        resultList.Add(item);
    //                    }

    //                    info.SetValue(TEntity, isList ? resultList : resultList[0], null);
    //                }
    //            }
    //            return TEntity;
    //        }
    //    }

    //    /// <summary>
    //    /// این دستور دارای دو باگ می باشد 
    //    /// توجه شود در صورتی که خروجی سلکت ها یکسان باشد باید ترتیب خروجی ها با ترتیب در کلاس مدل یکسان باشد
    //    /// دوم اینکه نتیجه ی خروجی اس پی با کلاسی که قرار است به آن مپ شود کاملا یکشان باشد
    //    ///  
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <returns></returns>
    //    public T ExecuteWithoutOrder<T>() where T : new()
    //    {
    //        var results = new List<IEnumerable>();
    //        var TEntity = new T();
    //        using (var connection = _db.Database.Connection)
    //        {
    //            var entity = TEntity.GetType();
    //            var props = entity.GetProperties(BindingFlags.Instance | BindingFlags.Public);



    //            connection.Open();
    //            var command = connection.CreateCommand();
    //            command.CommandText = _storedProcedure;
    //            command.CommandType = CommandType.StoredProcedure;
    //            command.Parameters.AddRange(_sqlParameters.ToArray());

    //            var propDict = props.ToDictionary(p => p.Name.ToUpper(), p => p);
    //            foreach (var prop in props)
    //            {
    //                var info = propDict[prop.Name.ToUpper()];
    //                var baseType = typeof(List<>);
    //                Type genericType = baseType.MakeGenericType(prop.PropertyType);
    //                bool isPrimitiveType = prop.PropertyType.IsPrimitive || prop.PropertyType.IsValueType || (prop.PropertyType == typeof(string) || prop.PropertyType == typeof(int) || prop.PropertyType == typeof(decimal));
    //                bool isList = prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>);
    //                object objValue = isPrimitiveType ? null : isList ? (IList)Activator.CreateInstance(genericType.GetGenericArguments()[0]) : Activator.CreateInstance(prop.PropertyType);
    //                info.SetValue(TEntity, objValue, null);
    //            }

    //            using (var reader = command.ExecuteReader())
    //            {
    //                var adapter = ((IObjectContextAdapter)_db);
    //                while (reader.FieldCount > 0)
    //                {

    //                    var columns = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();
    //                    foreach (var prop in props)
    //                    {
    //                        var info = propDict[prop.Name.ToUpper()];
    //                        var propIsList = prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>);
    //                        bool isPrimitiveType = prop.PropertyType.IsPrimitive || prop.PropertyType.IsValueType || (prop.PropertyType == typeof(string) || prop.PropertyType == typeof(int) || prop.PropertyType == typeof(decimal));
    //                        var resultList = propIsList ? (IList)info.GetValue(TEntity, null) : info.GetValue(TEntity, null);
    //                        var isNull = isPrimitiveType ? true : propIsList ? ((IList)resultList).Count <= 0 : !resultList.GetType().GetProperties().Select(pi => pi.GetValue(resultList)).Any(value => value != null && value != "0");
    //                        IList objValue = new ArrayList();
    //                        var mainObj = propIsList ? Activator.CreateInstance(resultList.GetType().GetProperty("Item").PropertyType) : resultList;

    //                        if ((isNull && !isPrimitiveType && mainObj.GetType().GetProperties().Select(c => c.Name).Count() == columns.Count() && mainObj.GetType().GetProperties().Select(c => c.Name).All(columns.Contains)) || (isPrimitiveType && prop.Name == columns[0]))
    //                        {
    //                            var objProps = !isPrimitiveType ? mainObj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public) : new PropertyInfo[] { };
    //                            var objPropDict = objProps.ToDictionary(p => p.Name.ToUpper(), p => p);

    //                            while (reader.Read())
    //                            {
    //                                var obj = propIsList ? Activator.CreateInstance(resultList.GetType().GetProperty("Item").PropertyType) : resultList;
    //                                if (!isPrimitiveType)
    //                                {
    //                                    foreach (var objProp in obj.GetType().GetProperties())
    //                                    {
    //                                        var objinfo = objPropDict[objProp.Name.ToUpper()];
    //                                        objinfo.SetValue(obj, reader[objProp.Name] == DBNull.Value ? null : reader[objProp.Name], null);

    //                                    }

    //                                    objValue.Add(obj);
    //                                }
    //                                else
    //                                {
    //                                    objValue.Add(reader[columns[0]]);
    //                                }

    //                            }

    //                            if ((info != null) && info.CanWrite && objValue.Count > 0)
    //                            {
    //                                if (propIsList)
    //                                {
    //                                    IList objLastVal = (IList)info.GetValue(TEntity, null);
    //                                    foreach (var val in objValue)
    //                                    {
    //                                        objLastVal.Add(val);
    //                                    }

    //                                    info.SetValue(TEntity, resultList, null);
    //                                }
    //                                else if (!isPrimitiveType)
    //                                {
    //                                    info.SetValue(TEntity, objValue[0], null);
    //                                }
    //                                else
    //                                {

    //                                }
    //                            }

    //                            break;
    //                        }

    //                    }
    //                    reader.NextResult();
    //                }

    //            }


    //            return TEntity;
    //        }
    //    }


    //}
    //public class DrawingObjects
    //{
    //    Point leftPoint = new Point(0, 0);
    //    Point rightPoint = new Point(0, 0);
    //    Color backcolor;
    //    int intLastLineHeight = 0;
    //    int intMaxRightWidth = 0;
    //    int intMaxLeftwidth = 0;
    //    public int widthMargin { get; set; }
    //    public int heightMargin { get; set; }
    //    public Point LastTextPoint { get { return rightPoint; } }
    //    public Point LastImgPoint { get { return leftPoint; } }
    //    Image img = new Bitmap(1, 1);
    //    GraphicsUnit graphicsUnit;
    //    public Image Image { get { return img; } }
    //    Graphics drawing;
    //    public int Width
    //    {
    //        get { return img.Width; }
    //    }
    //    public int Height { get; set; }
    //    /// <summary>
    //    /// initial a new instance of DrawingObjects for create a image with text and pic
    //    /// for each instance create a pic and each function add object to this pic
    //    /// </summary>
    //    /// <param name="intWidth">size of width in output image</param>
    //    /// <param name="intHeight">size of height in output image</param>
    //    /// <param name="backColor">back ground color of output image</param>
    //    /// <param name="intWidthMargin">width margin for adding objects to output pic</param>
    //    /// <param name="intHeightMargin">Height margin for adding objects to output pic</param>
    //    public DrawingObjects(int intWidth, int intHeight, Color backColor, int intWidthMargin, int intHeightMargin)
    //    {
    //        img = new Bitmap(intWidth, intHeight);
    //        drawing = Graphics.FromImage(img);
    //        widthMargin = intWidthMargin;
    //        heightMargin = intHeightMargin;
    //        drawing.Clear(backColor);
    //        backcolor = backColor;
    //    }


    //    private DrawingObjects()
    //    {

    //    }
    //    /// <summary>
    //    /// set Alignment of drawing object to main pic
    //    /// </summary>s
    //    public enum AlignmentObject
    //    {
    //        Left,
    //        Right

    //    }
    //    /// <summary>
    //    /// add text in new line to this instance of object pic
    //    /// </summary>
    //    /// <param name="text"></param>
    //    /// <param name="font"></param>
    //    /// <param name="textColor"></param>
    //    /// <param name="alignment"></param>
    //    public void DrawText(String text, Font font, Color textColor, AlignmentObject alignment)
    //    {

    //        SizeF textSize = drawing.MeasureString(text, font);

    //        Point writepoint = new Point();
    //        switch (alignment)
    //        {
    //            case AlignmentObject.Left:
    //                if (img.Size.Height < (leftPoint.Y + Convert.ToInt32(textSize.Height) + heightMargin + 5))
    //                {
    //                    leftPoint.Y = intLastLineHeight;
    //                    intMaxLeftwidth = leftPoint.X;
    //                }
    //                writepoint = new Point(widthMargin + intMaxLeftwidth, (leftPoint.Y > 0 ? leftPoint.Y : heightMargin) + 5);
    //                leftPoint = new Point(leftPoint.X, writepoint.Y + Convert.ToInt32(textSize.Height));
    //                leftPoint.X = Math.Max((int)textSize.Width, leftPoint.X);
    //                break;
    //            case AlignmentObject.Right:
    //                if (img.Size.Height < (rightPoint.Y + Convert.ToInt32(textSize.Height) + heightMargin + 5))
    //                {
    //                    rightPoint.Y = intLastLineHeight;
    //                    intMaxRightWidth = rightPoint.X;
    //                }
    //                writepoint = new Point((img.Width - (widthMargin + Convert.ToInt32(textSize.Width)) - intMaxRightWidth),
    //                    (rightPoint.Y > 0 ? rightPoint.Y : heightMargin) + 5);
    //                rightPoint = new Point(Math.Max((int)textSize.Width, rightPoint.X), writepoint.Y + Convert.ToInt32(textSize.Height));

    //                break;
    //            default:
    //                break;
    //        }
    //        string strDecoded = HttpUtility.UrlDecode(text, Encoding.UTF8);
    //        drawing.DrawString(text, font, new SolidBrush(textColor), writepoint.X, writepoint.Y);
    //        drawing.Save();
    //    }

    //    /// <summary>
    //    /// add image to this instance of objcet pic
    //    /// </summary>
    //    /// <param name="image"></param>
    //    /// <param name="alignment"></param>
    //    public void DrawImage(Image image, Size? newImageSize, bool bolAspectRatio, AlignmentObject alignment)
    //    {
    //        double y = image.Height;
    //        double x = image.Width;
    //        double factor = 1;
    //        Bitmap imgOut = new Bitmap(1, 1);
    //        if (newImageSize == null)
    //        {
    //            newImageSize = image.Size;
    //            imgOut = new Bitmap(image);
    //        }
    //        else if (bolAspectRatio)
    //        {



    //            if (newImageSize.Value.Width > 0)
    //            {
    //                factor = newImageSize.Value.Width / x;
    //            }
    //            imgOut = new Bitmap((int)(x * factor), (int)(y * factor));

    //            // Set DPI of image (xDpi, yDpi)
    //            imgOut.SetResolution(96, 96);
    //            Graphics g = Graphics.FromImage(imgOut);
    //            g.Clear(backcolor);
    //            g.DrawImage(image, new Rectangle(0, 0, (int)(factor * x), (int)(factor * y)),
    //              new Rectangle(0, 0, (int)x, (int)y), GraphicsUnit.Pixel);
    //            newImageSize = imgOut.Size;
    //        }
    //        if (newImageSize.Value.Width + widthMargin + 5 > img.Size.Width || newImageSize.Value.Height + heightMargin + 5 > img.Height)
    //        {
    //            return;
    //        }

    //        Point drawPoint = new Point();
    //        switch (alignment)
    //        {
    //            case AlignmentObject.Left:
    //                if (img.Size.Height < (leftPoint.Y + Convert.ToInt32(newImageSize.Value.Height) + heightMargin + 5))
    //                {
    //                    leftPoint.Y = intLastLineHeight;
    //                    intMaxLeftwidth = leftPoint.X;
    //                }
    //                drawPoint = new Point(widthMargin + intMaxLeftwidth, (leftPoint.Y > 0 ? leftPoint.Y : heightMargin) + 5);
    //                leftPoint = new Point(Math.Max(drawPoint.X, leftPoint.X), drawPoint.Y + newImageSize.Value.Height);
    //                leftPoint.X = Math.Max(newImageSize.Value.Width, leftPoint.X);
    //                break;
    //            case AlignmentObject.Right:
    //                if (img.Size.Height < (rightPoint.Y + Convert.ToInt32(newImageSize.Value.Height) + heightMargin + 5))
    //                {
    //                    rightPoint.Y = intLastLineHeight;
    //                    intMaxRightWidth = rightPoint.X;
    //                }
    //                drawPoint = new Point(img.Width - (newImageSize.Value.Width + widthMargin) - intMaxRightWidth,
    //                    (rightPoint.Y > 0 ? rightPoint.Y : heightMargin) + 5);
    //                rightPoint = new Point(drawPoint.X, drawPoint.Y + newImageSize.Value.Height);
    //                rightPoint.X = Math.Max(newImageSize.Value.Width, rightPoint.X);
    //                break;
    //            default:
    //                break;
    //        }



    //        drawing.DrawImage(imgOut, drawPoint);
    //        drawing.Save();
    //    }
    //    /// <summary>
    //    /// make main pic to balck and white image
    //    /// </summary>
    //    public void MakeBW()
    //    {
    //        ImageAttributes imageAttributes = new ImageAttributes();

    //        float[][] colorMatrixElements = {
    //              new float[]{.3f, .3f, .3f, 0, 0},
    //                   new float[] { .59f, .59f, .59f, 0, 0 },
    //                   new float[] { .11f, .11f, .11f, 0, 0 },
    //                   new float[] { 0, 0, 0, 1, 0 },
    //                   new float[] { 0, 0, 0, 0, 1 } };

    //        ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);

    //        imageAttributes.SetColorMatrix(
    //           colorMatrix,
    //           ColorMatrixFlag.Default,
    //           ColorAdjustType.Bitmap);
    //        drawing.DrawImage(
    //           img,
    //           new Rectangle(0, 0, img.Width, img.Height),  // destination rectangle 
    //           0, 0,        // upper-left corner of source rectangle 
    //           img.Width,       // width of source rectangle
    //           img.Height,      // height of source rectangle
    //           GraphicsUnit.Pixel,
    //           imageAttributes);
    //        drawing.Save();
    //    }
    //    /// <summary>
    //    /// Draw line in pic add this line to last available  height point
    //    /// </summary>
    //    /// <param name="lineColor">color of line</param>
    //    /// <param name="thickness">thickness for drawing line</param>
    //    /// <param name="lineWidth"></param>
    //    /// <param name="alignment"></param>
    //    public void DrawLine(Color lineColor, float thickness, int? lineWidth, AlignmentObject alignment)
    //    {

    //        if (lineWidth == null)
    //        {
    //            lineWidth = img.Width - widthMargin;
    //        }
    //        else
    //        {
    //            lineWidth = lineWidth > img.Width - widthMargin ? img.Width - widthMargin : lineWidth + widthMargin;
    //        }
    //        Pen pen = new Pen(lineColor, thickness);
    //        int lastHeight = Math.Max(leftPoint.Y, rightPoint.Y);
    //        if (lineWidth > img.Size.Width / 2)
    //        {
    //            leftPoint.Y = rightPoint.Y = intLastLineHeight = lastHeight + (int)thickness + 5;
    //            leftPoint.X = rightPoint.X = intMaxLeftwidth = intMaxRightWidth = 0;
    //        }
    //        else if (alignment == AlignmentObject.Right)
    //        {
    //            rightPoint.Y = intLastLineHeight = lastHeight + (int)thickness + 5;
    //            rightPoint.X = intMaxRightWidth = 0;
    //        }
    //        else
    //        {
    //            leftPoint.Y = intLastLineHeight = lastHeight + (int)thickness + 5;
    //            leftPoint.X = intMaxLeftwidth = 0;
    //        }

    //        switch (alignment)
    //        {
    //            case AlignmentObject.Left:
    //                drawing.DrawLine(pen, new Point(widthMargin, lastHeight), new Point((int)lineWidth, lastHeight));
    //                break;
    //            case AlignmentObject.Right:
    //                drawing.DrawLine(pen, new Point(img.Width - widthMargin, lastHeight),
    //                    new Point((img.Width - widthMargin) - (int)lineWidth - widthMargin, lastHeight));
    //                break;
    //            default:
    //                break;
    //        }


    //    }
    //    /// <summary>
    //    /// change current image size with Milimeter measure
    //    /// </summary>
    //    /// <param name="intWidth"></param>
    //    /// <param name="intHeight"></param>
    //    /// <param name="bolMaintionAspectRatio">resize image With Aspect ratio -calc with each one is greater than 0-</param>
    //    /// <returns></returns>
    //    public Image GetImageMM(int intWidth, int intHeight, bool bolMaintionAspectRatio)
    //    {
    //        float pWidth = intWidth * drawing.DpiY / 2.54f;
    //        float pHeight = intHeight * drawing.DpiY / 2.54f;
    //        return ResizeImage((int)pWidth, (int)pHeight, bolMaintionAspectRatio);
    //    }
    //    /// <summary>
    //    /// rezsize image to new size
    //    /// </summary>
    //    /// <param name="intWidth"></param>
    //    /// <param name="intHeight"></param>
    //    /// <param name="bolMaintionAspectRatio">resize image With Aspect ratio -calc with each one is greater than 0-</param>
    //    /// <returns></returns>
    //    public Image ResizeImage(int intWidth, int intHeight, bool bolMaintionAspectRatio)
    //    {
    //        double y = img.Height;
    //        double x = img.Width;

    //        double factor = 1;
    //        if (intWidth > 0)
    //        {
    //            factor = intWidth / x;
    //        }
    //        if (!bolMaintionAspectRatio && intHeight > 0)
    //        {
    //            factor = intHeight / y;
    //        }
    //        else if (bolMaintionAspectRatio && intHeight > 0)
    //        {
    //            factor = intHeight / y;
    //        }
    //        Bitmap imgOut = new Bitmap((int)(x * factor), (int)(y * factor));

    //        // Set DPI of image (xDpi, yDpi)
    //        imgOut.SetResolution(96, 96);

    //        Graphics g = Graphics.FromImage(imgOut);
    //        g.Clear(backcolor);
    //        g.DrawImage(img, new Rectangle(0, 0, (int)(factor * x), (int)(factor * y)),
    //          new Rectangle(0, 0, (int)x, (int)y), GraphicsUnit.Pixel);

    //        return imgOut;

    //    }
    //    /// <summary>
    //    /// dispose instance class
    //    /// </summary>
    //    public void Dispose()
    //    {
    //        drawing.Dispose();
    //    }
    //    /// <summary>
    //    /// use this method for adding speace verticaly between object while add object
    //    /// </summary>
    //    /// <param name="alignment"></param>
    //    /// <param name="intSpace"></param>
    //    public void AddVerticalSpace(AlignmentObject alignment, int intSpace)
    //    {
    //        switch (alignment)
    //        {
    //            case AlignmentObject.Left:
    //                leftPoint = new Point(leftPoint.X, leftPoint.Y + intSpace);
    //                break;
    //            case AlignmentObject.Right:
    //                rightPoint = new Point(rightPoint.X, rightPoint.Y + intSpace);
    //                break;
    //            default:
    //                break;
    //        }
    //    }
    //    /// <summary>
    //    /// use this method for adding speace Horizentaly between object while add object in new column
    //    /// </summary>
    //    /// <param name="alignment"></param>
    //    /// <param name="intSpace"></param>
    //    public void AddHorizentalSpace(AlignmentObject alignment, int intSpace)
    //    {
    //        switch (alignment)
    //        {
    //            case AlignmentObject.Left:
    //                intMaxLeftwidth = leftPoint.X + intSpace;
    //                break;
    //            case AlignmentObject.Right:
    //                intMaxRightWidth = rightPoint.X + intSpace;
    //                break;
    //            default:
    //                break;
    //        }
    //    }
    //    /// <summary>
    //    /// draw new opbject to new column next of current(left or right) column
    //    /// </summary>
    //    /// <param name="alignment"></param>
    //    public void AddNewColumn(AlignmentObject alignment)
    //    {

    //        switch (alignment)
    //        {
    //            case AlignmentObject.Left:
    //                leftPoint.Y = intLastLineHeight;
    //                intMaxLeftwidth = leftPoint.X;
    //                break;
    //            case AlignmentObject.Right:
    //                rightPoint.Y = intLastLineHeight;
    //                intMaxRightWidth = rightPoint.X;
    //                break;
    //            default:
    //                break;
    //        }

    //    }
    //    public Image GetGhomInqueryImage(RegModel.GetInfoAndImageResponse getInfoAndImage, bool bolConvertToBW)
    //    {
    //        if (getInfoAndImage == null)
    //        {
    //            return null;
    //        }
    //        if (getInfoAndImage.payload == null)
    //        {
    //            return null;
    //        }
    //        Image headerImage = Resources.GhomHeaderInqueryImage;

    //        DrawImage(headerImage, new Size(img.Width / 3, img.Height / 3), true, AlignmentObject.Right);
    //        DrawText("سامانه استعلام هویتی ثبت احوال", new Font("B Nazanin", Image.Width / 46, FontStyle.Bold),
    //            Color.Black, DrawingObjects.AlignmentObject.Right);
    //        DrawLine(Color.Black, 1, null, DrawingObjects.AlignmentObject.Right);
    //        Font font = new Font("B Nazanin", Image.Width / 50, FontStyle.Bold);
    //        AlignmentObject rAlign = DrawingObjects.AlignmentObject.Right;
    //        AlignmentObject lAlign = DrawingObjects.AlignmentObject.Left;
    //        DrawText(IsNull(getInfoAndImage.conversationId, "") + " " + ": کد رهگیری استعلام", new Font("Times New Roman", Image.Width / 50, FontStyle.Bold), Color.Black, rAlign);
    //        DrawText("کد ملی : " + getInfoAndImage.payload.nin.Value.ToString(), font, Color.Black, rAlign);
    //        DrawText("نام : " + IsNull(getInfoAndImage.payload.name, ""), font, Color.Black, rAlign);
    //        DrawText("نام خانوادگی : " + IsNull(getInfoAndImage.payload.family, ""), font, Color.Black, rAlign);
    //        DrawText("تاریخ تولد : " + getInfoAndImage.payload.birthDate.Value.ToShamsiDate(), font, Color.Black, rAlign);
    //        DrawText("نام پدر : " + IsNull(getInfoAndImage.payload.fatherName, ""), font, Color.Black, rAlign);
    //        DrawText("شماره شناسنامه : " + (getInfoAndImage.payload.shenasnameNo.HasValue == true ?
    //            getInfoAndImage.payload.shenasnameNo.Value.ToString() : ""), font, Color.Black, rAlign);
    //        DrawText("سریال شناسنامه : " + (getInfoAndImage.payload.shenasnameSerial.HasValue == true ?
    //            getInfoAndImage.payload.shenasnameSerial.Value.ToString() : ""), font, Color.Black, rAlign);
    //        AddNewColumn(AlignmentObject.Right);

    //        AddVerticalSpace(AlignmentObject.Right, 30);
    //        AddHorizentalSpace(AlignmentObject.Right, -80);
    //        DrawText("جنسیت : " + (getInfoAndImage.payload.gender == 1 ? "مرد" : "زن"), font, Color.Black, rAlign);
    //        DrawText("وضعیت حیات : " + (getInfoAndImage.payload.deathDate == null ? "در قید حیات نمی باشد" : "در قید حیات"), font, Color.Black, rAlign);
    //        DrawText("شماره مجلد : " + (getInfoAndImage.payload.bookNo.HasValue == true ?
    //            getInfoAndImage.payload.bookNo.Value.ToString() : ""), font, Color.Black, rAlign);
    //        DrawText("ردیف مجلد : " + (getInfoAndImage.payload.bookRow.HasValue == true ?
    //            getInfoAndImage.payload.bookRow.Value.ToString() : ""), font, Color.Black, rAlign);
    //        DrawText("کد محل ثبت : " + (getInfoAndImage.payload.officeCode.HasValue == true ?
    //            getInfoAndImage.payload.officeCode.Value.ToString() : ""), font, Color.Black, rAlign);
    //        DrawText("سریال کارت ملی: : " + IsNull(getInfoAndImage.payload.cardSrno, ""), font, Color.Black, rAlign);
    //        DrawText("نام محل ثبت : " + IsNull(getInfoAndImage.payload.officeName, ""), font, Color.Black, rAlign);
    //        DrawText("تاریخ استعلام : " + DateTime.Now.ToLongShamsiDateTime(), new Font("B Nazanin", Image.Width / 58, FontStyle.Italic), Color.Black, lAlign);

    //        if (true)
    //        {

    //        }
    //        Image temp = new Bitmap((getInfoAndImage.payload.images == null || getInfoAndImage.payload.images[0].image == null ?
    //            Resources.ImageNotFound : getInfoAndImage.payload.images[0].image.ToImage()));
    //        DrawImage(temp, new Size(Image.Width / 5, Image.Height / 5), true, AlignmentObject.Left);

    //        if (bolConvertToBW)
    //        {
    //            MakeBW();
    //        }
    //        return Image;
    //    }
    //    private T IsNull<T>(T input, T replacement)
    //    {
    //        return (input == null ? replacement : input);
    //    }
    //}
    //public static class ExtenstionMethod
    //{
    //    public static Image ToImage(this string strimage)
    //    {
    //        byte[] bytes = Convert.FromBase64String(strimage);
    //        using (MemoryStream ms = new MemoryStream(bytes))
    //        {
    //            return Image.FromStream(ms);
    //        }
    //    }

    //    public static string ToBase64(this Image image)
    //    {
    //        using (Image tmpimage = image)
    //        {
    //            using (MemoryStream m = new MemoryStream())
    //            {
    //                tmpimage.Save(m, ImageFormat.Jpeg);
    //                byte[] imageBytes = m.ToArray();
    //                string base64String = Convert.ToBase64String(imageBytes);
    //                return base64String;
    //            }
    //        }
    //    }

    //    public static string ToShamsiDate(this int intDate)
    //    {
    //        string temp = intDate.ToString();
    //        temp = temp.Substring(0, 4) + "/" + temp.Substring(4, 2) + "/" + temp.Substring(6, 2);
    //        return temp;
    //    }

    //    public static string ToLongShamsiDateTime(this DateTime dateTime)
    //    {
    //        System.Globalization.PersianCalendar pc = new System.Globalization.PersianCalendar();
    //        return pc.GetYear(dateTime).ToString("0000") + "/" + pc.GetMonth(dateTime).ToString("00") + "/" + pc.GetDayOfMonth(dateTime).ToString("00") + " " +
    //               pc.GetHour(dateTime).ToString("00") + ":" + pc.GetMinute(dateTime).ToString("00") + ":" + pc.GetSecond(dateTime).ToString("00");
    //    }
    }
}
