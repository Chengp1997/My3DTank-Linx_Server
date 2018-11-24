using System;
using System.Collections;
using System.Linq;
public class ProtocolBytes : ProtocolBase
{
    //传输的字节流
    public byte[] bytes;
    //解码器
    public override ProtocolBase Decode(byte[] readbuff, int start, int length)
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.bytes = new byte[length];
        Array.Copy(readbuff, start, protocol.bytes, 0, length);
        return protocol;
    }
    //编码器
    public override byte[] Encode()
    {
        return bytes;
    }
    //协议名称
    public override string GetName()
    {
        return GetString(0);
    }
    //描述    提取每一个字节然后组装成字符串
    public override string GetDesc()
    {
        string str = "";
        if (bytes == null) return str;
        for (int i = 0; i < bytes.Length; i++)
        {
            int b = (int)bytes[i];
            str += b.ToString() + " ";
        }
        return str;
    }

    /// 辅助方法
    //添加字符串
    public void AddString(string str)
    {
        Int32 len = str.Length;
        byte[] lenBytes = BitConverter.GetBytes(len);
        byte[] strBytes = System.Text.Encoding.UTF8.GetBytes(str);
        if (bytes == null)
            bytes = lenBytes.Concat(strBytes).ToArray();
        else
            bytes = bytes.Concat(lenBytes).Concat(strBytes).ToArray();
    }
    //从字节数组的start 处开始读取字符串   这里是从一个完整的协议开始读   即start是表示字符串大小字节的4个字节的开头
    public string GetString(int start, ref int end)
    {
        if (bytes == null)  //字节数组为空
            return "";
        if (bytes.Length < start + sizeof(Int32))   //start后面小于4个字节
            return "";
        //这个函数需要说明一下，因为Int32只占4个字节 所以是从start开始的4个字节
        Int32 strLen = BitConverter.ToInt32(bytes, start);
        //字节数小于字符串的长度 代表那串字符串的字节数至少与字符串长度相等
        if (bytes.Length < start + sizeof(Int32) + strLen)
            return "";
        string str = System.Text.Encoding.UTF8.GetString(bytes, start + sizeof(Int32), strLen);
        end = start + sizeof(Int32) + strLen;
        return str;
    }
    public string GetString(int start)
    {
        int end = 0;
        return GetString(start, ref end);
    }
    //Int
    public void AddInt(int num)
    {
        byte[] numBytes = BitConverter.GetBytes(num);
        if (bytes == null)
            bytes = numBytes;
        else
            bytes = bytes.Concat(numBytes).ToArray();
    }
    public int GetInt(int start, ref int end)
    {
        //以下返回的0都是默认值
        if (bytes == null)
            return 0;
        //Int32 占 4个字节 如果后面都没有四个字节 还谈什么
        if (bytes.Length < start + sizeof(Int32))
            return 0;
        end = start + sizeof(Int32);
        return BitConverter.ToInt32(bytes, start);
    }
    public int GetInt(int start){
        int end = 0;
        return GetInt(start,ref end);
    }
    //Float
    public void AddFloat(float num){
        byte[] numBytes = BitConverter.GetBytes(num);
        if (bytes == null)
            bytes = numBytes;
        else
            bytes = bytes.Concat(numBytes).ToArray();
    }
    public float GetFloat(int start,ref int end)
    {
        if (bytes == null)
            return 0;
        if(bytes.Length<start+sizeof(float))
            return 0;
        end = start + sizeof(float);
        return BitConverter.ToSingle(bytes,start);
    }
    public float GetFloat(int start)
    {
        int end = 0;
        return GetFloat(start,ref end);
    }
}