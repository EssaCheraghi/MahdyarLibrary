using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Xml.Linq;
//using ExcelDataReader;
using excel = Microsoft.Office.Interop.Excel;

namespace Mahdyar_Library
{
    public enum FileTypes
    {
        Image
        ,Text
        ,Video
        ,Sound
        ,Other
    };
    public static partial class Methods
    {
      public static class FileMethods
      {
          public static FileTypes GetFileType(string Filename)
          {
              switch (System.IO.Path.GetExtension(Filename).ToLower())
              {
                  case ".gif":
                  case ".jpeg":
                  case ".icon":
                  case ".wmf":
                  case ".emf":
                  case ".exif":
                  case ".bmp":
                  case ".tiff":
                  case ".png":
                  case ".jpg":return FileTypes.Image;
                    case ".mp3":
                    case ".wav":
                    case ".ogg":
                    case ".mdi":return  FileTypes.Sound;

                }
                return FileTypes.Other;
          }
      }


      public static class ProcessMethods
      {
          public static bool Ext_ProcessExists(string name)
          {
              var a = Process.GetProcesses().Where(x => x.ProcessName.ToLower() == name.ToLower()).ToArray().Length;
              if (a == 0) return false;
              return true;
          }
      }

      


        public class Cls_NumberRange
        {
        public static bool IsBetween(int value, int min, int max,bool containsmaxandmin=false)
            {
                if(containsmaxandmin)
                return ((value >= min) && (value <= max));
                return ((value > min) && (value < max));
            }
            /// <summary>
            /// example : 4,5,9-14
            /// </summary>
            /// <param name="Pattern">example : 4,5,9-14</param>
            /// <returns></returns>
            public static int[] GetNumbers(string Pattern)
            {
                var result = Pattern.Split(',')
                                  .Select(x => x.Split('-'))
                                  .Select(p => new { First = int.Parse(p.First()), Last = int.Parse(p.Last()) })
                                  .SelectMany(x => Enumerable.Range(x.First, x.Last - x.First + 1)).Distinct()
                                  .OrderBy(z => z);
                return result.ToArray();
            }
        }

        public class cls_DynamicXml : DynamicObject
        {
            XElement _root;
            private cls_DynamicXml(XElement root)
            {
                _root = root;
            }

            public static cls_DynamicXml Parse(string Filename)
            {
                return new cls_DynamicXml(XDocument.Parse(File.ReadAllText(Filename)).Root);
            }

            public static cls_DynamicXml Load(string filename)
            {
                return new cls_DynamicXml(XDocument.Load(filename).Root);
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                result = null;

                var att = _root.Attribute(binder.Name);
                if (att != null)
                {
                    result = att.Value;
                    return true;
                }

                var nodes = _root.Elements(binder.Name);
                if (nodes.Count() > 1)
                {
                    result = nodes.Select(n => n.HasElements ? (object)new cls_DynamicXml(n) : n.Value).ToList();
                    return true;
                }

                var node = _root.Element(binder.Name);
                if (node != null)
                {
                    result = node.HasElements ? (object)new cls_DynamicXml(node) : node.Value;
                    return true;
                }

                return true;
            }
        }


        public class PeriodJob<T>
        {
            public void PeriodJobDaily(int HourToPerform,int IntervalMilisecond, T Object, Action<T> PerformJobAction)
            {

           PerformJobAction.Invoke(Object);
            }


            public void Start()
            {
                
            }
        }

        public enum EPeriodJob
        {
            Yearly=0,Monthly,Daily,Hourly,MinutLy,Secoundly
        }

        public static class ReflactMethods
        {
            public class PropertyParameter
            {
                public string ClassName { get; set; }
                public string Name { get; set; }
                public Type type { get; set; }
                public bool Isout { get; set; }
                public bool IsArray { get; set; }
                public bool IsReturn { get; set; }
            }

            public static List<PropertyParameter> RetrievePrimitiveTypes(Type type1)
            {
                List<PropertyParameter> Primitives = new List<PropertyParameter>();
              
                bool b1 = IsPrimitive(type1);
                bool b2 = IsArray(type1);

                if (b1)
                {
                    Primitives.Add(new PropertyParameter() {ClassName = type1.ReflectedType?.FullName, Name = type1.Name, type = type1, IsReturn = true});
                }
                else
                {
                    if (b2) type1 = type1.GetElementType();
                    foreach (var param in type1.GetProperties())
                    {
                        bool b = IsPrimitive(param.PropertyType);
                        if (b) Primitives.Add(new PropertyParameter() { ClassName =param.ReflectedType?.FullName, Name = param.Name, type = param.PropertyType, IsReturn = true, IsArray = b2 });
                        else
                            Primitives.AddRange(RetrievePrimitiveTypes2(param.PropertyType).Select(x => new PropertyParameter() {ClassName = x.ReflectedType?.FullName, Name = x.Name, type = x.PropertyType, IsReturn = true, IsArray = b2 }));
                    }}return Primitives;
            }
            public static List<PropertyParameter> RetrievePrimitiveTypes(ParameterInfo[] params1)
            {
                List<PropertyParameter> Primitives = new List<PropertyParameter>();
                foreach (var param in params1)
                {
                    Type tp1 = param.ParameterType;
                    
                    bool b = IsPrimitive(tp1);
                    bool b2 = IsArray(tp1);
                    if (b) Primitives.Add(new PropertyParameter() { ClassName = param.ParameterType.FullName,Name =  param.Name, type = tp1, Isout = param.IsOut});
                    else
                    {if (b2) tp1 = tp1.GetElementType();
                        var primitivetypes = RetrievePrimitiveTypes2(tp1).ToArray();
                        for (int u = 0; u < primitivetypes.Length; u++)
                        {
                            var tp2 = primitivetypes[u].PropertyType;
                            Primitives.Add(new PropertyParameter() { ClassName = primitivetypes[u].ReflectedType?.FullName, Name = primitivetypes[u].Name, type = tp2, Isout = param.IsOut, IsArray = b2 });

                        }

                    }
                }
                return Primitives;
            }

             static IEnumerable<PropertyInfo> RetrievePrimitiveTypes2(Type t)
            {
                var visitedTypes = new HashSet<Type>();
                var result = new List<PropertyInfo>();
                InternalVisit(t, visitedTypes, result);
                return result;
            }

            static void InternalVisit(Type t, HashSet<Type> visitedTypes, IList<PropertyInfo> result)
            {
                if (visitedTypes.Contains(t))
                {
                    return;
                }

                if (!IsPrimitive(t)){
                    visitedTypes.Add(t);
                    foreach (var property in t.GetProperties())
                    {
                        if (IsPrimitive(property.PropertyType))
                        {
                            result.Add(property);
                        }
                        else if(property.PropertyType.GetInterfaces().Contains(typeof(IEnumerable)))
                            throw new Exception("Array Just for root properties are Supported");
                        InternalVisit(property.PropertyType, visitedTypes, result);
                    }
                }
                else
                {
                    //result.Add(t);
                }
            }

            public static bool IsArray(Type t)
            {
                return t.GetInterfaces().Contains(typeof (IEnumerable)) && t != typeof (string);
            }
            public static bool IsPrimitive(Type t)
            {
                // TODO: put any type here that you consider as primitive as I didn't
                // quite understand what your definition of primitive type is
                
                List<Type> h = new List<Type>()
                {
                    typeof (string),
                    typeof (char),
                    typeof (byte),
                    typeof (sbyte),
                    typeof (ushort),
                    typeof (short),
                    typeof (uint),
                    typeof (int),
                    typeof (ulong),
                    typeof (long),
                    typeof (float),
                    typeof (double),
                    typeof (decimal),
                    typeof (DateTime),
                    typeof (char?),
                    typeof (byte?),
                    typeof (sbyte?),
                    typeof (ushort?),
                    typeof (short?),
                    typeof (uint?),
                    typeof (int?),
                    typeof (ulong?),
                    typeof (long?),
                    typeof (float?),
                    typeof (double?),
                    typeof (decimal?),
                    typeof (DateTime?),
                };
                return h.Contains(t);
            }
        }

        public static class ExcelMethods
        {
            //public static DataTable ReadExcel(string originalFileName, string SheetName = "Sheet1")
            //{
            //    try
            //    {
            //        //var file = new FileInfo(originalFileName);
            //        string extension = Path.GetExtension(originalFileName);
            //        using (
            //            var stream = File.Open(originalFileName, FileMode.Open, FileAccess.Read))
            //        {
            //            IExcelDataReader reader;

            //            if (extension.Equals(".xls"))
            //                reader = ExcelDataReader.ExcelReaderFactory.CreateBinaryReader(stream);
            //            else if (extension.Equals(".xlsx"))
            //                reader = ExcelDataReader.ExcelReaderFactory.CreateOpenXmlReader(stream);
            //            else
            //                throw new Exception("Invalid FileName");

            //            //// reader.IsFirstRowAsColumnNames
            //            var conf = new ExcelDataSetConfiguration
            //            {
            //                ConfigureDataTable = _ => new ExcelDataTableConfiguration
            //                {
            //                    UseHeaderRow = true
            //                }
            //            };

            //            var dataSet = reader.AsDataSet(conf);
            //            var dataTable = dataSet.Tables[SheetName];

            //            return dataTable;
            //        }
            //    }
            //    catch (Exception Exp)
            //    {
            //        throw Exp;
            //    }
            //}

            //public static string[] GetExcelSheetNames(string originalFileName)
            //{
            //    string extension = Path.GetExtension(originalFileName);
            //    using (
            //        var stream = File.Open(originalFileName, FileMode.Open, FileAccess.Read))
            //    {
            //        IExcelDataReader reader;

            //        if (extension.Equals(".xls"))
            //            reader = ExcelDataReader.ExcelReaderFactory.CreateBinaryReader(stream);
            //        else if (extension.Equals(".xlsx"))
            //            reader = ExcelDataReader.ExcelReaderFactory.CreateOpenXmlReader(stream);
            //        else
            //            throw new Exception("Invalid FileName");

            //        //// reader.IsFirstRowAsColumnNames
            //        var conf = new ExcelDataSetConfiguration
            //        {
            //            ConfigureDataTable = _ => new ExcelDataTableConfiguration
            //            {
            //                UseHeaderRow = true
            //            }
            //        };

            //        var dataSet = reader.AsDataSet(conf);
            //        return dataSet.Tables.Cast<DataTable>().Select(x=>x.TableName).ToArray();
                    
            //    }
            //}

            //public static List<string> ReadExcelColumn(string fileName, string SheetName, string ColName,bool SkipRepeatedItems = false)
            //{
            //    var datatable1 = ReadExcel(fileName, SheetName);
            //    List<string> _data=new List<string>();
            //    foreach (DataRow row in datatable1.Rows)
            //    {
            //        string val = (string) Convert.ToString(row[ColName]);
            //        if (SkipRepeatedItems && _data.Contains(val)) continue;_data.Add(val);
            //    }
            //    return _data;
            //}


            public static bool WriteToExcel(System.Data.DataTable dt, string location)
            {
                //instantiate excel objects (application, workbook, worksheets)
                excel.Application XlObj = new excel.Application();
                XlObj.Visible = false;
                excel._Workbook WbObj = (excel.Workbook)(XlObj.Workbooks.Add(""));
                excel._Worksheet WsObj = (excel.Worksheet)WbObj.ActiveSheet;

                //run through datatable and assign cells to values of datatable
                try
                {
                    int row = 1; int col = 1;
                    foreach (DataColumn column in dt.Columns)
                    {
                        //adding columns
                        WsObj.Cells[row, col] = column.ColumnName;
                        col++;
                    }
                    //reset column and row variables
                    col = 1;
                    row++;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        //adding data
                        foreach (var cell in dt.Rows[i].ItemArray)
                        {
                            WsObj.Cells[row, col] = "'" + cell;
                            col++;
                        }
                        col = 1;
                        row++;
                    }
                    WbObj.SaveAs(location);
                }
                catch (COMException x)
                {
                    return false;
                    // ErrorHandler.Handle(x);
                }
                catch (Exception ex)
                {
                    return false;
                    //  ErrorHandler.Handle(ex);
                }
                finally
                {
                    WbObj.Close();
                }
                return true;
            }
        }
        public static class Sizing
        {
            public static class Pages
            {
                public static Size A4
                {
                    get
                    {
                        return new Size((int)CmToPx(21),(int)CmToPx(29.7));
                    }
                }
            }
            private struct PixelUnitFactor
            {
                public const double Px = 1.0;
                public const double Inch = 96.0;
                public const double Cm = 37.7952755905512;
                public const double Pt = 1.33333333333333;
                
            }

            public static double CmToPx(double cm)
            {
                return cm * PixelUnitFactor.Cm;
            }

            public static double PxToCm(double px)
            {
                return px / PixelUnitFactor.Cm;
            }
        }
    }


}
