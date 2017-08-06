#AutoControlMapper
一个通过Contorl.Tag属性，将变量值与控件自动映射的工具类，使用非常简单。

---

###使用示例

```C#
Form frm = new Control();
AutoControlMapper _acm = new AutoControlMapper();//实例化对象
_acm.RegisterVariableObject(null, obj);//注册变量
_acm.UpdateToUI(frm );//更新到控件
_acm.UpdateFromUI(frm );//更新到变量
```

###具体步骤
我们假设有一个窗体控件
> Form frm_main;  
>  |-TextBox tb_name;  
>  |-TextBox tb_age;  

和一个待显示的变量
> Person person;  
>  |-string Name;  
>  |-string Age;  

1. **修改控件的Tag属性**

```C#
tb_name.Tag= "Name";//指向person.Name
tb_age.Tag= "Age";//指向person.Age
``` 

2.**编辑代码**

```C#
//更新变量到控件
AutoControlMapper _acm = new AutoControlMapper();//实例化对象
_acm.RegisterVariableObject(null, person);//注册 person
_acm.UpdateToUI(frm_main);//导入frm_main,他会将变量的值更新到frm_main下面的所有控件(不包括它自身)
``` 

```C#
//更新控件到变量
AutoControlMapper _acm = new AutoControlMapper();//实例化对象
_acm.RegisterVariableObject(null, person);//注册 person
_acm.UpdateFromUI(frm_main);//导入frm_main,他会更新frm_main下面的所有控件的值到已注册的变量(不包括它自身)
``` 


###注意事项  
1.这个版本暂时只支持同一个属性的单次注册，如果同一个属性，多次注册，则会报错。  
2.当变量的值改变时，需要重新注册，控件的值不会自动更新；所以推荐每次更新空间的时候，都重新注册变量。
