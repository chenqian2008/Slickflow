﻿using System;
using System.Data;
using Slickflow.Data;
using Slickflow.Module.Localize;
using Slickflow.Engine.Common;
using Slickflow.Engine.Core.Result;
using Slickflow.Engine.Business.Entity;
using Slickflow.Engine.Business.Manager;



namespace Slickflow.Engine.Xpdl
{
    /// <summary>
    /// 流程模型工厂类
    /// </summary>
    public class ProcessModelFactory
    {
        /// <summary>
        /// 创建流程模型实例
        /// </summary>
        /// <param name="processGUID">流程GUID</param>
        /// <param name="version">版本</param>
        /// <returns>流程模型</returns>
        public static IProcessModel Create(string processGUID, string version)
        {
            using (var session = SessionFactory.CreateSession())
            {
                return Create(session.Connection, processGUID, version, session.Transaction);
            }
        }

        /// <summary>
        /// 创建流程模型实例
        /// </summary>
        /// <param name="conn">链接</param>
        /// <param name="processGUID">流程GUID</param>
        /// <param name="version">版本</param>
        /// <param name="trans">事务</param>
        /// <returns>流程模型</returns>
        internal static IProcessModel Create(IDbConnection conn, 
            string processGUID, 
            string version,
            IDbTransaction trans)
        {
            ProcessEntity entity = null;
            var pm = new ProcessManager();
            if (!string.IsNullOrEmpty(version))
            {
                entity = pm.GetByVersion(conn, processGUID, version, false, trans);
            }
            else
            {
                //如果版本信息缺省，默认取当前IsUsing=1的流程记录
                entity = pm.GetVersionUsing(conn, processGUID, trans);
            }

            if (entity == null)
            {
                throw new WfXpdlException(LocalizeHelper.GetEngineMessage("processmodel.factory.processnullexception"));
            }
            IProcessModel processModel = new ProcessModel(entity);
            return processModel;
        }
    }
}
