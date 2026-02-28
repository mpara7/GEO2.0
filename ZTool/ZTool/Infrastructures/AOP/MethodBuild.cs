using System.Reflection;
using System.Reflection.Emit;

namespace ZTool.Infrastructures.AOP;
public static class MethodBuild
{
    public static Action Pack(Action<InvocationContext> action, InvocationContext obj)
    {
        return () => action(obj);
    }

    public static T Create<T>()
    {
        ZAOP.CreateAttriFunc = ZAOP.CreateAttriFunc ?? new Func<Type, object>((t) =>
        {
            return t.GetConstructors()[0].Invoke(new object[] { });
        });
        object o = ZAOP.CreateAttriFunc(typeof(T));
        return (T)o;
    }
    public static void ConstructAction(TypeBuilder tb, ILGenerator overrideIL, List<Attribute> attris, MethodInfo orgMethod)
    {
        MethodInfo baseCallMethod;
        List<FieldInfo> fieldInfos = new();
        //调用顺序 override->_1->_2->...->_base->base
        //定义字段
        {
            for (int i = 0; i < attris.Count; i++)
            {
                var attr = attris[i];
                string fieldName = $"{orgMethod.Name}_f_{i}_周启新";
                var fb = tb.DefineField(fieldName, attr.GetType(), FieldAttributes.Private);
                fieldInfos.Add(fb);
            }
        }
        //定义调用的基类函数的函数
        {
            //定义il
            string methodName = $"{orgMethod.Name}_base_周启新";
            var mb = tb.DefineMethod(methodName, MethodAttributes.Private, typeof(void), new Type[] { typeof(InvocationContext) });
            var subIL = mb.GetILGenerator();
            //Func为了最后赋值
            //subIL.Emit(OpCodes.Ldarg_1);

            subIL.Emit(OpCodes.Ldarg_0);

            //从InvokingContext.Parameters加载参数
            int i = 0;
            foreach (var param in orgMethod.GetParameters())
            {
                subIL.Emit(OpCodes.Ldarg_1);
                subIL.Emit(OpCodes.Ldfld, typeof(InvocationContext).GetField(nameof(InvocationContext.Parameters)));
                subIL.Emit(OpCodes.Ldc_I4, i++);
                subIL.Emit(OpCodes.Callvirt, typeof(object[]).GetMethod("GetValue", new Type[] { typeof(int) }));
                //不是引用类型需要拆箱
                if (!param.ParameterType.IsClass)
                {
                    subIL.Emit(OpCodes.Unbox_Any, param.ParameterType);
                }
            }
            //调用原函数
            subIL.Emit(OpCodes.Call, orgMethod);

            //！没有 赋值返回值

            subIL.Emit(OpCodes.Ret);
            baseCallMethod = mb;
        }
        //额外的函数
        List<MethodInfo> exMethodInfos = new();
        //定义各个Invoker的函数
        {
            for (int i = 0; i < attris.Count; i++)
            {
                var attr = attris[i];
                string methodName = $"{orgMethod.Name}_{i}_周启新";
                var mb = tb.DefineMethod(methodName, MethodAttributes.Private, typeof(void), new Type[] { typeof(InvocationContext) });
                var subIL = mb.GetILGenerator();
                //获取invoker
                subIL.Emit(OpCodes.Ldarg_0);
                subIL.Emit(OpCodes.Ldfld, fieldInfos[i]);
                //调用函数
                subIL.Emit(OpCodes.Ldarg_1);
                subIL.Emit(OpCodes.Callvirt, attr.GetType().GetMethod(nameof(InvokerAttribute.Invoke)));
                subIL.Emit(OpCodes.Ret);
                exMethodInfos.Add(mb);
            }
        }
        MethodInfo initInfo;
        //定义初始化函数
        {
            var mb = tb.DefineMethod($"{orgMethod.Name}_init_周启新", MethodAttributes.Private, typeof(void), new Type[] { typeof(InvocationContext) });
            initInfo = mb;
            var initIL = mb.GetILGenerator();
            //检查并创建中间类
            for (int j = 0; j < attris.Count; j++)
            {

                var label = initIL.DefineLabel();
                //取值
                initIL.Emit(OpCodes.Ldarg_0);
                initIL.Emit(OpCodes.Ldfld, fieldInfos[j]);
                //检查是否已有
                initIL.Emit(OpCodes.Ldnull);
                initIL.Emit(OpCodes.Ceq);
                initIL.Emit(OpCodes.Brfalse_S, label);
                //赋值
                initIL.Emit(OpCodes.Ldarg_0);
                initIL.Emit(OpCodes.Call, typeof(MethodBuild).GetMethod(nameof(Create)).MakeGenericMethod(attris[j].GetType()));
                initIL.Emit(OpCodes.Stfld, fieldInfos[j]);
                //已有则跳过
                initIL.MarkLabel(label);
            }

            //Next赋值
            for (int k = 0; k < fieldInfos.Count; k++)
            {
                //打包成Action<>
                initIL.Emit(OpCodes.Ldarg_0);
                initIL.Emit(OpCodes.Ldfld, fieldInfos[k]);
                initIL.Emit(OpCodes.Ldarg_0);
                if (k + 1 < fieldInfos.Count)
                    initIL.Emit(OpCodes.Ldftn, exMethodInfos[k + 1]);
                else
                    initIL.Emit(OpCodes.Ldftn, baseCallMethod);
                initIL.Emit(OpCodes.Newobj, typeof(Action<InvocationContext>).GetConstructors()[0]);
                //打包成Action
                initIL.Emit(OpCodes.Ldarg_1);
                initIL.Emit(OpCodes.Call, typeof(MethodBuild).GetMethod(nameof(Pack)));
                ////赋值到next中
                initIL.Emit(OpCodes.Stfld, typeof(InvokerAttribute).GetField(nameof(InvokerAttribute.Next)));
            }
            initIL.Emit(OpCodes.Ret);

        }
        //定义重写函数
        {
            overrideIL.DeclareLocal(typeof(InvocationContext));
            overrideIL.DeclareLocal(typeof(List<object>));

            ////创建上下文
            overrideIL.Emit(OpCodes.Newobj, typeof(InvocationContext).GetConstructors()[0]);
            overrideIL.Emit(OpCodes.Stloc_0);

            ////创建参数列表
            overrideIL.Emit(OpCodes.Newobj, typeof(List<object>).GetConstructors()[0]);
            overrideIL.Emit(OpCodes.Stloc_1);
            ////将参数写入参数列表中，该装箱的装箱
            int i = 1;
            foreach (var param in orgMethod.GetParameters())
            {
                overrideIL.Emit(OpCodes.Ldloc_1);
                overrideIL.Emit(OpCodes.Ldarg_S, i++);
                if (!param.ParameterType.IsClass)
                {
                    overrideIL.Emit(OpCodes.Box, param.ParameterType);
                }
                overrideIL.Emit(OpCodes.Callvirt, typeof(List<object>).GetMethod(nameof(List<object>.Add)));
            }
            ////列表转为数组赋值到Parameters
            overrideIL.Emit(OpCodes.Ldloc_0);
            overrideIL.Emit(OpCodes.Ldloc_1);
            overrideIL.Emit(OpCodes.Callvirt, typeof(List<object>).GetMethod(nameof(List<object>.ToArray)));
            overrideIL.Emit(OpCodes.Stfld, typeof(InvocationContext).GetField(nameof(InvocationContext.Parameters)));

            //初始化
            overrideIL.Emit(OpCodes.Ldarg_0);
            overrideIL.Emit(OpCodes.Ldloc_0);
            overrideIL.Emit(OpCodes.Call, initInfo);
            //////调用生成的函数0
            overrideIL.Emit(OpCodes.Ldarg_0);
            overrideIL.Emit(OpCodes.Ldloc_0);
            overrideIL.Emit(OpCodes.Call, exMethodInfos[0]);

            ////!!没有获取返回值
            overrideIL.Emit(OpCodes.Ret);
        }
    }

    public static void ConstructFunc(TypeBuilder tb, ILGenerator overrideIL, List<Attribute> attris, MethodInfo orgMethod)
    {
        MethodInfo baseCallMethod;
        List<FieldInfo> fieldInfos = new();
        //调用顺序 override->_1->_2->...->_base->base
        //定义字段
        {
            for (int i = 0; i < attris.Count; i++)
            {
                var attr = attris[i];
                string fieldName = $"{orgMethod.Name}_f_{i}_周启新";
                var fb = tb.DefineField(fieldName, attr.GetType(), FieldAttributes.Private);
                fieldInfos.Add(fb);
            }
        }
        //定义调用的基类函数的函数
        {
            //定义il
            string methodName = $"{orgMethod.Name}_base_周启新";
            var mb = tb.DefineMethod(methodName, MethodAttributes.Private, typeof(void), new Type[] { typeof(InvocationContext) });
            var subIL = mb.GetILGenerator();
            //Func为了最后赋值
            subIL.Emit(OpCodes.Ldarg_1);

            subIL.Emit(OpCodes.Ldarg_0);

            //从InvokingContext.Parameters加载参数
            int i = 0;
            foreach (var param in orgMethod.GetParameters())
            {
                subIL.Emit(OpCodes.Ldarg_1);
                subIL.Emit(OpCodes.Ldfld, typeof(InvocationContext).GetField(nameof(InvocationContext.Parameters)));
                subIL.Emit(OpCodes.Ldc_I4, i++);
                subIL.Emit(OpCodes.Callvirt, typeof(object[]).GetMethod("GetValue", new Type[] { typeof(int) }));
                //不是引用类型需要拆箱
                if (!param.ParameterType.IsClass)
                {
                    subIL.Emit(OpCodes.Unbox_Any, param.ParameterType);
                }
            }
            //调用原函数
            subIL.Emit(OpCodes.Call, orgMethod);

            //Func 赋值返回值
            if (!orgMethod.ReturnType.IsClass)
            {
                subIL.Emit(OpCodes.Box, orgMethod.ReturnType);
            }
            subIL.Emit(OpCodes.Stfld, typeof(InvocationContext).GetField(nameof(InvocationContext.ReturnValue)));
            subIL.Emit(OpCodes.Ret);

            baseCallMethod = mb;
        }
        //额外的函数
        List<MethodInfo> exMethodInfos = new();
        //定义各个Invoker的函数
        {
            for (int i = 0; i < attris.Count; i++)
            {
                var attr = attris[i];
                string methodName = $"{orgMethod.Name}_{i}_周启新";
                var mb = tb.DefineMethod(methodName, MethodAttributes.Private, typeof(void), new Type[] { typeof(InvocationContext) });
                var subIL = mb.GetILGenerator();
                //获取invoker
                subIL.Emit(OpCodes.Ldarg_0);
                subIL.Emit(OpCodes.Ldfld, fieldInfos[i]);
                //调用函数
                subIL.Emit(OpCodes.Ldarg_1);
                subIL.Emit(OpCodes.Callvirt, attr.GetType().GetMethod(nameof(InvokerAttribute.Invoke)));
                subIL.Emit(OpCodes.Ret);
                exMethodInfos.Add(mb);
            }
        }
        MethodInfo initInfo;
        //定义初始化函数
        {
            var mb = tb.DefineMethod($"{orgMethod.Name}_init_周启新", MethodAttributes.Private, typeof(void), new Type[] { typeof(InvocationContext) });
            initInfo = mb;
            var initIL = mb.GetILGenerator();
            //检查并创建中间类
            for (int j = 0; j < attris.Count; j++)
            {

                var label = initIL.DefineLabel();
                //取值
                initIL.Emit(OpCodes.Ldarg_0);
                initIL.Emit(OpCodes.Ldfld, fieldInfos[j]);
                //检查是否已有
                initIL.Emit(OpCodes.Ldnull);
                initIL.Emit(OpCodes.Ceq);
                initIL.Emit(OpCodes.Brfalse_S, label);
                //赋值
                initIL.Emit(OpCodes.Ldarg_0);
                initIL.Emit(OpCodes.Call, typeof(MethodBuild).GetMethod(nameof(Create)).MakeGenericMethod(attris[j].GetType()));
                initIL.Emit(OpCodes.Stfld, fieldInfos[j]);
                //已有则跳过
                initIL.MarkLabel(label);
            }

            //Next赋值
            for (int k = 0; k < fieldInfos.Count; k++)
            {
                //打包成Action<>
                initIL.Emit(OpCodes.Ldarg_0);
                initIL.Emit(OpCodes.Ldfld, fieldInfos[k]);
                initIL.Emit(OpCodes.Ldarg_0);
                if (k + 1 < fieldInfos.Count)
                    initIL.Emit(OpCodes.Ldftn, exMethodInfos[k + 1]);
                else
                    initIL.Emit(OpCodes.Ldftn, baseCallMethod);
                initIL.Emit(OpCodes.Newobj, typeof(Action<InvocationContext>).GetConstructors()[0]);
                //打包成Action
                initIL.Emit(OpCodes.Ldarg_1);
                initIL.Emit(OpCodes.Call, typeof(MethodBuild).GetMethod(nameof(Pack)));
                ////赋值到next中
                initIL.Emit(OpCodes.Stfld, typeof(InvokerAttribute).GetField(nameof(InvokerAttribute.Next)));
            }
            initIL.Emit(OpCodes.Ret);

        }
        //定义重写函数
        {
            overrideIL.DeclareLocal(typeof(InvocationContext));
            overrideIL.DeclareLocal(typeof(List<object>));

            ////创建上下文
            overrideIL.Emit(OpCodes.Newobj, typeof(InvocationContext).GetConstructors()[0]);
            overrideIL.Emit(OpCodes.Stloc_0);

            ////创建参数列表
            overrideIL.Emit(OpCodes.Newobj, typeof(List<object>).GetConstructors()[0]);
            overrideIL.Emit(OpCodes.Stloc_1);
            ////将参数写入参数列表中，该装箱的装箱
            int i = 1;
            foreach (var param in orgMethod.GetParameters())
            {
                overrideIL.Emit(OpCodes.Ldloc_1);
                overrideIL.Emit(OpCodes.Ldarg_S, i++);
                if (!param.ParameterType.IsClass)
                {
                    overrideIL.Emit(OpCodes.Box, param.ParameterType);
                }
                overrideIL.Emit(OpCodes.Callvirt, typeof(List<object>).GetMethod(nameof(List<object>.Add)));
            }
            ////列表转为数组赋值到Parameters
            overrideIL.Emit(OpCodes.Ldloc_0);
            overrideIL.Emit(OpCodes.Ldloc_1);
            overrideIL.Emit(OpCodes.Callvirt, typeof(List<object>).GetMethod(nameof(List<object>.ToArray)));
            overrideIL.Emit(OpCodes.Stfld, typeof(InvocationContext).GetField(nameof(InvocationContext.Parameters)));

            //初始化
            overrideIL.Emit(OpCodes.Ldarg_0);
            overrideIL.Emit(OpCodes.Ldloc_0);
            overrideIL.Emit(OpCodes.Call, initInfo);
            //////调用生成的函数0
            overrideIL.Emit(OpCodes.Ldarg_0);
            overrideIL.Emit(OpCodes.Ldloc_0);
            overrideIL.Emit(OpCodes.Call, exMethodInfos[0]);

            ////获取返回值
            overrideIL.Emit(OpCodes.Ldloc_0);
            overrideIL.Emit(OpCodes.Ldfld, typeof(InvocationContext).GetField(nameof(InvocationContext.ReturnValue)));

            if (!orgMethod.ReturnType.IsClass)
            {
                overrideIL.Emit(OpCodes.Unbox_Any, orgMethod.ReturnType);
            }
            overrideIL.Emit(OpCodes.Ret);
        }
    }
}
