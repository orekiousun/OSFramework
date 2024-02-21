using System;

namespace OSFramework
{
    /// <summary>
    /// 数据提供者
    /// </summary>
    /// <typeparam name="T">数据提供者的持有者类型</typeparam>
    internal sealed class DataProvider<T> : IDataProvider<T>
    {
        public event EventHandler<ReadDataSuccessEventArgs> ReadDataSuccess;
        public event EventHandler<ReadDataFailureEventArgs> ReadDataFailure;
        public event EventHandler<ReadDataUpdateEventArgs> ReadDataUpdate;
        public event EventHandler<ReadDataDependencyAssetEventArgs> ReadDataDependencyAsset;
        public void ReadData(string dataAssetName)
        {
            throw new NotImplementedException();
        }

        public void ReadData(string dataAssetName, int priority)
        {
            throw new NotImplementedException();
        }

        public void ReadData(string dataAssetName, object userData)
        {
            throw new NotImplementedException();
        }

        public void ReadData(string dataAssetName, int priority, object userData)
        {
            throw new NotImplementedException();
        }

        public bool ParseData(string dataString)
        {
            throw new NotImplementedException();
        }

        public bool ParseData(string dataString, object userData)
        {
            throw new NotImplementedException();
        }

        public bool ParseData(byte[] dataBytes)
        {
            throw new NotImplementedException();
        }

        public bool ParseData(byte[] dataBytes, object userData)
        {
            throw new NotImplementedException();
        }

        public bool ParseData(byte[] dataBytes, int startIndex, int length)
        {
            throw new NotImplementedException();
        }

        public bool ParseData(byte[] dataBytes, int startIndex, int length, object userData)
        {
            throw new NotImplementedException();
        }
    }
}