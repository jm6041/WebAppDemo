using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Learn.Services
{
    /// <summary>
    /// 可以作为验证的结果
    /// </summary>
    [Serializable]
    [DataContract]
    public class ResultDto
    {
        /// <summary>
        /// 验证的错误消息
        /// </summary>
        protected Dictionary<string, HashSet<string>> _errors { get; set; } = new Dictionary<string, HashSet<string>>();
        /// <summary>
        /// 是否成功
        /// </summary>
        [DataMember(Order = 1)]
        public bool IsSuccess => !_errors.Any();
        /// <summary>
        /// 添加异常
        /// </summary>
        /// <param name="error">异常信息</param>
        public void AddError(string error)
        {
            AddErrorInner(string.Empty, error);
        }
        /// <summary>
        /// 添加异常
        /// </summary>
        /// <param name="key">异常标识</param>
        /// <param name="error">异常信息</param>
        private void AddErrorInner(string key, string error)
        {
            if (!_errors.TryGetValue(key, out HashSet<string> val))
            {
                val = new HashSet<string>();
                _errors.Add(key, val);
            }
            AddError(error, ref val);
        }
        /// <summary>
        /// 添加异常
        /// </summary>
        /// <param name="key">异常标识</param>
        /// <param name="error">异常信息</param>
        public void AddError(string key, string error)
        {
            AddErrorInner(key, error);
        }
        /// <summary>
        /// 添加异常
        /// </summary>
        /// <param name="key">异常标识</param>
        /// <param name="errors">异常信息</param>
        public void AddErrors(string key, params string[] errors)
        {
            AddErrors(key, errors as ICollection<string>);
        }
        /// <summary>
        /// 添加异常
        /// </summary>
        /// <param name="key">异常标识</param>
        /// <param name="errors">异常信息</param>
        public void AddErrors(string key, ICollection<string> errors)
        {
            if (errors == null || !errors.Any())
            {
                return;
            }
            if (!_errors.TryGetValue(key, out HashSet<string> val))
            {
                val = new HashSet<string>();
                _errors.Add(key, val);
            }
            foreach (string error in errors)
            {
                //val.Add(error);
                AddError(error, ref val);
            }
        }

        /// <summary>
        /// 添加异常
        /// </summary>
        /// <param name="errDic">异常信息</param>
        public void AddErrorDic(Dictionary<string, List<string>> errDic)
        {
            if (errDic == null)
            {
                return;
            }
            foreach (var err in errDic)
            {
                string key = err.Key;
                if (!_errors.TryGetValue(key, out HashSet<string> val))
                {
                    val = new HashSet<string>();
                    foreach (string e in err.Value)
                    {
                        //val.Add(e);
                        AddError(e, ref val);
                    }
                    _errors.Add(key, val);
                }
                else
                {
                    foreach (string e in err.Value)
                    {
                        //val.Add(e);
                        AddError(e, ref val);
                    }
                }
            }
        }
        /// <summary>
        /// 获取异常字典
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, List<string>> GetErrorDic()
        {
            return _errors.ToDictionary((k) => k.Key, (v) => v.Value.ToList());
        }

        /// <summary>
        /// 合并结果
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public ResultDto Merge(ResultDto result)
        {
            this.AddErrorDic(result.GetErrorDic());
            return this;
        }

        /// <summary>
        /// 异常消息
        /// </summary>
        [DataMember(Order = 2)]
        public string ErrorMessage
        {
            get
            {
                if (_errors.Any())
                {
                    return this.ToString();
                }
                return string.Empty;
            }
            protected set { }
        }
        /// <summary>
        /// 重写 ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (!_errors.Any())
            {
                return base.ToString();
            }
            foreach (var err in _errors)
            {
                if (string.IsNullOrEmpty(err.Key))
                {
                    sb.AppendLine(string.Join(";", err.Value));
                }
                else
                {
                    sb.Append(err.Key).Append(":");
                    sb.AppendLine(string.Join(";", err.Value));
                }
            }
            return sb.ToString().TrimEnd();
        }

        private void AddError(string error, ref HashSet<string> val)
        {
            val.Add(error);
        }
    }
    /// <summary>
    /// 数据返回结果
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    [Serializable]
    [DataContract]
    public class DataResultDto<T> : ResultDto
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public DataResultDto()
        { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="data">数据</param>
        public DataResultDto(T data)
        {
            Data = data;
        }

        /// <summary>
        /// 涉及的数据
        /// </summary>
        [DataMember(Order = 5)]
        public T Data { get; set; }
    }
}
