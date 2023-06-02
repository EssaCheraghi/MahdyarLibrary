using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Mahdyar_Library
{
    public static partial class Cls_CSharpExtention
    {
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> pSeq)
        {
            return pSeq ?? Enumerable.Empty<T>();
        }
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
                    string val = (string)prop.GetValue(model, null);
                    if (val == "")
                        prop.SetValue(model, null,null);
                }
                else if (!emptyStringOnly && prop.PropertyType == typeof(long?))
                {
                    long? val = (long?)prop.GetValue(model, null);
                    if (val == 0)
                        prop.SetValue(model, null, null);
                }
                else if (!emptyStringOnly && prop.PropertyType == typeof(int?))
                {
                    int? val = (int?)prop.GetValue(model, null);
                    if (val == 0)
                        prop.SetValue(model, null, null);
                }
                else if (!emptyStringOnly && prop.PropertyType == typeof(short?))
                {
                    short? val = (short?)prop.GetValue(model, null);
                    if (val == 0)
                        prop.SetValue(model, null, null);
                }
            }
            return model;
        }
        public static T ParseEnum<T>(this string s)
        {
            return (T)Enum.Parse(typeof(T), s);
        }
        public static Color Ext_GetInvert(this Color originalColor)
        {
            Color invertedColor = new Color();
            invertedColor.ScR = 1.0F - originalColor.ScR;
            invertedColor.ScG = 1.0F - originalColor.ScG;
            invertedColor.ScB = 1.0F - originalColor.ScB;
            invertedColor.ScA = originalColor.ScA;
            return invertedColor;
        }


      /// <summary>Serializes an object of type T in to an xml string</summary>
      /// <typeparam name="T">Any class type</typeparam>
      /// <param name="obj">Object to serialize</param>
      /// <returns>A string that represents Xml, empty otherwise</returns>
      public static string Ext_XmlSerialize<T>(this T obj) //where T : class, new()
      {
          if (obj == null) throw new ArgumentNullException("obj");

          var serializer = new XmlSerializer(typeof(T));
          using (var writer = new StringWriter())
          {
              serializer.Serialize(writer, obj);
              return writer.ToString();
          }
      }

      /// <summary>Deserializes an xml string in to an object of Type T</summary>
      /// <typeparam name="T">Any class type</typeparam>
      /// <param name="xml">Xml as string to deserialize from</param>
      /// <returns>A new object of type T is successful, null if failed</returns>
      public static T Ext_XmlDeserialize<T>(this string xml) where T : class, new()
      {
          if (xml == null) throw new ArgumentNullException("xml");

          var serializer = new XmlSerializer(typeof(T));
          using (var reader = new StringReader(xml))
          {
              try
              {
                  return (T) serializer.Deserialize(reader);
              }
              catch (Exception ex)
              {
                  return null;
              } // Could not be deserialized to this type.
          }
      }

        /// <summary>
        /// not ready
        /// </summary>
        /// <param name="F"></param>
        /// <param name="mod"></param>
        /// <returns></returns>
      public static int Ext_LengthInKB(this FileInfo F, bool mod = false)
      {
       if (mod) return (int)F.Length % 1024;
       return (int)F.Length / 1024;
      }
        /// <summary>
      ///  not ready
        /// </summary>
        /// <param name="F"></param>
        /// <param name="mod"></param>
        /// <returns></returns>
      public static int Ext_LengthInMB(this FileInfo F, bool mod = false)
      {
          if (mod) return (int)F.Ext_LengthInKB() % 1024;
          return (int)F.Ext_LengthInKB() / 1024;
      }
        /// <summary>
      ///  not ready
        /// </summary>
        /// <param name="F"></param>
        /// <param name="mod"></param>
        /// <returns></returns>
      public static int Ext_LengthInGB(this FileInfo F, bool mod = false)
      {
          if (mod) return (int)F.Ext_LengthInMB() % 1024;
          return (int)F.Ext_LengthInMB() / 1024;
      }
      public static T Ext_ConvertTo<T>(this IConvertible obj)
      {
          return (T)Convert.ChangeType(obj, typeof(T));
      }

    

        public static string Ext_InsertSeparator(this string Price, int insertEvery, string insert)
        {
            if (string.IsNullOrEmpty(Price) || string.IsNullOrEmpty(insert)) return Price;
            string res = "";
            for (int u = 0; u < Price.Length; u++)
            {
                res = Price[u] + res;
                if ((Price.Length - u - 1) % insertEvery == 0) res = insert + res;
            }
            return res.Substring(1).Ext_Reverse();
        }
        public static string Ext_ToPersianString(this int i)
        {
            string persianstring = "";
            return persianstring;
        }

      public static string Ext_ToString(this TimeSpan Ts,bool ShowHour=false)
      {
          if (!ShowHour) return Ts.Minutes.ToString() + ":" + Ts.Seconds.ToString();
          return Ts.Hours.ToString() + ":" + Ts.Minutes.ToString() + ":" + Ts.Seconds.ToString();
      }
        /// <summary>
      /// use it in AppDomain.CurrentDomain.UnhandledException 
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="Action1"></param>
      public static void Ext_Handle(this Exception ex,Action<string> Action1)
      {
          string stacks = "Exception Message = " + ex.Message + Environment.NewLine;
          if (ex.InnerException != null) stacks += "Inner Exception Message = " + ex.InnerException.Message + Environment.NewLine;
          stacks += "Stack Trace" + Environment.NewLine;
          foreach (var f in ex.StackTrace.Split(new string[]{"at"},StringSplitOptions.None))
          {
              if (f.Contains("line")) stacks += f + Environment.NewLine;
          }

          Action1.Invoke(stacks);
      }
        /// <summary>
        /// use it in AppDomain.CurrentDomain.UnhandledException 
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="Action1"></param>
        public static string Ext_GetFullMessage(this Exception ex,bool containsStacktrace = false)
        {
            string stacks = "Exception Message = " + ex.Message;
            if (ex.InnerException != null) stacks += Environment.NewLine + "Inner Exception Message = " + ex.InnerException.Message;
            if (!containsStacktrace) return stacks;
            stacks += Environment.NewLine + "Stack Trace" + Environment.NewLine;
            foreach (var f in ex.StackTrace.Split(new string[] { "at" }, StringSplitOptions.None))
            {
                if (f.Contains("line")) stacks += f + Environment.NewLine;
            }

            return stacks;
        }

        public static bool Ext_HasAttribute(this PropertyInfo T1, Type AttributeType)
        {
            return T1.GetCustomAttributes(AttributeType, false).Length > 0;
        }

        public static T Ext_GetAttributeProperty<T>(this PropertyInfo T1)
        {
            return (T)T1.GetCustomAttributes(typeof(T), false)[0];
        }
        public static bool Ext_HasMethod(this MethodInfo T1, Type AttributeType)
        {
            return T1.GetCustomAttributes(AttributeType, false).Length > 0;
        }
        public static T Ext_GetAttributeMethod<T>(this MethodInfo T1)
        {
            return (T)T1.GetCustomAttributes(typeof(T), false)[0];
        }
        public static bool Ext_HasMethod(this Type T1, Type AttributeType)
        {
            return T1.GetCustomAttributes(AttributeType, false).Length > 0;
        }
        public static T Ext_GetAttributeMethod<T>(this Type T1)
        {
            return (T)T1.GetCustomAttributes(typeof(T), false).FirstOrDefault();
        }

        // This extension method is broken out so you can use a similar pattern with 
        // other MetaData elements in the future. This is your base method for each.
        public static T GetAttribute<T>(this Enum value) where T : Attribute
        {
            var type = value.GetType();
            var memberInfo = type.GetMember(value.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
            return (T)attributes[0];
        }

        // This method creates a specific call to the above method, requesting the
        // Description MetaData attribute.
        public static string ToName(this Enum value)
        {
            var attribute = value.GetAttribute<DescriptionAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }

        //public static void SetPropertyValue(this object obj, string propName, object value)
        //{
        //    obj.GetType().GetProperty(propName).SetValue(obj, value, null);
        //}
        //public static Object GetPropValue(this Object obj, String propName)
        //{
        //    string[] nameParts = propName.Split('.');
        //    if (nameParts.Length == 1)
        //    {
        //        return obj.GetType().GetProperty(propName).GetValue(obj, null);
        //    }

        //    foreach (String part in nameParts)
        //    {
        //        if (obj == null) { return null; }

        //        Type type = obj.GetType();
        //        PropertyInfo info = type.GetProperty(part);
        //        if (info == null) { return null; }

        //        obj = info.GetValue(obj, null);
        //    }
        //    return obj;
        //}
        public static void SetNestedProperty(this object target,string compoundProperty, object value)
        {
            string[] bits = compoundProperty.Split('.');
            for (int i = 0; i < bits.Length - 1; i++)
            {
                PropertyInfo propertyToGet = target.GetType().GetProperty(bits[i]);
                target = propertyToGet.GetValue(target, null);
            }
            PropertyInfo propertyToSet = target.GetType().GetProperty(bits.Last());
            propertyToSet.SetValue(target, value, null);
        }
        public static PropertyInfo GetNestedProperty(this object t, string PropertFullName)
        {
            if (t.GetType().GetProperties().Count(p => p.Name == PropertFullName.Split('.')[0]) == 0)
                throw new ArgumentNullException(string.Format("Property {0}, is not exists in object {1}", PropertFullName, t.ToString()));
            if (PropertFullName.Split('.').Length == 1)
                return t.GetType().GetProperty(PropertFullName);
            else
                return GetNestedProperty(t.GetType().GetProperty(PropertFullName.Split('.')[0]).GetValue(t, null), PropertFullName.Split('.')[1]);
        }
    }
   
}
