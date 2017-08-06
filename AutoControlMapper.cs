using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PMMSUtil
{
    /// <summary>

    /// 控件值自动映射类，此类通过Control.Tag属性，将注册的变量的值，自动更新到相应的控件上，免去了手动更新的麻烦。只需简单的3步就能自动更新变量

    /// AutoControlMapper acm = new AutoControlMapper();    //创建实例

    /// acm.RegisterVariableObject( null, val );    //注册变量

    /// acm.UpdateToUI( frm_map );  //更新到控件

    /// </summary>

    public class AutoControlMapper
    {
        public AutoControlMapper() { }


        protected Dictionary<string, object> dic_objs = new Dictionary<string, object>();
        protected Dictionary<string, object> dic_vals = new Dictionary<string, object>();
        protected Dictionary<string, PropertyInfo> dic_typs = new Dictionary<string, PropertyInfo>();

        /// <summary>

        /// 注册变量

        /// </summary>

        /// <param name="objName">对象的名称，如果此属性名称为唯一注册，可以为null，用于在注册相同名称属性的多个实例时，唯一识别之用。</param>

        /// <param name="obj">对象本身</param>

        public void RegisterVariableObject(string objName, object obj)
        {
            string str_header = null;
            if (objName != null)
                str_header = objName + ".";
            else
                str_header = "";

            System.Type typ = obj.GetType();
            foreach (System.Reflection.PropertyInfo typ_item in typ.GetProperties())
            {
                try
                {//将注册的属性及值添加到字典

                    dic_objs.Add(str_header + typ_item.Name, obj);  //注册对象本身

                    dic_typs.Add(str_header + typ_item.Name, typ_item); //注册对象的属性

                    dic_vals.Add(str_header + typ_item.Name, typ_item.GetValue(obj));   //注册对象的值

                }
                catch (Exception ex)
                {
                    SysMisc.LogHelper.Error(typeof(AutoControlMapper), ex);
                }
            }
        }

        public void UpdateToUI(Control ctl_in)
        {
            Debug.Assert(ctl_in != null);
            //枚举所有属性

            Control.ControlCollection ctl_col = ctl_in.Controls;

            foreach (Control ctl in ctl_col)
            {
                //SysMisc.LogHelper.Debug(typeof(AutoControlMapper), ctl.GetType().ToString());

                string propertyName = ctl.Tag as string;
                if (propertyName != null)
                {
                    switch (ctl.GetType().ToString())
                    {
                        case "DevComponents.DotNetBar.Controls.TextBoxX":
                            ctl.Text = GetValueByFullName(propertyName) as string;
                            break;
                        case "DevComponents.DotNetBar.Controls.CheckBoxX":
                            (ctl as DevComponents.DotNetBar.Controls.CheckBoxX).Checked = (bool)GetValueByFullName(propertyName);
                            break;
                        case "DevComponents.Editors.IntegerInput":
                            (ctl as DevComponents.Editors.IntegerInput).Value = (int)GetValueByFullName(propertyName);
                            break;
                        case "DevComponents.Editors.DateTimeAdv.DateTimeInput":
                            (ctl as DevComponents.Editors.DateTimeAdv.DateTimeInput).Value = (DateTime)GetValueByFullName(propertyName);
                            break;
                        default:
                            break;
                    }
                }

                //向下继续搜索

                if (ctl.Controls != null)
                    UpdateToUI(ctl);
            }
        }

        public void UpdateFromUI( Control ctl_in)
        {
            Debug.Assert(ctl_in != null);
            //枚举所有属性

            Control.ControlCollection ctl_col = ctl_in.Controls;

            foreach (Control ctl in ctl_col)
            {
                //SysMisc.LogHelper.Debug(typeof(AutoControlMapper), ctl.GetType().ToString());

                string propertyName = ctl.Tag as string;
                if (propertyName != null)
                {
                    switch (ctl.GetType().ToString())
                    {
                        case "DevComponents.DotNetBar.Controls.TextBoxX":
                            SetValueByFullName(typeof(string), propertyName, ctl.Text);
                            break;
                        case "DevComponents.DotNetBar.Controls.CheckBoxX":
                            SetValueByFullName(typeof(bool), propertyName, (ctl as DevComponents.DotNetBar.Controls.CheckBoxX).Checked);
                            break;
                        case "DevComponents.Editors.IntegerInput":
                            SetValueByFullName(typeof(int), propertyName, (ctl as DevComponents.Editors.IntegerInput).Value);
                            break;
                        case "DevComponents.Editors.DateTimeAdv.DateTimeInput":
                            SetValueByFullName(typeof(DateTime), propertyName, (ctl as DevComponents.Editors.DateTimeAdv.DateTimeInput).Value);
                            break;
                        default:
                            break;
                    }
                }

                //向下继续搜索

                if (ctl.Controls != null)
                    UpdateFromUI(ctl);
            }
        }

        //从注册的变量中取值

        protected object GetValueByFullName(string fullName)
        {
            object typ_item = null;
            bool finded = dic_vals.TryGetValue(fullName, out typ_item);
            if (false == finded)
                SysMisc.LogHelper.Error(typeof(AutoControlMapper), string.Format("未找到注册名称[{0}]对应的值信息！"));
            return typ_item;
        }

        //将值更新到注册的变量中

        protected void SetValueByFullName( Type typ, string fullName, object val)
        {
            PropertyInfo typ_item;
            bool finded = dic_typs.TryGetValue(fullName, out typ_item);

            if (false == finded)
            {
                SysMisc.LogHelper.Error(typeof(AutoControlMapper), string.Format("未找到注册名称[{0}]对应的属性信息！"));
                return;
            }

            //如果上面的找到了，则认为这个肯定能找到（一般也确实是这样）

            object obj = dic_objs[fullName];
            typ_item.SetValue(obj, val);
        }
    }
}
