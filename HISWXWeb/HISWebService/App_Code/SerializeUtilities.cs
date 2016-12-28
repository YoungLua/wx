using System;
using System.Data;
using System.Configuration; 
using System.Web; 
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

/// <summary>
///SerializeUtilities 的摘要说明
/// </summary>
public class SerializeUtilities
{
    public SerializeUtilities()
    {
        //
        //TODO: 在此处添加构造函数逻辑
        //
    }

    /// <summary>
    /// 序列化 对象到字符串
    /// </summary>
    /// <param name="obj">泛型对象</param>
    /// <returns>序列化后的字符串</returns>
    public static string Serialize<T>(T obj)
    {
        try
        {
            IFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, obj);
            stream.Position = 0;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            stream.Flush();
            stream.Close();
            return Convert.ToBase64String(buffer);
        }
        catch (Exception ex)
        {
            throw new Exception("序列化失败,原因:" + ex.Message);
        }
    }

    /// <summary>
    /// 反序列化 字符串到对象
    /// </summary>
    /// <param name="obj">泛型对象</param>
    /// <param name="str">要转换为对象的字符串</param>
    /// <returns>反序列化出来的对象</returns>
    public static T Desrialize<T>(T obj, string str)
    {
        try
        {
            obj = default(T);
            IFormatter formatter = new BinaryFormatter();
            byte[] buffer = Convert.FromBase64String(str);
            MemoryStream stream = new MemoryStream(buffer);
            obj = (T)formatter.Deserialize(stream);
            stream.Flush();
            stream.Close();
        }
        catch (Exception ex)
        {
            throw new Exception("反序列化失败,原因:" + ex.Message);
        }
        return obj;
    }



    /// <summary>
    /// 序列化
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string SerializeObject(Object obj)
    {
        System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(obj.GetType());
        MemoryStream ms = new MemoryStream();
        xs.Serialize(ms, obj);
        ms.Seek(0, SeekOrigin.Begin);
        byte[] buffer = new byte[ms.Length];
        int n = ms.Read(buffer, 0, buffer.Length);
        ms.Close();
        return System.Text.Encoding.UTF8.GetString(buffer, 0, n);


    }
    /// <summary>
    /// 反序列化
    /// </summary>
    /// <param name="elPatRecordInfo"></param>
    /// <returns></returns>
    public static Object DeserializeString(Type type, string elPatRecordInfo)
    {
        MemoryStream ms = new MemoryStream();
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(elPatRecordInfo);
        ms.Write(buffer, 0, buffer.Length);
        ms.Seek(0, SeekOrigin.Begin);
        System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(type);
        Object objectInfo = xs.Deserialize(ms);
        ms.Close();
        return objectInfo;
    } 
}
