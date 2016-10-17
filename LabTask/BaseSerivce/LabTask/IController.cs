﻿using LabMCESystem.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.BaseSerivce.LabTask
{
    /// <summary>
    /// 可执行控制器
    /// 提供可执行控制设备的基本操作和事件
    /// </summary>
    public interface IController
    {
        event ExecuteSetterEventHandler ExecuteSetter;

        event ExceuteMultipleSettersEventHandler ExecuteMultipleSetters;

    }

    /// <summary>
    /// 单个临时控制设定项事件委托
    /// </summary>
    /// <param name="sender">事件源，一般为TaskDistributeCenter对象</param>
    /// <param name="setter">需要完成的设定</param>
    public delegate void ExecuteSetterEventHandler(object sender, Setter setter);

    /// <summary>
    /// 多重临时控制设定项事件委托
    /// </summary>
    /// <param name="sender">一般为TaskDistributeCenter对象</param>
    /// <param name="setters">需要完成的多个设定项列表</param>
    public delegate void ExceuteMultipleSettersEventHandler(object sender, List<Setter> setters);
}
